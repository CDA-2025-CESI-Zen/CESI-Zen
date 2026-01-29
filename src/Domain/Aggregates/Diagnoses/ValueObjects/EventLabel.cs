using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Diagnoses.ValueObjects;
public readonly partial record struct EventLabel(string Value) {

    public static IResponse<EventLabel> TryCreate(string value) {
        
        value = value.Trim();
        
        if (string.IsNullOrWhiteSpace(value))
            return Response.Failure<EventLabel>(new InvariantException<EventLabel>("Un libellé d'évènement ne peut être vide !"));

        if (value.Length is < 4 or > 128)
            return Response.Failure<EventLabel>(new InvariantException<EventLabel>("Un libellé d'évènement doit contenir entre 4 et 128 caractères !"));
    
        return Response.Success(new EventLabel(value));
    }
}