using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KingdeeClient;

internal sealed class AuthHttpMessageHandle : DelegatingHandler
{
    private readonly static ConcurrentDictionary<string, DateTime> tokenDictionary = new ConcurrentDictionary<string, DateTime>();
    private const string tokenKey = "token";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null)
        {
            if (!tokenDictionary.ContainsKey(tokenKey))
            {
                var token = await GetToken(request, cancellationToken);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    tokenDictionary[tokenKey] = DateTime.UtcNow.AddMinutes(30).AddSeconds(-10); // 假设token有效期为30分钟
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            else
            {
                if (tokenDictionary.TryGetValue(tokenKey, out DateTime dt) && dt > DateTime.UtcNow)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenKey);
                }
                else
                {
                    var token = await GetToken(request, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        tokenDictionary[tokenKey] = DateTime.UtcNow.AddMinutes(30).AddSeconds(-10); // 假设token有效期为30分钟
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetToken(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var host = request.RequestUri?.Authority;
        var body = new
        {
            UserName = "admin",
            Password = "admin",
        };

        using var tokenRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"http:{host}/api/accout/login"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        using var tokenResponse = await base.SendAsync(tokenRequest, cancellationToken);
        var responseResult = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TokenResult>(responseResult);
        if (result == null || string.IsNullOrWhiteSpace(result.token))
        {
            throw new ArgumentNullException("token is empty");
        }
        else
        {
            return result.token;
        }
    }
}


internal sealed record TokenResult(string token);
