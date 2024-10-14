
//using Amazon;
//using Amazon.SimpleEmail;
//using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
//using System.Net;
//using System.Net.Mail;
using System.Threading.Tasks;
//using MailKit.Net.Smtp;
//using MimeKit;
//using MailKit.Security;

namespace papya_api.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public EmailSettings EmailSettings { get; }

        public AuthMessageSender(IOptions<EmailSettings> emailSettings)
        {
            EmailSettings = emailSettings.Value;
        }
        /// <summary>
        /// Enviars the email aws ses.
        /// </summary>
        /// <returns>The email aws ses.</returns>
        /// <param name="mailto">
        /// Replace recipient@example.com with a "To" address. If your account
        /// is still in the sandbox, this address must be verified.
        /// </param>
        /// <param name="mailSubject">The subject line for the email.</param>
        /// <param name="htmlBody">The HTML body of the email.</param>
        /// <param name="textBody">The email body for recipients with non-HTML email clients.</param>
        public async Task SendEmailAsync(string mailto, string mailSubject, string htmlBody, string textBody)
        {
            try
            {
                await EnviarEmailKit("Papya", EmailSettings.UsernameEmail, "Humberto Lins", mailto, htmlBody);
                //await Execute(mailto, mailSubject, htmlBody, htmlBody);
                //await EnviarEmailAWS_SES(EmailSettings.FromEmail, mailto, mailSubject, htmlBody, textBody);
                //return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EnviarEmailKit(string nomeRemetente, string emailRemetente, string subject, string emailDestinario, string mensagem)
        {
            // instanciar classe de mensagem 'mimemessage' 
            var message = new MimeMessage();

            //from address
            message.From.Add(new MailboxAddress(nomeRemetente, emailRemetente));

            // subject
            message.Subject = subject;

            //to address
            message.To.Add(new MailboxAddress(emailDestinario, emailDestinario));

            //body
            message.Body = new TextPart("html")
            {
                Text = mensagem,
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(EmailSettings.PrimaryDomain, EmailSettings.PrimaryPort, true);

                client.Authenticate(EmailSettings.UsernameEmail, EmailSettings.UsernamePassword);

                client.Send(message);

                client.Disconnect(true);
            }
        }
        #region "Funções arquivadas"
        
        //public async Task Execute(string email, string mailSubject, string mailBody, string textBody)
        //{
        //    try
        //    {
        //        string toEmail = string.IsNullOrEmpty(email) ? EmailSettings.ToEmail : email;

        //        MailMessage mail = new MailMessage()
        //        {
        //            From = new MailAddress(EmailSettings.UsernameEmail, "Tingle Bar")
        //        };

        //        mail.To.Add(new MailAddress(toEmail));
        //        mail.CC.Add(new MailAddress(EmailSettings.CcEmail));

        //        mail.Subject = "Papya - " + mailSubject;
        //        mail.Body = mailBody;
        //        mail.IsBodyHtml = true;
        //        mail.Priority = MailPriority.High;


        //        //outras opções
        //        //mail.Attachments.Add(new Attachment(arquivo));
        //        //

        //        using (SmtpClient smtp = new SmtpClient(EmailSettings.PrimaryDomain, EmailSettings.PrimaryPort))
        //        {
        //            smtp.EnableSsl = true;
        //            smtp.UseDefaultCredentials = false;
        //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            smtp.Credentials = new NetworkCredential(EmailSettings.UsernameEmail, EmailSettings.UsernamePassword);
        //            await smtp.SendMailAsync(mail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task EnviarEmailAWS_SES(string mailfrom, string mailto, string mailSubject,string htmlBody, string textBody)
        //{
        //    // Replace USWest2 with the AWS Region you're using for Amazon SES.
        //    // Acceptable values are EUWest1, USEast1, and USWest2.
        //    using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USWest2))
        //    {
        //        var sendRequest = new SendEmailRequest
        //        {
        //            Source = mailfrom,
        //            Destination = new Destination
        //            {
        //                ToAddresses =
        //                new List<string> { mailto }
        //            },
        //            Message = new Message
        //            {
        //                Subject = new Content(mailSubject),
        //                Body = new Body
        //                {
        //                    Html = new Content
        //                    {
        //                        Charset = "UTF-8",
        //                        Data = htmlBody
        //                    },
        //                    Text = new Content
        //                    {
        //                        Charset = "UTF-8",
        //                        Data = textBody
        //                    }
        //                }
        //            },
        //            // If you are not using a configuration set, comment
        //            // or remove the following line 
        //            //ConfigurationSetName = "ConfigSet"

        //        };
        //        try
        //        {
        //            await client.SendEmailAsync(sendRequest);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //            //Console.WriteLine("The email was not sent.");
        //            //Console.WriteLine("Error message: " + ex.Message);

        //        }
        //    }
        //}
        #endregion

    }
}
