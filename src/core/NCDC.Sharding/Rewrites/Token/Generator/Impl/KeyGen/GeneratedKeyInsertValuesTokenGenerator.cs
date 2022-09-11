using OpenConnector.Base;
using NCDC.CommandParser.Command.DML;
using NCDC.ShardingParser.Command.DML;
using NCDC.ShardingParser.Segment.Insert.Values.Expression;
using OpenConnector.Extensions;
using NCDC.Sharding.Rewrites.Sql.Token.SimpleObject;
using NCDC.Sharding.Rewrites.Sql.Token.SimpleObject.Generic;

namespace NCDC.ShardingRewrite.Token.Generator.Impl.KeyGen
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 27 April 2021 21:05:46
* @Email: 326308290@qq.com
*/
    public sealed class GeneratedKeyInsertValuesTokenGenerator:BaseGeneratedKeyTokenGenerator
    {
        private readonly ICollection<SqlToken> _previousSqlTokens;

        public GeneratedKeyInsertValuesTokenGenerator(ICollection<SqlToken> previousSqlTokens)
        {
            _previousSqlTokens = previousSqlTokens;
        }
        public override SqlToken GenerateSqlToken(InsertCommandContext sqlCommandContext)
        {
            var result = FindPreviousSqlToken();
           ShardingAssert.ShouldBeNotNull(result,"not find PreviousSqlToken");
            var generatedKey = sqlCommandContext.GetGeneratedKeyContext();
           ShardingAssert.ShouldBeNotNull(generatedKey,"generatedKey is required");
           var generatedValues = generatedKey.GetGeneratedValues().Reverse().GetEnumerator();
            int count = 0;
            foreach (var insertValueContext in sqlCommandContext.GetInsertValueContexts())
            {
                var insertValueToken = result.InsertValues[count];
                IDerivedSimpleExpressionSegment expressionSegment;
                if (IsToAddDerivedLiteralExpression(sqlCommandContext, count))
                {
                    expressionSegment = new DerivedLiteralExpressionSegment(generatedValues.Next());
                }
                else
                {
                    expressionSegment = new DerivedParameterMarkerExpressionSegment(insertValueContext.GetParametersCount(), string.Empty);
                }
                insertValueToken.GetValues().Add(expressionSegment);
                count++;
            }
            return result;
        }
    
        private InsertValuesToken FindPreviousSqlToken() {
            foreach (var previousSqlToken in _previousSqlTokens)
            {
                if (previousSqlToken is InsertValuesToken insertValuesToken)
                    return insertValuesToken;
            }
            return null;
        }
    
        private bool IsToAddDerivedLiteralExpression(InsertCommandContext insertCommandContext, int insertValueCount) {
            return insertCommandContext.GetGroupedParameters()[insertValueCount].IsEmpty();
        }

        public override bool IsGenerateSqlToken(InsertCommand insertCommand)
        {
            return insertCommand.Values.Any();
        }
    }
}