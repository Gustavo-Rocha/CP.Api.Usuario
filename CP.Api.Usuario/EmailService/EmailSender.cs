using CP.Api.Usuario.EmailConfiguration;
using System.Net;
using System.Net.Mail;

namespace CP.Api.Usuario.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly NotificationMetadata _notificationMetadata;

        public EmailSender(NotificationMetadata _notificationMetadata)
        {
            this._notificationMetadata = _notificationMetadata;
        }


        public bool SendEmail(string senha, string reciever)
        {
            var fromAddress = new MailAddress(_notificationMetadata.Sender, "Gustavo Oliveira");
            var toAddress = new MailAddress(reciever);
            const string fromPassword = "66214582@gu";
            //const string subject = "MAMADA COLOSSAL";
            //const string body = senha;

            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = _notificationMetadata.SmtpServer,
                Port = _notificationMetadata.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _notificationMetadata.Password)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Nova Senha", 
                Body = $"Sua nova senha para o sistema de Agendamento é: {senha}\nRedefina sua senha assim que possível!"
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch (System.Exception e)
                {

                    throw e.InnerException;
                }
                
            }

            return true;
        }
    }
}
