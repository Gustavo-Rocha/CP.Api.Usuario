namespace CP.Api.Usuario.EmailService
{
    public interface IEmailSender
    {
        bool SendEmail(string senha ,string reciever);
    }
}