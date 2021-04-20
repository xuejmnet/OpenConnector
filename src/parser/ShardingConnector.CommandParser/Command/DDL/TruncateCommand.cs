using System;
using System.Collections.Generic;
using ShardingConnector.CommandParser.Segment.Generic.Table;

namespace ShardingConnector.CommandParser.Command.DDL
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 20 April 2021 21:57:53
* @Email: 326308290@qq.com
*/
    public sealed class TruncateCommand:DDLCommand
    {
        public readonly ICollection<SimpleTableSegment> Tables = new LinkedList<SimpleTableSegment>();

    }
}