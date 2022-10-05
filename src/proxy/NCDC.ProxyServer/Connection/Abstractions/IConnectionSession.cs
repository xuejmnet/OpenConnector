using System.Collections.Immutable;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using NCDC.Basic.Metadatas;
using NCDC.ProxyServer.AppServices;
using NCDC.ProxyServer.Connection.Metadatas;
using NCDC.ProxyServer.Connection.User;
using NCDC.ProxyServer.Contexts;
using NCDC.ProxyServer.Runtimes;

namespace NCDC.ProxyServer.Connection.Abstractions;

public interface IConnectionSession:IDisposable
{
    string? DatabaseName { get; }
    IServerConnection ServerConnection { get; }
    IChannel Channel { get; }
    ILogicDatabase? LogicDatabase { get; }
    IRuntimeContext? RuntimeContext { get; }
    IAppRuntimeManager AppRuntimeManager { get; }
    IReadOnlyCollection<string> GetAllDatabaseNames();
    IReadOnlyCollection<string> GetAuthorizeDatabases();
    
    bool DatabaseExists(string database);
    bool GetIsAutoCommit();

    TransactionStatus GetTransactionStatus();

    int GetConnectionId();

    void SetConnectionId(int connectionId);

    Grantee GetGrantee();

    void SetGrantee(Grantee grantee);

    void SetCurrentDatabaseName(string? databaseName);
    Task WaitChannelIsWritableAsync(CancellationToken cancellationToken = default);
    void NotifyChannelIsWritable();

    void CloseServerConnection()
    {
        ServerConnection.CloseCurrentCommandReader();
    }
}