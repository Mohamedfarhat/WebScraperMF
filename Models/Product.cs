using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperMF.Models
{
    public class Product
    {
        [Key]
        public int productId { get; set; }
        public string productWebSite { get; set; }
        public string productName { get; set; }
        public string productTitle { get; set; }
        public string productUrl { get; set; }
        public string productImgUrl { get; set; }
        public double productPrice { get; set; }
        public string productPriceCurrency { get; set; }
        public DateTime productSearchDate { get; set; }
    }
}
