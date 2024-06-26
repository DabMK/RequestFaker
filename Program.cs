using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;

#pragma warning disable CS0618
#pragma warning disable SYSLIB0039
namespace RequestFaker
{
    internal class Program
    {
        private static string? URI = null, method = null, body = null, customVersion = null, protocol = "1.1";
        private static string[]? proxy = null;
        private static bool autoRedirects = true, ssl = false;
        private static Dictionary<string, string>? headers = null, cookies = null;
        private static Request.ProtocolVersion? version = null;
        private static HttpVersionPolicy? policy = null;
        private static SslProtocols? SSLprotocol = null;
        private static DecompressionMethods? decompression = null;
        public static string? Protocol { set { protocol = value; } }

        private static void Main(string[] args)
        {
            bool ok = true;
            while (true)
            {
                Initialize();
                Console.Clear();
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine("REQUESTFAKER:\n");
                Console.WriteLine("Usage: RequestFaker <URI> <method> <headersNumberWithoutCookies> <body> <cookiesNumber> <version> <versionPolicy> <useProxy> <allowRedirects> <SSLVerification> <SSLProtocol> <decompressionMethod>");
                Console.WriteLine("---------------------------------------------------------\n\n");

                // URI
                if (args.Length > 0)
                {
                    URI = args[0];
                    Console.WriteLine($"\nURI: {URI}");
                }
                else
                {
                    Console.Write("\nURI: ");
                    URI = Console.ReadLine();
                }

                // METHOD
                if (args.Length > 1)
                {
                    method = args[1];
                    Console.WriteLine($"\nMethod: {method}");
                }
                else
                {
                    Console.Write("\nMethod: ");
                    method = Console.ReadLine();
                }

                // HEADERS
                string? headersNumber;
                if (args.Length > 2)
                {
                    headersNumber = args[2];
                    Console.WriteLine($"\nHeaders Number (Without Cookies): {headersNumber}");
                }
                else
                {
                    Console.Write("\nHeaders Number (Without Cookies): ");
                    headersNumber = Console.ReadLine();
                }
                if (int.TryParse(headersNumber, out int hNum) && hNum >= 1)
                {
                    headers = new(hNum);
                    for (int i = 0; i < hNum; i++)
                    {
                        Console.Write($"- Header {i + 1} Key: ");
                        string? key = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(key)) { ok = false; break; }
                        Console.Write($"- Header {i + 1} Value: ");
                        string? value = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) { ok = false; break; }
                        headers.Add(key, value);
                    }
                    if (!ok) { continue; }
                }

                // BODY
                if (args.Length > 3)
                {
                    body = args[3];
                    Console.WriteLine($"\nBody: {body}");
                }
                else
                {
                    Console.Write("\nBody: ");
                    body = Console.ReadLine();
                }

                // COOKIES
                string? cookiesNumber;
                if (args.Length > 4)
                {
                    cookiesNumber = args[4];
                    Console.WriteLine($"\nCookies Number: {cookiesNumber}");
                }
                else
                {
                    Console.Write("\nCookies Number: ");
                    cookiesNumber = Console.ReadLine();
                }
                if (int.TryParse(cookiesNumber, out int cNum) && cNum >= 1)
                {
                    cookies = new(cNum);
                    for (int i = 0; i < cNum; i++)
                    {
                        Console.Write($"- Cookie {i + 1} Name: ");
                        string? key = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(key)) { ok = false; break; }
                        Console.Write($"- Cookie {i + 1} Value: ");
                        string? value = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) { ok = false; break; }
                        cookies.Add(key, value);
                    }
                    if (!ok) { continue; }
                }

                // VERSION
                string? sVersion;
                if (args.Length > 5)
                {
                    sVersion = args[5];
                    Console.WriteLine($"\nVersion (Default is 1.1): {sVersion}");
                }
                else
                {
                    Console.Write("\nVersion (Default is 1.1): ");
                    sVersion = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(sVersion) && !sVersion.StartsWith("0"))
                {
                    if (sVersion == "1" || sVersion == "1.0" || sVersion == "10" || sVersion == "v10" || sVersion == "v1.0" || sVersion == "v1")
                    {
                        version = Request.ProtocolVersion.v10;
                    }
                    else if (sVersion == "1.1" || sVersion == "11" || sVersion == "v11" || sVersion == "v1.1")
                    {
                        version = Request.ProtocolVersion.v11;
                    }
                    else if (sVersion == "2" || sVersion == "2.0" || sVersion == "20" || sVersion == "v20" || sVersion == "v2.0" || sVersion == "v2")
                    {
                        version = Request.ProtocolVersion.v20;
                    }
                    else if (sVersion == "3" || sVersion == "3.0" || sVersion == "30" || sVersion == "v30" || sVersion == "v3.0" || sVersion == "v3")
                    {
                        version = Request.ProtocolVersion.v30;
                    }
                    else if (sVersion.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
                    {
                        version = Request.ProtocolVersion.Unknown;
                    }
                    else
                    {
                        customVersion = sVersion;
                    }
                }

                // VERSION POLICY
                string? pVersion;
                if (args.Length > 6)
                {
                    pVersion = args[6];
                    Console.WriteLine($"\nVersion Policy: {pVersion}");
                }
                else
                {
                    Console.Write("\nVersion Policy: ");
                    pVersion = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(pVersion))
                {
                    if (pVersion.Equals("RequestVersionExact", StringComparison.OrdinalIgnoreCase) || pVersion.Equals("Exact", StringComparison.OrdinalIgnoreCase))
                    {
                        policy = HttpVersionPolicy.RequestVersionExact;
                    }
                    else if (pVersion.Equals("RequestVersionOrHigher", StringComparison.OrdinalIgnoreCase) || pVersion.Equals("Higher", StringComparison.OrdinalIgnoreCase))
                    {
                        policy = HttpVersionPolicy.RequestVersionOrHigher;
                    }
                    else if (pVersion.Equals("RequestVersionOrLower", StringComparison.OrdinalIgnoreCase) || pVersion.Equals("Lower", StringComparison.OrdinalIgnoreCase))
                    {
                        policy = HttpVersionPolicy.RequestVersionOrLower;
                    }
                }

                // PROXY
                string? proxyConfS;
                if (args.Length > 7)
                {
                    proxyConfS = args[7];
                    Console.WriteLine($"\nUse Proxy (yes/no) (Default is No)? {proxyConfS}");
                }
                else
                {
                    Console.Write("\nUse Proxy (yes/no) (Default is No)? ");
                    proxyConfS = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(proxyConfS) && (proxyConfS.Equals("y", StringComparison.OrdinalIgnoreCase) || proxyConfS.Equals("yes", StringComparison.OrdinalIgnoreCase) || proxyConfS.Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    proxy = new string[3];
                    Console.Write("\n- Proxy URI: ");
                    string? proxyURI = Console.ReadLine();
                    if (!string.IsNullOrEmpty(proxyURI))
                    {
                        proxy[0] = proxyURI;
                    }
                    Console.Write("\n- Use Credentials? ");
                    string? credentialsConfS = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(credentialsConfS) && (credentialsConfS.Equals("y", StringComparison.OrdinalIgnoreCase) || credentialsConfS.Equals("yes", StringComparison.OrdinalIgnoreCase) || credentialsConfS.Equals("true", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.Write("\n- Username: ");
                        string? proxyUsername = Console.ReadLine();
                        Console.Write("\n- Password: ");
                        string? proxyPassword = Console.ReadLine();
                        if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
                        {
                            proxy[1] = proxyUsername;
                            proxy[2] = proxyPassword;
                        }
                    }
                }

                // REDIRECTS
                string? redirectConfS;
                if (args.Length > 8)
                {
                    redirectConfS = args[8];
                    Console.WriteLine($"\nAllow AutoRedirects (Default is True)?: {redirectConfS}");
                }
                else
                {
                    Console.Write("\nAllow AutoRedirects (Default is True)? ");
                    redirectConfS = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(redirectConfS) && (redirectConfS.Equals("n", StringComparison.OrdinalIgnoreCase) || redirectConfS.Equals("no", StringComparison.OrdinalIgnoreCase) || redirectConfS.Equals("false", StringComparison.OrdinalIgnoreCase)))
                {
                    autoRedirects = false;
                }

                // SSL VERIFICATION
                string? SSLConfS;
                if (args.Length > 9)
                {
                    SSLConfS = args[9];
                    Console.WriteLine($"\nUse SSL Verification (Default is False)? {SSLConfS}");
                }
                else
                {
                    Console.Write("\nUse SSL Verification (Default is False)? ");
                    SSLConfS = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(SSLConfS) && (SSLConfS.Equals("y", StringComparison.OrdinalIgnoreCase) || SSLConfS.Equals("yes", StringComparison.OrdinalIgnoreCase) || SSLConfS.Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    ssl = true;
                }

                // SSL PROTOCOL
                string? SSLp;
                if (args.Length > 10)
                {
                    SSLp = args[10];
                    Console.WriteLine($"\nSSL Protocol: {SSLp}");
                }
                else
                {
                    Console.Write("\nSSL Protocol (Default is System Automatic): ");
                    SSLp = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(SSLp))
                {
                    if (SSLp.Equals("n", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("no", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.None;
                    }
                    else if (SSLp.Equals("d", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("default", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Default;
                    }
                    else if (SSLp.Equals("SSL2", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("SSL 2", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Ssl2;
                    }
                    else if (SSLp.Equals("SSL3", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("SSL 3", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Ssl3;
                    }
                    else if (SSLp.Equals("TLS", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Tls;
                    }
                    else if (SSLp.Equals("TLS11", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS1.1", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 11", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 1.1", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Tls11;
                    }
                    else if (SSLp.Equals("TLS12", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS1.2", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 12", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 1.2", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Tls12;
                    }
                    else if (SSLp.Equals("TLS13", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS1.3", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 13", StringComparison.OrdinalIgnoreCase) || SSLp.Equals("TLS 1.3", StringComparison.OrdinalIgnoreCase))
                    {
                        SSLprotocol = SslProtocols.Tls13;
                    }
                }

                // DECOMPRESSION METHOD
                string? decP;
                if (args.Length > 11)
                {
                    decP = args[11];
                    Console.WriteLine($"\nDecompression Method: {decP}");
                }
                else
                {
                    Console.Write("\nDecompression Method: ");
                    decP = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(decP))
                {
                    if (decP.Equals("a", StringComparison.OrdinalIgnoreCase) || decP.Equals("all", StringComparison.OrdinalIgnoreCase) || decP.Equals("everything", StringComparison.OrdinalIgnoreCase))
                    {
                        decompression = DecompressionMethods.All;
                    }
                    else if (decP.Equals("b", StringComparison.OrdinalIgnoreCase) || decP.Equals("brotli", StringComparison.OrdinalIgnoreCase))
                    {
                        decompression = DecompressionMethods.Brotli;
                    }
                    else if (decP.Equals("d", StringComparison.OrdinalIgnoreCase) || decP.Equals("deflate", StringComparison.OrdinalIgnoreCase))
                    {
                        decompression = DecompressionMethods.Deflate;
                    }
                    else if (decP.Equals("g", StringComparison.OrdinalIgnoreCase) || decP.Equals("gzip", StringComparison.OrdinalIgnoreCase))
                    {
                        decompression = DecompressionMethods.GZip;
                    }
                    else if (decP.Equals("n", StringComparison.OrdinalIgnoreCase) || decP.Equals("no", StringComparison.OrdinalIgnoreCase) || decP.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        decompression = DecompressionMethods.None;
                    }
                }


                Console.Clear();
                HttpResponseMessage? response = Request.Create(URI, method, headers, body, cookies, version, customVersion, policy, proxy, autoRedirects, ssl, SSLprotocol, decompression).GetAwaiter().GetResult();

                if (response != null)
                {
                    Dictionary<string, string> heads = response.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value));
                    Dictionary<string, string> trailingHeaders = response.TrailingHeaders.ToDictionary(a => a.Key, a => string.Join(";", a.Value));

                    Console.WriteLine("RESPONSE:\n-----------------------------\n");
                    Console.WriteLine("Status Code: " + response.StatusCode);
                    if (!string.IsNullOrWhiteSpace(response.ReasonPhrase)) { Console.WriteLine("Reason Phrase: " + response.ReasonPhrase); }
                    Console.WriteLine($"Protocol Version (Request -> Response): {protocol} -> {response.Version}");
                    Console.WriteLine("Headers:");
                    for (int i = 0; i < heads.Count; i++)
                    {
                        Console.WriteLine($"- {heads.ElementAt(i).Key}: {heads.ElementAt(i).Value}");
                    }
                    Console.WriteLine("Trailing Headers:");
                    for (int i = 0; i < trailingHeaders.Count; i++)
                    {
                        Console.WriteLine($"- {trailingHeaders.ElementAt(i).Key}: {trailingHeaders.ElementAt(i).Value}");
                    }
                    Console.Write("\nBody:\n" + response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                }
                if (args.Length > 0)
                {
                    Console.WriteLine(Environment.NewLine);
                    break;
                }
                else
                {
                    Console.ReadKey();
                }
            }
        }


        private static void Initialize()
        {
            URI = null;
            method = null;
            body = null;
            customVersion = null;
            protocol = "1.1";
            proxy = null;
            autoRedirects = true;
            ssl = false;
            headers = null;
            cookies = null;
            version = null;
            policy = null;
            SSLprotocol = null;
            decompression = null;
        }
    }
}