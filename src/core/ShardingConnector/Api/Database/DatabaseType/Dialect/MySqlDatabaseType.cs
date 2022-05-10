﻿using System.Collections.Generic;
using ShardingConnector.Api.Database.MetaData.Dialect;
using ShardingConnector.Spi.DataBase.DataBaseType;
using ShardingConnector.Spi.DataBase.MetaData;

namespace ShardingConnector.Api.Database.DatabaseType.Dialect
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/04/19 11:06:53
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */

    /// <summary>
    /// 
    /// </summary>
    public class MySqlDatabaseType:IDatabaseType
    {
        public string GetName()
        {
            return "MySQL";
        }

        public ICollection<string> GetAdoNetUrlPrefixAlias()
        {
            return new List<string>() { "adonet:mysql:mysql:" };
        }

        public IDataSourceMetaData GetDataSourceMetaData(string url, string username)
        {
            return new MySqlDataSourceMetaData(url);
        }
    }
}