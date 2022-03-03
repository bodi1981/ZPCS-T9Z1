using InvoiceManager.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceManager.Models.Repositories
{
    public class ProductRepository
    {
        public List<Product> GetProducts()
        {
            using(var dbContext = new ApplicationDbContext())
            {
                var prods = dbContext.Products.ToList();
                return dbContext.Products.ToList();
            }
        }

        public Product GetProduct(int productId)
        {
            using(var dbContext = new ApplicationDbContext())
            {
                return dbContext.Products.Single(x => x.Id == productId);
            }
        }
    }
}