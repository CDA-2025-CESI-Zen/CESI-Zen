using System.Net.Mail;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;
public class UserMailAddress(string value) : MailAddress(value) {
    public static IResponse<UserMailAddress> TryCreate(string value) {

        UserMailAddress mailAdress;
        
        try { mailAdress = new (value); }
        catch { return Response.Failure<UserMailAddress>(new InvariantException<UserMailAddress>("Une adresse électronique doit avoir le format 'nom@domaine' !")); }
        
        return Response.Success(mailAdress);
    }
}