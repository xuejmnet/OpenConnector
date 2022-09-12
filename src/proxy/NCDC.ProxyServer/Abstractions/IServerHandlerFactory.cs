using NCDC.Basic.Connection.Abstractions;
using NCDC.CommandParser.Abstractions;
using OpenConnector.Enums;

namespace NCDC.ProxyServer.Abstractions;

public interface IServerHandlerFactory
{
    IServerHandler Create(DatabaseTypeEnum databaseType, string sql, ISqlCommand sqlCommand,
        IConnectionSession connectionSession);
}