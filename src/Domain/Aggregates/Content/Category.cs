using CesiZen.Domain.Aggregates.Content.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Content;
public record Category : AggregateRoot<Category> {

    #region PROPERTIES

        public Title             Title { get; protected init; }
        public ICollection<Page> Pages { get; protected init; } = [];

    #endregion
    #region CONSTRUCTORS
    
        public static IResponse<Category> TryCreate(string title) =>
            Title.TryCreate(title).OnSuccess(title => new Category { Title = title });

    #endregion
    #region METHODS

        public IResponse<Category> TryWithTitle(string value) =>
            Title.TryCreate(value).OnSuccess(title => this with { Title = title });

    #endregion
    
}