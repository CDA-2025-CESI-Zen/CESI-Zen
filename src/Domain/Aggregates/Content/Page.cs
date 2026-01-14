using CesiZen.Domain.Aggregates.Content.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Content;
public record Page : AggregateRoot<Page> {

    #region PROPERTIES

        public Title    Title    { get; protected init; }
        public Category Category { get; protected init; } = null!;
        public string   Content  { get; protected init; } = null!;

    #endregion
    #region CONSTRUCTORS

        public static IResponse<Page> TryCreate(string title, Category category, string content) =>
            Title.TryCreate(title).OnSuccess(title => new Page { Title = title, Category = category, Content = content });

    #endregion
    #region METHODS

        public IResponse<Page> TryWithTitle(string value) =>
            Title.TryCreate(value).OnSuccess(title => this with { Title = title });

        public Page WithContent(string value) =>
            this with { Content = value };

        public Page WithCategory(Category value) =>
            this with { Category = value };

    #endregion
    
}