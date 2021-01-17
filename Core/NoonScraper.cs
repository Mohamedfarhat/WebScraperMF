using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScraperMF.Models;

namespace WebScraperMF.Core
{
    public class NoonScraper : IScraperService
    {
        public async Task<List<Product>> SearchProductAsync(string searchTerm)
        {
            List<Product> pList = new List<Product>();

            ChromeOptions options = new ChromeOptions();

            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://www.noon.com/uae-en/search?q=" + searchTerm);

                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                System.Threading.Thread.Sleep(100);
                for (int i = 1000; i <= 10000; i += 1000)
                {
                    js.ExecuteScript($"window.scrollTo(0,{i});");
                    System.Threading.Thread.Sleep(30);
                }

                var userNameField = driver.FindElementByTagName("img").GetAttribute("src");
                string pageSource = driver.PageSource;


                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(pageSource);

                Console.WriteLine();

                var productsHtml = htmlDoc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Contains("productContainer")).ToList();
                foreach (var item in productsHtml)
                {

                    string pTitle = item.Descendants("div").FirstOrDefault().GetAttributeValue("title", "").Trim('\r', '\n', '\t');

                    string pUrl = item.Descendants("a")
                                    .Where(node => node.GetAttributeValue("class", "")
                                    .Contains("sc-7vj7do-0 ftlAjW")).FirstOrDefault().GetAttributeValue("href", "");

                    string pImg = item.Descendants("div")
                                        .Where(node => node.GetAttributeValue("class", "")
                                        .Equals("productImage")).FirstOrDefault().Descendants("div")
                                        .Where(node => node.GetAttributeValue("class", "")
                                        .Contains("puv25r-2 cwZEwU")).FirstOrDefault().Descendants("img").FirstOrDefault().GetAttributeValue("src", "");

                    string pCurrency = item.Descendants("span")
                                        .Where(node => node.GetAttributeValue("class", "")
                                        .Equals("currency")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                    string pPrice = item.Descendants("strong").FirstOrDefault().InnerText.Trim('\r', '\n', '\t');
                    pList.Add(new Product
                    {
                        productName = searchTerm,
                        productTitle = pTitle,
                        productUrl = "https://www.noon.com" + pUrl,
                        productImgUrl = pImg,
                        productPrice = double.Parse(pPrice),
                        productPriceCurrency = pCurrency,
                        productWebSite = "noon",
                        productSearchDate = DateTime.Now

                    });
                }

            }
            return pList;
        }
    }
}
