using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ShardingConnector.ProtocolCore.Packets;
using ShardingConnector.ProtocolCore.Payloads;

namespace ShardingConnector.ProtocolCore.Codecs;

public sealed class MessagePackEncoder:MessageToByteEncoder<IDatabasePacket<IPacketPayload>>
{
    private readonly ILogger<MessagePackEncoder> _logger;
    private readonly bool _isDebugEnabled;
    private readonly IDatabasePacketCodecEngine _databasePacketCodecEngine;

    public MessagePackEncoder(ILogger<MessagePackEncoder> logger,IDatabasePacketCodecEngine databasePacketCodecEngine)
    {
        _logger = logger;
        _isDebugEnabled=logger.IsEnabled(LogLevel.Debug);
        _databasePacketCodecEngine = databasePacketCodecEngine;
    }
    public override bool IsSharable => true;
    protected override void Encode(IChannelHandlerContext context, IDatabasePacket<IPacketPayload> message, IByteBuffer output)
    {
        // try (MySQLPacketPayload payload = new MySQLPacketPayload(context.alloc().buffer())) {
        //     message.write(payload);
        //         out.writeMediumLE(payload.getByteBuf().readableBytes());
        //         out.writeByte(message.getSequenceId());
        //         out.writeBytes(payload.getByteBuf());
        // }
        _databasePacketCodecEngine.Encode(context,message,output);
        if (_isDebugEnabled)
        {
            _logger.LogDebug($"write to client {context.Channel.Id.AsShortText()} : \n{ByteBufferUtil.PrettyHexDump(output)}");
        }
        // var mysqlPacket = message as IMysqlPacket;
        // using (var mySqlPacketPayload = new MySqlPacketPayload(context.Allocator.Buffer(),Encoding.Default))
        // {
        //     mysqlPacket.Write(mySqlPacketPayload);
        //     var byteBuffer = mySqlPacketPayload.GetByteBuffer();
        //     output.WriteMediumLE(byteBuffer.ReadableBytes);
        //     output.WriteByte(mysqlPacket.GetSequenceId());
        //     output.WriteBytes(byteBuffer);
        // }
    }
}