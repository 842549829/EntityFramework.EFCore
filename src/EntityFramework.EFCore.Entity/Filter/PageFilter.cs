﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework.EFCore.Entity.Filter
{
    public class PageFilter
    {
        /// <summary>跳过条目数</summary>
        public int Skip { get; set; } = 0;

        /// <summary>获取条目数</summary>
        public int Take { get; set; } = 10;
    }
}
