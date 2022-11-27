using System.Text;
using NCDC.Protocol.Common;

namespace NCDC.Protocol.MySql.Constant.CharacterSets;

public class MySqlCharacterSet:Enumeration<CharacterSetEnum,MySqlCharacterSet>
{
    public static MySqlCharacterSet NONE = new MySqlCharacterSet(CharacterSetEnum.None,Encoding.Default,"NONE");
    public static MySqlCharacterSet BIG5_CHINESE_CI = new MySqlCharacterSet(CharacterSetEnum.Big5ChineseCaseInsensitive,Encoding.GetEncoding("big5"),"BIG5_CHINESE_CI");
    public static MySqlCharacterSet LATIN2_CZECH_CS = new MySqlCharacterSet(CharacterSetEnum.Latin2CzechCaseSensitive,Encoding.GetEncoding("latin2"),"LATIN2_CZECH_CS");
    public static MySqlCharacterSet UTF8_GENERAL_CI = new MySqlCharacterSet(CharacterSetEnum.Utf8Mb3GeneralCaseInsensitive, Encoding.UTF8,"UTF8_GENERAL_CI");
    public static MySqlCharacterSet UTF8MB4_GENERAL_CI = new MySqlCharacterSet(CharacterSetEnum.Utf8Mb4GeneralCaseInsensitive, Encoding.UTF8,"UTF8MB4_GENERAL_CI");

    private MySqlCharacterSet(CharacterSetEnum value, Encoding encoding, string name) : base(value, encoding, name)
    {
    }
}