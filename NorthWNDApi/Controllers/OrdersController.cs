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
    public class OrdersController : ControllerBase
    {
        private readonly NORTHWNDContext dbContext;
        [HttpGet]
        public IActionResult Get([FromQuery] OrdersFilterModel filter)
        {
            var res = new List<Order>();
            var query = dbContext.Orders.AsQueryable();
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
    }
}
