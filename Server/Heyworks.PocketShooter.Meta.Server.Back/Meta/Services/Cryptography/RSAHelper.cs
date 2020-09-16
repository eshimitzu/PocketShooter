using System;
using System.Security.Cryptography;
using System.Text;

namespace Heyworks.PocketShooter.Meta.Services.Cryptography
{
    /// <summary>
    /// Represents a class responsible for signing and verifying data using RSA algorithm.
    /// </summary>
    public class RSAHelper
    {
        private readonly RSAParameters parameters;
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Initializes a new instance of the <see cref="RSAHelper"/> class.
        /// </summary>
        /// <param name="parameters">The RSA parameters.</param>
        public RSAHelper(RSAParameters parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSAHelper"/> class and export parameters from XML string.
        /// </summary>
        /// <param name="xmlString">The XML string with RSA parameters/</param>
        public RSAHelper(string xmlString)
        {
            xmlString.NotNull();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlString);

                parameters = rsa.ExportParameters(true);
            }
        }

        /// <summary>
        /// Signs the data and returns a base64 string.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        public string Sign(string data)
        {
            data.NotNull();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(parameters);

                byte[] toSign = encoding.GetBytes(data);
                byte[] signature = rsa.SignData(toSign, "SHA1");
                string base64Signature = Convert.ToBase64String(signature);

                return base64Signature;
            }
        }

        /// <summary>
        /// Verifies the specified signature data.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="base64Signature">The signature data to be verified.</param>
        /// <returns>true if the signature verifies as valid; otherwise, false.</returns>
        public bool Verify(string data, string base64Signature)
        {
            data.NotNull();
            base64Signature.NotNull();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(parameters);

                bool verified;
                try
                {
                    verified = rsa.VerifyData(encoding.GetBytes(data), "SHA1", Convert.FromBase64String(base64Signature));
                }
                catch (FormatException)
                {
                    verified = false;
                }

                return verified;
            }
        }

        /// <summary>
        /// Encrypts the specified data and returns a base64 string.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        public string Encrypt(string data)
        {
            data.NotNull();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(parameters);

                byte[] toEncrypt = encoding.GetBytes(data);
                byte[] encryptedData = rsa.Encrypt(toEncrypt, false);
                string base64EncryptedData = Convert.ToBase64String(encryptedData);

                return base64EncryptedData;
            }
        }

        /// <summary>
        /// Decrypts the base64 encrypted string.
        /// </summary>
        /// <param name="base64EncryptedData">The base64 encrypted string.</param>
        public string Decrypt(string base64EncryptedData)
        {
            base64EncryptedData.NotNull();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(parameters);

                byte[] toDecrypt = Convert.FromBase64String(base64EncryptedData);
                byte[] decryptedData = rsa.Decrypt(toDecrypt, false);
                string data = encoding.GetString(decryptedData);

                return data;
            }
        }
    }
}
