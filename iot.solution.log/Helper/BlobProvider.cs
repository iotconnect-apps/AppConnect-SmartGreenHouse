using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace component.logger.data.log.Helper
{
    public static class BlobProvider
    {
        public static string GetFileAsString(string fileName, string BlobDomainUrl)
        {
            string content = string.Empty;

            using (WebClient webclient = new WebClient())
            {
                content = webclient.DownloadString($"{BlobDomainUrl}/public/emailtemplate/{fileName}");
            }
            return content;
        }
    }
}
