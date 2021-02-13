#if net40
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Represents a multipart/form-data content. Even if a collection of HttpContent is stored, " +
        "suffix Collection is not appropriate.")]
    public class MultipartFormDataContent : MultipartContent
    {
        private const string formData = "form-data";

        public MultipartFormDataContent()
            : base(formData)
        {
        }

        public MultipartFormDataContent(string boundary)
            : base(formData, boundary)
        { 
        }

        public override void Add(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            Contract.EndContractBlock();

            if (content.Headers.ContentDisposition == null)
            {
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(formData);
            }

            base.Add(content);
        }

        public void Add(HttpContent content, string name)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorCodes.NetHttpArgumentEmptyString, "name");
            }
            Contract.EndContractBlock();

            AddInternal(content, name, null);
        }

        public void Add(HttpContent content, string name, string fileName)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorCodes.NetHttpArgumentEmptyString, "name");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(ErrorCodes.NetHttpArgumentEmptyString, "fileName");
            }
            Contract.EndContractBlock();

            AddInternal(content, name, fileName);
        }

        private void AddInternal(HttpContent content, string name, string fileName)
        {
            if (content.Headers.ContentDisposition == null)
            {
                ContentDispositionHeaderValue header = new ContentDispositionHeaderValue(formData);
                header.Name = name;
                header.FileName = fileName;
                header.FileNameStar = fileName;

                content.Headers.ContentDisposition = header;
            }
            base.Add(content);
        }
    }
}
#endif