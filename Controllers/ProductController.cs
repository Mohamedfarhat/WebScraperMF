using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebScraperMF.Data;
using WebScraperMF.Models;

namespace WebScraperMF.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public async Task<IActionResult> searchProduct(string term)
        {
            var list = await _context.Products.Where(p => p.productName.Equals(term) && p.productSearchDate.Date >= DateTime.Today).OrderByDescending(o => o.productPrice).ToListAsync();
            if (list.Count > 0)
            {
                return View("Index", list);
            }
            else
            {
                List<Product> ppp = await SearchAmazonAsync(term);
                IEnumerable<Product> p = ppp.OrderByDescending(o => o.productPrice);
                _context.AddRange(p);
                await _context.SaveChangesAsync();
                //var newPList = searchAmazonAsync(term);
                return View("Index", p);
            }
        }




        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.productId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.productId == id);
        }

        public static async Task<List<Product>> SearchAmazonAsync(string searchTerm)
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
