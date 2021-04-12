﻿using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Parser.Sql.Constant;
using ShardingConnector.Parser.Sql.Util;

namespace ShardingConnector.Parser.Sql.Segment.DML.Item
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 8:43:24
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class AggregationDistinctProjectionSegment:AggregationProjectionSegment
    {
        private readonly string _distinctExpression;
        public AggregationDistinctProjectionSegment(int startIndex, int stopIndex, AggregationTypeEnum type, int innerExpressionStartIndex,string distinctExpression) : base(startIndex, stopIndex, type, innerExpressionStartIndex)
        {
            this._distinctExpression = SqlUtil.GetExpressionWithoutOutsideParentheses(distinctExpression);
        }

        public string GetDistinctExpression()
        {
            return _distinctExpression;
        }
    }
}
