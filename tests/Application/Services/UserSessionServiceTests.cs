using CesiZen.Application.Core.ValueObjects;
using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Core.Exceptions;
using CesiZen.Infrastructure.Core.ValueObjects;
using CesiZen.Infrastructure.Services;
using FluentResponse;
using Moq;

namespace CesiZen.Application.Tests.Services;
public class UserSessionServiceTests {

    #region INITIALIZATION

        private readonly Mock<IRepository<User>>                   repository;
        private readonly Mock<IUserAuthService>                    authService;
        private readonly Mock<IRegistrationValidationCacheService> registrationCacheService;
        private readonly Mock<IPasswordResetCacheService>          passwordResetCacheService;
        private readonly Mock<IMailService>                        mailService;

        private readonly UserSessionService service;

        public UserSessionServiceTests() {
            this.repository                = new();
            this.authService               = new();
            this.registrationCacheService  = new();
            this.passwordResetCacheService = new();
            this.mailService               = new();

            this.service = new (
                repository.Object,
                authService.Object,
                registrationCacheService.Object,
                passwordResetCacheService.Object,
                mailService.Object
            );
        }

    #endregion
    #region TryAuthAsync

        [Fact]
        public async Task TryAuthAsync_WithValidCredentials_ShouldSucceed() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";

            var user = User.TryCreate(mailAddress, password).Unwrap();

            repository
                .Setup(r => r.TryGetAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(Response.Success(user));

            var token = "token";
            authService
                .Setup(a => a.TryGenerateToken(user))
                .Returns(Response.Success(token));

            // Act
            var response = await service.TryAuthAsync(mailAddress, password);

            // Assert
            response
                .Should().BeAssignableTo<ISuccess<UserSession>>()
                .Which.Value
                .Should().Be(new UserSession(token, user));
        }

        [Fact]
        public async Task TryAuthAsync_WithWrongPassword_ShouldFail() {
            
            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";

            var user = User.TryCreate(mailAddress, password).Unwrap();

            repository
                .Setup(r => r.TryGetAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(Response.Success(user));

            // Act
            var response = await service.TryAuthAsync(mailAddress, "efghEFGH5678");

            // Assert
            response.Should().BeAssignableTo<IFailure>();
        }

    #endregion
    #region TryRegisterAsync

        [Fact]
        public async Task TryRegisterAsync_WithValidPin_ShouldCreateUserAndReturnSession() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";
            var pin         = new Pin();

            registrationCacheService
                .Setup(c => c.TryGet(mailAddress))
                .Returns(Response.Success(pin));

            var user = User.TryCreate(mailAddress, password).Unwrap();

            repository
                .Setup(r => r.TryAddAsync(It.IsAny<User>()))
                .ReturnsAsync(Response.Success(user));

            var token = "token";
            authService
                .Setup(a => a.TryGenerateToken(user))
                .Returns(Response.Success(token));

            // Act
            var response = await service.TryRegisterAsync(mailAddress, password, pin);

            // Assert
            response
                .Should().BeAssignableTo<ISuccess<UserSession>>()
                .Which.Value
                .Should().Be(new UserSession(token, user));

            registrationCacheService.Verify(c => c.TryDelete(mailAddress), Times.Once);
        }

        [Fact]
        public async Task TryRegisterAsync_WithWrongPin_ShouldFail() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";
            var correctPin  = new Pin(1234);
            var wrongPin    = new Pin(5678);

            registrationCacheService
                .Setup(c => c.TryGet(mailAddress))
                .Returns(Response.Success(correctPin));

            // Act
            var response = await service.TryRegisterAsync(mailAddress, password, wrongPin);

            // Assert
            response
                .Should().BeAssignableTo<IFailure>()
                .Which.Exception
                .Should().BeAssignableTo<UnauthorizedAccessException>();

            repository.Verify(r => r.TryAddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task TryRegisterAsync_WithMissingPinInCache_ShouldFail() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";
            var pin         = new Pin();

            registrationCacheService
                .Setup(c => c.TryGet(mailAddress))
                .Returns(Response.Failure<Pin>(new KeyNotFoundException()));

            // Act
            var response = await service.TryRegisterAsync(mailAddress, password, pin);

            // Assert
            response.Should().BeAssignableTo<IFailure>();
        }

    #endregion
    #region TryResetPasswordAsync

        [Fact]
        public async Task TryResetPasswordAsync_WithValidPin_ShouldUpdatePasswordAndReturnSession() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var newPassword = "abcdABCD1234";
            var pin         = new Pin();

            var user = User.TryCreate(mailAddress, "efghEFGH5678").Unwrap();

            repository
                .Setup(r => r.TryGetAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(Response.Success(user));

            passwordResetCacheService
                .Setup(c => c.TryGet(user.Id))
                .Returns(Response.Success(pin));

            repository
                .Setup(r => r.TryUpdateAsync(user.Id, It.IsAny<Func<User, IResponse<User>>>()))
                .ReturnsAsync(Response.Success(user));

            var token = "token";
            authService
                .Setup(a => a.TryGenerateToken(user))
                .Returns(Response.Success(token));

            // Act
            var response = await service.TryResetPasswordAsync(mailAddress, newPassword, pin);

            // Assert
            response.Should().BeAssignableTo<ISuccess<UserSession>>();
            passwordResetCacheService.Verify(c => c.TryDelete(user.Id), Times.Once);
        }

        [Fact]
        public async Task TryResetPasswordAsync_WithWrongPin_ShouldFail() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var newPassword = "abcdABCD1234";
            var correctPin  = new Pin(1234);
            var wrongPin    = new Pin(5678);

            var user = User.TryCreate(mailAddress, "efghEFGH5678").Unwrap();

            repository
                .Setup(r => r.TryGetAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(Response.Success(user));

            passwordResetCacheService
                .Setup(c => c.TryGet(user.Id))
                .Returns(Response.Success(correctPin));

            // Act
            var response = await service.TryResetPasswordAsync(mailAddress, newPassword, wrongPin);

            // Assert
            response
                .Should().BeAssignableTo<IFailure>()
                .Which.Exception
                .Should().BeAssignableTo<UnauthorizedAccessException>();

            repository.Verify(r => r.TryUpdateAsync(It.IsAny<Id>(), It.IsAny<Func<User, IResponse<User>>>()), Times.Never);
        }

    #endregion
    #region TryUpdateAsync

        [Fact]
        public async Task TryUpdateAsync_WithCorrectPassword_ShouldApplyTransformAndReturnNewSession() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";

            var user = User.TryCreate(mailAddress, password).Unwrap();
            repository
                .Setup(r => r.TryGetAsync(user.Id))
                .ReturnsAsync(Response.Success(user));

            Func<User, IResponse<User>> transform = u => Response.Success(u);

            repository
                .Setup(r => r.TryUpdateAsync(user.Id, transform))
                .ReturnsAsync(Response.Success(user));

            var token = "token";
            authService
                .Setup(a => a.TryGenerateToken(user))
                .Returns(Response.Success(token));

            // Act
            var response = await service.TryUpdateAsync(user.Id, password, transform);

            // Assert
            response.Should().BeAssignableTo<ISuccess<UserSession>>();
        }

    #endregion
    #region TryRequestRegistrationPINAsync

        [Fact]
        public async Task TryRequestRegistrationPINAsync_WithNewMail_ShouldStorePinAndSendMail() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var pin         = new Pin();

            repository
                .Setup(r => r.AnyAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(false);

            registrationCacheService
                .Setup(c => c.TryAdd(mailAddress, It.IsAny<Pin>()))
                .Returns(Response.Success(pin));

            mailService
                .Setup(m => m.TrySendEmailAsync(
                    It.IsAny<UserMailAddress>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                ).ReturnsAsync(Response.Success());

            // Act
            var response = await service.TryRequestRegistrationPINAsync(mailAddress);

            // Assert
            response.Should().BeAssignableTo<ISuccess>();
            mailService.Verify(
                m => m.TrySendEmailAsync(
                    It.Is<UserMailAddress>(ma => ma.Address == mailAddress),
                    It.IsAny<string>(),
                    It.Is<string>(body => body.Contains(pin.ToString()))
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task TryRequestRegistrationPINAsync_WithExistingUser_ShouldFailWithEntityConflict() {

            // Arrange
            var mailAddress = "nom@domaine.fr";

            repository
                .Setup(r => r.AnyAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(true);

            // Act
            var response = await service.TryRequestRegistrationPINAsync(mailAddress);

            // Assert
            response
                .Should().BeAssignableTo<IFailure>()
                .Which.Exception
                .Should().BeAssignableTo<EntityConflictException>();
        }

    #endregion
    #region TryRequestPasswordResetPINAsync

        [Fact]
        public async Task TryRequestPasswordResetPINAsync_WithExistingUser_ShouldStorePinAndSendMail() {

            // Arrange
            var mailAddress = "nom@domaine.fr";
            var password    = "abcdABCD1234";
            var user        = User.TryCreate(mailAddress, password).Unwrap();
            var pin         = new Pin();

            repository
                .Setup(r => r.TryGetAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(Response.Success(user));

            passwordResetCacheService
                .Setup(c => c.TryAdd(user.Id, It.IsAny<Pin>()))
                .Returns(Response.Success(pin));

            mailService
                .Setup(m => m.TrySendEmailAsync(
                    user.MailAddress!,
                    It.IsAny<string>(),
                    It.IsAny<string>())
                ).ReturnsAsync(Response.Success());

            // Act
            var response = await service.TryRequestPasswordResetPINAsync(mailAddress);

            // Assert
            response.Should().BeAssignableTo<ISuccess>();
            mailService.Verify(
                m => m.TrySendEmailAsync(
                    user.MailAddress!,
                    It.IsAny<string>(),
                    It.Is<string>(body => body.Contains(pin.ToString()))
                ),
                Times.Once
            );
        }

    #endregion

}
