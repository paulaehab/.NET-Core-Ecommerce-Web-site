﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.DataAccess.Data;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }




        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart =  new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);




        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
