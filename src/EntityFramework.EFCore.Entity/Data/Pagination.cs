using System.Collections.Generic;

namespace EntityFramework.EFCore.Entity.Data
{
    public class Pagination<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}