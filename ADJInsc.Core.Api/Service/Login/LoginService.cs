﻿namespace ADJInsc.Core.Api.Service.Login
{
    using ADJInsc.Core.Logica;
    using ADJInsc.Core.Logica.Service.Interface;
    using ADJInsc.Models.ViewModels;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using System.Transactions;

    public class LoginService 
    {
        public string _connectionString { get; set; }
        private SqlConnection con;
        private LogicHelper _helper;
        //private readonly IMailService _mailService;

        public LoginService(string connectionString)
        {
            _connectionString = connectionString;
           // _mailService = mailService;
            _helper = new LogicHelper(_connectionString);
        }
        private void Connection()
        {
            //string constr = ConfigurationManager.ConnectionStrings["InscConnection"].ToString();     //PRODUCCION                                                                                                        //string constr = ConfigurationManager.ConnectionStrings["TestConnection"].ToString();       //TEST
            con = new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Metodo que cambia el estado a "E" porque validó su correo, solamente, al completar
        /// la carga de datos recien va a cambiar el estado a "I".
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<bool> VerificarGuid(Guid guid)
        {
            Connection();

            using TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // E : es parte de un estado,en este caso significa Validado(se valido el token)
                // A : es parte de un estado,en esta caso significa que estoy registrado
                var fechaActual = DateTime.Now.ToShortDateString();
                var query = "UPDATE Inscriptos  SET ins_estado = 'E', ins_fecins = @fechaActual where CodigoVerificador = @codigo and ins_estado = 'A' ;" +
                    " Select ins_id from Inscriptos where CodigoVerificador = @codigo and ins_estado = 'E'";

                using var cmd1 = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };

                //cmd1.Parameters.Add(new SqlParameter("@estado", "E"));
                cmd1.Parameters.Add(new SqlParameter("@codigo", guid));
                cmd1.Parameters.Add(new SqlParameter("@fechaActual", fechaActual));

                if (con.State == ConnectionState.Closed)
                    await con.OpenAsync();

                var result = await cmd1.ExecuteScalarAsync();

                if (result != null)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch
            {
                ts.Dispose();
                return false;
            }
            finally
            {
                await con.CloseAsync();
            }

        }

        public async Task<Guid> ResetPasword(string email)
        {
            Connection();

            using TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
               
                var query = "UPDATE Inscriptos  SET CodigoVerificador = newid(), ins_estado = 'A' output inserted.CodigoVerificador where ins_email = @email";

                using var cmd1 = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };

                cmd1.Parameters.Add(new SqlParameter("@email", email));
                
                if (con.State == ConnectionState.Closed)
                    await con.OpenAsync();

                var result = await cmd1.ExecuteScalarAsync();

                if (result != null && result.GetType() == typeof(Guid))
                {
                    ts.Complete();
                    return new Guid((string)result);
                }              
                else
                {
                    ts.Dispose();
                    return Guid.Empty;
                }
            }
            catch
            {
                ts.Dispose();
                return Guid.Empty; 
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<InscViewModel> GetLogin(string usuario, string pasword)
        {
            var result = await _helper.GetLogin(usuario,pasword);
            return result;
        }

        public async Task<string> Recuperar(string email, int dni)
        {
            try
            {
                var claveUsuario = await _helper.Recuperar(email.Trim(), dni);
                if (string.IsNullOrEmpty(claveUsuario)) return "NoSend";

                var helperSendEmail = new Helper.EmailSender(claveUsuario, email);

                var result = await helperSendEmail.Recuperar();
                if (result) return "Ok";
                return "NoSend";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
