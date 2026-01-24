using System.Text.Json.Serialization;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Presentation.FrontOffice.Api.Controllers;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class PageResource(Page page) : IResource<PageResource, Page> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = page.Id;

        public string Title   { get; } = page.Title.Value;
        public string Content { get; } = page.Content;

        public PageLinks Links { get; } = new(
            Self     : new (method : HttpMethod.GET, title : page.Title.Value, PageController.ROUTE, page.Id),
            Category : new (method : HttpMethod.GET, title : page.Category.Title.Value, CategoryController.ROUTE, page.Category.Id)
        );

        public readonly record struct PageLinks(
            AnnotatedLink Self,
            AnnotatedLink Category
        );

    #endregion
    #region METHODS

        public static implicit operator PageResource(Page ressource) => new (ressource);

        public static PageResource From(Page ressource) => ressource;
        public static IEnumerable<PageResource> From(IEnumerable<Page> ressources) => ressources.Select(From);

    #endregion

}