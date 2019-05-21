﻿// -----------------------------------------------------------------------
//  <copyright file="HttpClient.cs" company="Binary Overdrive">
//      Copyright (c) Binary Overdrive.
//  </copyright>
//  <project>BotSuite.Net</project>
//  <purpose>Framework for creating automation applications.</purpose>
//  <homepage>https://bitbucket.org/KarillEndusa/botsuite.net</homepage>
//  <license>https://bitbucket.org/KarillEndusa/botsuite.net/wiki/license</license>
// -----------------------------------------------------------------------

namespace BotSuite.Net
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Net;
	using System.Text;

	/// <summary>
	///     HttpClient class for making bots for Browsergames
	/// </summary>
	public class HttpClient
	{
		/// <summary>
		///     Gets all bytes of the last response
		/// </summary>
		public byte[] Cache { get; private set; }

		/// <summary>
		///     Contains all cookies for this instance of the HttpClient class
		/// </summary>
		private readonly CookieContainer _cookies = new CookieContainer();

		/// <summary>
		///     Gets all cookies for this instance of the HttpClient class
		/// </summary>
		public CookieContainer Cookies
		{
			get
			{
				return this._cookies;
			}
		}

		/// <summary>
		///     the user agent string which is used for requests
		/// </summary>
		private readonly UserAgent _userAgent = null;

		/// <summary>
		///     a collection of all headers of the last response
		/// </summary>
		private readonly HttpHeaderCollection _headers = new HttpHeaderCollection();

		/// <summary>
		///     Gets a collection of all headers of the last response
		/// </summary>
		public HttpHeaderCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether the HttpClient should adjust the referer at every change of the URL
		/// </summary>
		public bool AutoReferer { get; set; }

		/// <summary>
		///     Gets or sets the current referer
		/// </summary>
		public string Referer { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the HttpClient should use the HttpClient.Proxy for requests
		/// </summary>
		public bool UseProxy { get; set; }

		/// <summary>
		///     Gets or sets a proxy object with settings for proxy usage for requests (when HttpClient.UseProxy is set to true)
		/// </summary>
		public HttpProxy Proxy { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether unsafe header parsing should be used
		/// </summary>
        public bool UseUnsafeHeaderParsing
		{
            get { return false; }

            set
            {
                throw new Exception("unable to set useUnsafeHeaderParsing in configuration");
            }
		}

		/// <summary>
		///     Gets or sets a value indicating whether auto-redirects should be allowed (false might cause more work for the
		///     developer since more requests have to be made manually)
		/// </summary>
		public bool AllowAutoRedirect { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether BotSuites internal redirect method should be used (will only work if
		///     .AllowAutoRedirect is false), else false
		/// </summary>
		public bool AllowBotSuiteAutoRedirect { get; set; }

		/// <summary>
		///     Gets or sets the maximum of consecutive requests for AllowAutoRedirect and AllowBotSuiteAutoRedirect
		/// </summary>
		public int MaximumRedirectCount { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether Expect100Continue of the HttpWebRequest.ServiceProvider is used upon
		///     request
		/// </summary>
		public bool Expect100Continue { get; set; }

		/// <summary>
		///     Gets the encoding of the last web-response
		/// </summary>
		public Encoding LastResponseEncoding { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether or not the HttpClient should ignore SSL/TLS certificate validation failures
		/// </summary>
		public bool IgnoreCertificateValidationFailures { get; set; }

		/// <summary>
		///     Gets or sets the decompression method.
		/// </summary>
		public DecompressionMethods DecompressionMethod { get; set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="HttpClient" /> class
		/// </summary>
		/// <param name="userAgent">
		///     a useragent string
		/// </param>
		/// <param name="initialReferer">
		///     the Referer the first request will be sent from, i.e. www.google.com
		/// </param>
		public HttpClient(string userAgent, string initialReferer = null)
		{
			this.DecompressionMethod = DecompressionMethods.None;
			this.MaximumRedirectCount = 100;
			this.AllowAutoRedirect = true;
			this.AutoReferer = true;
			this._userAgent = new UserAgent(userAgent, "HttpClient");
			this.Referer = initialReferer;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="HttpClient" /> class
		/// </summary>
		/// <param name="userAgent">
		///     an instance of the <see cref="UserAgent"/> class
		/// </param>
		/// <param name="initialReferer">
		///     the Referer the first request will be sent from, i.e. www.google.com
		/// </param>
		public HttpClient(UserAgent userAgent, string initialReferer = null)
		{
			this.DecompressionMethod = DecompressionMethods.None;
			this.MaximumRedirectCount = 100;
			this.AllowAutoRedirect = true;
			this.AutoReferer = true;
			this._userAgent = userAgent;
			this.Referer = initialReferer;
		}

		/// <summary>
		///     sends a HTTP POST request to a given URL with given POST data and a optional referer
		/// </summary>
		/// <typeparam name="TRespType">
		///     return type of the Post request
		/// </typeparam>
		/// <param name="url">
		///     the URL to send the post request to
		/// </param>
		/// <param name="postdata">
		///     the POST data
		/// </param>
		/// <param name="referer">
		///     the referer to send the request from
		/// </param>
		/// <returns>
		///     the response as TRespType
		/// </returns>
		public TRespType Post<TRespType>(string url, HttpPostDataCollection postdata, string referer = null)
			where TRespType : class
		{
			return this.Post<TRespType>(url, postdata.ToString(), referer);
		}

		/// <summary>
		///     sends a HTTP POST request to a given URL with given POST data and a optional referer...
		///     (better use overload with HttpPostDataCollection parameter, it's easier to use and more flexible)
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     the URL to send the post request to
		/// </param>
		/// <param name="postdata">
		///     the POST data
		/// </param>
		/// <param name="referer">
		///     the referer to send the request from
		/// </param>
		/// <returns>
		///     the response as TRespType
		/// </returns>
		public TRespType Post<TRespType>(string url, string postdata, string referer = null) where TRespType : class
		{
			if(!this.AllowBotSuiteAutoRedirect || this.AllowAutoRedirect)
			{
				return this.PostInternal<TRespType>(url, postdata, referer);
			}

			TRespType response = null;
			int requestCount = 0;
			while(true)
			{
				if(requestCount > this.MaximumRedirectCount)
				{
					throw new HttpRequestException("too many automatic redirects");
				}

				if(requestCount <= 0)
				{
					response = this.PostInternal<TRespType>(url, postdata, referer);
				}
				else if(this._headers.Contains(HttpConstants.HeaderNames.LOCATION))
				{
					response = this.GetInternal<TRespType>(this.Headers[HttpConstants.HeaderNames.LOCATION].Value);
				}
				else
				{
					break;
				}

				requestCount++;
			}

			return response;
		}

		/// <summary>
		///     internal POST method
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     url to send post request to
		/// </param>
		/// <param name="postdata">
		///     data of the post request
		/// </param>
		/// <param name="referer">
		///     referer for the post request
		/// </param>
		/// <returns>
		///     response as TRespType of site
		/// </returns>
		private TRespType PostInternal<TRespType>(string url, string postdata, string referer = null) where TRespType : class
		{
			this._headers.Clear(); // DerpyHooves 2013-06-21

			if(referer != null)
			{
				this.Referer = referer;
			}

			url = CorrectUrl(url);

			TRespType response;
			try
			{
				HttpWebRequest req = this.PrepareRequest(url, "POST");
				req.ContentType = "application/x-www-form-urlencoded";
				byte[] data = Encoding.Default.GetBytes(postdata);
				req.ContentLength = data.Length;
				using(Stream s = req.GetRequestStream())
				{
					s.Write(data, 0, data.Length);
				}

				response = this.GetResponse<TRespType>(req);
			}
			catch(Exception ex)
			{
				throw new HttpRequestException("POST request to " + url + " failed.", ex);
			}

			return response;
		}

		/// <summary>
		///     sends a HTTP GET request to a given URL with a optional referer
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     the URL to send the request to
		/// </param>
		/// <param name="referer">
		///     the referer to send the request from
		/// </param>
		/// <returns>
		///     returns the response as TRespType of the requested website
		/// </returns>
		public TRespType Get<TRespType>(string url, string referer = null) where TRespType : class
		{
			if(!this.AllowBotSuiteAutoRedirect || this.AllowAutoRedirect)
			{
				return this.GetInternal<TRespType>(url, referer);
			}

			TRespType response = null;
			int requestCount = 0;
			while(true)
			{
				if(requestCount > this.MaximumRedirectCount)
				{
					throw new HttpRequestException("too many automatic redirects");
				}

				if(requestCount <= 0)
				{
					response = this.GetInternal<TRespType>(url, referer);
				}
				else if(this._headers.Contains(HttpConstants.HeaderNames.LOCATION))
				{
					response = this.GetInternal<TRespType>(this.Headers[HttpConstants.HeaderNames.LOCATION].Value);
				}
				else
				{
					break;
				}

				requestCount++;
			}

			return response;
		}

		/// <summary>
		///     internal GET method
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     url to send request to
		/// </param>
		/// <param name="referer">
		///     referer for the request
		/// </param>
		/// <returns>
		///     response as TRespType of the requested website
		/// </returns>
		private TRespType GetInternal<TRespType>(string url, string referer = null) where TRespType : class
		{
			this._headers.Clear();

			if(referer != null)
			{
				this.Referer = referer;
			}

			url = CorrectUrl(url);

			TRespType response;
			try
			{
				HttpWebRequest req = this.PrepareRequest(url, "GET");
				response = this.GetResponse<TRespType>(req);
			}
			catch(Exception ex)
			{
				throw new HttpRequestException("GET request to " + url + " failed.", ex);
			}

			return response;
		}

		/// <summary>
		///     sends a HTTP HEAD request to a given URL with a optional referer
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     the URL to send the request to
		/// </param>
		/// <param name="referer">
		///     the referer to send the request from
		/// </param>
		public void Head<TRespType>(string url, string referer = null) where TRespType : class
		{
			if(this.AllowBotSuiteAutoRedirect && !this.AllowAutoRedirect)
			{
				int requestCount = 0;
				while(true)
				{
					if(requestCount > this.MaximumRedirectCount)
					{
						throw new HttpRequestException("too many automatic redirects");
					}

					if(requestCount <= 0)
					{
						this.HeadInternal<TRespType>(url, referer);
					}
					else if(this._headers.Contains(HttpConstants.HeaderNames.LOCATION))
					{
						this.HeadInternal<TRespType>(this.Headers[HttpConstants.HeaderNames.LOCATION].Value);
					}
					else
					{
						break;
					}

					requestCount++;
				}
			}
			else
			{
				this.HeadInternal<TRespType>(url, referer);
			}
		}

		/// <summary>
		///     internal HEAD request method
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="url">
		///     url for the head request
		/// </param>
		/// <param name="referer">
		///     referer for the head request
		/// </param>
		private void HeadInternal<TRespType>(string url, string referer = null) where TRespType : class
		{
			this._headers.Clear(); // DerpyHooves 2013-06-21

			if(referer != null)
			{
				this.Referer = referer;
			}

			url = CorrectUrl(url);

			try
			{
				HttpWebRequest req = this.PrepareRequest(url, "HEAD");
				this.GetResponse<TRespType>(req);
			}
			catch(Exception ex)
			{
				throw new HttpRequestException("HEAD request to " + url + " failed.", ex);
			}
		}

		/// <summary>
		///     corrects some standard-failures made when someone writes and URL, like forgetting the protocol at the beginning
		/// </summary>
		/// <param name="url">
		///     the URL that probably needs some correction
		/// </param>
		/// <returns>
		///     the, if it was necessary, corrected URL
		/// </returns>
		private static string CorrectUrl(string url)
		{
			if(!url.StartsWith("http://") && !url.StartsWith("https://"))
			{
				url = "http://" + url;
			}

			return url;
		}

		/// <summary>
		///     Creates a HttpWebRequest and prepares it for usage (e.g. sets method and other parameters)
		/// </summary>
		/// <param name="url">
		///     The url to request will be used for
		/// </param>
		/// <param name="method">
		///     the method to use, e.g. POST or GET
		/// </param>
		/// <returns>
		///     a fully prepared HttpWebRequest
		/// </returns>
		private HttpWebRequest PrepareRequest(string url, string method)
		{
			this.Cache = null;

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

			ServicePointManager.ServerCertificateValidationCallback =
				(sender, cert, chain, errors) => this.IgnoreCertificateValidationFailures;

			req.CookieContainer = this._cookies;
			req.Method = method;
			req.UserAgent = this._userAgent.UserAgentString;
			req.AutomaticDecompression = this.DecompressionMethod;
			req.ServicePoint.Expect100Continue = this.Expect100Continue;
			req.AllowAutoRedirect = this.AllowAutoRedirect;
			req.Credentials = CredentialCache.DefaultCredentials;
			if(this.Referer != null)
			{
				req.Referer = this.Referer;
			}

			if(this.UseProxy && (this.Proxy != null))
			{
				req.Proxy = this.Proxy.GetWebProxy();
				req.Credentials = req.Proxy.Credentials;
			}

			return req;
		}

		/// <summary>
		///     Receives the response for a HttpWebRequest
		/// </summary>
		/// <typeparam name="TRespType">
		///     the return type for the response
		/// </typeparam>
		/// <param name="req">
		///     the HttpWebRequest the return is wanted for
		/// </param>
		/// <returns>
		///     the response as TRespType for the request
		/// </returns>
		private TRespType GetResponse<TRespType>(HttpWebRequest req) where TRespType : class
		{
			TRespType response = null;
			using(HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
			{
				foreach(string headerKey in resp.Headers.AllKeys)
				{
					this.Headers.Add(new HttpHeader(headerKey, resp.Headers[headerKey]));
				}

				foreach(Cookie c in resp.Cookies)
				{
					this.RepairCookie(resp.ResponseUri, c);
				}

				using(Stream s = resp.GetResponseStream())
				{
					this.SetLastReponseEncoding(resp.CharacterSet);

					if(s != null)
					{
						using(BinaryReader br = new BinaryReader(s, this.LastResponseEncoding))
						{
							List<byte> bytes = new List<byte>();
							byte[] buffer = new byte[1];

							while(br.Read(buffer, 0, buffer.Length) > 0)
							{
								bytes.AddRange(buffer);
							}

							this.Cache = bytes.ToArray();
						}
					}

					if(this.Cache == null)
					{
						return null;
					}

					if(typeof(TRespType) == typeof(string))
					{
						response = this.LoadStringFromCache() as TRespType;
					}
					else if(typeof(TRespType) == typeof(Image))
					{
						response = this.LoadImageFromCache() as TRespType;
					}

					if(this.AutoReferer)
					{
						this.Referer = resp.ResponseUri.AbsolutePath;
					}
				}
			}

			return response;
		}

		/// <summary>
		///     Sets the LastResponseEncoding to the specified charset
		/// </summary>
		/// <param name="charset">The charset</param>
		private void SetLastReponseEncoding(string charset)
		{
			if(charset != null)
			{
				try
				{
					this.LastResponseEncoding = Encoding.GetEncoding(charset);
				}
				catch(Exception)
				{
					this.LastResponseEncoding = Encoding.Default;
				}
			}
			else
			{
				this.LastResponseEncoding = Encoding.Default;
			}
		}

		/// <summary>
		///     Converts the cache of this HttpClient into a string
		/// </summary>
		/// <returns>the string from the cache</returns>
		private string LoadStringFromCache()
		{
			string retStr = null;

			if(this.Cache != null)
			{
				retStr = (this.LastResponseEncoding ?? Encoding.Default).GetString(this.Cache);
			}

			return retStr;
		}

		/// <summary>
		///     Converts the cache of this HttpClient into an image
		/// </summary>
		/// <returns>the image from the cache</returns>
		private Image LoadImageFromCache()
		{
			Image retImg;

			if(this.Cache == null)
			{
				return null;
			}

			try
			{
				using(MemoryStream ms = new MemoryStream())
				{
					BinaryWriter bw = new BinaryWriter(ms, this.LastResponseEncoding ?? Encoding.Default);
					bw.Write(this.Cache);
					ms.Seek(0, SeekOrigin.Begin);
					retImg = Image.FromStream(ms);
					bw.Dispose();
				}
			}
			catch
			{
				return null;
			}

			return retImg;
		}

		/// <summary>
		///     repairs a cookie (path and uri)
		/// </summary>
		/// <param name="u">
		///     the uri the cookie is for
		/// </param>
		/// <param name="c">
		///     the cookie to repair
		/// </param>
		private void RepairCookie(Uri u, Cookie c)
		{
			string path = c.Path;
			if(!path.EndsWith("/"))
			{
				if(!path.Contains("/"))
				{
					path = "/" + path;
				}

				path = path.Remove(path.LastIndexOf('/') + 1);
			}

			Cookie nc = new Cookie(c.Name, c.Value, path, c.Domain);
			this._cookies.Add(u, nc);
		}
	}
}