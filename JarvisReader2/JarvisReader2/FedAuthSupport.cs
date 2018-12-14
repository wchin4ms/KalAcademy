using System;
using System.Collections.Specialized;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Xml;

namespace JarvisReader
{

    internal class NativeMethods
    {
        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool InternetGetCookie(string url, string cookieName, StringBuilder cookieData, ref int size);
    }


    public class FedAuthSupport
    {
        private class STSWrapper
        {
            private string _stshost = "corp.sts.microsoft.com";

            private string username;

            private string password;

            private Uri serviceUri;

            private Uri cachedCredentialUri;

            private NetworkCredential cachedCredential = new NetworkCredential();

            private SecurityToken cachedToken;

            private RequestSecurityTokenResponse cachedTokenResponse;

            private object cachedTokenLock = new object();

            public string STSLoginUri
            {
                get
                {
                    return "https://" + this._stshost + "/adfs/ls/";
                }
            }

            public string STSWindowsTransport
            {
                get
                {
                    return "https://" + this._stshost + "/adfs/services/trust/13/windowstransport";
                }
            }

            public string STSUsernameMixed
            {
                get
                {
                    return "https://" + this._stshost + "/adfs/services/trust/13/usernamemixed";
                }
            }

            public string Token
            {
                get
                {
                    return (this.BaseToken != null) ? (this.BaseToken as GenericXmlSecurityToken).TokenXml.OuterXml : string.Empty;
                }
            }

            public RequestSecurityTokenResponse TokenResponse
            {
                get
                {
                    return (this.BaseToken != null) ? this.cachedTokenResponse : null;
                }
            }

            private SecurityToken BaseToken
            {
                get
                {
                    SecurityToken result;
                    lock (this.cachedTokenLock)
                    {
                        if (this.IsNewTokenNeeded())
                        {
                            this.cachedToken = this.GetTokenFromSts(out this.cachedTokenResponse);
                            this.CacheCredential();
                        }
                        result = this.cachedToken;
                    }
                    return result;
                }
            }

            public STSWrapper(Uri serviceUri, string stsHost = null)
            {
                this.serviceUri = serviceUri;
                if (!string.IsNullOrWhiteSpace(stsHost))
                {
                    this._stshost = stsHost;
                }
            }

            public STSWrapper(Uri serviceUri, string username, string password, string stsHost = null) : this(serviceUri, stsHost)
            {
                this.username = username;
                this.password = password;
            }

            private bool IsNewTokenNeeded()
            {
                return this.cachedCredential.UserName != (this.username ?? string.Empty) || this.cachedCredential.Password != (this.password ?? string.Empty) || this.cachedCredentialUri != this.serviceUri || (this.cachedToken == null || this.cachedToken.ValidTo.Subtract(TimeSpan.FromMinutes(1.0)).CompareTo(DateTime.UtcNow) <= 0);
            }

            private void CacheCredential()
            {
                this.cachedCredential.UserName = this.username;
                this.cachedCredential.Password = this.password;
                this.cachedCredentialUri = this.serviceUri;
            }

            private SecurityToken GetTokenFromSts(out RequestSecurityTokenResponse rstr)
            {
                int num = 3;
                SecurityToken result;
                while (true)
                {
                    try
                    {
                        if (this.username == null && this.password == null)
                        {
                            result = this.GetLoggedInUserToken(out rstr);
                            break;
                        }
                        result = this.GetImpersonatedUserToken(out rstr);
                        break;
                    }
                    catch (TimeoutException)
                    {
                        if (--num == 0)
                        {
                            throw;
                        }
                    }
                    catch (ServerTooBusyException)
                    {
                        if (--num == 0)
                        {
                            throw;
                        }
                    }
                }
                return result;
            }

            private SecurityToken GetLoggedInUserToken(out RequestSecurityTokenResponse rstr)
            {
                WSHttpBinding wSHttpBinding = new WSHttpBinding(SecurityMode.Transport);
                wSHttpBinding.AllowCookies = true;
                EndpointAddress remoteAddress = new EndpointAddress(new Uri(this.STSWindowsTransport), new AddressHeader[0]);
                SecurityToken result;
                using (WSTrustChannelFactory wSTrustChannelFactory = new WSTrustChannelFactory(wSHttpBinding, remoteAddress))
                {
                    wSTrustChannelFactory.Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
                    result = this.CreateChannelAndFetchToken(wSTrustChannelFactory, out rstr);
                }
                return result;
            }

            private SecurityToken GetImpersonatedUserToken(out RequestSecurityTokenResponse rstr)
            {
                WSHttpBinding wSHttpBinding = new WSHttpBinding(SecurityMode.Transport);
                wSHttpBinding.AllowCookies = true;
                EndpointAddress remoteAddress = new EndpointAddress(new Uri(this.STSUsernameMixed), new AddressHeader[0]);
                SecurityToken result;
                using (WSTrustChannelFactory wSTrustChannelFactory = new WSTrustChannelFactory(wSHttpBinding, remoteAddress))
                {
                    wSTrustChannelFactory.Credentials.UserName.UserName = this.username;
                    wSTrustChannelFactory.Credentials.UserName.Password = this.password;
                    result = this.CreateChannelAndFetchToken(wSTrustChannelFactory, out rstr);
                }
                return result;
            }

            private SecurityToken CreateChannelAndFetchToken(WSTrustChannelFactory trustChannelFactory, out RequestSecurityTokenResponse rstr)
            {
                trustChannelFactory.TrustVersion = TrustVersion.WSTrust13;
                trustChannelFactory.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
                SecurityToken result = null;
                RequestSecurityToken rst = new RequestSecurityToken
                {
                    RequestType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue",
                    KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer",
                    AppliesTo = new EndpointReference(this.serviceUri.AbsoluteUri)
                };
                WSTrustChannel wSTrustChannel = (WSTrustChannel)trustChannelFactory.CreateChannel();
                try
                {
                    result = wSTrustChannel.Issue(rst, out rstr);
                    if (wSTrustChannel.State == CommunicationState.Opened || wSTrustChannel.State == CommunicationState.Opening)
                    {
                        wSTrustChannel.Close(TimeSpan.FromSeconds(5.0));
                    }
                }
                finally
                {
                    if (wSTrustChannel.State != CommunicationState.Closed)
                    {
                        wSTrustChannel.Abort();
                    }
                }
                return result;
            }
        }

        public static string GetSAMLToken(Uri serviceUri, string stsHost = null)
        {
            FedAuthSupport.STSWrapper sTSWrapper = new FedAuthSupport.STSWrapper(serviceUri, stsHost);
            string result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(sTSWrapper.Token);
                    deflateStream.Write(bytes, 0, bytes.Length);
                }
                result = "SAML" + Convert.ToBase64String(memoryStream.ToArray());
            }
            return result;
        }

        public static CookieContainer GetCookies(Uri serviceUri, WebProxy webproxy = null, Uri loginUri = null, string stsHost = null)
        {
            FedAuthSupport.STSWrapper sTSWrapper = new FedAuthSupport.STSWrapper(serviceUri, stsHost);
            string text = null;
            HttpWebRequest httpWebRequest = FedAuthSupport.CreateBaseWebRequest("GET", serviceUri, webproxy, sTSWrapper);
            using (WebResponse response = httpWebRequest.GetResponse())
            {
                string text2 = response.Headers["Location"];
                if (text2 != null)
                {
                    if (text2.Contains("wctx"))
                    {
                        Uri uri = new Uri(text2);
                        NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
                        text = nameValueCollection["wctx"];
                    }
                    else
                    {
                        loginUri = new Uri(serviceUri, text2);
                    }
                }
            }
            if (loginUri == null)
            {
                loginUri = serviceUri;
            }
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(stringBuilder));
            WSTrust13ResponseSerializer wSTrust13ResponseSerializer = new WSTrust13ResponseSerializer();
            wSTrust13ResponseSerializer.WriteXml(sTSWrapper.TokenResponse, writer, new WSTrustSerializationContext());
            string xml = stringBuilder.ToString();
            HttpWebRequest httpWebRequest2 = FedAuthSupport.CreateBaseWebRequest("POST", loginUri, webproxy, sTSWrapper);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("trust", "http://docs.oasis-open.org/ws-sx/ws-trust/200512");
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//trust:RequestSecurityTokenResponse", xmlNamespaceManager);
            if (xmlNode == null)
            {
                throw new InvalidDataException("No RequestSecurityTokenResponse found in ADFS query.");
            }
            string s;
            if (!string.IsNullOrWhiteSpace(text))
            {
                s = string.Format("wa=wsignin1.0&wctx={0}&wresult={1}", HttpUtility.UrlEncode(text), HttpUtility.UrlEncode(xmlNode.OuterXml));
            }
            else
            {
                s = string.Format("wa=wsignin1.0&wresult={1}", HttpUtility.UrlEncode(xmlNode.OuterXml));
            }
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            using (Stream requestStream = httpWebRequest2.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            CookieContainer result;
            using (HttpWebResponse httpWebResponse = httpWebRequest2.GetResponse() as HttpWebResponse)
            {
                if (httpWebResponse != null)
                {
                    if (httpWebResponse.Cookies != null && httpWebResponse.Cookies.Count > 0)
                    {
                        CookieContainer cookieContainer = new CookieContainer();
                        cookieContainer.Add(serviceUri, httpWebResponse.Cookies);
                        result = cookieContainer;
                        return result;
                    }
                    if (httpWebResponse.Headers.AllKeys.Contains("Set-Cookie"))
                    {
                        CookieContainer cookieContainer = new CookieContainer();
                        string[] array = httpWebResponse.Headers["Set-Cookie"].Split(new char[]
                        {
                            ','
                        });
                        string[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string text3 = array2[i];
                            string[] array3 = text3.Split(new char[]
                            {
                                ';'
                            });
                            int num = array3[0].IndexOf('=');
                            if (num > 0)
                            {
                                Uri uri2 = serviceUri;
                                string name = array3[0].Substring(0, num);
                                string value = array3[0].Substring(num + 1);
                                for (int j = 1; j < array3.Length; j++)
                                {
                                    string[] array4 = array3[j].Split(new char[]
                                    {
                                        '='
                                    });
                                    if (array4[0].Trim().Equals("domain"))
                                    {
                                        uri2 = new Uri(uri2.Scheme + "://" + array4[1].Trim());
                                    }
                                }
                                Cookie cookie = new Cookie(name, value);
                                cookieContainer.Add(uri2, cookie);
                            }
                        }
                        if (cookieContainer.Count > 0)
                        {
                            result = cookieContainer;
                            return result;
                        }
                    }
                }
                result = null;
            }
            return result;
        }

        private static CookieContainer GetMSIAuthCookies(Uri uri, params string[] cookiefilter)
        {
            CookieContainer cookieContainer = null;
            int num = 256;
            StringBuilder stringBuilder = new StringBuilder(num);
            CookieContainer result;
            if (!NativeMethods.InternetGetCookie(uri.ToString(), null, stringBuilder, ref num))
            {
                if (num < 0)
                {
                    result = null;
                    return result;
                }
                stringBuilder = new StringBuilder(num);
                if (!NativeMethods.InternetGetCookie(uri.ToString(), null, stringBuilder, ref num))
                {
                    result = null;
                    return result;
                }
            }
            if (stringBuilder.Length > 0)
            {
                cookieContainer = new CookieContainer();
                cookieContainer.SetCookies(uri, stringBuilder.ToString().Replace(';', ','));
                if (cookiefilter.Count<string>() > 0)
                {
                    CookieContainer cookieContainer2 = new CookieContainer();
                    CookieCollection cookies = cookieContainer.GetCookies(uri);
                    for (int i = 0; i < cookiefilter.Length; i++)
                    {
                        string name = cookiefilter[i];
                        if (cookies[name] != null)
                        {
                            cookieContainer2.Add(uri, cookies[name]);
                        }
                    }
                    result = cookieContainer2;
                    return result;
                }
            }
            result = cookieContainer;
            return result;
        }

        private static HttpWebRequest CreateBaseWebRequest(string method, Uri serviceUri, WebProxy webProxy, FedAuthSupport.STSWrapper stsWrapper)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(serviceUri) as HttpWebRequest;
            httpWebRequest.Method = method;
            if (method == "POST")
            {
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            }
            httpWebRequest.CookieContainer = FedAuthSupport.GetMSIAuthCookies(new Uri(stsWrapper.STSLoginUri), new string[0]);
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Proxy = webProxy;
            httpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            httpWebRequest.Timeout = 300000;
            return httpWebRequest;
        }
    }
}
