using System.Threading.Tasks;
namespace papya_api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync( string mailto, string mailSubject, string htmlBody, string textBody);
    }
}
