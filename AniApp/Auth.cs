using System.Text.Json;
using System.Web;

namespace AniApp
{
    public class AuthCredentials
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string TokenType { get; set; }
        public required int ExpiresIn { get; set; }
        public required string Scope { get; set; }
        public required long CreatedAt { get; set; }

        public bool IsTokenExpired()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() >= (CreatedAt + ExpiresIn);
        }
    }

    public static class Auth
    {
        const string clientId = "Vp5O_f_5ya9gmQSeM42G7SEBy0vBJ235CzeIUmjZn38";
        const string clientSecret = "CQeMVfUKF4u3bmG9ocHy81P_sQjtE66dM9SJsm0aNyM";
        const string redirectUri = "http://localhost:7777/callback";
        const string scope = "user_rates";

        public static string ConstructShikimoriAuthUrl() {
            return "https://shikimori.one/oauth/authorize" +
                $"?client_id={Uri.EscapeDataString(clientId)}" +
                $"&response_type=code" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&scope={Uri.EscapeDataString(scope)}";
        }

        public async static Task<AuthCredentials?> GetCredentials()
        {
            var credentialsRaw = await SecureStorage.GetAsync("credentials");

            if (credentialsRaw == null)
                return null;

            return JsonSerializer.Deserialize<AuthCredentials>(credentialsRaw);
        }

        public async static Task<AuthCredentials?> RetrieveCredentials(string url) {
            Uri redirectCodeUri = new Uri(url);

            string? code = HttpUtility.ParseQueryString(redirectCodeUri.Query).Get("code");
            if (code == null)
                return null;

            var formList = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                };
            var tokenContent = new FormUrlEncodedContent(formList);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "AniApp");

                var response = await client.PostAsync("https://shikimori.one/oauth/token", tokenContent);
                if (response.IsSuccessStatusCode)
                {
                    // TODO: Refactoring to function
                    var responseText = await response.Content.ReadAsStringAsync();
                    var credentialsDoc = JsonDocument.Parse(responseText);
                    var credentialsJson = credentialsDoc.RootElement;

                    var credentials = new AuthCredentials {
                        AccessToken = credentialsJson.GetProperty("access_token").GetString()!,
                        RefreshToken = credentialsJson.GetProperty("refresh_token").GetString()!,
                        Scope = credentialsJson.GetProperty("scope").GetString()!,
                        TokenType = credentialsJson.GetProperty("token_type").GetString()!,
                        ExpiresIn = credentialsJson.GetProperty("expires_in").GetInt32()!,
                        CreatedAt = credentialsJson.GetProperty("created_at").GetInt64()!
                    };

                    await SecureStorage.SetAsync("credentials", JsonSerializer.Serialize(credentials));

                    return credentials;
                }
            }

            return null;
        }

        public async static Task<AuthCredentials?> RefreshToken(string refreshToken)
        {
            var formList = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                };
            var tokenContent = new FormUrlEncodedContent(formList);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "AniApp");

                var response = await client.PostAsync("https://shikimori.one/oauth/token", tokenContent);
                if (response.IsSuccessStatusCode)
                {
                    // TODO: Refactoring to function
                    var responseText = await response.Content.ReadAsStringAsync();
                    var credentialsDoc = JsonDocument.Parse(responseText);
                    var credentialsJson = credentialsDoc.RootElement;

                    var credentials = new AuthCredentials
                    {
                        AccessToken = credentialsJson.GetProperty("access_token").GetString()!,
                        RefreshToken = credentialsJson.GetProperty("refresh_token").GetString()!,
                        Scope = credentialsJson.GetProperty("scope").GetString()!,
                        TokenType = credentialsJson.GetProperty("token_type").GetString()!,
                        ExpiresIn = credentialsJson.GetProperty("expires_in").GetInt32()!,
                        CreatedAt = credentialsJson.GetProperty("created_at").GetInt64()!
                    };

                    await SecureStorage.SetAsync("credentials", JsonSerializer.Serialize(credentials));

                    return credentials;
                }
            }

            return null;
        }
    }
}
