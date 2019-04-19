
namespace EntityFramework.EFCore.Entity.Filter
{
    public class OrderFilter : PageFilter

    {
        public string Remake { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sort { get; set; } = "Id";

        /// <summary>
        /// 排序类型
        /// </summary>
        public OrderDirection SortType { get; set; } = OrderDirection.DESC;
    }

    public enum OrderDirection
    {
        /// <summary>正序</summary>
        AES,
        /// <summary>倒序</summary>
        DESC,
    }
}