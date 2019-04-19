using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework.EFCore.Entity.Db.Extensions
{
    public class OrdersExtensions : Orders
    {
        public IEnumerable<Product> Product { get; set; }
    }
}
