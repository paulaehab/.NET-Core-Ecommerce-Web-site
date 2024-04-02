using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository: Repository<Product> , IProductRepository
    {

        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }



        public void Update(Product product)
        {

            var objFromDB = _db.products.FirstOrDefault(u=>u.ID==product.ID);
            if (objFromDB != null)
            {
                objFromDB.Title = product.Title;
                objFromDB.Description = product.Description;
                objFromDB.ISBN = product.ISBN;
                objFromDB.Price = product.Price;
                objFromDB.ListPrice = product.ListPrice;
                objFromDB.Price100 = product.Price100;
                objFromDB.CategoryId = product.CategoryId;
                objFromDB.Author = product.Author;

                if(product.ImageUrl != null)
                {
                    objFromDB.ImageUrl = product.ImageUrl;
                }

            }
        }
    }
}
