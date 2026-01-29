using System.Net.Mail;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;
public class AdminMailAddress(string value) : MailAddress(value) {
    public static IResponse<AdminMailAddress> TryCreate(string value) {

        AdminMailAddress mailAdress;
        
        try { mailAdress = new (value); }
        catch { return Response.Failure<AdminMailAddress>(new InvariantException<AdminMailAddress>("L'adresse électronique d'un administrateur doit avoir le format 'nom@domaine' !")); }
        
        if (mailAdress.Host != "cesizen.fr") return Response.Failure<AdminMailAddress>(new InvariantException<AdminMailAddress>("L'adresse électronique d'un administrateur doit être interne à cesizen.fr !"));
        else                                 return Response.Success(mailAdress);
    }
}