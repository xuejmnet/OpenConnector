﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShardingConnector.Kernels.Parse;
using ShardingConnector.Parser.Binder.Command;
using ShardingConnector.Parser.Sql.Command;

namespace ShardingConnector.Common.Log
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/13 14:09:48
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */

    public delegate void Log(string msg);
    public class SqlLogger
    {
        public event Log Show;

        private SqlLogger()
        {
            
        }

        private static SqlLogger _sqlLogger;

        static SqlLogger()
        {
            _sqlLogger = new SqlLogger();
        }

        public static void AddLog(Log log)
        {
            _sqlLogger.Show += log;
        }
        /// <summary>
        /// 记录sql
        /// </summary>
        /// <param name="logicSql"></param>
        /// <param name="showSimple">简单记录</param>
        /// <param name="sqlCommandContext"></param>
        /// <param name="executionUnits"></param>
        public static void LogSql(string logicSql, bool showSimple, ISqlCommandContext<ISqlCommand> sqlCommandContext, ICollection<ExecutionUnit> executionUnits)
        {
            if (_sqlLogger.Show != null)
            {
                _sqlLogger.Show($"Logic SQL: {logicSql}");
                _sqlLogger.Show($"SqlCommand: {sqlCommandContext.GetSqlCommand()}");
                if (showSimple)
                {
                    LogSimpleMode(executionUnits);
                }
                else
                {
                    LogNormalMode(executionUnits);
                }
            }
        }

        private static void LogSimpleMode(ICollection<ExecutionUnit> executionUnits)
        {
            ISet<string> dataSourceNames = new HashSet<string>();
            foreach (var executionUnit in executionUnits)
            {
                dataSourceNames.Add(executionUnit.GetDataSourceName());

            }
            _sqlLogger.Show($"Actual SQL(simple): {dataSourceNames} ::: {executionUnits.Count}");
        }

        private static void LogNormalMode(ICollection<ExecutionUnit> executionUnits)
        {
            foreach (var executionUnit in executionUnits)
            {
                if (!executionUnit.GetSqlUnit().GetParameters().Any())
                {
                    _sqlLogger.Show($"Actual SQL: {executionUnit.GetDataSourceName()} ::: {executionUnit.GetSqlUnit().GetSql()}");
                }
                else
                {
                    _sqlLogger.Show($"Actual SQL: {executionUnit.GetDataSourceName()} ::: {executionUnit.GetSqlUnit().GetSql()} ::: {string.Join(",", executionUnit.GetSqlUnit().GetParameters())}");
                }
            }
        }
    }
}