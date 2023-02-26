using AcquireX.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static AcquireX.Models.Products;

namespace AcquireX
{
    internal static class ApiInterface
    {
        private static readonly string ClientId = ConfigurationManager.AppSettings["client_id"].ToString();
        private static readonly string ClientSecret = ConfigurationManager.AppSettings["client_secret"].ToString();
        private static Token? Token;
        private static ProductList? RSHughesProduct;
        private static ProductList? BannerProduct;
        public static async Task GenerateExportCalculationReport() 
        {
            await GenerateTokenAsync();
            await GetRSHughesProducts();
            await GetBannerProducts();
            DisplayExportCalculationReport();
        }

        public static async Task GenerateTokenAsync()
        {
            var tokenParams = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "products"),
            };
            string url = ApiEndPoints.TokenUrl;
            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(tokenParams));
            if (response.IsSuccessStatusCode)
            {
                var tokenString = await response.Content.ReadAsStringAsync();
                Token = JsonConvert.DeserializeObject<Token>(tokenString);
            }
        }

        public static async Task GetRSHughesProducts()
        {
            string url = ApiEndPoints.RSHughesUrl;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.access_token);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                RSHughesProduct = JsonConvert.DeserializeObject<ProductList>(result);
            }
        }

        public static async Task GetBannerProducts()
        {
            string url = ApiEndPoints.BannerUrl;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.access_token);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var doc = XDocument.Parse(result);
                var productNode = doc.Element("ProductsResponse");
                var jsonResult = JsonConvert.SerializeXNode(productNode);
                jsonResult = jsonResult.Remove(1, 125);
                jsonResult = jsonResult.Remove(jsonResult.LastIndexOf('}'))
                    .Replace("product" + "\"", "products" + "\"");
                BannerProduct = JsonConvert.DeserializeObject<ProductList>(jsonResult);
            }
        }

        public static void DisplayExportCalculationReport() 
        {
            var CombineProducts = RSHughesProduct.products.Join(BannerProduct.products,
                               r => r.Upc,
                                b => b.Upc,
                               (r, b) => new
                               {
                                   Upc = r.Upc,
                                   itemCode_Banner = b.ItemCode,
                                   itemCode_RSHughes = r.ItemCode
                               });
            Console.WriteLine(String.Format("{0,46}", "____________________________________________"));
            Console.WriteLine(String.Format("|{0,12}|{1,15}|{2,17}|", "UPC", "ItemCode Banner", "ItemCode RSHughes"));
            Console.WriteLine(String.Format("{0,46}", "____________________________________________"));
            foreach (var product in CombineProducts)
            {
                Console.WriteLine(String.Format("|{0,12}|{1,15}|{2,17}|", product.Upc, product.itemCode_Banner, product.itemCode_RSHughes));
            }
            Console.WriteLine(String.Format("{0,46}", "____________________________________________"));
            Console.ReadKey();
        }
    }
}
