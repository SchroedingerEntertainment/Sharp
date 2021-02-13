// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SE.Web
{
    public partial class RestClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public Task<Stream> GetStream(string requestUri)
        {
            return http.GetStreamAsync(requestUri);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public Task<Stream> GetStream(Uri requestUri)
        {
            return http.GetStreamAsync(requestUri);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetStream()
        {
            return http.GetStreamAsync(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public Task<Any<Stream>> TryGetStream(string requestUri)
        {
            return http.GetStreamAsync(requestUri)
                       .TryGetResult();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public Task<Any<Stream>> TryGetStream(Uri requestUri)
        {
            return http.GetStreamAsync(requestUri)
                       .TryGetResult();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Any<Stream>> TryGetStream()
        {
            return http.GetStreamAsync(string.Empty)
                       .TryGetResult();
        }
    }
}
