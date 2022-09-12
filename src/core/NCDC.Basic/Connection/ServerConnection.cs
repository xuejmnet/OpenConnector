using NCDC.Basic.Connection.Abstractions;
using NCDC.Enums;
using NCDC.Exceptions;
using OpenConnector.Base;
using OpenConnector.Transaction;

namespace NCDC.Basic.Connection;

public class ServerConnection : IServerConnection, IDisposable, IAdoMethodReplier<IServerDbConnection>
{
    private const int MAXIMUM_RETRY_COUNT = 5;

    private readonly OneByOneChecker _oneByOne = new OneByOneChecker();

    private TransactionTypeEnum _transactionType;
    private bool _supportHint;
    public int ConnectionId { get; set; }
    public string UserName { get; set; }

    public IDictionary<string, List<IServerDbConnection>> CachedConnections { get; } =
        new Dictionary<string, List<IServerDbConnection>>();

    public ServerConnection(IConnectionSession connectionSession)
    {
        _transactionType = TransactionTypeEnum.LOCAL;
        ConnectionSession = connectionSession;
        Replier = new LinkedList<Action<IServerDbConnection>>();
    }

    public IConnectionSession ConnectionSession { get; }

    public List<IServerDbConnection> GetConnections(ConnectionModeEnum connectionMode, string dataSourceName,
        int connectionSize)
    {
        OneByOneLock();
        try
        {

            if (!CachedConnections.TryGetValue(dataSourceName, out var cachedConnections))
            {
                cachedConnections = new List<IServerDbConnection>(Math.Max(connectionSize * 2,
                    Environment.ProcessorCount));
                CachedConnections.TryAdd(dataSourceName, cachedConnections);
            }

            if (cachedConnections.Count >= connectionSize)
            {
                return cachedConnections.Take(connectionSize).ToList();
            }
            else
            {
                var dbConnections = new List<IServerDbConnection>(connectionSize);
                dbConnections.AddRange(cachedConnections);
                var serverDbConnections = GetServerDbConnectionFromContext(connectionMode, dataSourceName,
                    connectionSize - cachedConnections.Count());
                dbConnections.AddRange(serverDbConnections);
                cachedConnections.AddRange(serverDbConnections);
                return dbConnections;
            }
        }
        finally
        {
            OneByOneUnLock();
        }
    }

    private List<IServerDbConnection> GetServerDbConnectionFromContext(ConnectionModeEnum connectionMode,
        string dataSourceName, int connectionSize)
    {
        if (ConnectionSession.LogicDatabase == null)
        {
            throw new ArgumentException("current logic database is null");
        }

        return ConnectionSession.LogicDatabase.GetServerDbConnections(connectionMode, dataSourceName, connectionSize,
            _transactionType);
    }

    public LinkedList<Action<IServerDbConnection>> Replier { get; }
    private void OneByOneLock()
    {
        //是否触发并发了
        var acquired = _oneByOne.Start();
        if (!acquired)
        {
            throw new ShardingException($"{nameof(OneByOneLock)} cant parallel use in connection ");
        }

    }
    private void OneByOneUnLock()
    {
        _oneByOne.Stop();
    }
    public void Dispose()
    {
        CloseCurrentCommandReader();
        CachedConnections.Clear();
    }

    public void CloseCurrentCommandReader()
    {
        foreach (var serverConnectionCachedConnection in CachedConnections)
        {
            foreach (var serverDbConnection in serverConnectionCachedConnection.Value)
            {
                serverDbConnection.ServerDbDataReader?.Dispose();
                serverDbConnection.ServerDbCommand?.Dispose();
            }
        }
    }
}