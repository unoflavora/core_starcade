namespace Agate.Starcade.Scripts.Runtime.Login
{
    public class BindData
    {
        public string IdToken { get; set; }

        public BindData(string idToken)
        {
            IdToken = idToken;
        }
    }
}