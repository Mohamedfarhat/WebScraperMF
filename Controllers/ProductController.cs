using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScraperMF.Core;
using WebScraperMF.Data;
using WebScraperMF.Models;

namespace WebScraperMF.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IEnumerable<IScraperService> Scrapers { get; set; }
        public ProductController(ApplicationDbContext context, IEnumerable<IScraperService> _Scrapers)
        {
            _context = context;

            Scrapers = _Scrapers;
        }

        

        public async Task<IActionResult> searchProduct(string term)
        {
        
            if(term == null)
            {
                return RedirectToAction("Index","Home");
            }
            var list = await _context.Products.Where(p => p.productName.Equals(term) && p.productSearchDate.Date >= DateTime.Today).OrderByDescending(o => o.productPrice).ToListAsync();
            if (list.Count > 0)
            {
                return View("Index", list);
            }
            else
            {
                List<Product> pAmazon = await Scrapers.First().SearchProductAsync(term);
                List<Product> pNoon = await Scrapers.Last().SearchProductAsync(term);

                if (pAmazon.Count < 0 && pNoon.Count < 0)
                {
                    return NotFound();
                }

                List<Product> pAll = new List<Product>();
                pAll.AddRange(pAmazon);
                pAll.AddRange(pNoon);

                IEnumerable<Product> p = pAll.OrderByDescending(o => o.productPrice);

                _context.AddRange(p);
                await _context.SaveChangesAsync();
                return View("Index", p);
            }
        }




    }


}
