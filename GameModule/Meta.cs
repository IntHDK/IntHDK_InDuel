using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    public static class MetaConstants
    {
        public const int INITIAL_HAND_SIZE = 4;
    }
    //TODO: enum인것들은 차후에 string 형태로 뺄것

    public enum MetaEffectCode
    {
        GAMERULE_GAMELOSS_DRAWEMPTYDECK //덱 비어있을 때 드로우하면 패배처리 대기상태인 효과
    }

    public class MetaGameCardInfo
    {
        public string CardCode { get; set; }
        public string CardFunctionallyCode { get; set; }
        public GameCardType Type { get; set; }
        public List<GameCardSubType> SubTypes { get; set; } = [];
        public int Level { get; set; }
        public int Power { get; set; }
        public int Link { get; set; }
        public List<MetaGameCardTextInfo> TextInfos { get; set; } = [];
    }
    public enum MetaGameCardTextAbilityType
    {
        //능력 중 활성화, 격발, 정적
        None,
        Rule, // 특수 룰의 경우(효과로 간주하지 않는 것들)
        Activation,
        Trigger,
        Static
    }
    public class MetaGameCardTextInfo
    {
        public string CardTextCode { get; set; }

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
