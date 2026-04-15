using System.Text;

namespace KingdeeClient;

internal class ApiHeaderContainer
{
    private readonly Dictionary<string, string> headers;

    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string XApiAuthVersion { get; set; }

    public string XApiClientID { get; set; }

    public string XApiSec { get; set; }

    public string XApiSignHeaders { get; set; }

    public string XKDAppSec { get; set; }

    public string XKDAppKey
    {
        get
        {
            return GetValue("X-Kd-Appkey");
        }
        set
        {
            SetValue("X-Kd-Appkey", value);
        }
    }

    public string XKDAcctID
    {
        get
        {
            return GetValue("X-KD-AcctID");
        }
        set
        {
            SetValue("X-KD-AcctID", value);
        }
    }

    public string XKDLCID
    {
        get
        {
            return GetValue("X-KD-LCID");
        }
        set
        {
            SetValue("X-KD-LCID", value);
        }
    }

    public string XKDUserName
    {
        get
        {
            return GetValue("X-KD-UserName");
        }
        set
        {
            SetValue("X-KD-UserName", value);
        }
    }

    public string XKDOrgNum
    {
        get
        {
            return GetValue("X-KD-OrgNum");
        }
        set
        {
            SetValue("X-KD-OrgNum", value);
        }
    }

    public string SID { get; set; }

    private string GetValue(string key)
    {
        if (!headers.ContainsKey(key))
        {
            return null;
        }
        return headers[key];
    }

    private ApiHeaderContainer SetValue(string key, string value)
    {
        if (headers.ContainsKey(key))
        {
            headers[key] = value;
        }
        else
        {
            headers.Add(key, value);
        }
        return this;
    }

    public ApiHeaderContainer()
    {
        headers = [];
        XApiAuthVersion = "V2.0";
        XApiSignHeaders = "x-api-timestamp,x-api-nonce";
    }

    internal Dictionary<string, string> GetHeaders(Uri uri)
    {
        Dictionary<string, string> dictionary = [];
        string text = GetTimestamp().ToString();
        string nonce = GetNonce();
        if (!string.IsNullOrWhiteSpace(XApiClientID))
        {
            dictionary.Add("X-Api-ClientID", XApiClientID);
            dictionary.Add("X-Api-Auth-Version", "2.0");
            dictionary.Add("x-api-signheaders", XApiSignHeaders);
            dictionary.Add("x-api-nonce", nonce);
            dictionary.Add("x-api-timestamp", text);
            string message = string.Format("{0}\n{1}\n\n{2}\n{3}\n", "POST", EnDecode.UrlEncodeWithUpperCode(uri.PathAndQuery, Encoding.ASCII), "x-api-nonce:" + nonce, "x-api-timestamp:" + text);
            dictionary.Add("X-Api-Signature", EnDecode.HmacSHA256(message, XApiSec, Encoding.ASCII, isHex: true));
        }
        dictionary.Add("X-Kd-Appkey", XKDAppKey);
        string text2 = $"{XKDAcctID},{XKDUserName},{XKDLCID},{XKDOrgNum}";
        dictionary.Add("X-Kd-Appdata", Convert.ToBase64String(Encoding.UTF8.GetBytes(text2)));
        dictionary.Add("X-Kd-Signature", EnDecode.HmacSHA256(XKDAppKey + text2, XKDAppSec, Encoding.UTF8, isHex: true));
        if (!string.IsNullOrWhiteSpace(SID))
        {
            dictionary.Add("kdservice-sessionid", SID);
        }
        return dictionary;
    }

    private static string GetNonce()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }

    private static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

    private static long GetTimestamp()
    {
        return CurrentTimeMillis();
    }
}