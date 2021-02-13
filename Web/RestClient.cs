// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SE.Web
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RestClient : IDisposable
    {
        public const string JsonMimeType = "application/json";

        protected readonly HttpClient http;

        /// <summary>
        /// 
        /// </summary>
        public Uri BaseAddress
        {
            get { return http.BaseAddress; }
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> Language
        {
            get { return http.DefaultRequestHeaders.AcceptLanguage; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AccessToken
        {
            get 
            {
                if (http.DefaultRequestHeaders.Authorization != null)
                {
                    return http.DefaultRequestHeaders.Authorization.Parameter;
                }
                else return string.Empty;
            }
            set { http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpRequestHeaders Headers
        {
            get { return http.DefaultRequestHeaders; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Timeout
        {
            get { return http.Timeout; }
            set { http.Timeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public RestClient(Uri target)
        {
            switch (target.Scheme.ToLowerInvariant())
            {
                case "http": break;
                case "https": if (!SslContext.Enabled)
                    {
                        goto default;
                    }
                    break;
                default: throw new UriFormatException();
            }
            HttpClientHandler handler = new HttpClientHandler { AllowAutoRedirect = true };
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            this.http = new HttpClient(handler);
            this.http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMimeType));
            this.http.BaseAddress = target;
        }
        public void Dispose()
        {
            http.Dispose();
        }
    }
}
