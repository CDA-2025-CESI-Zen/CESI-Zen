using System.Text.Json.Serialization;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod {
    GET = default,
    POST,
    PUT,
    PATCH,
    DELETE
}