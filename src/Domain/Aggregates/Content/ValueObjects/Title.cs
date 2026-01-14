using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Content.ValueObjects;
public readonly record struct Title(string Value) {
    public static IResponse<Title> TryCreate(string value) {
        
        value = value.Trim();
        
        if (string.IsNullOrWhiteSpace(value))
            return Response.Failure<Title>(new InvariantException<Title>("Un titre ne peut être vide !"));

        if (value.Length is < 4 or > 64)
            return Response.Failure<Title>(new InvariantException<Title>("Un titre doit contenir entre 4 et 64 caractères !"));
    
        return Response.Success(new Title(value));
    }
}