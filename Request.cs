using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.ComponentModel;
using System.Security.Authentication;

namespace RequestFaker
{
    internal class Request
    {
        public static async Task<HttpResponseMessage?> Create(string? URI, string? method, Dictionary<string, string>? headers = null, string? body = null, Dictionary<string, string>? cookies = null, ProtocolVersion? version = null, string? customVersion = null, HttpVersionPolicy? policy = null, string[]? proxy = null, bool autoRedirects = true, bool ssl = false, SslProtocols? SSLProtocol = null, DecompressionMethods? decompression = null)
        {
            HttpClient client;
            HttpResponseMessage response = new();
            HttpClientHandler handler = new();
            CookieContainer container = new();
            if (string.IsNullOrWhiteSpace(URI) || string.IsNullOrWhiteSpace(method)) { return new(); }

            // FORGING REQUEST
            using (HttpRequestMessage request = new(new(method), URI))
            {
                // HEADERS
                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        request.Headers.Add(headers.Keys.ToList()[i], headers.Values.ToList()[i]);
                    }
                }

                // CONTENT
                if (body != null) { request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body)); }

                // VERSION
                if (version != null)
                {
                    switch (version)
                    {
                        case ProtocolVersion.v10:
                            request.Version = HttpVersion.Version10;
                            Program.Protocol = "1.0";
                            break;
                        case ProtocolVersion.v11:
                            request.Version = HttpVersion.Version11;
                            break;
                        case ProtocolVersion.v20:
                            request.Version = HttpVersion.Version20;
                            Program.Protocol = "2.0";
                            break;
                        case ProtocolVersion.v30:
                            request.Version = HttpVersion.Version30;
                            Program.Protocol = "3.0";
                            break;
                        case ProtocolVersion.Unknown:
                            request.Version = HttpVersion.Unknown;
                            Program.Protocol = "Unknown";
                            break;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(customVersion))
                {
                    request.Version = new(customVersion);
                    Program.Protocol = customVersion;
                }

                // VERSION POLICY
                if (policy != null) { request.VersionPolicy = (HttpVersionPolicy)policy; }

                // COOKIES
                if (cookies != null)
                {
                    handler.UseCookies = true;
                    for (int i = 0; i < cookies.Count; i++)
                    {
                        container.Add(new Uri(URI), new Cookie(cookies.ElementAt(i).Key, cookies.ElementAt(i).Value));
                    }
                }
                else
                {
                    handler.UseCookies = false;
                }

                // PROXY
                if (proxy != null)
                {
                    handler.UseProxy = true;

                    WebProxy webProxy = new()
                    {
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false
                    };
                    if (proxy[0] != null)
                    {
                        webProxy.Address = new(proxy[0]);
                    }
                    if (proxy[1] != null && proxy[2] != null)
                    {
                        webProxy.Credentials = new NetworkCredential(proxy[1], proxy[2]);
                    }
                    handler.Proxy = webProxy;
                }

                // SSL SETTINGS
                if (!ssl) { handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; }
                if (SSLProtocol != null) { handler.SslProtocols = (SslProtocols)SSLProtocol; }

                // ALLOW REDIRECTS
                handler.AllowAutoRedirect = autoRedirects;

                // DECOMPRESSION METHOD
                if (decompression != null) { handler.AutomaticDecompression = (DecompressionMethods)decompression; }

                // SEND
                handler.CookieContainer = container;
                client = new(handler, true);
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Win32Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
            }
            return response;
        }


        public enum ProtocolVersion
        {
            v10, v11, v20, v30, Unknown
        };
    }
}