using System.Threading.Tasks;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Popups;
using UniRx.Async;

namespace Heyworks.PocketShooter.Authorization
{
    public class UIAccessTokenProvider : IAccessTokenProvider
    {
        private readonly AuthorizationController authorizationController;

        private bool gdprAccepted;

        public UIAccessTokenProvider(AuthorizationController authorizationController)
        {
            this.authorizationController = authorizationController;
        }

        public async Task<string> GetAccessToken()
        {
            var loginResponse = await authorizationController.Login();

            if (loginResponse.IsError)
            {
                if (loginResponse.Error.Code == ApiErrorCode.UserNotFound)
                {
                    await Register();
                    loginResponse = await authorizationController.Login();
                }
                else
                {
                    UnitySynchronizationContext.Current.Post(_ => AnalyticsManager.Instance.SendLogin(false), null);
                }
            }

            while (loginResponse.IsError)
            {
                if (loginResponse.Error.Code == ApiErrorCode.InvalidClientVersion)
                {
                    UnitySynchronizationContext.Current.Post(
                        _ =>
                        {
                            ProgressHUD.Instance.Hide();
                            ScreensController.Instance.ShowPopup<VersionInfoPopup>();
                        },
                        null);
                    await UniTask.WaitWhile(() => true);
                }
                else
                {
//                    var retry = new UniTaskCompletionSource();
//                    var boxParams = new MessageBoxParams(){Message = "Login error, retry?", Button1Text = "Retry",Button1Action = () => retry.TrySetResult(),};
//                    EasyMessageBox.Show(boxParams);

//                    await retry.Task;
                }

                loginResponse = await authorizationController.Login();
            }

            UnitySynchronizationContext.Current.Post(_ => AnalyticsManager.Instance.SendLogin(true), null);

            return loginResponse.Ok.Data.AuthToken;
        }

        private async UniTask Register()
        {
            if (!gdprAccepted)
            {
                await AcceptGDPR();
            }

            ResponseOption<RegisterResponseData> registerResponse = await authorizationController.RegisterWithDevice();

            while (registerResponse.IsError)
            {
                if (registerResponse.Error.Code == ApiErrorCode.DeviceAlreadyExist)
                {
                    return;
                }

//                var retryRegistration = new UniTaskCompletionSource();
//                var boxParams1 = new MessageBoxParams(){Message = "Register error, retry?", Button1Text = "Retry",Button1Action = () => retryRegistration.TrySetResult(),};
//                EasyMessageBox.Show(boxParams1);

//                await retryRegistration.Task;

                registerResponse = await authorizationController.RegisterWithDevice();
            }
        }

        private async UniTask AcceptGDPR()
        {
            var acceptGdpr = new UniTaskCompletionSource();

            UnitySynchronizationContext.Current.Post(
                async _ =>
                {
                    void TermsPopupOkHandler() => acceptGdpr.TrySetResult();
#if UNITY_ANDROID
                    var termsPopup = ScreensController.Instance.ShowPopup<AndroidTermsPopup>();
#else
                    var termsPopup = ScreensController.Instance.ShowPopup<IosTermsPopup>();
#endif
                    termsPopup.Successed += TermsPopupOkHandler;
                    await acceptGdpr.Task;
                    termsPopup.Successed -= TermsPopupOkHandler;
                },
                null);

            await acceptGdpr.Task;
            gdprAccepted = true;
        }
    }
}