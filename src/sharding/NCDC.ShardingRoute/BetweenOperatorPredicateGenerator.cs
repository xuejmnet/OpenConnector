using NCDC.CommandParser.Segment.DML.Predicate.Value;
using NCDC.Enums;
using NCDC.ShardingAdoNet;
using NCDC.ShardingParser;
using NCDC.ShardingRoute.Expressions;
using NCDC.ShardingRoute.Helpers;

namespace NCDC.ShardingRoute;

public sealed class BetweenOperatorPredicateGenerator
{
    private BetweenOperatorPredicateGenerator()
    {
    }

    public static BetweenOperatorPredicateGenerator Instance { get; } = new BetweenOperatorPredicateGenerator();

    public RoutePredicateExpression Get(
        Func<IComparable, ShardingOperatorEnum, string, Func<string, bool>> keyTranslateFilter, string columnName,
        PredicateBetweenRightValue predicateBetweenRightValue,
        ParameterContext parameterContext)
    {
        var routePredicateExpression = RoutePredicateExpression.Default;
        IComparable? betweenStartRouteValue = new ConditionValue(predicateBetweenRightValue.BetweenExpression, parameterContext).GetValue();
        if (betweenStartRouteValue == null)
        {
            if (ExpressionConditionHelper.IsNowExpression(predicateBetweenRightValue.BetweenExpression))
            {
                betweenStartRouteValue = DateTime.Now;
            }
        }

        if (betweenStartRouteValue != null)
        {
            var translateFilter = keyTranslateFilter(betweenStartRouteValue,ShardingOperatorEnum.GREATER_THAN_OR_EQUAL,columnName);
            routePredicateExpression = routePredicateExpression.And(new RoutePredicateExpression(translateFilter));
        }
        IComparable? betweenEndRouteValue = new ConditionValue(predicateBetweenRightValue.AndExpression, parameterContext).GetValue();
        if (betweenEndRouteValue == null)
        {
            if (ExpressionConditionHelper.IsNowExpression(predicateBetweenRightValue.BetweenExpression))
            {
                betweenEndRouteValue = DateTime.Now;
            }
        }

        if (betweenEndRouteValue != null)
        {
            var translateFilter = keyTranslateFilter(betweenEndRouteValue,ShardingOperatorEnum.LESS_THAN_OR_EQUAL,columnName);
            routePredicateExpression = routePredicateExpression.And(new RoutePredicateExpression(translateFilter));
        }
        
        return routePredicateExpression;
    }
}