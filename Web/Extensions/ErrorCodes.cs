// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

#if net40
using System;
using System.Collections.Generic;

namespace System.Net.Http
{
    internal class ErrorCodes
    {
        public const string NetHttpArgumentEmptyString = "The value cannot be null or empty.";
        public const string NetHttpClientAbsoluteBaseaddressRequired = "The base address must be an absolute URI.";
        public const string NetHttpClientContentHeaders = "Content headers";
        public const string NetHttpClientExecutionError = "An error occurred while sending the request.";
        public const string NetHttpClientHttpBaseaddressRequired = "Only 'http' and 'https' schemes are allowed.";
        public const string NetHttpClientInvalidRequesturi = "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.";
        public const string NetHttpClientOperationStarted = "The HttpClient instance already started one or more requests. Properties can only be modified before sending the first request.";
        public const string NetHttpClientRequestAlreadySent = "The request message was already sent. Cannot send the same request message multiple times.";
        public const string NetHttpClientRequestHeaders = "Request headers";
        public const string NetHttpClientResponseHeaders = "Response headers";
        public const string NetHttpClientSendCanceled = "Request for {0} was canceled.";
        public const string NetHttpClientSendCompleted = "Request for {0} completed successfully. Returning response {1}: {2}";
        public const string NetHttpClientSendError = "An error occurred while sending {0}. {1}";
        public const string NetHttpContentBuffersizeExceeded = "Cannot write more bytes to the buffer than the configured maximum buffer size: {0}.";
        public const string NetHttpContentEncodingSet = "A content encoding was applied to the content (see 'Content-Type' header). Cannot read content as string if a content encoding was applied.";
        public const string NetHttpContentFieldTooLong = "The field cannot be longer than {0} characters.";
        public const string NetHttpContentInvalidCharset = "The character set provided in ContentType is invalid. Cannot read content as string using an invalid character set.";
        public const string NetHttpContentNoTaskReturned = "The async operation did not return a System.Threading.Tasks.Task object.";
        public const string NetHttpContentReadonlyStream = "The stream does not support writing.";
        public const string NetHttpContentStreamAlreadyRead = "The stream was already consumed. It cannot be read again.";
        public const string NetHttpContentStreamCopyError = "Error while copying content to a stream.";
        public const string NetHttpCopytoArrayTooSmall = "The number of elements is greater than the available space from arrayIndex to the end of the destination array.";
        public const string NetHttpHandlerNocontentlength = "The content length of the request content can't be determined. Either set TransferEncodingChunked to true, load content into buffer, or set AllowRequestContentBuffering to true.";
        public const string NetHttpHandlerNorequest = "A request message must be provided. It cannot be null.";
        public const string NetHttpHandlerNoresponse = "Handler did not return a response message.";
        public const string NetHttpHandlerNotAssigned = "The inner handler has not been assigned.";
        public const string NetHttpHandlerCantAddAnyToCollection = "Cannot add the 'Any' entity tag to a collection with items. The 'Any' entity tag can only be added to an empty collection.";
        public const string NetHttpHandlerInvalidEtagName = "The specified value is not a valid quoted string.";
        public const string NetHttpHandlerInvalidFromHeader = "The specified value is not a valid 'From' header string.";
        public const string NetHttpHeadersInvalidHeaderName = "The header name format is invalid.";
        public const string NetHttpHeadersInvalidHostHeader = "The specified value is not a valid 'Host' header string.";
        public const string NetHttpHeadersInvalidRange = "Invalid range. At least one of the two parameters must not be null.";
        public const string NetHttpHeadersInvalidValue = "The format of value '{0}' is invalid.";
        public const string NetHttpHeadersNoNewlines = "New-line characters in header values must be followed by a white-space character.";
        public const string NetHttpHeadersNotAllowedHeaderName = "The header cannot be added. Make sure to add request headers to HttpRequestMessage, response headers to HttpResponseMessage, and content headers to HttpContent objects.";
        public const string NetHttpHeadersNotFound = "The given header was not found.";
        public const string NetHttpHeadersSingleValueHeader = "Cannot add value because header '{0}' does not support multiple values.";
        public const string NetHttpHttpmethodFormatError = "The format of the HTTP method is invalid.";
        public const string NetHttpInvalidEnableFirst = "The {0} property must be set to '{1}' to use this property.";
        public const string NetHttpLogContentNoTaskReturnedCopytoasync = "Type '{0}.CopyToAsync()' did not return a System.Threading.Tasks.Task object.";
        public const string NetHttpLogContentNull = "Content set to '<null>'.";
        public const string NetHttpLogContentOffloadAsync = "Offloading asynchronous operations to avoid stack overflow.";
        public const string NetHttpLogHeadersInvalidQuality = "The 'q' value is invalid: '{0}'.";
        public const string NetHttpLogHeadersInvalidValue = "Value for header '{0}' has invalid format. Value: '{1}'.";
        public const string NetHttpLogHeadersNoNewlines = "Value for header '{0}' contains invalid new-line characters. Value: '{1}'.";
        public const string NetHttpLogHeadersWrongEmailFormat = "Value '{0}' is not a valid email address. Error: {1}";
        public const string NetHttpMessageNotSuccessStatuscode = "Response status code does not indicate success: {0} ({1}).";
        public const string NetHttpParserInvalidBase64String = "Value '{0}' is not a valid Base64 string. Error: {1}";
        public const string NetHttpParserInvalidDateFormat = "The input does not contain a valid string representation of a date and time: '{0}'.";
        public const string NetHttpReadError = "The read operation failed, see inner exception.";
        public const string NetHttpReasonphraseFormatError = "The reason phrase must not contain new-line characters.";
        public const string NetLogException = "Exception in {0}::{1} - {2}.";
    }
}
#endif