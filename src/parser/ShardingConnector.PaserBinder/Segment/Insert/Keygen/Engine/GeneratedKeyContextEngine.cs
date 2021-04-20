﻿using System;
using System.Collections.Generic;
using System.Linq;
using ShardingConnector.CommandParser.Command.DML;
using ShardingConnector.CommandParser.Segment.DML.Expr;
using ShardingConnector.CommandParser.Segment.DML.Expr.Simple;
using ShardingConnector.ParserBinder.MetaData.Schema;

namespace ShardingConnector.ParserBinder.Segment.Insert.Keygen.Engine
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/13 9:10:24
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class GeneratedKeyContextEngine
    {
        private readonly SchemaMetaData _schemaMetaData;

        public GeneratedKeyContextEngine(SchemaMetaData schemaMetaData)
        {
            _schemaMetaData = schemaMetaData;
        }

        /**
     * Create generate key context.
     *
     * @param parameters SQL parameters
     * @param insertStatement insert statement
     * @return generate key context
     */
        public ParserBinder.Segment.Insert.Keygen.GeneratedKeyContext CreateGenerateKeyContext(List<object> parameters, InsertCommand insertCommand)
        {
            string tableName = insertCommand.Table.GetTableName().GetIdentifier().GetValue();
            var generateKeyColumnName = FindGenerateKeyColumn(tableName);
            if (generateKeyColumnName != null)
            {
                if (ContainsGenerateKey(insertCommand, generateKeyColumnName))
                {
                    return FindGeneratedKey(parameters, insertCommand, generateKeyColumnName);
                }

                return new ParserBinder.Segment.Insert.Keygen.GeneratedKeyContext(generateKeyColumnName, true);
            }

            return null;
        }

        private string FindGenerateKeyColumn(string tableName)
        {
            if (!_schemaMetaData.ContainsTable(tableName))
            {
                return null;
            }
            foreach (var columnKV in _schemaMetaData.Get(tableName).GetColumns())
            {
                if (columnKV.Value.Generated)
                {
                    return columnKV.Key;
                }
            }
            return null;
        }

        private bool ContainsGenerateKey(InsertCommand insertCommand, string generateKeyColumnName)
        {
            if (!insertCommand.GetColumnNames().Any())
            {
                return _schemaMetaData.GetAllColumnNames(insertCommand.Table.GetTableName().GetIdentifier().GetValue())
                    .Count == insertCommand.GetValueCountForPerGroup();
            }
            return insertCommand.GetColumnNames().Contains(generateKeyColumnName);
        }

        private ParserBinder.Segment.Insert.Keygen.GeneratedKeyContext FindGeneratedKey(List<object> parameters, InsertCommand insertCommand, string generateKeyColumnName)
        {
            ParserBinder.Segment.Insert.Keygen.GeneratedKeyContext result = new ParserBinder.Segment.Insert.Keygen.GeneratedKeyContext(generateKeyColumnName, false);
            foreach (var expression in FindGenerateKeyExpressions(insertCommand, generateKeyColumnName))
            {
                if (expression is ParameterMarkerExpressionSegment parameterMarkerExpressionSegment)
                {
                    result.GetGeneratedValues().Add((IComparable)parameters[parameterMarkerExpressionSegment.GetParameterMarkerIndex()]);
                }
                else if (expression is LiteralExpressionSegment literalExpressionSegment)
                {
                    result.GetGeneratedValues().Add((IComparable)literalExpressionSegment.GetLiterals());
                }
            }
            return result;
        }

        private ICollection<IExpressionSegment> FindGenerateKeyExpressions(InsertCommand insertCommand, string generateKeyColumnName)
        {
            ICollection<IExpressionSegment> result = new LinkedList<IExpressionSegment>();
            foreach (var expression in insertCommand.GetAllValueExpressions())
            {
                result.Add(expression[FindGenerateKeyIndex(insertCommand, generateKeyColumnName.ToLower())]);
            }
            return result;
        }

        private int FindGenerateKeyIndex(InsertCommand insertCommand, string generateKeyColumnName)
        {
            if (!insertCommand.GetColumnNames().Any())
            {
                return _schemaMetaData
                    .GetAllColumnNames(insertCommand.Table.GetTableName().GetIdentifier().GetValue())
                    .IndexOf(generateKeyColumnName);
            }
            return insertCommand.GetColumnNames().IndexOf(generateKeyColumnName);
        }
    }
}