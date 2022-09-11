using NCDC.CommandParser.Abstractions;
using NCDC.CommandParserBinder.Command;
using NCDC.Sharding.Rewrites.Sql.Token.SimpleObject;

namespace NCDC.Sharding.Rewrites.Sql.Token.Generator
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: Monday, 12 April 2021 21:46:30
    * @Email: 326308290@qq.com
    */
    public interface ICollectionSqlTokenGenerator:ISqlTokenGenerator
    {
        ICollection<SqlToken> GenerateSqlTokens(ISqlCommandContext<ISqlCommand> sqlCommandContext);
    }
    public interface ICollectionSqlTokenGenerator<in T> : ICollectionSqlTokenGenerator where T : ISqlCommandContext<ISqlCommand>
    {
        ICollection<SqlToken> GenerateSqlTokens(T sqlCommandContext);
    }
}