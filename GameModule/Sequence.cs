using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    public class Sequence
    {
        public Guid ID { get; set; }
        public SequenceType Type { get; set; }
        public bool IsDone { get; set; } //결과값을 도출했는지 여부

        //해당 시퀀스가 진행되기 위해 필요한 것
        public SequenceFulfillmentDescriber SequenceFulfillmentDescriber { get; set; }

        //해당 시퀀스에 필요한 파라메터 지정
        public Dictionary<string, object> SequenceFulfillments { get; set; }

        //해당 시퀀스가 진행되어 나타난 결과
        public SequenceResultDecriber SequenceResultDescriber { get; set; }
    }
    public enum SequenceType
    {
        None,
        DetermineTurnOrder,
        MakeFirstHand,
        End
    }
    public class SequenceFulfillmentDescriber
    {
        // 시퀀스에서 뭐가 필요한지
        public Dictionary<string, SequenceFulfillmentParameter> Parameters { get; set; } = [];
    }
    public class SequenceFulfillmentParameter
    {
        public SequenceFulfillmentParameterType Type { get; set; }
        public string Name { get; set; }
        public GamePlayer? AnswerPlayer { get; set; } // 해당 파라메터의 값을 결정할 플레이어
    }
    public enum SequenceFulfillmentParameterType
    {
        Number,
        String,
        GameObject,
    }
    public class SequenceResultDecriber
    {
        public List<SequenceResultAction> SequenceResultActions { get; set; }
    }
    public class SequenceResultAction
    {
        public SequenceResultActionType SequenceResultActionType { get; set; }
        public Dictionary<string, object> ResultParameters { get; set; }
    }
    public enum SequenceResultActionType
    {
        MakeGlobalValue //게임 전역 변수값 생성 및 세팅
    }
}
