using Google.Apis.Auth.OAuth2;
using Jil;

namespace AmeliaRSVP.Core;

public class Config
{
    public static Config Instance { get; private set; }

    public string SpreadsheetUrl { get; private set; }
    public ServiceAccountCredential AccountCredential { get; private set; }
    public string Token { get; private set; }

    public static void Init(string path)
    {
        if (Instance != null)
        {
            return;
        }

        var data = JSON.Deserialize<ConfigData>(File.ReadAllText(path));
        Instance = new Config { SpreadsheetUrl = data.SpreadsheetUrl, Token = data.Token, AccountCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(data.Email) { Scopes = new[] { "https://www.googleapis.com/auth/spreadsheets" }, UseJwtAccessWithScopes = true }.FromPrivateKey(data.PrivateKey)) };
    }

    private class ConfigData
    {
        public string Email { get; set; }
        public string PrivateKey { get; set; }
        public string SpreadsheetUrl { get; set; }
        public string Token { get; set; }
    }
}
