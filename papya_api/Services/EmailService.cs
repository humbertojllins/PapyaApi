namespace papya_api.Services
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using System.Threading.Tasks;

    public class EmailService
    {
        public async Task EnviarEmail(string nomeRemetente, string emailRemetente, string nomeDestinario, string emailDestinario, string mensagem)
        {
            // instanciar classe de mensagem 'mimemessage' 
            var message = new MimeMessage();

            //from address
            message.From.Add(new MailboxAddress(nomeRemetente, emailRemetente));

            // subject
            message.Subject = "Mensagem Web API";

            //to address
            message.To.Add(new MailboxAddress(nomeDestinario, emailDestinario));

            //body
            message.Body = new TextPart("html")
            {
                Text = mensagem,
            };

            using (var client = new SmtpClient())
            {
                client.Connect("Host do seu servidor de email", 666, true); //666 é a porta do seu servidor de email

                client.Authenticate(emailRemetente, "SenhaDoEmail");

                client.Send(message);

                client.Disconnect(true);
            }
        }

    }
}
