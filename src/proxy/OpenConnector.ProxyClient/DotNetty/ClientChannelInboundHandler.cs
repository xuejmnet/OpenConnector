using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using OpenConnector.Logger;
using OpenConnector.Protocol.MySql.Constant;
using OpenConnector.Protocol.MySql.Packet.Generic;
using OpenConnector.ProxyClient.Command;
using OpenConnector.ProxyServer;
using OpenConnector.ProxyServer.Session;
using OpenConnector.ShardingCommon.User;
using OpenConnector.Transaction;

namespace OpenConnector.ProxyClient.DotNetty;

public class ClientChannelInboundHandler:ChannelHandlerAdapter
{
    private static readonly ILogger<ClientChannelInboundHandler> _logger =
        InternalLoggerFactory.CreateLogger<ClientChannelInboundHandler>();
    private readonly IDatabaseProtocolClientEngine _databaseProtocolClientEngine;

    private readonly ConnectionSession _connectionSession;
    // private readonly IAuthenticationEngine _authenticationEngine = new MySqlAuthenticationEngine();

    private  volatile bool _authenticated;
    public ClientChannelInboundHandler(IDatabaseProtocolClientEngine databaseProtocolClientEngine,ISocketChannel channel)
    {
        _databaseProtocolClientEngine = databaseProtocolClientEngine;
        _connectionSession = new ConnectionSession(TransactionTypeEnum.LOCAL,channel);
    }
    public override void ChannelActive(IChannelHandlerContext context)
    {
        var connectionId = _databaseProtocolClientEngine.GetAuthenticationEngine().Handshake(context);
        Console.WriteLine($"connectionId:{connectionId}");
        _connectionSession.SetConnectionId(connectionId);
    }


    public override void ChannelRead(IChannelHandlerContext ctx, object msg)
    {
        var byteBuffer = (IByteBuffer)msg;
        // var s = byteBuffer.ToString(Encoding.UTF8);
        // Console.WriteLine(s);
         // byteBuffer.Retain();
        // Console.WriteLine("收到消息");
        Console.WriteLine("authorized:"+_authenticated);
        if (!_authenticated)
        {
            _authenticated = Authenticate(ctx, byteBuffer);
            return;
        }

        Console.WriteLine("认证："+_authenticated);
        var clientCommand = new ClientCommandExecutor(_databaseProtocolClientEngine,_connectionSession,ctx,byteBuffer);
        Task.Run(async () => await clientCommand.ExecuteAsync().ConfigureAwait(false));
        // var appCommand = new AppCommand(this._serverConnection,ctx,msg);
        // Task.Run(async () =>
        // {
        //     await appCommand.ExecuteAsync();
        // });

        // var byteBuffer = (IByteBuffer)msg;
        // var s = byteBuffer.ToString(Encoding.Default);
        // Console.WriteLine(s);
        // byteBuffer.Retain();
    }

    private bool Authenticate(IChannelHandlerContext context,IByteBuffer message)
    {
        using (var payload = _databaseProtocolClientEngine.GetPacketCodec().CreatePacketPayload(message,context.GetAttribute(CommonConstants.CHARSET_ATTRIBUTE_KEY).Get()))
        {
            try
            {
                var authenticationResult = _databaseProtocolClientEngine.GetAuthenticationEngine().Authenticate(context,payload);
                if (authenticationResult.Finished)
                {
                    _connectionSession.SetGrantee(new Grantee(authenticationResult.Username,authenticationResult.Hostname));
                    _connectionSession.SetCurrentDatabaseName(authenticationResult.Database);
                }

                return authenticationResult.Finished;
            }
            catch (Exception e)
            {
                Console.WriteLine("exception occur:");
                Console.WriteLine($"{e}");
                context.WriteAndFlushAsync(new MySqlErrPacket(1, MySqlServerErrorCode.ER_NO_DB_ERROR));
                context.CloseAsync();
            }
        }

        return false;
    }

    public override void ChannelWritabilityChanged(IChannelHandlerContext context)
    {
        _logger.LogWarning($"current channel writable changed:{context.Channel.IsWritable},connection id:{_connectionSession.GetConnectionId()}");
        if (context.Channel.IsWritable)
        {
            _connectionSession.NotifyChannelIsWritable();
        }
    }
}