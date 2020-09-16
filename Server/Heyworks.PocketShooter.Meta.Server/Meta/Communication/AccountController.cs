using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Server;
using Heyworks.PocketShooter.Meta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    [Authorize]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IClusterClient orleansClient;

        public AccountController(IAccountService accountService, IClusterClient orleansClient)
        {
            this.accountService = accountService;
            this.orleansClient = orleansClient;
        }

        [AllowAnonymous]
        [HttpPost("register/device")]
        public async Task<IActionResult> RegisterWithDevice([FromBody]RegisterRequest registerRequest)
        {
            if (registerRequest == null || !IsRegisterRequestValid(registerRequest))
            {
                return Conflict(ResponseError.Create(ApiErrorCode.InvalidRegisterRequest, "Invalid register request data."));
            }

            var registrationProperties = new RegistrationProperties
            {
                DeviceId = registerRequest.DeviceId,
                BundleId = registerRequest.BundleId,
                ApplicationStore = registerRequest.ApplicationStore,
                ClientVersion = registerRequest.ClientVersion,
                Country = registerRequest.Country,
            };

            var result = await accountService.RegisterWithDeviceAsync(registrationProperties);

            switch (result.ReturnCode)
            {
                case RegistrationReturnCode.Success:
                    return Ok(ResponseOk.Create(
                        new RegisterResponseData
                        {
                            GameConfigVersion = result.GameConfigVersion,
                        }));
                case RegistrationReturnCode.FailDeviceIdExists:
                    return Conflict(ResponseError.Create(
                        ApiErrorCode.DeviceAlreadyExist, "The user with same device id is already registered"));
                default:
                    throw new NotImplementedException($"The registration return code {result.ReturnCode} is not supported for device registration");
            }
        }

        [AllowAnonymous]
        [HttpPost("login/device")]
        public async Task<IActionResult> LoginWithDevice([FromBody]LoginRequest loginRequest)
        {
            if (loginRequest == null || !IsDeviceLoginRequestValid(loginRequest))
            {
                return Conflict(ResponseError.Create(ApiErrorCode.InvalidLoginRequest, "Invalid device login request data."));
            }

            var loginProperties = new LoginProperties
            {
                DeviceId = loginRequest.DeviceId,
                BundleId = loginRequest.BundleId,
                ApplicationStore = loginRequest.ApplicationStore,
                ClientVersion = loginRequest.ClientVersion,
            };

            var result = await accountService.LoginWithDeviceAsync(loginProperties);

            switch (result.ReturnCode)
            {
                case LoginReturnCode.Success:
                    return Ok(ResponseOk.Create(new LoginResponseData
                    {
                        AuthToken = GenerateToken(result.User.Id),
                    }));
                case LoginReturnCode.FailUserNotFound:
                    return Conflict(ResponseError.Create(
                        ApiErrorCode.UserNotFound,
                        $"No user was found in DB with device id '{loginRequest.DeviceId}'"));
                default:
                    throw new NotImplementedException($"The login return code {result.ReturnCode} is not supported for device login");
            }
        }

        [AllowAnonymous]
        [HttpPost("login/google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody]GoogleLoginRequest loginRequest)
        {
            if (loginRequest == null || !IsGoogleLoginRequestValid(loginRequest))
            {
                return Conflict(ResponseError.Create(ApiErrorCode.InvalidLoginRequest, "Invalid Google login request data."));
            }

            var loginProperties = new GoogleLoginProperties
            {
                DeviceId = loginRequest.DeviceId,
                BundleId = loginRequest.BundleId,
                ApplicationStore = loginRequest.ApplicationStore,
                ClientVersion = loginRequest.ClientVersion,
                SocialId = loginRequest.SocialId,
                LoginToken = loginRequest.LoginToken,
                AccessToken = loginRequest.ClientAccessToken,
            };

            var result = await accountService.LoginWithGoogleAsync(loginProperties);

            switch (result.ReturnCode)
            {
                case LoginReturnCode.Success:
                    return Ok(ResponseOk.Create(new SocialLoginResponseData
                    {
                        AuthToken = GenerateToken(result.User.Id),
                        SocialConnection = result.User.SocialConnections.Google.GetState(),
                    }));
                case LoginReturnCode.FailUserNotFound:
                    return Conflict(ResponseError.Create(
                        ApiErrorCode.UserNotFound,
                        $"No user was found in DB with social id '{loginRequest.SocialId}'"));
                case LoginReturnCode.FailUserSocialCredentialsAreInvalid:
                    return Conflict(ResponseError.Create(
                        ApiErrorCode.InvalidSocialCredentials,
                        "The provided social credentials are expired or invalid."));
                default:
                    throw new NotImplementedException($"The login return code {result.ReturnCode} is not supported for Google login");
            }
        }

        [HttpPost("connect/google")]
        public async Task<IActionResult> ConnectGoogle([FromBody]GoogleConnectRequest connectRequest)
        {
            if (connectRequest == null || !IsGoogleConnectRequestValid(connectRequest))
            {
                return Conflict(ResponseError.Create(ApiErrorCode.InvalidSocialConnectRequest, "Invalid Google connect request data."));
            }

            var connectProperties = new GoogleConnectProperties
            {
                SocialId = connectRequest.SocialId,
                AccessToken = connectRequest.ClientAccessToken,
            };

            var result = await accountService.ConnectGoogleAsync(User.Identity.Name, connectProperties);

            switch (result.ReturnCode)
            {
                case SocialConnectReturnCode.Success:
                    return Ok(ResponseOk.Create(new SocialConnectResponseData
                    {
                        SocialConnection = result.SocialConnection.GetState(),
                    }));
                case SocialConnectReturnCode.FailUserSocialCredentialsAreInvalid:
                    return Conflict(ResponseError.Create(
                        ApiErrorCode.InvalidSocialCredentials,
                        "The provided social credentials are expired or invalid."));
                case SocialConnectReturnCode.FailSocialAccountConnectedToAnotherUser:
                    return Conflict(new SocialConnectResponseError(
                        ApiErrorCode.SocialAccountConnectedToAnotherUser,
                        $"The social account with id '{result.SocialId}' is connected to another user '{result.SocialUserNickname}'." +
                        $" The current user '{result.DeviceUserNickname}' can be logged out and switched with another account.")
                        {
                            DeviceUserNickname = result.DeviceUserNickname,
                            SocialUserNickname = result.SocialUserNickname,
                        });
                case SocialConnectReturnCode.FailUserConnectedToAnotherSocialAccount:
                    return Conflict(new SocialConnectResponseError(
                        ApiErrorCode.UserAlreadyConnected,
                        $"The social account '{result.SocialId}' is not connected to any existing army." +
                        $" The current device army '{result.DeviceUserNickname}' has already been connected to another social account.")
                        {
                            DeviceUserNickname = result.DeviceUserNickname,
                        });
                default:
                    throw new NotImplementedException($"The login return code {result.ReturnCode} is not supported for Google connect");
            }
        }

        private static string GenerateToken(string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var credentials = new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                "pocketshooter",
                "pocketshooter",
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool IsDeviceLoginRequestValid(LoginRequest loginRequest)
        {
            return
                !string.IsNullOrEmpty(loginRequest.DeviceId) &&
                !string.IsNullOrEmpty(loginRequest.BundleId) &&
                (loginRequest.ApplicationStore == ApplicationStoreName.Google || loginRequest.ApplicationStore == ApplicationStoreName.Apple) &&
                !string.IsNullOrEmpty(loginRequest.ClientVersion);
        }

        private static bool IsGoogleLoginRequestValid(GoogleLoginRequest loginRequest)
        {
            return
                IsDeviceLoginRequestValid(loginRequest) &&
                !string.IsNullOrEmpty(loginRequest.SocialId) &&
                !string.IsNullOrEmpty(loginRequest.ClientAccessToken);
        }

        private static bool IsRegisterRequestValid(RegisterRequest registerRequest)
        {
            return
                IsDeviceLoginRequestValid(registerRequest) &&
                !string.IsNullOrEmpty(registerRequest.Country);
        }

        private static bool IsGoogleConnectRequestValid(GoogleConnectRequest connectRequest)
        {
            return
                !string.IsNullOrEmpty(connectRequest.SocialId) &&
                !string.IsNullOrEmpty(connectRequest.ClientAccessToken);
        }
    }
}