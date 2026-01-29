using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CesiZen.Application.Tests.Services;
public class UserAnonymizationServiceTests {

    #region PROPERTIES

        private readonly Mock<IServiceProvider> serviceProvider;
        private readonly Mock<IServiceScopeFactory> serviceScopeFactory;
        private readonly Mock<IServiceScope> serviceScope;
        private readonly Mock<IRepository<User>> userRepository;
        private readonly Mock<ILogger<UserAnonymizationService>> logger;

        private readonly UserAnonymizationService service;

        public UserAnonymizationServiceTests() {
            this.serviceProvider     = new();
            this.serviceScopeFactory = new();
            this.serviceScope        = new();
            this.userRepository      = new();
            this.logger              = new();

            this.serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(this.serviceScopeFactory.Object);

            this.serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(this.serviceScope.Object);

            this.serviceScope
                .SetupGet(x => x.ServiceProvider)
                .Returns(Mock.Of<IServiceProvider>(x =>
                    x.GetService(typeof(IRepository<User>)) == this.userRepository.Object
                ));

            this.service = new(
                this.serviceProvider.Object,
                this.logger.Object
            );
        }

    #endregion
    #region HandleAsync

    [Fact]
        public async Task HandleAsync_Should_Not_Start_Anonymization_When_Less_Than_35_Months_Inactive_And_Not_Started() {

            // Arrange
            var now = DateTime.UtcNow;
            var lastActivity = now.AddMonths(-34);
            var mailAddress = "nom@domaine.fr";
            var password = "abcdABCD1234";

            var user = User.TryCreate(mailAddress, password).Unwrap() with {
                LastActivity = lastActivity,
            };

            var updatedUser = user with { AnonymizationProcessStartedAt = now };

            var users = new List<User> { user };
            userRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync([]);

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(It.IsAny<Func<User, bool>>()),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    user.Id,
                    It.IsAny<Func<User, IResponse<User>>>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Start_Anonymization_When_35_Months_Inactive_And_Not_Started() {

            // Arrange
            var now = DateTime.UtcNow;
            var lastActivity = now.AddMonths(-35).AddDays(-1);
            var mailAddress = "nom@domaine.fr";
            var password = "abcdABCD1234";

            var user = User.TryCreate(mailAddress, password).Unwrap() with {
                LastActivity = lastActivity,
            };

            var updatedUser = user with { AnonymizationProcessStartedAt = now };

            var users = new List<User> { user };
            userRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);

            userRepository
                .Setup(x => x.TryUpdateAsync(user.Id, It.IsAny<Func<User, IResponse<User>>>()))
                .ReturnsAsync(Response.Success(updatedUser));

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(It.IsAny<Func<User, bool>>()),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    user.Id,
                    It.IsAny<Func<User, IResponse<User>>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Anonymize_When_Started_And_One_Month_Passed() {

            // Arrange
            var now = new DateTime(2026, 1, 24, 12, 0, 0, DateTimeKind.Utc);
            var lastActivity = now.AddMonths(-35).AddDays(-1);
            var startedAt = now.AddMonths(-1).AddDays(-1);

            var mailAddress = "nom@domaine.fr";
            var password = "abcdABCD1234";
            var user = User.TryCreate(mailAddress, password).Unwrap() with {
                LastActivity = lastActivity
            };

            var updatedUser = user.AsAnonymized();

            var users = new List<User> { user };

            userRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(users);

            userRepository
                .Setup(x => x.TryUpdateAsync(user.Id, It.IsAny<Func<User, User>>()))
                .ReturnsAsync(Response.Success(updatedUser));

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(It.IsAny<Func<User, bool>>()),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    It.IsAny<Id>(),
                    It.IsAny<Func<User, IResponse<User>>>()),
                Times.Once
            );
        }

    #endregion

}
