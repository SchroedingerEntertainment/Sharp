// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SE.Json;

namespace SE.Web
{
    public partial class RestClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<JsonDocument> GetJson(string requestUri)
        {
            return ParseStream(await http.GetStreamAsync(requestUri));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<JsonDocument> GetJson(Uri requestUri)
        {
            return ParseStream(await http.GetStreamAsync(requestUri));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonDocument> GetJson()
        {
            return ParseStream(await http.GetStreamAsync(string.Empty));
        }

        JsonDocument ParseStream(Stream stream)
        {
            using (StreamBuffer buffer = new StreamBuffer((Stream)stream, 128))
            {
                JsonDocument document = new JsonDocument();
                if (!document.Load(buffer))
                {
                    List<Exception> errors = CollectionPool<List<Exception>, Exception>.Get();
                    try
                    {
                        foreach (string error in document.Errors)
                        {
                            errors.Add(new Exception(error));
                        }
                        throw new AggregateException(errors);
                    }
                    finally
                    {
                        CollectionPool<List<Exception>, Exception>.Return(errors);
                    }
                }
                else return document;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<Any<JsonDocument>> TryGetJson(string requestUri)
        {
            Any<Stream> stream = await TryGetStream(requestUri);
            if (stream)
            {
                return TryParseStream(stream.Value);
            }
            else return Any<JsonDocument>.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<Any<JsonDocument>> TryGetJson(Uri requestUri)
        {
            Any<Stream> stream = await TryGetStream(requestUri);
            if (stream)
            {
                return TryParseStream(stream.Value);
            }
            else return Any<JsonDocument>.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Any<JsonDocument>> TryGetJson()
        {
            Any<Stream> stream = await TryGetStream(string.Empty);
            if (stream)
            {
                return TryParseStream(stream.Value);
            }
            else return Any<JsonDocument>.Empty;
        }

        Any<JsonDocument> TryParseStream(Stream stream)
        {
            using (StreamBuffer buffer = new StreamBuffer((Stream)stream, 128))
            {
                JsonDocument document = new JsonDocument();
                if (document.Load(buffer))
                {
                    return new Any<JsonDocument>(document);
                }
                else return Any<JsonDocument>.Empty;
            }
        }
    }
}
