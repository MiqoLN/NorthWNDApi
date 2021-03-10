using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorthWNDApi.Models.DB;

namespace NorthWNDApi.Models.Model
{
    public class OrdersFilterModel
    {
        public int? OrderId { get; set; }
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public decimal? Freight { get; set; }
        public string ShipCountry { get; set; }
        public int Skip { get; set; }
        public int? Take { get; set; }
       
    }
}
