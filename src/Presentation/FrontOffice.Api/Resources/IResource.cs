namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public interface IResource<T, TFrom> {
    static abstract T From(TFrom from);
    static abstract IEnumerable<T> From(IEnumerable<TFrom> from);
}