using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ADJInsc.Helper
{
   

    public class EmailSender
    {
        public string codigo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public EmailSender(string CodigoVerificador, string email)
        {
            codigo = CodigoVerificador;
            Email = email;
        }
              

        public Task<bool> SendEmail()
        {
            try
            {
                //http://localhost:49999/api/verificador/9809jsijd89a7sekjnpdf

                /*Produccion
                  var url = "<a href=http://190.52.39.140/Test.Incs/Api/Verificador/" + codigo + " > Validar </a>";
                
                 */

                 var url = "<a href=http://190.52.39.140/insc/Api/Verificador/" + codigo + " > Validar </a>";
               
                var request = new MailRequest
                {
                    ToEmail = Email,
                    Subject = "Instituto de Vivienda y Urbanismo de Jujuy",
                    
                    Body = string.Format("Según nuestros registros, usted creó un usuario en el Sistema de Inscripción/Actualización de Datos del IVUJ, haga clic en el siguiente enlace para validar sus datos {0} , Una vez hecho esto, podrá avanzar en la carga de información. En caso de que no valide, copie y pegue el link en el navegador. <br /> <br /> <strong>NO RESPONDA ESTE CORREO, EL MISMO NO ESTÁ HABILITADO PARA RECIBIR MENSAJES.</strong> ", url)//,
                    //Attachments = null
                };

                MailMessage message = new MailMessage("info_inscripcion@ivuj.gob.ar", Email, request.Subject, request.Body)
                {
                    IsBodyHtml = true
                };
                var result = SendEmailAsync(message);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> Recuperar()
        {
            try
            {
                
                var request = new MailRequest
                {
                    ToEmail = Email,
                    Subject = "Instituto de Vivienda y Urbanismo de Jujuy",

                    Body = string.Format("Según nuestros registros, usted creó un usuario en el Sistema de Inscripción/Actualización de Datos del IVUJ <br /> Usuario: {0} <br /> Contraseña: {1}  <br /> <br /> <strong>NO RESPONDA ESTE CORREO, EL MISMO NO ESTÁ HABILITADO PARA RECIBIR MENSAJES.</strong> ", Email, codigo)//,
                    //Attachments = null
                };

                MailMessage message = new MailMessage("info_inscripcion@ivuj.gob.ar", Email, request.Subject, request.Body)
                {
                    IsBodyHtml = true
                };
                return await SendEmailAsync(message);
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<bool> SendEmailAsync(MailMessage mailRequest)
        {
            try
            {
                using(SmtpClient smtp = new SmtpClient("mail.ivuj.gob.ar"))
                {
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new NetworkCredential("info_inscripcion@ivuj.gob.ar", "iYd_HF@7zKuc");
                    smtp.EnableSsl = false;
                    smtp.Port = 587;

                    await smtp.SendMailAsync(mailRequest);
                }

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
