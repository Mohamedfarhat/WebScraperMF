using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebScraperMF.Models;

namespace WebScraperMF.Core
{
    public class AmazonScraper : IScraperService
    {
        public async Task<List<Product>> SearchProductAsync(string searchTerm)
        {

            string search = "";
            var array = searchTerm.Split(' ');
            if (array.Length > 0)
            {
                search = string.Join("+", array);
            }
            else
            {
                search = searchTerm;
            }
            var url = "https://www.amazon.ae/s?k=" + search + "&ref=nb_sb_noss_2";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);


            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);


            var productsHtml = htmlDoc.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("data-component-type", "")
                .Contains("s-search-results")).ToList();


            var productListItems = productsHtml[0].Descendants("div")
                .Where(node => node.GetAttributeValue("data-component-type", "")
                .Contains("s-search-result")).ToList();



            List<Product> pList = new List<Product>();
            foreach (var item in productListItems)
            {

                var isPriceExsit = (item.Descendants("span")
                                     .Where(node => node.GetAttributeValue("class", "")
                                     .Contains("a-price")).FirstOrDefault());
                string currency = "";

                var pPrice = "";
                if (isPriceExsit != null)
                {
                    currency = isPriceExsit.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("a-price-symbol")).FirstOrDefault().InnerText;

                    pPrice = isPriceExsit.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("a-price-whole")).FirstOrDefault().GetDirectInnerText();
                }
                else
                {
                    pPrice = "";
                    continue;
                }


                string pTitle = item.Descendants("span")
                                    .Where(node => node.GetAttributeValue("class", "")
                                    .Contains("a-size-base-plus a-color-base a-text-normal")).FirstOrDefault().InnerText;

                string pUrl = item.Descendants("a")
                                    .Where(node => node.GetAttributeValue("class", "")
                                    .Contains("a-link-normal s-no-outline")).FirstOrDefault().GetAttributeValue("href", "");

                string pImg = item.Descendants("img")
                                    .Where(node => node.GetAttributeValue("class", "")
                                    .Equals("s-image")).FirstOrDefault().GetAttributeValue("src", "");

                pList.Add(new Product
                {
                    productName = searchTerm,
                    productTitle = pTitle,
                    productUrl = "https://www.amazon.ae" + pUrl,
                    productImgUrl = pImg,
                    productPrice = double.Parse(pPrice),
                    productPriceCurrency = currency,
                    productWebSite = "amazon",
                    productSearchDate = DateTime.Now

                });

            }

            return pList;
        }
    }
}
