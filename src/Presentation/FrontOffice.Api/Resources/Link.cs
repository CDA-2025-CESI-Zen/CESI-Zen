namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public readonly record struct Link(HttpMethod Method, string Href) {
    public Link(HttpMethod Method = default, params object[] values) : this(Method, string.Join('/', values)) {}
}
