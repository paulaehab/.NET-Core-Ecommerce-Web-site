
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
  [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)

        {
            _UnitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _UnitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           
            // in View you can only path one object 
            return View(products);
        }

        //upsert combine the update and create together
        public IActionResult Upsert(int? id)
        {
            // you will use projection here where the return items will be projected to SElectlistitem and not a list 
            // Selectlistitem is for render a list in HTML
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category.
                GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ID.ToString()
                });
            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()

            };
            // create
            if (id ==null || id == 0)
            {
                return View(productVM);
            }
            //update
            else
            {
                productVM.Product = _UnitOfWork.Product.Get(u => u.ID == id);
                return View(productVM);

            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
           
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file!=null)

                {
                    // this will create random name for the file
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        // deleter the old image
                        var oldImagePath = Path.Combine(wwwRootPath,obj.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\Images\product\" + fileName;
                }
                if (obj.Product.ID == 0)
                {
                    _UnitOfWork.Product.Add(obj.Product);

                }
                else
                {
                    _UnitOfWork.Product.Update(obj.Product);

                }

                _UnitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList= _UnitOfWork.Category.
                GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ID.ToString()
                });
                return View(obj);

            }

        }


       
/*
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product ProductFromDb = _UnitOfWork.Product.Get(u => u.ID == id);
            if (ProductFromDb == null)
            {
                return NotFound();
            }
            return View(ProductFromDb);*/
    //    }
       /* [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            *//*if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Displayorder cannot exactly match the name");
            }*/
            /*if (obj.Name == "test")
            {
                ModelState.AddModelError("", "The Displayorder cannot exactly match the name");
            }*//*

            Product? obj = _UnitOfWork.Product.Get(u => u.ID == id);
            if (obj == null)
            {
                return NotFound();
            }

            _UnitOfWork.Product.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");


        }*/

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });

        }
        public IActionResult Delete(int?id)
        {
            var productToBeDeleted = _UnitOfWork.Product.Get(u => u.ID == id);
            if (productToBeDeleted == null)
            {
                return Json(new { succes=false, message = "Error whil deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _UnitOfWork.Product.Remove(productToBeDeleted);
            _UnitOfWork.Save();
            return Json(new { succes = true, message = "Delete succeful" });
        }
        #endregion
    }
}
