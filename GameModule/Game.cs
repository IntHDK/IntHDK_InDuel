using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameModule
{
    public class GamePlayerInitInfo
    {
        public List<DeckCardInfoRecord> StartingDeck { get; set; } = [];
    }
    public partial class Game
    {
        //메타정보
        public Dictionary<string, MetaGameCardInfo> MetaGameCardInfos { get; set; }

        private Random randomizer = new();
        public List<Sequence> Sequences { get; set; } = new List<Sequence>(); //진행중, 진행된, 또는 진행 예정의 시퀀스 리스트
        public int Sequence_Index { get; set; } = 0;
        public List<GamePlayer> GamePlayers { get; set; } = new List<GamePlayer>(); //게임 참여 플레이어
        public List<GamePlayer> PlayerOrder { get; set; } = new List<GamePlayer>(); //플레이어 턴 순서
        public Queue<GameEffect> GameResolvingEffect { get; set; } = new Queue<GameEffect>(); // 카드 및 효과 처리 전 대기장소

        //턴
        public GamePlayer? TurnOwner { get; set; } = null;
        public int Turn = 0;
        public TurnPhase TurnPhase { get; set; } = TurnPhase.None;

        public Game(Dictionary<string, MetaGameCardInfo> meta, List<GamePlayerInitInfo> Players)
        {
            this.MetaGameCardInfos = meta.ToDictionary();
            //게임 플레이어 등록
            GamePlayers = [];
            foreach (var pinit in Players)
            {
                GamePlayers.Add(new GamePlayer()
                {
                    ID = Guid.NewGuid(),
                    StartingDeck = [.. pinit.StartingDeck]
                });
            }

            //플레이어별로 존 만들어주기
            foreach (var player in GamePlayers)
            {
                var handzone = new GameZone()
                {
                    ID = Guid.NewGuid(),
                    Owner = player,
                };
                var battlefield = new GameZone()
                {
                    ID = Guid.NewGuid(),
                    Owner = player,
                };
                var graveyard = new GameZone()
                {
                    ID = Guid.NewGuid(),
                    Owner = player,
                };
                var deck = new GameZone()
                {
                    ID = Guid.NewGuid(),
                    Owner = player,
                };
                foreach(var deckcardinfo in player.StartingDeck)
                {
                    //메타에 있는것만 추가
                    if (MetaGameCardInfos.TryGetValue(deckcardinfo.CardCode, out var cardinfo))
                    {
                        if (cardinfo.Type == GameCardType.HQ)
                        {
                            //덱에 들어가지 않고 최초세팅
                            var hq = new GameCardObject()
                            {
                                CardCode = cardinfo.CardCode,
                                ID = Guid.NewGuid(),
                                IsFaceDown = false,
                                Owner = battlefield,
                                CardMetaInfo = cardinfo,
                            };
                            battlefield.GameObjects.Add(hq);
                        }
                        else
                        {
                            var addingcard = new GameCardObject()
                            {
                                CardCode = deckcardinfo.CardCode,
                                ID = Guid.NewGuid(),
                                IsFaceDown = true,
                                Owner = deck,
                                CardMetaInfo = cardinfo,
                                PlayersWhoViewAsFaceUp = []
                            };
                            deck.GameObjects.Add(addingcard);
                        }
                    }
                    
                }
                //덱 셔플
                shuffleZoneCardObjectOrder(deck);

                player.Hand = handzone;
                player.Battlefield = battlefield;
                player.Graveyard = graveyard;
                player.Deck = deck;
            }

            //첫 시퀀스
            Sequences =
            [
                new Sequence()
                {
                    ID = Guid.NewGuid(),
                    Type = SequenceType.DetermineTurnOrder,
                    IsDone = false,
                    SequenceFulfillmentDescriber = new SequenceFulfillmentDescriber(){
                        Parameters = []
                    },
                    SequenceFulfillments = [],
                    SequenceResultDescriber = null
                },
            ];
        }

        
    }

    

    
}