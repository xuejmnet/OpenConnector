using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using ShardingConnector.ProtocolCore;
using ShardingConnector.ProtocolCore.Codecs;
using ShardingConnector.ProtocolCore.Packets;
using ShardingConnector.ProtocolCore.Payloads;
using ShardingConnector.ProtocolMysql.Packets.Generics;
using ShardingConnector.ProtocolMysql.Payloads;

namespace ShardingConnector.ProtocolMysql.Codecs;

public sealed class MySqlPacketCodecEngine : IDatabasePacketCodecEngine
{
    /// <summary>
    /// 16MB
    /// </summary>
    public const int MAX_PACKET_LENGTH = 0xFFFFFF;
    /// <summary>
    /// 
    /// </summary>
    public const int PAYLOAD_LENGTH = 3;
    /// <summary>
    /// 序号长度
    /// </summary>
    public const int SEQUENCE_LENGTH = 1;
    /// <summary>
    /// 最小二进制长度
    /// </summary>
    public const int READABLE_BYTES_MIN_LENGTH = PAYLOAD_LENGTH+SEQUENCE_LENGTH;

    private readonly List<IByteBuffer> _pendingmessages = new List<IByteBuffer>();

    public bool IsValidHeader(int readableBytes)
    {
        return readableBytes >= READABLE_BYTES_MIN_LENGTH;
    }

    public void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        var payloadLength = input.MarkReaderIndex().ReadUnsignedMediumLE();
        var remainPayloadLength = SEQUENCE_LENGTH+payloadLength;
        if (input.ReadableBytes < remainPayloadLength)
        {
            input.ResetReaderIndex();
            return;
        }

        var message = input.ReadRetainedSlice(SEQUENCE_LENGTH+payloadLength);
        if (MAX_PACKET_LENGTH == payloadLength)
        {
            _pendingmessages.Add(message);
        } else if (_pendingmessages.Count == 0)
        {
            output.Add(message);
        }
        else
        {
            AggregateMessages(context, message, output);
        }
    }

    private void AggregateMessages(IChannelHandlerContext context, IByteBuffer lastMessage, List<object> output)
    {
        var result = context.Allocator.CompositeBuffer(_pendingmessages.Count+1);
        int i = 0;
        foreach (var byteBuffer in _pendingmessages)
        {
            result.AddComponent(true, i > 0? byteBuffer.SkipBytes(1):byteBuffer);
            i++;
        }

        if (lastMessage.ReadableBytes > 1)
        {
            result.AddComponent(true, lastMessage.SkipBytes(1));
        }
        output.Add(result);
        _pendingmessages.Clear();
    }

    public void Encode(IChannelHandlerContext context, IDatabasePacket<IPacketPayload> message, IByteBuffer output)
    {
        var markWriterIndex = PrepareMessageHeader(output).MarkWriterIndex();
        var payload = new MySqlPacketPayload(markWriterIndex,context.Channel.GetAttribute(CommonConstants.CHARSET_ATTRIBUTE_KEY).Get());
        try
        {
            message.Write(payload);
        }
        catch (Exception ex)
        {
            output.ResetWriterIndex();
            new MySqlErrPacket(1,common)
        }
    }

    private IByteBuffer PrepareMessageHeader(IByteBuffer output)
    {
        return output.WriteInt(0);
    }

    public IPacketPayload CreatePacketPayload(IByteBuffer message, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}