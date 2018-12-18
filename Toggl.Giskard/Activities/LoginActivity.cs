using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Extensions.Reactive;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme.WhiteStatusBar",
              ScreenOrientation = ScreenOrientation.Portrait,
              WindowSoftInputMode = SoftInput.AdjustResize,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class LoginActivity : ReactiveActivity<LoginViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.ChangeStatusBarColor(Color.White, true);
            SetContentView(Resource.Layout.LoginActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out);

            InitializeViews();

            setupBindings();

            loginWithEmailTextField.SetFocus();
        }

        private void setupBindings()
        {
            backButton.Rx()
                .BindAction(ViewModel.Back)
                .DisposedBy(DisposeBag);

            loginWithEmailTextField.Rx().Text()
                .Subscribe(ViewModel.EmailRelay.Accept)
                .DisposedBy(DisposeBag);

            loginWithEmailTextField.Rx().EditorActionSent()
                .SelectUnit()
                .Subscribe(ViewModel.LoginWithEmail.Inputs)
                .DisposedBy(DisposeBag);

            loginWithEmailButton.Rx()
                .BindAction(ViewModel.LoginWithEmail)
                .DisposedBy(DisposeBag);

            googleLoginButton.Rx()
                .BindAction(ViewModel.LoginWithGoogle)
                .DisposedBy(DisposeBag);

            ViewModel.LoginWithEmail.Errors
                .Select(e => e.Message)
                .Subscribe(loginWithEmailErrorLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.LoginWithGoogle.Errors
                .Select(e => e.Message)
                .Subscribe(loginWithEmailErrorLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.LoginWithEmail.Errors
                .SelectValue(true)
                .Subscribe(loginWithEmailErrorLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.ClearEmailScreenError
                .SelectValue(string.Empty)
                .Subscribe(loginWithEmailErrorLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            // second screen
            forgotPasswordButton.Rx()
                .BindAction(ViewModel.ForgotPassword)
                .DisposedBy(DisposeBag);

            loginButton.Rx()
                .BindAction(ViewModel.Login)
                .DisposedBy(DisposeBag);

            passwordTextField.Rx().EditorActionSent()
                .SelectUnit()
                .Subscribe(ViewModel.Login.Inputs)
                .DisposedBy(DisposeBag);

            secondScreenEmailTextField.Rx().EditorActionSent()
                .Subscribe(_ => passwordTextField.SetFocus())
                .DisposedBy(DisposeBag);

            secondScreenEmailTextField.Rx().Text()
                .Subscribe(ViewModel.EmailRelay.Accept)
                .DisposedBy(DisposeBag);

            needHelpContactUsButton.Rx()
                .BindAction(ViewModel.ContactUs)
                .DisposedBy(DisposeBag);

            ViewModel.EmailRelay
                .Subscribe(secondScreenEmailTextField.Rx().TextObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.EmailRelay
                .Subscribe(secondScreenEmailLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsEmailFieldEdittable
                .Subscribe(secondScreenEmailLayout.Rx().IsVisible(false))
                .DisposedBy(DisposeBag);

            ViewModel.IsEmailFieldEdittable
                .Invert()
                .Subscribe(loggingInGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            passwordTextField.Rx().Text()
                .Subscribe(ViewModel.PasswordRelay.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.SuggestContactSupport
                .Subscribe(needHelpContactUsButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.ClearPasswordScreenError
                .SelectValue(string.Empty)
                .Subscribe(secondScreenErrorLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Login.Errors
                .Select(e => e.Message)
                .Subscribe(secondScreenErrorLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Login.Errors
                .SelectValue(true)
                .Subscribe(secondScreenErrorLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PasswordRelay
                .Subscribe(passwordTextField.Rx().TextObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.IsLoggingIn
                .Subscribe(activityIndicator.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoggingIn
                .Select(loginButtonTitle)
                .Subscribe(loginButton.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsInSecondScreen
                .Invert()
                .Do(setFocus(loginWithEmailTextField))
                .Subscribe(pageOneGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsInSecondScreen
                .Invert()
                .Subscribe(loginWithEmailTextField.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsInSecondScreen
                .Do(setFocus(passwordTextField))
                .Subscribe(pageTwoGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsInSecondScreen
               .Subscribe(loggingInGroup.Rx().IsVisible())
               .DisposedBy(DisposeBag);
        }

        private Action<bool> setFocus(EditText editText)
        {
            return shouldFocus =>
            {
                if (shouldFocus)
                    editText.SetFocus();
            };
        }

        private string loginButtonTitle(bool isLoading)
            => isLoading ? "" : GetString(Resource.String.Login);
    }
}
