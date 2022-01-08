using Google.Apis.Auth.OAuth2;
using Jil;

namespace AmeliaRSVP.Core;

public class Config
{
    public static Config Instance { get; private set; }

    public string SpreadsheetUrl { get; private set; }
    public EmailData Email { get; private set; }
    public ServiceAccountCredential AccountCredential { get; private set; }

    public static void Init(string path)
    {
        if (Instance != null)
        {
            return;
        }

        var data = JSON.Deserialize<ConfigData>(File.ReadAllText(path));
        Instance = new Config
        {
            SpreadsheetUrl = data.SpreadsheetUrl,
            Email = data.EmailData,
            AccountCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(data.Email)
            {
                Scopes = new[] { "https://www.googleapis.com/auth/spreadsheets" },
                UseJwtAccessWithScopes = true
            }.FromPrivateKey(data.PrivateKey))
        };
    }

    private class ConfigData
    {
        public string Email { get; set; }
        public string PrivateKey { get; set; }
        public string SpreadsheetUrl { get; set; }
        public EmailData EmailData { get; set; }
    }

    public class EmailData
    {
        public string SendgridApiKey { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
    }
}
