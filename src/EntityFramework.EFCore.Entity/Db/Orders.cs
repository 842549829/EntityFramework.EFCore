using System.Collections.Generic;

namespace EntityFramework.EFCore.Entity.Db
{
    public class Orders
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public int Status { get; set; }

        public string Remake { get; set; }
    }
}