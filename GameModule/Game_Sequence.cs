using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    public partial class Game
    {
        public Sequence? CurrentSequence
        {
            get
            {
                if (Sequences.Count != 0)
                {
                    return Sequences[Sequence_Index];
                }
                return Sequences.FirstOrDefault();
            }
        }
        private void AddSequenceToFront(List<Sequence> sequences)
        {
            this.Sequences = [.. sequences, .. this.Sequences];
        }
        private void AddSequence(Sequence sequence)
        {
            Sequence_Index++;
            Sequences.Add(sequence);
        }
        public void Next()
        {
            var curseq = CurrentSequence;
            if (curseq != null)
            {
                switch (curseq.Type)
                {
                    case SequenceType.DetermineTurnOrder:
                        //파라메터 필요없음
                        curseq.IsDone = true;
                        //랜덤 플레이어 순위 셔플
                        PlayerOrder = GamePlayers.OrderBy(_ => randomizer.Next(0, GamePlayers.Count() - 1)).ToList();
                        curseq.SequenceResultDescriber = new SequenceResultDecriber()
                        {
                            SequenceResultActions = [
                                new SequenceResultAction(){
                                        SequenceResultActionType = SequenceResultActionType.MakeGlobalValue,
                                        ResultParameters = {{"order", PlayerOrder.ToList() }}
                                    }
                                ]
                        };

                        //다음 시퀀스
                        AddSequence(new Sequence()
                        {
                            ID = Guid.NewGuid(),
                            IsDone = false,
                            SequenceFulfillmentDescriber = new SequenceFulfillmentDescriber()
                            {
                                Parameters = []
                            },
                            Type = SequenceType.MakeFirstHand,
                        });
                        break;

                    case SequenceType.MakeFirstHand:
                        //파라메터 필요없음
                        curseq.IsDone = true;
                        foreach(var player in GamePlayers)
                        {
                            drawCards(player, MetaConstants.INITIAL_HAND_SIZE);
                        }
                        AddSequence(new Sequence()
                        {
                            ID = Guid.NewGuid(),
                            IsDone = false,
                            SequenceFulfillmentDescriber = new SequenceFulfillmentDescriber()
                            {
                                Parameters = []
                            },
                            Type = SequenceType.MakeFirstTurn,
                        });
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
