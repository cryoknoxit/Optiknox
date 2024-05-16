using Azure.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OptiKnoxAPI.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OptiKnoxAPI.Models
{
    public class clsDecryption
    {
        private readonly RequestDelegate _next;
        //private static readonly Logger objLogger = new Logger();
        //private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public clsDecryption(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            string contentType = httpContext.Request.ContentType;
            if(contentType != null && contentType.Contains("application/json"))
            {
                httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
                if (httpContext.Request.QueryString.HasValue)
                {
                    string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
                    var result = decryptedString.Split("&tenantName");
                    var newUrl = result[0]; clsSingleton.TenantName = result[1].ToString().Replace("=", "");

                    httpContext.Request.QueryString = new QueryString($"?{newUrl}");
                    //httpContext.Request.QueryString = new QueryString(newUri.Query);
                    // httpContext.Request.QueryString = new QueryString($ "?{decryptedString}");
                }
            }
            
            await _next(httpContext);

            //string tenantName = httpContext.Request.Query["tenantName"].ToString();
            //clsSingleton.TenantName = tenantName;

            //var requestUrl = httpContext.Request.GetEncodedUrl();
            //string url = requestUrl;
            //// Remove the specified parameter from the URL
            //var newUrl = RemoveParameterFromUrl(url, parameterToRemove);
            //var newUri = new Uri(newUrl);
            ////HttpContext newhttpContext = newUri;
            //httpContext.Request.QueryString = new QueryString(newUri.Query);
            //await _next(httpContext);
        }
        // Specify the parameter you want to remove
        string parameterToRemove = "tenantName";
        static string RemoveParameterFromUrl(string url, string parameterToRemove)
        {
            var uriBuilder = new UriBuilder(url);
            var queryParams = uriBuilder.Query.TrimStart('?').Split('&');

            // Filter out the specified parameter
            var filteredParams = queryParams
                .Where(param => !param.StartsWith(parameterToRemove + "="))
                .ToList();

            // Rebuild the query string without the specified parameter
            uriBuilder.Query = string.Join("&", filteredParams);

            return uriBuilder.ToString();
        }


        //public async Task Invoke(HttpContext httpContext)
        //{
        //    //List<string> excludeURL = GetExcludeURLList();
        //    //if (!excludeURL.Contains(httpContext.Request.Path.Value))
        //    //{
        //        httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
        //        if (httpContext.Request.QueryString.HasValue)
        //        {
        //            string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
        //            var result = decryptedString.Split("&tenantName");
        //            var newUrl = result[0]; clsSingleton.TenantName = result[1].ToString().Replace("=","");

        //            httpContext.Request.QueryString = new QueryString($"?{newUrl}");
        //            //httpContext.Request.QueryString = new QueryString(newUri.Query);
        //            // httpContext.Request.QueryString = new QueryString($ "?{decryptedString}");
        //        }
        //    //}
        //    await _next(httpContext);
        //}


        private CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();
            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);
            return cryptoStream;
        }
        static byte[] Encrypt(string plainText)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
        }
        // This are main functions that we decrypt the payload and  parameter which we pass from the angular service.
        private Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();
            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }
        private string DecryptString(string cipherText)
        {
            try
            {
                Aes aes = GetEncryptionAlgorithm();
                byte[] buffer = Convert.FromBase64String(cipherText);
                MemoryStream memoryStream = new MemoryStream(buffer);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);
                return streamReader.ReadToEnd();
            }
            catch(Exception ex) {
                return "error";
            }
           
        }
        // We have to use same KEY and IV as we use for encryption in angular side.
        // _appSettings.EncryptKey= 1203199320052021
        // _appSettings.EncryptIV = 1203199320052021
        private Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            var secret_key = Encoding.UTF8.GetBytes("1203199320052021");
            var initialization_vector = Encoding.UTF8.GetBytes("1203199320052021");
            aes.Key = secret_key;
            aes.IV = initialization_vector;
            return aes;
        }
        // This are excluded URL from encrypt- decrypt that already we added in angular side and as well as in ASP.NET CORE side.
        private List<string> GetExcludeURLList()
        {
            List<string> excludeURL = new List<string>();
            excludeURL.Add("/api/Common/commonFileuploaddata");
            excludeURL.Add("/api/Users/UploadProfilePicture");
            excludeURL.Add("/api/Common/downloadattachedfile");
            return excludeURL;
        }

    }
}
