using System.Collections.Immutable;
using System.Text;
using Microsoft.Extensions.Logging;
using ShardingConnector.Logger;

namespace ShardingConnector.ProtocolMysql.Constant;

public class MySqlCharacterSet
{
    private static readonly ILogger<MySqlCharacterSet>
        _logger = InternalLoggerFactory.CreateLogger<MySqlCharacterSet>();
    public int DbEncoding { get; }
    public Encoding Charset { get; }

    public MySqlCharacterSet(int dbEncoding,Encoding languageEncoding)
    {
        DbEncoding = dbEncoding;
        Charset = languageEncoding;
    }
    public MySqlCharacterSet(int dbEncoding,string encodingName)
    {
        DbEncoding = dbEncoding;
        try
        {
            Charset = Encoding.GetEncoding(encodingName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"not support languageEncoding:{encodingName},{ex}");
        }
    }

    public static MySqlCharacterSet BIG5_CHINESE_CI { get; } = new MySqlCharacterSet(1, "big5");
    public static MySqlCharacterSet LATIN2_CZECH_CS { get; } = new MySqlCharacterSet(2, "latin2");
    public static MySqlCharacterSet UTF8_GENERAL_CI { get; } = new MySqlCharacterSet(33, Encoding.UTF8);
    public static MySqlCharacterSet UTF8MB4_GENERAL_CI { get; } = new MySqlCharacterSet(45, Encoding.UTF8);

    private static readonly ImmutableDictionary<int, MySqlCharacterSet> _characterSets;

    static MySqlCharacterSet()
    {
        _characterSets = ImmutableDictionary.CreateRange<int, MySqlCharacterSet>(new Dictionary<int, MySqlCharacterSet>()
        {
            {BIG5_CHINESE_CI.DbEncoding,BIG5_CHINESE_CI},
            {LATIN2_CZECH_CS.DbEncoding,LATIN2_CZECH_CS},
            {UTF8_GENERAL_CI.DbEncoding,UTF8_GENERAL_CI},
            {UTF8MB4_GENERAL_CI.DbEncoding,UTF8MB4_GENERAL_CI}
        });
    }

    public static MySqlCharacterSet FindById(int id)
    {
        if (!_characterSets.TryGetValue(id, out var mySqlCharacterSet))
        {
            throw new NotSupportedException($"character set corresponding to dbEncoding {id} not found");
        }

        return mySqlCharacterSet;
    }
}