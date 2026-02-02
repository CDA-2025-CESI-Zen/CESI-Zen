using System.Net.Mail;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Ports;
public interface IMailService {
    Task<IResponse> TrySendEmailAsync(MailAddress toEmail, string subject, string body);
}