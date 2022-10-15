﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NCDC.CommandParser.Common.Value.Literal.Impl
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/20 15:38:34
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class OtherLiteralValue:ILiteralValue<string>
    {

        public OtherLiteralValue(string value)
        {
            Value = value;
        }
        public string Value { get; }
    }
}