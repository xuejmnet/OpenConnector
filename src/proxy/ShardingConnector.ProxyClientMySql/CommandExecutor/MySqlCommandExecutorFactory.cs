using ShardingConnector.Common;
using ShardingConnector.ProtocolCore.Packets;
using ShardingConnector.ProtocolCore.Packets.Executor;
using ShardingConnector.ProtocolCore.Payloads;
using ShardingConnector.ProtocolMysql.Packet.Command;
using ShardingConnector.ProtocolMysql.Packet.ServerCommand.Query;
using ShardingConnector.ProtocolMysql.Payload;
using ShardingConnector.ProxyClient.Abstractions;
using ShardingConnector.ProxyClientMySql.Command.Query.Text.Query;
using ShardingConnector.ProxyClientMySql.ServerCommand.Query;
using ShardingConnector.ProxyClientMySql.ServerCommand.Quit;
using ShardingConnector.ProxyServer.Abstractions;
using ShardingConnector.ProxyServer.Session;

namespace ShardingConnector.ProxyClientMySql.CommandExecutor;

public sealed class MySqlCommandExecutorFactory:ICommandExecutorFactory
{
    private readonly ITextProtocolHandlerFactory _textProtocolHandlerFactory;

    public MySqlCommandExecutorFactory(ITextProtocolHandlerFactory textProtocolHandlerFactory)
    {
        _textProtocolHandlerFactory = textProtocolHandlerFactory;
    }
    public ICommandExecutor Create(IPacketPayload payload, ConnectionSession connectionSession)
    {
        var mySqlPacketPayload = (MySqlPacketPayload)payload;
        var commandType = GetCommandType(mySqlPacketPayload);
        return CreateServerCommandExecutor(commandType, mySqlPacketPayload, connectionSession);
    }

    public ICommandExecutor CreateServerCommandExecutor(MySqlCommandTypeEnum commandType, MySqlPacketPayload payload,ConnectionSession connectionSession)
    {
        switch (commandType)
        {
            case MySqlCommandTypeEnum.COM_QUERY:return new MySqlQueryServerCommandExecutor(new MySqlQueryServerCommandPacket(payload),
                connectionSession, _textProtocolHandlerFactory);
            case MySqlCommandTypeEnum.COM_QUIT: return new MySqlQuitServerCommandExecutor();
        }

        throw new NotSupportedException();
    }

    
    private MySqlCommandTypeEnum GetCommandType(MySqlPacketPayload payload)
    {
        return MySqlCommandTypeProvider.GetMySqlCommandType(payload);
    }
}