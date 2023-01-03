using Microsoft.AspNetCore.Mvc;

namespace Attendr.IdentityServer.Helpers
{
    public class VerificationHelper
    {
        public enum VerificationStatusCodes
        {
            InvalidUsername,
            AccountAlreadyActivated,
            InvalidVerificationCode,
            VerificationCodeExpired,
            Success,
        }

        public static ContentResult GenerateVerificationResponse(VerificationStatusCodes verificationStatus)
        {
            string html = string.Empty;
            switch (verificationStatus)
            {
                case (VerificationStatusCodes.InvalidUsername):
                    html = "<p>Invalid Username<p>";
                    break;
                case (VerificationStatusCodes.AccountAlreadyActivated):
                    html = "<p>Account is already activated!<p>";
                    break;
                case (VerificationStatusCodes.InvalidVerificationCode):
                    html = "<p>Invalid Verification Code!<p>";
                    break;
                case (VerificationStatusCodes.VerificationCodeExpired):
                    html = "<p>Verification code is expired please register again!<p>";
                    break;
                case (VerificationStatusCodes.Success):
                    html = "<p>Congrats! Your account is verified. Now you can continue using Attendr!<p>";
                    break;
            }
            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }
    }
}
