﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopProjectAsp_PhamVanLinh.Data;
using ShopProjectAsp_PhamVanLinh.Models;
using ShopProjectAsp_PhamVanLinh.ViewModels;
using PagedList;

namespace ShopProjectAsp_PhamVanLinh.Controllers
{
    public class ProductController : Controller
    {
        private readonly string _datafile = @"Data\data.xml";           

        public HashSet<Product> products { get; set; }

        public Product Get(int id) => products.FirstOrDefault(b => b.Id == id);

        private readonly Context _context;

        public ProductController(Context context)
        {
            _context = context;
        }
        // GET: Product
        public async Task<IActionResult> Index(int? id, int? page) 
        {
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Categories = _context.categories.ToList();
            //var cate = _context.categories.ToListAsync();
            if (id == null || id == 0)
            {
                viewModel.Products = _context.products.ToList();
                
                return View(viewModel);
            }
            else
            {
                viewModel.Products = _context.products.Where(h => h.Category_id == id);

                return View(viewModel);
            }
            if (page == null) page = 1;

            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(viewModel.Products.ToPagedList(pageNumber, pageSize));
            //return View(await _context.products.ToListAsync());
        }


        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,img,Description,Price,Quantity,Category_id")] Product product, IFormFile file) 
        {
            if (ModelState.IsValid)
            {
                Upload(product, file);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,img,Description,Price,Quantity,Category_id")] Product product, IFormFile file)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Upload(product, file);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.products == null)
            {
                return Problem("Entity set 'Context.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.products.Any(e => e.Id == id);
        }

        public void Upload(Product product, IFormFile file)
        {
            if (file != null)
            {
                var path = GetDataPath(file.FileName);

                using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);

                file.CopyTo(stream);

                product.img = $"\\images\\{file.FileName}";
            }
        }
        public string GetDataPath(string file) => $"wwwroot\\images\\{file} ";

    }
}
