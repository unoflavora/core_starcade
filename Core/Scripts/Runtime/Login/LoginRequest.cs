namespace Agate.Starcade.Scripts.Runtime.Login
{
    public class LoginRequest
    {
        public string IdToken { get; set; }

        public LoginRequest(string idToken)
        {
            IdToken = idToken;
        }
    }
}