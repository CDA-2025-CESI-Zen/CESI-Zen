using System.Text.Json.Serialization;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Presentation.FrontOffice.Api.Controllers;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class CategoryResource(Category category) : IResource<CategoryResource, Category> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = category.Id;

        public string Title { get; } = category.Title.Value;

        public PageLinks Links { get; } = new(
            Self : new (
                method : HttpMethod.GET,
                title  : category.Title.Value,
                CategoryController.ROUTE, category.Id
            ),
            Pages : category.Pages.Select(page => new AnnotatedLink(
                method : HttpMethod.GET,
                title  : page.Title.Value,
                PageController.ROUTE, page.Id
            ))
        );

        public readonly record struct PageLinks(
            AnnotatedLink              Self,
            IEnumerable<AnnotatedLink> Pages
        );

    #endregion
    #region METHODS

        public static implicit operator CategoryResource(Category ressource) => new (ressource);

        public static CategoryResource From(Category ressource) => ressource;
        public static IEnumerable<CategoryResource> From(IEnumerable<Category> ressources) => ressources.Select(From);

    #endregion

}