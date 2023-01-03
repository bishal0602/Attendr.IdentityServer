namespace Attendr.IdentityServer.Helpers
{
    /// <summary>
    /// Custom helper class library for verifying email
    /// </summary>
    public class EmailHelper
    {
        private static readonly List<string> _allowedEmails = new List<string>()
        {
            // appreoved emails
        };
        /// <summary>
        /// Checks whether the email is approved or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmailApproved(string email)
        {
            var emailCleaned = email.Trim().ToLower();
            return _allowedEmails.Any(e => e.Trim().ToLower() == emailCleaned);
        }
        /// <summary>
        /// Creates verification URL for user to actiavte account
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="username"></param>
        /// <param name="verificationCode"></param>
        /// <returns></returns>
        public static string CreateVerificationUrl(string baseUrl, string username, string verificationCode)
        {
            string verificationEndpoint = "/account/verify";
            string urlQueries = $"?user={username}&verify={verificationCode}";
            return baseUrl + verificationEndpoint + urlQueries;

        }
    }
}
