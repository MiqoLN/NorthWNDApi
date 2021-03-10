using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorthWNDApi.Models.DB;
using NorthWNDApi.Models.Model;
namespace NorthWNDApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly NORTHWNDContext _dbContext;
        public ProductsController(NORTHWNDContext context)
        {
            _dbContext = context;
        }
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var products = _dbContext.Products.AsQueryable();
            var categories = _dbContext.Categories.AsQueryable();
            var res = from p in products
                      group p by p.CategoryId into pp
                      orderby pp.Count() descending
                      select new
                      {
                          Id = pp.Key,
                          Count = pp.Count()
                      };
            var final = (from r in res
                        join c in categories
                        on r.Id equals c.CategoryId
                        select new
                        {
                            Name = c.CategoryName,
                            Count = r.Count
                        }).ToList();
            return Ok(final);
        }
        [HttpGet("toreorder")]
        public IActionResult GetReorderingProducts()
        {
            var products = _dbContext.Products.AsQueryable();
            var res = (from p in products
                      where (p.UnitsInStock+p.UnitsOnOrder<p.ReorderLevel) && p.Discontinued==false
                      orderby p.ProductId
                      select p).ToList();
            return Ok(res);
        }
    }
}
