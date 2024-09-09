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
                if (sequences.Count != 0)
                {
                    return sequences[sequence_index];
                }
                return sequences.FirstOrDefault();
            }
        }
        private void AddSequenceToFront(List<Sequence> sequences)
        {
            this.sequences = [.. sequences, .. this.sequences];
        }
        private void AddSequence(Sequence sequence)
        {
            sequence_index++;
            sequences.Add(sequence);
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
                        playerOrder = gamePlayers.OrderBy(_ => randomizer.Next(0, gamePlayers.Count() - 1)).ToList();
                        curseq.SequenceResultDescriber = new SequenceResultDecriber()
                        {
                            SequenceResultActions = [
                                new SequenceResultAction(){
                                        SequenceResultActionType = SequenceResultActionType.MakeGlobalValue,
                                        ResultParameters = {{"order", playerOrder.ToList() }}
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

                        break;

                    default:
                        break;
                }
            }
        }
    }
}
