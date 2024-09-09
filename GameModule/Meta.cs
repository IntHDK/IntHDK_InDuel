using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    //TODO: enum인것들은 차후에 string 형태로 뺄것

    public enum MetaGameCardCode // 카드 자체 분류
    {
        None,
        TEST_001,
        TEST_002,
        TEST_003,
        TEST_004,
        TEST_005,
        TEST_006,
        TEST_007,
        TEST_008,
        TEST_009,
        TEST_010,
        TEST_011,
        TEST_012,
        TEST_013,
        TEST_014,
        TEST_015,
        TEST_016,
        TEST_017,
        TEST_018,
        TEST_019,
        TEST_020,
        TEST_021,
        TEST_022,
        TEST_023,
    }
    public enum MetaGameCardFunctionallyName // 카드 기능에 따른 같은 종류 분류 (ex : 다른 버전 카드라도 같은 취급인 경우)
    {
        None,
        TEST_001,
        TEST_002,
        TEST_003,
        TEST_004,
        TEST_005,
        TEST_006,
        TEST_007,
        TEST_008,
        TEST_009,
        TEST_010,
        TEST_011,
        TEST_012,
        TEST_013,
        TEST_014,
        TEST_015,
        TEST_016,
        TEST_017,
        TEST_018,
        TEST_019,
        TEST_020,
        TEST_021,
        TEST_022,
        TEST_023,
    }
    public enum MetaGameCardTextCode
    {
        HQ_LVUP,
        HQ_DRAW,
        HQ_POWERUP,
        UNIT_Guardian,
        UNIT_Backdoor,
        CMD_Draw,
        CMD_DestroyUnit,
        CMD_DestroyAll,
        CMD_DamageAny,
    }
    public enum MetaEffectCode
    {
        GAMERULE_GAMELOSS_DRAWEMPTYDECK //덱 비어있을 때 드로우하면 패배처리 대기상태인 효과
    }

    public class MetaGameCardInfo
    {
        public MetaGameCardCode CardCode { get; set; }
        public MetaGameCardFunctionallyName CardFunctionallyCode { get; set; }
        public GameCardType Type { get; set; }
        public List<GameCardSubType> SubTypes { get; set; } = [];
        public int Level { get; set; }
        public int Power { get; set; }
        public int Link { get; set; }
        public List<MetaGameCardTextInfo> TextInfos { get; set; } = [];
    }
    public class MetaGameCardTextInfo
    {
        public MetaGameCardTextCode CardTextCode { get; set; }
        public List<MetaGameCardTextParameterInfo> Parameters { get; set; }
    }
    public enum MetaGameCardTextParameterType
    {
        Cost,
        Number,
    }
    public class MetaGameCardTextParameterInfo
    {
        public MetaGameCardTextParameterType ParameterType { get; set; }
        public object Value { get; set; }
    }
}
