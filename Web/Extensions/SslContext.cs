// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Http
{
    /// <summary>
    /// SSL context
    /// </summary>
    public static class SslContext
    {
        /// <summary>
        /// Specifies the Transport Layer Security(TLS) 1.1 security protocol.The TLS 1.1 protocol is defined in IETF RFC 4346. On Windows systems, this value is supported starting with Windows 7.
        /// </summary>
        public const int Tls11 = 768;
        /// <summary>
        /// Specifies the Transport Layer Security (TLS) 1.2 security protocol. The TLS 1.2 protocol is defined in IETF RFC 5246. On Windows systems, this value is supported starting with Windows 7.
        /// </summary>
        public const int Tls12 = 3072;
        /// <summary>
        /// Specifies the TLS 1.3 security protocol. The TLS protocol is defined in IETF RFC 8446.
        /// </summary>
        public const int Tls13 = 12288;

        private static SecurityProtocolType protocols;
        /// <summary>
        /// SSL protocols
        /// </summary>
        public static SecurityProtocolType Protocols
        {
            get { return protocols; }
            set 
            {
                ServicePointManager.SecurityProtocol = value;
                protocols = value;
            }
        }

        private static X509Certificate certificate;
        /// <summary>
        /// SSL certificate
        /// </summary>
        public static X509Certificate Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }

        private static X509Certificate2Collection certificates;
        /// <summary>
        /// SSL certificates collection
        /// </summary>
        public static X509Certificate2Collection Certificates
        {
            get { return certificates; }
            set { certificates = value; }
        }

        private static RemoteCertificateValidationCallback certificateValidationCallback;
        /// <summary>
        /// SSL certificate validation callback
        /// </summary>
        public static RemoteCertificateValidationCallback CertificateValidationCallback 
        {
            get { return certificateValidationCallback; }
            set 
            {
                ServicePointManager.ServerCertificateValidationCallback = value;
                certificateValidationCallback = value;
            }
        }

        /// <summary>
        /// Determines if SSL is explicitly enabled
        /// </summary>
        public static bool Enabled
        {
            get { return (protocols != (SecurityProtocolType)0); }
        }

        static SslContext()
        {
            Protocols = SecurityProtocolType.Tls | (SecurityProtocolType)Tls11 | (SecurityProtocolType)Tls12 | (SecurityProtocolType)Tls13;
        }

        /// <summary>
        /// Accepts any SSL request
        /// </summary>
        public static bool DefaultValidator(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}