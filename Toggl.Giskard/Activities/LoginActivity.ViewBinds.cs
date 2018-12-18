using System;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Giskard.Activities
{
    public sealed partial class LoginActivity
    {
        private Group pageOneGroup;
        private Group pageTwoGroup;
        private Group loggingInGroup;

        private ImageView backButton;

        private TextInputEditText secondScreenEmailTextField;
        private TextInputEditText passwordTextField;
        private TextInputEditText loginWithEmailTextField;

        private TextInputLayout secondScreenEmailLayout;

        private TextView loginWithEmailErrorLabel;
        private TextView secondScreenErrorLabel;
        private TextView needHelpContactUsButton;
        private TextView secondScreenEmailLabel;

        private ProgressBar activityIndicator;

        private Button loginWithEmailButton;
        private Button googleLoginButton;
        private TextView forgotPasswordButton;
        private Button loginButton;

        protected override void InitializeViews()
        {
            pageOneGroup = FindViewById<Group>(Resource.Id.PageOneGroup);
            pageTwoGroup = FindViewById<Group>(Resource.Id.PageTwoGroup);
            loggingInGroup = FindViewById<Group>(Resource.Id.LoggingInGroup);

            backButton = FindViewById<ImageView>(Resource.Id.BackButton);

            secondScreenEmailTextField = FindViewById<TextInputEditText>(Resource.Id.SecondScreenEmailTextField);
            passwordTextField = FindViewById<TextInputEditText>(Resource.Id.PasswordTextField);
            loginWithEmailTextField = FindViewById<TextInputEditText>(Resource.Id.LoginWithEmailTextField);

            secondScreenEmailLayout = FindViewById<TextInputLayout>(Resource.Id.SecondScreenEmailLayout);

            loginWithEmailErrorLabel = FindViewById<TextView>(Resource.Id.LoginWithEmailErrorLabel);
            secondScreenErrorLabel = FindViewById<TextView>(Resource.Id.SecondScreenErrorLabel);
            needHelpContactUsButton = FindViewById<TextView>(Resource.Id.NeedHelpContactUsButton);
            secondScreenEmailLabel = FindViewById<TextView>(Resource.Id.SecondScreenEmailLabel);

            activityIndicator = FindViewById<ProgressBar>(Resource.Id.ActivityIndicator);

            loginWithEmailButton = FindViewById<Button>(Resource.Id.LoginWithEmailButton);
            googleLoginButton = FindViewById<Button>(Resource.Id.GoogleLoginButton);
            forgotPasswordButton = FindViewById<TextView>(Resource.Id.ForgotPasswordButton);
            loginButton = FindViewById<Button>(Resource.Id.LoginButton);
        }
    }
}
