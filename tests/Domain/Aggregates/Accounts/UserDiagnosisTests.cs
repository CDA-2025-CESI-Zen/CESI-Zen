using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class UserDiagnosisTests {

    [Fact]
    public void WithNewDiagnosisResult_FirstTime_ShouldSetBothValues() {
        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap();

        // Act
        var updated = user.WithNewDiagnosisResult(1);

        // Assert
        updated.FirstDiagnosisResult!.Value.Score.Should().Be(1);
        updated.LastDiagnosisResult!.Value.Score.Should().Be(1);
    }

    [Fact]
    public void WithNewDiagnosisResult_Subsequent_ShouldPreserveFirst() {

        // Arrange
        var user = User.TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap()
            .WithNewDiagnosisResult(1);

        // Act
        var updated = user.WithNewDiagnosisResult(2);

        // Assert
        updated.FirstDiagnosisResult!.Value.Score.Should().Be(1);
        updated.LastDiagnosisResult!.Value.Score.Should().Be(2);
    }
}
