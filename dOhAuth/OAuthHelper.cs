//OAuth Helper written by Daniel Pickford.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace dOhAuth
{
    public class OAuthHelper
    {
        private readonly Random _random;
        private readonly Dictionary<String, String> _params;
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        //Constructor setup Params.
        #region "ctor"
        
        /// <summary>
        ///   The default public constructor.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This constructor initializes the internal fields in the
        ///     Manager instance to default values.
        ///   </para>
        /// </remarks>
        public OAuthHelper()
        {
            _random = new Random();
            _params = new Dictionary<String,String>();
            _params["callback"] = "oob"; // presume "desktop" consumer
            _params["consumer_key"] = "";
            _params["consumer_secret"] = "";
            _params["timestamp"] = GenerateTimeStamp();
            _params["nonce"] = GenerateNonce();
            _params["signature_method"] = "HMAC-SHA1";
            _params["signature"] = "";
            _params["token"] = "";
            _params["token_secret"] = "";
            _params["version"] = "1.0";
        }

        /// <summary>
        ///   The constructor to use when using OAuth when you already
        ///   have an OAuth access token.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The parameters for this constructor all have the
        ///     meaning you would expect.  The token and tokenSecret
        ///     are set in oauth_token, and oauth_token_secret.
        ///     These are *Access* tokens, obtained after a call
        ///     to AcquireAccessToken.  The application can store
        ///     those tokens and re-use them on successive runs.
        ///     the access tokens never expire.
        ///   </para>
        /// </remarks>
        ///
        /// <param name="consumerKey">The oauth_consumer_key parameter for
        /// OAuth.
        /// </param>
        /// <param name="consumerSecret">The oauth_consumer_secret
        /// parameter for oauth.</param>
        /// <param name="token">The oauth_token parameter for
        /// oauth. This is sometimes called the Access Token.</param>
        /// <param name="tokenSecret">The oauth_token_secret parameter for
        /// oauth. This is sometimes called the Access Token Secret.</param>
        public OAuthHelper(string consumerKey,string consumerSecret,string token,string tokenSecret) : this()
        {
            _params["consumer_key"] = consumerKey;
            _params["consumer_secret"] = consumerSecret;
            _params["token"] = token;
            _params["token_secret"] = tokenSecret;
        }

        /// <summary>
        ///   The constructor to use when using OAuth when you already
        ///   have an OAuth consumer key and sercret, but need to
        ///   acquire an oauth access token.
        /// </summary>
        /// <param name="consumerKey">The oauth_consumer_key parameter for
        /// oauth. Get this, along with the consumerSecret
        /// </param>
        ///
        /// <param name="consumerSecret">The oauth_consumer_secret
        /// parameter for oauth.</param>
        public OAuthHelper(string consumerKey, string consumerSecret)
            : this()
        {
            _params["consumer_key"] = consumerKey;
            _params["consumer_secret"] = consumerSecret;
        }

        #endregion

        public OAuthResponse AcquireRequestToken(string uri, string method)
        {
            _params["timestamp"] = GenerateTimeStamp();
            _params["nonce"] = GenerateNonce();
            var authzHeader =  GetAuthorizationHeader(uri, method);

            // prepare the token request
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Headers.Add("Authorization", authzHeader);
            request.Method = method;

            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var r = new OAuthResponse(reader.ReadToEnd());
                    this._params["token"] = r["oauth_token"];

                    // Sometimes the request_token URL gives us an access token,
                    // with no user interaction required. Eg, when prior approval
                    // has already been granted.
                    try
                    {
                        if (r["oauth_token_secret"] != null)
                            this._params["token_secret"] = r["oauth_token_secret"];
                    }
                    catch { }
                    return r;
                }
            }
        }

        public OAuthResponse AcquireAccessToken(string uri, string method, string pin)
        {
            _params["timestamp"] = GenerateTimeStamp();
            _params["nonce"] = GenerateNonce();
            _params["verifier"] = pin;

            var authzHeader = GetAuthorizationHeader(uri, method);

            // prepare the token request
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Headers.Add("Authorization", authzHeader);
            request.Method = method;

            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var r = new OAuthResponse(reader.ReadToEnd());
                    this._params["token"] = r["oauth_token"];
                    this._params["token_secret"] = r["oauth_token_secret"];
                    return r;
                }
            }
        }

        public string executeFunction(string uri, string method, Dictionary<String, String> functionParameters)
        {
            _params["timestamp"] = GenerateTimeStamp();
            _params["nonce"] = GenerateNonce();
            foreach (var functionParameter in functionParameters)
            {
                _params[functionParameter.Key] = functionParameter.Value;
            }

            var authzHeader = GetAuthorizationHeader(uri, method);

            // prepare the token request
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Headers.Add("Authorization", authzHeader);
            request.Method = method;

            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var r = (reader.ReadToEnd());
                    return r;
                }
            }
        }

        public string GenerateNonce()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int g = _random.Next(3);
                switch (g)
                {
                    case 0:
                        // lowercase alpha
                        sb.Append((char)(_random.Next(26) + 97), 1);
                        break;
                    default:
                        // numeric digits
                        sb.Append((char)(_random.Next(10) + 48), 1);
                        break;
                }
            }
            return sb.ToString();
        }

        public string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - Epoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public string GetAuthorizationHeader(string uri, string method)
        {
            return GetAuthorizationHeader(uri, method, null);
        }

        private string GetAuthorizationHeader(string uri, string method, string realm)
        {
            if (string.IsNullOrEmpty(this._params["consumer_key"]))
                throw new ArgumentNullException("consumer_key");

            if (string.IsNullOrEmpty(this._params["signature_method"]))
                throw new ArgumentNullException("signature_method");

            Sign(uri, method);

            var erp = EncodeRequestParameters(this._params);
            return (String.IsNullOrEmpty(realm))
                ? "OAuth " + erp
                : String.Format("OAuth realm=\"{0}\", ", realm) + erp;
        }

        public string GetSignature(string uri, string method)
        {
            var signatureBase = GetSignatureBase(uri, method);
            var hash = GetHash();

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        private void Sign(string uri, string method)
        {
            var signatureBase = GetSignatureBase(uri, method);
            var hash = GetHash();

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);

            this._params["signature"] = Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Formats the list of request parameters into "signature base" string as
        /// defined by RFC 5849.  This will then be MAC'd with a suitable hash.
        /// </summary>
        private string GetSignatureBase(string url, string method)
        {
            // normalize the URI
            var uri = new Uri(url);
            var normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // the sigbase starts with the method and the encoded URI
            var sb = new System.Text.StringBuilder();
            sb.Append(method)
                .Append('&')
                .Append(UrlEncode(normUrl))
                .Append('&');

            // The parameters follow. This must include all oauth params
            // plus any query params on the uri.  Also, each uri may
            // have a distinct set of query params.

            // first, get the query params
            var p = ExtractQueryParameters(uri.Query);

            // add to that list all non-empty oauth params
            foreach (var p1 in this._params)
            {
                // Exclude all oauth params that are secret or
                // signatures; any secrets must not be shared,
                // and any existing signature will be invalid.

                if (!String.IsNullOrEmpty(this._params[p1.Key]) &&
                    !p1.Key.EndsWith("_secret") &&
                    !p1.Key.EndsWith("signature"))
                {
                    // workitem 15756 - handle non-oob scenarios
                    p.Add("oauth_" + p1.Key,
                          (p1.Key == "callback") ? UrlEncode(p1.Value) : p1.Value);
                }
            }

            // concat+format the sorted list of all those params
            var sb1 = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                // even "empty" params need to be encoded this way.
                sb1.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            // append the UrlEncoded version of that string to the sigbase
            sb.Append(UrlEncode(sb1.ToString().TrimEnd('&')));
            var result = sb.ToString();
            return result;
        }
        /// <summary>
        /// Internal function to extract from a URL all query string
        /// parameters that are not related to oauth - in other words all
        /// parameters not begining with "oauth_".
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     For example, given a url like http://foo?a=7&amp;guff, the
        ///     returned value will be a Dictionary of string-to-string
        ///     relations.  There will be 2 entries in the Dictionary: "a"=>7,
        ///     and "guff"=>"".
        ///   </para>
        /// </remarks>
        ///
        /// <param name="queryString">The query string part of the Url</param>
        ///
        /// <returns>A Dictionary containing the set of
        /// parameter names and associated values</returns>
        private Dictionary<String, String> ExtractQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Remove(0, 1);

            var result = new Dictionary<String, String>();

            if (string.IsNullOrEmpty(queryString))
                return result;

            foreach (string s in queryString.Split('&'))
            {
                if (!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                {
                    if (s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(temp[0], temp[1]);
                    }
                    else
                        result.Add(s, string.Empty);
                }
            }

            return result;
        }
        private static string EncodeRequestParameters(ICollection<KeyValuePair<String, String>> p)
        {
            var sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                if (!String.IsNullOrEmpty(item.Value) &&
                    !item.Key.EndsWith("secret"))
                    sb.AppendFormat("oauth_{0}=\"{1}\", ",
                                    item.Key,
                                    UrlEncode(item.Value));
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }

        private HashAlgorithm GetHash()
        {
            if (this._params["signature_method"] != "HMAC-SHA1")
                throw new NotImplementedException();

            string keystring = string.Format("{0}&{1}",
                                             UrlEncode(this._params["consumer_secret"]),
                                             UrlEncode(this._params["token_secret"]));
            var hmacsha1 = new HMACSHA1
            {
                Key = System.Text.Encoding.ASCII.GetBytes(keystring)
            };
            return hmacsha1;
        }
        /// <summary>
        ///   This method performs oauth-compliant Url Encoding.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This class provides an OAuth-friendly URL encoder.  .NET includes
        ///     a Url encoder in the base class library; see <see
        ///     href='http://msdn.microsoft.com/en-us/library/zttxte6w(v=VS.90).aspx'>
        ///     HttpServerUtility.UrlEncode</see>. But that encoder is not
        ///     sufficient for use with OAuth.
        ///   </para>
        ///   <para>
        ///     The builtin encoder emits the percent encoding in lower case,
        ///     which works for HTTP purposes, as described in the latest HTTP
        ///     specification (see <see
        ///     href="http://tools.ietf.org/html/rfc3986">RFC 3986</see>). But the
        ///     Oauth specification, provided in <see
        ///     href="http://tools.ietf.org/html/rfc5849">RFC 5849</see>, requires
        ///     that the encoding characters be upper case throughout OAuth.
        ///   </para>
        ///   <para>
        ///     For example, if you try to post a tweet message that includes a
        ///     forward slash, the slash needs to be encoded as %2F, and the
        ///     second hex digit needs to be uppercase.
        ///   </para>
        ///   <para>
        ///     It's not enough to simply convert the entire message to uppercase,
        ///     because that would of course convert un-encoded characters to
        ///     uppercase as well, which is undesirable.  This class provides an
        ///     OAuth-friendly encoder to do the right thing.
        ///   </para>
        /// </remarks>
        ///
        /// <param name="value">The value to encode</param>
        /// <returns>the Url-encoded version of that string</returns>
        public static string UrlEncode(string value)
        {
            var result = new StringBuilder();
            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                {
                    foreach (byte b in Encoding.UTF8.GetBytes(symbol.ToString()))
                    {
                        result.Append('%' + String.Format("{0:X2}", b));
                    }
                }
            }
            return result.ToString();
        }

    }

    /// <summary>
    ///   A class to hold an OAuth response message.
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        ///   All of the text in the response. This is useful if the app wants
        ///   to do its own parsing.
        /// </summary>
        public string AllText { get; set; }
        private readonly Dictionary<String, String> _params;

        /// <summary>
        ///   a Dictionary of response parameters.
        /// </summary>
        public string this[string ix]
        {
            get
            {
                return _params[ix];
            }
        }

        /// <summary>
        ///   Constructor for the response to one transmission in an oauth dialogue.
        ///   An application or may not not want direct access to this response.
        /// </summary>
        internal OAuthResponse(string alltext)
        {
            AllText = alltext;
            _params = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                _params.Add(kv[0], kv[1]);
            }
            // expected keys:
            //   oauth_token, oauth_token_secret, user_id, screen_name, etc
        }
    }
}
