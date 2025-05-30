namespace ADJInsc.Core.Logica
{   
    using ADJInsc.Models.ViewModels;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using ADJInsc.Models.ViewModels.UpLoad;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Threading.Tasks;
    using System.Transactions;

    public class AdhesionHelper
    {
        public string _connectionString { get; set; }
        private SqlConnection con;


        public AdhesionHelper(string ConnectioonString)
        {
            _connectionString = ConnectioonString;

        }
        private void Connection()
        {
            //string constr = ConfigurationManager.ConnectionStrings["InscConnection"].ToString();     //PRODUCCION                                                                                                        //string constr = ConfigurationManager.ConnectionStrings["TestConnection"].ToString();       //TEST
            con = new SqlConnection(_connectionString);
        }
          
        public async Task<InscViewModel> PostModeloAdhesion(InscViewModel model)
        {
            InscViewModel inscViewModel = new InscViewModel();
            inscViewModel = model;

            var adheridoId = await GetAdherido(inscViewModel.AdherirViewModel);
            string insertAdherido = "INSERT INTO Adhesion  ( FechaAdhesion, Estado,  ModuloId, InscriptoId, ProgramaId) VALUES " +
                                                   " (  @fechaAdhesion, @estado, @moduloId, @inscriptoId, @programaId ); SELECT SCOPE_IDENTITY();";
            if (adheridoId != 0)
            {
                inscViewModel.AdhesionViewModel.Success = false;
                inscViewModel.AdherirViewModel.AdhesionId = adheridoId;
                return inscViewModel;
            }
            Connection();
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (adheridoId == 0)
                    {
                        using SqlCommand cmdInsertAdherido = new SqlCommand(insertAdherido, con);
                        cmdInsertAdherido.CommandType = CommandType.Text;

                        cmdInsertAdherido.Parameters.Add(new SqlParameter("@fechaAdhesion", inscViewModel.AdherirViewModel.FechaAdhesion));
                        cmdInsertAdherido.Parameters.Add(new SqlParameter("@estado", 1)); //1 true
                        cmdInsertAdherido.Parameters.Add(new SqlParameter("@moduloId", inscViewModel.AdherirViewModel.ModuloId));
                        cmdInsertAdherido.Parameters.Add(new SqlParameter("@inscriptoId", inscViewModel.AdherirViewModel.InscriptoId));
                        cmdInsertAdherido.Parameters.Add(new SqlParameter("@programaId", inscViewModel.AdherirViewModel.ProgramaId));

                        if (con.State == ConnectionState.Closed)
                            await con.OpenAsync();
                        var objetoId = new object();  //limpio el objeto   
                        objetoId = await cmdInsertAdherido.ExecuteScalarAsync();

                        decimal adhesionId = (objetoId != null) ? Convert.ToDecimal(objetoId) : 0;

                        inscViewModel.AdherirViewModel.AdhesionId = (int)adhesionId;
                        if (adhesionId > 0)
                        {
                            //1_ subir el archivo
                            var tempVM = await GetUpload(inscViewModel, (int)adhesionId);
                            if (tempVM.FileUploadViewModel.Count == 0)
                            {
                                inscViewModel.AdhesionViewModel.Success = false;
                                return inscViewModel;
                            }
                            inscViewModel.FileUploadViewModel = tempVM.FileUploadViewModel;
                            //2_ incertar datos en base
                            tempVM = await InsertArchivos(inscViewModel);
                            if (tempVM.FileUploadViewModel.Count == 0)
                            {
                                inscViewModel.AdhesionViewModel.Success = false;
                                return inscViewModel;
                            }
                            inscViewModel.FileUploadViewModel = tempVM.FileUploadViewModel;
                        }
                        inscViewModel.AdhesionViewModel.Success = true;
                        ts.Complete();
                    }
                    else
                    {
                        inscViewModel.AdhesionViewModel.Success = false;
                        inscViewModel.AdherirViewModel.AdhesionId = 0;
                        ts.Dispose();
                    }
                    return inscViewModel;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    await con.CloseAsync();
                    inscViewModel.AdhesionViewModel.Success = false;
                    inscViewModel.AdhesionViewModel.ErrorMsg = ex.Message;
                    return inscViewModel;
                }
                finally
                {
                    await con.CloseAsync();
                }
            }

        }

        private async Task<InscViewModel> InsertArchivos(InscViewModel modelo)
        {            
            var script = "INSERT INTO ArchivosAdhesion(Fecha, NombreArchivo, Tamano, TipoContenido, InscriptoId, RutaArchivo, TipoIngreso) " +
                                        " VALUES (@Fecha, @NombreArchivo, @Tamano, @TipoContenido, @InscriptoId, @RutaArchivo, @TipoIngreso); SELECT SCOPE_IDENTITY()";
            try
            {
                InscViewModel inscViewModel = new InscViewModel();
                inscViewModel = modelo;

                var archivos = new List<string>();
                foreach (var item in inscViewModel.FileUploadViewModel)
                {
                    using SqlCommand cmd = new SqlCommand(script, con);
                    cmd.Parameters.Add(new SqlParameter("@Fecha",item.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@NombreArchivo", item.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@Tamano", item.Tamano));
                    cmd.Parameters.Add(new SqlParameter("@TipoContenido", item.TipoContenido));

                    cmd.Parameters.Add(new SqlParameter("@InscriptoId", item.InscriptoId));
                    cmd.Parameters.Add(new SqlParameter("@RutaArchivo", item.RutaArchivo));
                    cmd.Parameters.Add(new SqlParameter("@TipoIngreso", item.TipoIngreso.Trim()));

                    var objetoId = new object();

                    if (con.State == ConnectionState.Closed)
                        await con.OpenAsync();

                    objetoId = await cmd.ExecuteScalarAsync();

                    await con.CloseAsync();
                    decimal archivoId = (objetoId != null) ? Convert.ToDecimal(objetoId) : 0;                   
                    
                    if (archivoId == 0)
                    {
                        inscViewModel.FileUploadViewModel = new List<FileUploadViewModel>();                        
                        return inscViewModel; //por si uno falla no deberia cargar nada
                    }
                   
                    item.ArchivoId = (int)archivoId;
                }
                return inscViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GuardarEnDisco(int adhesionId, string fecha, string nombreArchivo, byte[] fileContent)
        {
            try
            {
                string random = new Random().Next(1000, 9999999).ToString();
                string fileName = $"{adhesionId}_{fecha}_{random}_{nombreArchivo}";
                var ruta = CreateRutaAdherido(fileName, adhesionId);

                await File.WriteAllBytesAsync(ruta, fileContent);

                return ruta;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CreateRutaAdherido(string fileName, int adhesionId)
        {
            try
            {
                string basePath = "F:/ArchivosDeAdhesion/";
                string directoryPath = Path.Combine(basePath, adhesionId.ToString());
                if(!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath); // No es necesario verificar si existe

                return Path.Combine(directoryPath, fileName);
            }
            catch (Exception)
            {
                throw;
            }
           
        }
        private async Task<InscViewModel> GetUpload(InscViewModel model, int adhesionId)
        {
            InscViewModel inscViewModel = new InscViewModel();
            inscViewModel = model;

            try
            {               
                var filesData = new List<FileUploadViewModel>();                

                if (inscViewModel.FileUploadViewModel == null || inscViewModel.FileUploadViewModel.Count == 0)
                {
                    inscViewModel.AdhesionViewModel.Success = false;
                    inscViewModel.FileUploadViewModel = new List<FileUploadViewModel>();
                    return inscViewModel;
                }

                foreach (var item in inscViewModel.FileUploadViewModel)
                {
                    //string fileName = $"{inscViewModel.InsId}_{adheridoId}_{item.Fecha}_{item.NombreArchivo}_{random}";

                    filesData.Add(new FileUploadViewModel
                    {
                        FileContent = item.FileContent,
                        TipoContenido = item.TipoContenido,
                        NombreArchivo = item.NombreArchivo,
                        Fecha = item.Fecha,
                        Tamano = item.Tamano,
                        InscriptoId = inscViewModel.InsId,
                        AdheridoId = adhesionId,
                        TipoIngreso = item.TipoIngreso,
                        RutaArchivo = await GuardarEnDisco(adhesionId,item.Fecha.ToString(), item.NombreArchivo, item.FileContent)
                    });
                }
                inscViewModel.FileUploadViewModel = filesData;
                return inscViewModel;
                
            }
            catch (Exception)
            {
                return inscViewModel;
                throw;
            }            
        }

        public async Task<int> GetAdherido(AdherirViewModel adherirViewModel)
        {
            Connection();
            var adhesionVM = new AdhesionViewModel();

            try
            {
                string consultarAdherido = "SELECT AdhesionId FROM Adhesion WHERE InscriptoId = @inscriptoId and ProgramaId = @programaId  ";
                using SqlCommand cmdConsulta = new SqlCommand(consultarAdherido, con);

                //cmdConsulta.Parameters.Add(new SqlParameter("@moduloId", adherirViewModel.ModuloId));
                cmdConsulta.Parameters.Add(new SqlParameter("@inscriptoId", adherirViewModel.InscriptoId));
                cmdConsulta.Parameters.Add(new SqlParameter("@programaId", adherirViewModel.ProgramaId));

                var objetoId = new object();

                if (con.State == ConnectionState.Closed)
                    await con.OpenAsync();

                objetoId = null;  //limpio el objeto   
                objetoId = await cmdConsulta.ExecuteScalarAsync();

                await con.CloseAsync();
                decimal adhesionId = (objetoId != null) ? Convert.ToDecimal(objetoId) : 0;
                return (int)adhesionId;
            }
            catch (Exception ex)
            {
                adhesionVM.Success = false;
                adhesionVM.ErrorMsg = ex.Message;

                await con.CloseAsync();
                throw ex;
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<AdhesionViewModel> GetProgramaYModulos(AdhesionViewModel model)
        {           
            var adhesionVM = new AdhesionViewModel();
            AdhesionViewModel adhesionModel = new AdhesionViewModel();
            adhesionModel = model;
            var adhesionTempVM = await GetAdherido(adhesionModel);

            if(adhesionTempVM.AdhesionId != 0)
            {
                adhesionTempVM.Success = true;
                return adhesionTempVM;
            }

            Connection();

            try
            {
                /*
                 * SELECT ProgramaId, Descripcion ,FechaInicio ,FechaLimite ,Estado ,FechaCortePrograma ,MontoAdhesion FROM Programas where Estado = 1
                 *   SELECT ModuloId, Descripcion ,Plazo ,Costo ,IngresoMinimo, Estado ,ProgramaId FROM Modulos where @ProgramaId = 1
                 *   SELECT DepartamentoProgramaId, ProgramaId, DepartamentoId, LocalidadId  FROM DepartamentoPrograma where ProgramaId = 1
                 */
                string query = "SELECT ProgramaId, Descripcion ,FechaInicio ,FechaLimite ,Estado ,FechaCortePrograma ,MontoAdhesion FROM Programas where Estado = 1 ";
                string query1 = "SELECT ModuloId, Titulo, Descripcion ,Plazo ,Costo , CostoSinEntrega, CostoConEntrega, IngresoMinimo, Estado ,ProgramaId, CostoUvis , CostoSinEntregaUvis, CostoConEntregaUvis, IngresoMinimoUvis FROM Modulos where ProgramaId = @ProgramId ";
                string query2 = " SELECT DepartamentoProgramaId, ProgramaId, DepartamentoId, LocalidadId  FROM DepartamentoPrograma where ProgramaId = @ProgramId ";

                var dt =  await GetDataTable(query,0, "Programa");

                var modulosList = new List<ModulosVM>();
              
                var deptoProgramaList = new List<DepartamentoProgramaVM>();
                var programaId = 0;

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        programaId = ConvertFromReader<int>(item["ProgramaId"]);
                        adhesionVM.ProgramaVM = new ProgramaVM
                        {
                            ProgramaId = programaId,
                            Descripcion = ConvertFromReader<string>(item["Descripcion"]),
                            FechaInicio = ConvertFromReader<string>(item["FechaInicio"]), // is DBNull ? DateTime.MinValue : (DateTime)item["FechaInicio"],
                            FechaLimite = ConvertFromReader<string>(item["FechaLimite"]), // is DBNull ? DateTime.MinValue : (DateTime)item["FechaLimite"],
                            Estado = ConvertFromReader<int>(item["Estado"]),
                            FechaCortePrograma = ConvertFromReader<string>(item["FechaCortePrograma"]), // is DBNull ? DateTime.MinValue : (DateTime)item["FechaCortePrograma"],
                            MontoAdhesion = ConvertFromReader<decimal>(item["MontoAdhesion"])
                        };
                    }
                }

                dt = new DataTable();
                dt = await GetDataTable(query1, programaId, "Programa");
                
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        var moduloVM = new ModulosVM
                        {
                            ModuloId = ConvertFromReader<int>(item["ModuloId"]),
                            Descripcion = ConvertFromReader<string>(item["Descripcion"]),
                            Titulo = ConvertFromReader<string>(item["Titulo"]),
                            Plazo = ConvertFromReader<int>(item["Plazo"]),
                            Costo = ConvertFromReader<decimal>(item["Costo"]),
                            CostoSinEntrega = ConvertFromReader<decimal>(item["CostoSinEntrega"]),
                            CostoConEntrega = ConvertFromReader<decimal>(item["CostoConEntrega"]),
                            IngresoMinimo = ConvertFromReader<decimal>(item["IngresoMinimo"]),
                            Estado = ConvertFromReader<int>(item["Estado"]),
                            ProgramaId = ConvertFromReader<int>(item["ProgramaId"]),
                            CostoUvis = ConvertFromReader<decimal>(item["CostoUvis"]),
                            CostoSinEntregaUvis = ConvertFromReader<decimal>(item["CostoSinEntregaUvis"]),
                            CostoConEntregaUvis = ConvertFromReader<decimal>(item["CostoConEntregaUvis"]),
                            IngresoMinimoUvis = ConvertFromReader<decimal>(item["IngresoMinimoUvis"]),
                        };
                        modulosList.Add(moduloVM);
                    }
                }

                adhesionVM.ModulosVM = modulosList;

                dt = new DataTable();
                dt = await GetDataTable(query2, programaId, "Programa");
               
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        var deptoPrograma = new DepartamentoProgramaVM
                        {
                            DepartamentoProgramaId = ConvertFromReader<int>(item["DepartamentoProgramaId"]),
                            LocalidadId = ConvertFromReader<int>(item["LocalidadId"]),
                            ProgramaId = ConvertFromReader<int>(item["ProgramaId"]),
                            DepartamentoId = ConvertFromReader<int>(item["DepartamentoId"])
                        };

                        deptoProgramaList.Add(deptoPrograma);
                    }
                }
                
                adhesionVM.DepartamentoProgramas = deptoProgramaList;                
                adhesionVM.Success = true;
                return  adhesionVM;
            }
            catch (Exception ex)
            {
                adhesionVM.Success=false;
                adhesionVM.ErrorMsg = ex.Message;

                await con.CloseAsync();
                throw;
            }
            finally
            {
                await con.CloseAsync();
            }

        }

        private async Task<AdhesionViewModel> GetAdherido(AdhesionViewModel model)
        {           
            try
            {
                //solo el programa activo
                string consultarAdherido = "select a.AdhesionId, a.FechaAdhesion, m.Titulo from Adhesion a " +
                    " inner join modulos m on a.ModuloId = m.ModuloId " +
                    " inner join Programas p on p.ProgramaId = a.ProgramaId where p.Estado = 1 and a.InscriptoId = @inscriptoId";
                using SqlCommand cmdConsulta = new SqlCommand(consultarAdherido, con);

                var dt = await GetDataTable(consultarAdherido, model.InscriptoId, "Inscripto");
     
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        model.AdhesionId = ConvertFromReader<int>(item["AdhesionId"]);
                        model.FechaAdhesion = ConvertFromReader<string>(item["FechaAdhesion"]); //item["FechaAdhesion"] is DBNull ? null : ((DateTime)item["FechaAdhesion"]).ToString();
                        model.DescripcionModulo = ConvertFromReader<string>(item["Titulo"]);
                        break;
                    }
                }               
                return model;
            }
            catch (Exception ex)
            {
                model.Success = false;
                model.ErrorMsg = ex.Message;

                await con.CloseAsync();
                throw ex;
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        private async Task<DataTable> GetDataTable(string query, int param, string modo)
        {
            try
            {
                Connection();

                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();

                cmd = new SqlCommand(query, con)
                    {
                        CommandType = CommandType.Text
                    };
                
                if(param > 0 && modo  == "Programa")
                {
                    cmd.Parameters.Add(new SqlParameter("@ProgramId", param));
                }
                if(param > 0 && modo == "Inscripto")
                {
                    cmd.Parameters.Add(new SqlParameter("@inscriptoId", param));
                }

                da = new SqlDataAdapter(cmd);
                if (con.State == ConnectionState.Closed)
                    await con.OpenAsync();
                da.Fill(dt);

                await con.CloseAsync();
                return dt;
            }
            catch (Exception ex)
            {
                await con.CloseAsync();
                throw ex;
            }
        }

        public static T ConvertFromReader<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }

        public async Task<ModelPdf> GetModelPdf(ModelPdf modelPdf)
        {
            var model = new ModelPdf();
            var inscripto = modelPdf.InscriptoId;
            string script = "select A.AdhesionId, L.LocalidadDesc, I.ins_email, I.ins_nombre, I.cuit_cuil, I.ins_numdoc,A.FechaAdhesion, M.Titulo from Localidad L inner join InsDomici N on (N.IdDepartamento = L.DepartamentoKey and N.IdLocalidad = L.LocalidadKey) inner join Inscriptos I on I.IdDomicilio = N.insd_id inner join Adhesion A on I.ins_id = A.InscriptoId inner join Modulos M on (M.ProgramaId = A.ProgramaId and M.ModuloId = A.ModuloId) where A.InscriptoId = @inscriptoId";
            //@inscriptoId
            try
            {
                Connection();
                using SqlCommand cmdConsulta = new SqlCommand(script, con);

                var dt = await GetDataTable(script, int.Parse(inscripto), "Inscripto");
                model.InscriptoId = inscripto;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        model.AdheridoId = ConvertFromReader<int>(item["AdhesionId"]).ToString();
                        model.Localidad = ConvertFromReader<string>(item["LocalidadDesc"]);
                        model.Correo = ConvertFromReader<string>(item["ins_email"]);
                        model.NombreAdherido = ConvertFromReader<string>(item["ins_nombre"]);
                        model.CuitAdherido = ConvertFromReader<string>(item["cuit_cuil"]).ToString();
                        model.DniAdherido = ConvertFromReader<string>(item["ins_numdoc"]).ToString();
                        model.FechaAdhesion = ConvertFromReader<string>(item["FechaAdhesion"]).ToString();
                        model.TipologiaDescripcion = ConvertFromReader<string>(item["Titulo"]);
                        break;
                    }
                }
                return model;

            }
            catch (Exception ex)
            {
                model.NombreAdherido = ex.Message;

                await con.CloseAsync();
                throw ex;
            }
            finally
            {
                await con.CloseAsync();
            }
        }
    }
}
