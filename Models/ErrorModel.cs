namespace Attendr.IdentityServer.Models
{
    public class ErrorModel
    {
        public string Error { get; set; }

        public ErrorModel(string error)
        {
            Error = error;
        }
    }
}
