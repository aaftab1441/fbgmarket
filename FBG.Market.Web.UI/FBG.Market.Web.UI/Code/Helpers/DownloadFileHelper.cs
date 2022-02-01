using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace FBG.Market.Web.Identity.Code.Helpers
{
    public class DownloadFileHelper
    {
        public async Task DownloadFileAsync(string imageUrl, string path)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string downloadToDirectory = path;
                    webClient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    await webClient.DownloadFileTaskAsync(new Uri(imageUrl), @downloadToDirectory);

                    //Add them to the local
                    //Context.listOfLocalDirectories.Add(downloadToDirectory);
                }
            }
            catch (Exception)
            {
                //Errors.printError("Failed to download File: " + doc.docName);
            }
        }

        public async Task DownloadMultipleFilesAsync(List<string> imageUrls, string path)
        {
            await Task.WhenAll(imageUrls.Select(doc => DownloadFileAsync(doc, path)));
        }
    }
}