using Microsoft.AspNetCore.Mvc;
using NorthWNDApi.Models.DB;
using NorthWNDApi.Models.Model;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Globalization;
using System.Linq;

namespace NorthWNDApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly NORTHWNDContext _dbContext;
        public OrdersController(NORTHWNDContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult Get([FromQuery] OrdersFilterModel filter)
        {
            var res = new List<Order>();
            var query = _dbContext.Orders.AsQueryable();
            query = from q in query
                    where
                    (string.IsNullOrEmpty(filter.CustomerId) || filter.CustomerId == q.CustomerId)
                    && (!filter.EmployeeId.HasValue || filter.EmployeeId == q.EmployeeId)
                    && (!filter.OrderId.HasValue || filter.OrderId == q.OrderId)
                    && (!filter.Freight.HasValue || filter.Freight == q.Freight)
                    && (string.IsNullOrEmpty(filter.ShipCountry) || filter.ShipCountry == q.ShipCountry)
                    select q;
            query.Skip(filter.Skip);
            if (filter.Take.HasValue)
                query.Take((int)filter.Take);
            res = query.ToList();

            return Ok(res);
        }
        [HttpGet("freights/highest")]
        public IActionResult GetHighestFreights()
        {
            var orders = _dbContext.Orders.AsQueryable();
            var res = (from o in orders
                       group o by new { o.ShipCountry } into oo
                       orderby oo.Average(o => o.Freight) descending
                       select new
                       {
                           ShipCountry = oo.Key.ShipCountry,
                           Freight = oo.Average(o => o.Freight)
                       }).Take(3).ToList();
            return Ok(res);
        }
        [HttpGet("freights/highest/{year}")]
        public IActionResult GetHighestFreights([FromRoute] int year)
        {
            if (year < 1000 || year > 10000)
                return BadRequest();
            var orders = _dbContext.Orders.AsQueryable();
            var res = (from o in orders
                       where o.OrderDate.Value.Year == year 
                       group o by new { o.ShipCountry } into oo
                       orderby oo.Average(o => o.Freight) descending
                       select new
                       {
                           ShipCountry = oo.Key.ShipCountry,
                           Freight = oo.Average(o => o.Freight)
                       }).Take(3).ToList();
            return Ok(res);

        }
        [HttpGet("freights/highest/newest")]
        public IActionResult GetNewestFreights()
        {
            var orders = _dbContext.Orders.AsQueryable();
            var max = orders.Max(x => x.OrderDate);
            var res = (from o in orders
                       where o.OrderDate<max && max.Value.AddYears(-1)<o.OrderDate
                       group o by new { o.ShipCountry } into oo
                       orderby oo.Average(o => o.Freight) descending
                       select new
                       {
                           ShipCountry = oo.Key.ShipCountry,
                           Freight = oo.Average(o => o.Freight)
                       }).Take(3).ToList();
            return Ok(res);
        }
        [HttpGet("inventory")]
        public IActionResult GetInventory()
        {
            var orders = _dbContext.Orders.AsQueryable();
            var employees = _dbContext.Employees.AsQueryable();
            var orderDetails = _dbContext.OrderDetails.AsQueryable();
            var products = _dbContext.Products.AsQueryable();
            var res = (from e in employees
                       join o in orders
                       on e.EmployeeId equals o.EmployeeId
                       join od in orderDetails
                       on o.OrderId equals od.OrderId
                       join p in products
                       on od.ProductId equals p.ProductId
                       orderby o.OrderId,p.ProductId
                       select new
                       {
                           e.EmployeeId,
                           e.LastName,
                           o.OrderId,
                           p.ProductName,
                           od.Quantity
                       }).ToList();
            return Ok(res);
                      
        }
        [HttpGet("customers/noorder")]
        public IActionResult GetCustomersByPrice()
        {
            var orders = _dbContext.Orders.AsQueryable();
            var customers = _dbContext.Customers.AsQueryable();
            var res1 = from c in customers
                       select c;
            var res2 = from c in customers
                       join o in orders
                       on c.CustomerId equals o.CustomerId
                       select c;
            var res = res1.Except(res2).ToList();
            return Ok(res);
        }
    }
}
