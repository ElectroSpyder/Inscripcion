using MailKit.Net.Smtp;
using MimeKit;

using System;
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

        [Obsolete]
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

                MimeMessage mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress("info_inscripcion@ivuj.gob.ar"));
                mensaje.To.Add(new MailboxAddress(Email));
                mensaje.Subject = request.Subject;
                mensaje.Body = new TextPart("html")
                {
                    Text = request.Body
                };


                //MimeMessage message = new MimeMessage("info_inscripcion@ivuj.gob.ar", Email, request.Subject, request.Body)
                //{
                //    IsBodyHtml = true
                //};
                var result = SendEmailAsync(mensaje);
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
                MimeMessage mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress("info_inscripcion@ivuj.gob.ar"));
                mensaje.To.Add(new MailboxAddress(Email));
                mensaje.Subject = request.Subject;
                mensaje.Body = new TextPart("html")
                {
                    Text = request.Body
                };

                //MailMessage message = new MailMessage("info_inscripcion@ivuj.gob.ar", Email, request.Subject, request.Body)
                //{
                //    IsBodyHtml = true
                //};
                return await SendEmailAsync(mensaje);

            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<bool> SendEmailAsync(MimeMessage mailRequest)
        {
            try
            {

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("vps-3513664-x.dattaweb.com", 465, true);
                    smtp.Authenticate("info_inscripcion@ivuj.gob.ar", "sldXm/B7sJ");

                    await smtp.SendAsync(mailRequest);
                    //smtp.UseDefaultCredentials = true;                   

                    ////smtp.Port = 995; // antes este puerto 587;
                    //smtp.Credentials = new NetworkCredential("info_inscripcion@ivuj.gob.ar", "sldXm/B7sJ");
                    //smtp.EnableSsl = true;  //antes false
                    //await smtp.SendMailAsync(mailRequest);
                }

                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
