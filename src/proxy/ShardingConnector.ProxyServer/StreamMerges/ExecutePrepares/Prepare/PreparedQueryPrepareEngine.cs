using ShardingConnector.CommandParser.SqlParseEngines;
using ShardingConnector.Common.Config.Properties;
using ShardingConnector.Common.MetaData;
using ShardingConnector.Common.Rule;
using ShardingConnector.Route;
using ShardingConnector.Route.Context;
using ShardingConnector.ShardingAdoNet;

namespace ShardingConnector.ProxyServer.StreamMerges.ExecutePrepares.Prepare
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 13 April 2021 21:36:17
* @Email: 326308290@qq.com
*/
    public sealed class PreparedQueryPrepareEngine:ProxyServer.StreamMerges.ExecutePrepares.Prepare.BasePrepareEngine
    {
        public PreparedQueryPrepareEngine(ICollection<IBaseRule> rules, ConfigurationProperties properties, ShardingConnectorMetaData metaData, SqlParserEngine sqlParserEngine) : base(rules, properties, metaData, sqlParserEngine)
        {
        }

        protected override ParameterContext CloneParameters(ParameterContext parameterContext)
        {
            return parameterContext.CloneParameterContext();
        }

        protected override RouteContext Route(DataNodeRouter router, string sql, ParameterContext parameterContext)
        {
            return router.Route(sql, parameterContext,true);
        }
    }
}