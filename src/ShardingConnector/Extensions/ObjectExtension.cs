﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShardingConnector.Extensions
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 12:35:24
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public static class ObjectExtension
    {
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null)
                return false;
            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }
    }
}
