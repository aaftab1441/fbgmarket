using FBG.Market.Web.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FBG.Market.Web.Identity.Client
{
    public class WebAPIClient
    {
        //static HttpClient _client = new HttpClient();


        public static async Task<Uri> UpdateProductAsync(Product product, string ulr)
        {
            var httpClientHandler = new HttpClientHandler
            {
                // Return `true` to allow certificates that are untrusted/invalid
                ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(httpClientHandler);
            HttpResponseMessage response = await client.PutAsJsonAsync(
               ulr, product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        public static async Task<Uri> CreateProductAsync(Product product, string ulr)
        {
            var httpClientHandler = new HttpClientHandler
            {
                // Return `true` to allow certificates that are untrusted/invalid
                ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(httpClientHandler);
            HttpResponseMessage response = await client.PostAsJsonAsync(
               ulr, product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        public static async Task<Product> GetProductAsync(int pid, string path)
        {
            Product product = null;

            try
            {
                var httpClientHandler = new HttpClientHandler
                {
                    // Return `true` to allow certificates that are untrusted/invalid
                    ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                HttpClient client = new HttpClient(httpClientHandler);
                HttpResponseMessage response = await client.GetAsync(path+"/"+pid);
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsAsync<Product>();
                }
            }
            catch (Exception ex)
            {

            }
            return product;
        }

        public static async Task<List<Product>> GetAllProductsAsync(string path)
        {
            List<Product> products = null;
            var httpClientHandler = new HttpClientHandler
            {
                // Return `true` to allow certificates that are untrusted/invalid
                ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            HttpClient client = new HttpClient(httpClientHandler);
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    products = await response.Content.ReadAsAsync<List<Product>>();
                }
            }catch(Exception ex)
            {

            }
            return products;
        }
    }
}