using FluentResponse.Interfaces;
using FluentAssertions;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Content.ValueObjects;

namespace CesiZen.Domain.Tests.Aggregates.Content;
public class CategoryCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var title    = "Santé mentale au travail";
        var response = Category.TryCreate(title);

        // Assert
        response
            .Should().BeAssignableTo<ISuccess<Category>>()
            .Which.Value.Title.Value
            .Should().Be(title);
    }

    [Fact]
    public void TryCreate_WithInvalidTitle_ShouldFail() {

        // Act
        var response = Category.TryCreate("s    ");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<Title>>();
    }
}