namespace ADJInsc.Core.Logica
{
    using ADJInsc.Models.Models.DBAdhesion;
    using ADJInsc.Models.Models.DBInsc;
    using ADJInsc.Models.ViewModels;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using ADJInsc.Models.ViewModels.UpLoad;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
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
        public async Task<InscViewModel> PostModeloAdhesion(InscViewModel inscViewModel)
        {
            var adheridoId = await GetAdherido(inscViewModel.AdherirViewModel);
            string insertAdherido = "INSERT INTO Adhesion  ( FechaAdhesion, Estado,  ModuloId, InscriptoId, ProgramaId) VALUES " +
                                                   " (  @fechaAdhesion, @estado, @moduloId, @inscriptoId, @programaId ); SELECT SCOPE_IDENTITY();";
            if(adheridoId != 0)
            {
                inscViewModel.AdhesionViewModel.Success = false;
                inscViewModel.AdherirViewModel.AdhesionId = adheridoId;
                return inscViewModel;
            }
            using TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
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

                    decimal adhesionId = (objetoId != null) ? (int)objetoId : 0;

                    inscViewModel.AdherirViewModel.AdhesionId = (int)adhesionId;
                    if (adheridoId > 0)
                    {
                        //1_ subir el archivo
                        var tempVM = await GetUpload(inscViewModel, adheridoId);
                        if (tempVM.FileUploadViewModel.Count == 0)
                        {
                            inscViewModel.AdhesionViewModel.Success = false;                           
                            return inscViewModel;
                        }
                        inscViewModel.FileUploadViewModel = tempVM.FileUploadViewModel;
                        //2_ iincertar datos en base
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

        private async Task<InscViewModel> InsertArchivos(InscViewModel inscViewModel)
        {
            /*
             INSERT INTO ArchivosAdhesion(Fecha, NombreArchivo, Tamano, TipoContenido, InscriptoId, RutaArchivo)  VALUES
                    (@Fecha, @NombreArchivo, @Tamano, @TipoContenido, @InscriptoId, @RutaArchivo)GO
             */
            var script = "INSERT INTO ArchivosAdhesion(Fecha, NombreArchivo, Tamano, TipoContenido, InscriptoId, RutaArchivo) " + 
                                        " VALUES (@Fecha, @NombreArchivo, @Tamano, @TipoContenido, @InscriptoId, @RutaArchivo); SELECT SCOPE_IDENTITY()" ;
            try
            {
                var archivos = new List<string>();
                foreach (var item in inscViewModel.FileUploadViewModel)
                {
                    using SqlCommand cmd = new SqlCommand(script, con);
                    cmd.Parameters.Add(new SqlParameter("@Fecha",item.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@NombreArchivo", item.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@Tamano", item.Tamano));
                    cmd.Parameters.Add(new SqlParameter("@TipoContenido", item.TipoContenido));

                    cmd.Parameters.Add(new SqlParameter("@InscriptoId", item.InscriptoId));
                    cmd.Parameters.Add(new SqlParameter("@Tamano", item.Tamano));
                    cmd.Parameters.Add(new SqlParameter("@RutaArchivo", item.RutaArchivo));

                    var objetoId = new object();

                    if (con.State == ConnectionState.Closed)
                        await con.OpenAsync();

                    objetoId = await cmd.ExecuteScalarAsync();

                    await con.CloseAsync();
                    decimal archivoId = (objetoId != null) ? (int)objetoId : 0;
                    if (archivoId == 0)
                    {
                        inscViewModel.FileUploadViewModel = new List<FileUploadViewModel>();
                        return inscViewModel; //por si uno falla no deberia cargar nada
                    }
                   
                    item.ArchivoId = (int)archivoId;
                }
                return inscViewModel;
            }
            catch (Exception ex)
            {
                return inscViewModel;
            }
        }

        private async Task<string> GuardarEnDisco(int adheridoId, string fecha, string nombreArchivo, byte[] fileContent)
        {
            try
            {
                string random = new Random().Next(1000, 9999).ToString();
                string fileName = $"{adheridoId}_{fecha}_{nombreArchivo}_{random}";
                var ruta = CreateRutaAdherido(fileName, adheridoId);

                await File.WriteAllBytesAsync(ruta, fileContent);

                return ruta;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string CreateRutaAdherido(string fileName, int adheridoId)
        {
            try
            {
                string _uploadPath = "F:/ArchivosDeAdhesion/";
                
                _uploadPath = _uploadPath + adheridoId.ToString().Trim();
                if (!Directory.Exists(_uploadPath))
                    Directory.CreateDirectory(_uploadPath);
                return Path.Combine(_uploadPath, fileName);
            }
            catch (Exception)
            {
                throw;
            }
           
        }
        private async Task<InscViewModel> GetUpload(InscViewModel inscViewModel, int adheridoId)
        {
            try
            {               
                var filesData = new List<FileUploadViewModel>();

                string random = new Random().Next(1000, 9999999).ToString();

                if (inscViewModel.FileUploadViewModel == null || inscViewModel.FileUploadViewModel.Count == 0)
                {
                    inscViewModel.FileUploadViewModel = new List<FileUploadViewModel>();
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
                        AdheridoId = adheridoId,
                        RutaArchivo = await GuardarEnDisco(adheridoId,item.Fecha.ToString(), item.NombreArchivo, item.FileContent)
                    });
                }
                inscViewModel.FileUploadViewModel = filesData;
                return inscViewModel;
                ;
            }
            catch (Exception ex)
            {
                return inscViewModel;
                throw ex;
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
                decimal adhesionId = (objetoId != null) ? (int)objetoId : 0;
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
        public async Task<AdhesionViewModel> GetProgramaYModulos()
        {
            Connection();
            var adhesionVM = new AdhesionViewModel();

            try
            {
                /*
                 * SELECT ProgramaId, Descripcion ,FechaInicio ,FechaLimite ,Estado ,FechaCortePrograma ,MontoAdhesion FROM Programas where Estado = 1
                 *   SELECT ModuloId, Descripcion ,Plazo ,Costo ,IngresoMinimo, Estado ,ProgramaId FROM Modulos where @ProgramaId = 1
                 *   SELECT DepartamentoProgramaId, ProgramaId, DepartamentoId, LocalidadId  FROM DepartamentoPrograma where ProgramaId = 1
                 */
                string query = "SELECT ProgramaId, Descripcion ,FechaInicio ,FechaLimite ,Estado ,FechaCortePrograma ,MontoAdhesion FROM Programas where Estado = 1 ";
                string query1 = "SELECT ModuloId, Titulo, Descripcion ,Plazo ,Costo , CostoSinEntrega, CostoConEntrega, IngresoMinimo, Estado ,ProgramaId FROM Modulos where ProgramaId = @ProgramId ";
                string query2 = " SELECT DepartamentoProgramaId, ProgramaId, DepartamentoId, LocalidadId  FROM DepartamentoPrograma where ProgramaId = @ProgramId ";

                var dt =  await GetDataTable(query,0);

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
                            FechaInicio = item["FechaInicio"] is DBNull ? DateTime.MinValue : (DateTime)item["FechaInicio"],
                            FechaLimite = item["FechaLimite"] is DBNull ? DateTime.MinValue : (DateTime)item["FechaLimite"],
                            Estado = ConvertFromReader<int>(item["Estado"]),
                            FechaCortePrograma = item["FechaCortePrograma"] is DBNull ? DateTime.MinValue : (DateTime)item["FechaCortePrograma"],
                            MontoAdhesion = ConvertFromReader<decimal>(item["MontoAdhesion"])
                        };
                    }
                }

                dt = new DataTable();
                dt = await GetDataTable(query1, programaId);
                
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
                            ProgramaId = ConvertFromReader<int>(item["ProgramaId"])
                        };
                        modulosList.Add(moduloVM);
                    }
                }

                adhesionVM.ModulosVM = modulosList;

                dt = new DataTable();
                dt = await GetDataTable(query2, programaId);
               
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
                throw ex;
            }
            finally
            {
                await con.CloseAsync();
            }

        }

        private async Task<DataTable> GetDataTable(string query, int param)
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
                
                if(param > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@ProgramId", param));
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

    }
}
