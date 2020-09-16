using System;
using System.Net.Http;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Heyworks.PocketShooter.Meta.Services
{
    internal class GoogleCredentialsValidator : ISocialCredentialsValidator
    {
        private const string TokenInfoEndpointUrl = "https://oauth2.googleapis.com/tokeninfo";

        private readonly IOptions<GoogleSocialOptions> socialOptions;
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCredentialsValidator"/> class.
        /// </summary>
        /// <param name="socialOptions">The Google social options.</param>
        /// <param name="logger">The logger.</param>
        public GoogleCredentialsValidator(IOptions<GoogleSocialOptions> socialOptions, ILogger<GoogleCredentialsValidator> logger)
        {
            this.socialOptions = socialOptions.NotNull();
            this.logger = logger.NotNull();

            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Validates social credentials during connecting the existing account to some social account.
        /// Returns true if validation was successful.
        /// </summary>
        /// <param name="userToConnect">The user to connect social account.</param>
        /// <param name="connectProperties">The connection properties.</param>
        public Task<bool> ValidateConnectAsync(User userToConnect, SocialConnectProperties connectProperties)
        {
            var googleProperties = (GoogleConnectProperties)connectProperties;

            return ValidateSocialCredentials(googleProperties.SocialId, googleProperties.AccessToken);
        }

        /// <summary>
        /// Validates social credentials during connecting the existing account to some social account.
        /// </summary>
        /// <param name="userToLogin">The user to login.</param>
        /// <param name="loginProperties">The login properties.</param>
        public Task<bool> ValidateLoginAsync(User userToLogin, SocialLoginProperties loginProperties)
        {
            var googleProperties = (GoogleLoginProperties)loginProperties;

            return ValidateSocialCredentials(googleProperties.SocialId, googleProperties.AccessToken);
        }

        private async Task<bool> ValidateSocialCredentials(string socialId, string idToken)
        {
            string uri = string.Format("{0}?id_token={1}", TokenInfoEndpointUrl, idToken);

            try
            {
                var responseMessage = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string contentData = await responseMessage.Content.ReadAsStringAsync();
                    var tokenInfo = JObject.Parse(contentData);

                    string tokenUserId = tokenInfo["sub"].Value<string>();
                    string tokenAudience = tokenInfo["aud"].Value<string>();
                    string tokenIssuedTo = tokenInfo["azp"].Value<string>();

                    logger.LogInformation(
                        "Received the following Google auth claims for verification. " +
                        "user Id: {tokenUserId}, backend client id: {tokenAudience}, mobile client id: {tokenIssuedTo}.",
                        tokenUserId,
                        tokenAudience,
                        tokenIssuedTo);

                    return
                        socialOptions.Value.BackendClientId == tokenAudience &&
                        socialOptions.Value.MobileClientId == tokenIssuedTo;
                }
                else
                {
                    var errorContentString = await responseMessage.Content.ReadAsStringAsync();

                    logger.LogWarning(
                        "Error during token verification request. Status code: {statusCode}, Reason: {reasonPhrase}, Error content: {errorContent}",
                        responseMessage.StatusCode,
                        responseMessage.ReasonPhrase,
                        errorContentString);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Exception during token verification request. Id token: {idToken}", idToken);
            }

            return false;
        }
    }
}
