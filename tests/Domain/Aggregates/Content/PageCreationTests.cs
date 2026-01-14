using FluentResponse.Interfaces;
using FluentAssertions;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Content.ValueObjects;
using FluentResponse;

namespace CesiZen.Domain.Tests.Aggregates.Content;
public class PageCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var title    = "Liste des praticiens sur Lille (59)";
        var content  = "...";
        var category = Category.TryCreate("Santé mentale au travail").Unwrap();
        var response = Page.TryCreate(title, category, content);

        // Assert
        var assertion = response.Should().BeAssignableTo<ISuccess<Page>>().Which.Value;

        assertion.Title.Value.Should().Be(title);
        assertion.Category.Should().Be(category);
        assertion.Content.Should().Be(content);
    }

    [Fact]
    public void TryCreate_WithInvalidTitle_ShouldFail() {

        // Act
        var category = Category.TryCreate("Santé mentale au travail").Unwrap();
        var response = Page.TryCreate("""
            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
            Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        """, category, "...");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<Title>>();
    }
}