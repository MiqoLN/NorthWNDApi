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
    public class CustomerController : ControllerBase
    {
        private readonly NORTHWNDContext _dbContext;
        public CustomerController(NORTHWNDContext context)
        {
            _dbContext = context;
        }
        [HttpGet("country/city")]
        public IActionResult GetCountriesWithCities()
        {
            var customers = _dbContext.Customers.AsQueryable();
            var res = (from c in customers
                       group c by new { c.Country, c.City } into cc
                       orderby cc.Count() descending
                       select new
                       {
                           Country = cc.Key.Country,
                           City = cc.Key.City,
                           Count = cc.Count()
                       }).ToList();
            return Ok(res);
        }
        [HttpGet("regions")]
        public IActionResult GetRegions()
        {
            var customers = _dbContext.Customers.AsQueryable();
            var res = (from c in customers
                       orderby string.IsNullOrEmpty(c.Region), c.Region
                       select new
                       {
                           CustomerId = c.CustomerId,
                           CompanyName = c.CompanyName,
                           Region = c.Region
                       }).ToList();
            return Ok(res);
        }
    }

}
