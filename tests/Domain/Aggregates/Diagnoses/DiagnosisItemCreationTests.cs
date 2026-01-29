using FluentResponse.Interfaces;
using FluentAssertions;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Domain.Aggregates.Diagnoses.ValueObjects;

namespace CesiZen.Domain.Tests.Aggregates.Diagnoses;
public class DiagnosisItemCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var eventLabel = "Décès du conjoint ou d'un enfant";
        var score      = 100;
        var response   = DiagnosisItem.TryCreate(eventLabel, score);

        // Assert
        var assertion = response.Should().BeAssignableTo<ISuccess<DiagnosisItem>>().Which.Value;

        assertion.EventLabel.Value.Should().Be(eventLabel);
        assertion.Score.Should().Be(score);
    }

    [Fact]
    public void TryCreate_WithInvalidTitle_ShouldFail() {

        // Act
        var response = DiagnosisItem.TryCreate("""
            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
            Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        """, 100);

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<EventLabel>>();
    }
}