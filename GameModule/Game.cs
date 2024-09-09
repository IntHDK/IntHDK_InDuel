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
        private Dictionary<MetaGameCardCode, MetaGameCardInfo> metaGameCardInfos;

        private Random randomizer = new();
        private List<Sequence> sequences = new List<Sequence>(); //진행중, 진행된, 또는 진행 예정의 시퀀스 리스트
        private int sequence_index = 0;
        private List<GamePlayer> gamePlayers = new List<GamePlayer>(); //게임 참여 플레이어
        private List<GamePlayer> playerOrder = new List<GamePlayer>(); //플레이어 턴 순서
        private List<GameObject> gameObjects = new List<GameObject>(); //모든 오브젝트 풀
        private Stack<GameEffect> gameStack = new Stack<GameEffect>(); // 카드 및 효과 처리 전 대기장소

        //외부용
        public List<Sequence> Sequences { get {  return sequences.ToList(); } }
        public int SequenceIndex { get { return sequence_index; } }
        public List<GamePlayer> GamePlayers { get { return gamePlayers.ToList(); } }
        public List<GamePlayer> PlayerOrder { get { return playerOrder.ToList(); } }

        //턴


        public Game(Dictionary<MetaGameCardCode, MetaGameCardInfo> meta, List<GamePlayerInitInfo> Players)
        {
            this.metaGameCardInfos = meta.ToDictionary();
            //게임 플레이어 등록
            gamePlayers = [];
            foreach (var pinit in Players)
            {
                gamePlayers.Add(new GamePlayer()
                {
                    ID = Guid.NewGuid(),
                    StartingDeck = [.. pinit.StartingDeck]
                });
            }

            //플레이어별로 존 만들어주기
            foreach (var player in gamePlayers)
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
                    if (metaGameCardInfos.TryGetValue(deckcardinfo.CardCode, out var cardinfo))
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
                            gameObjects.Add(hq);
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
                            gameObjects.Add(addingcard);
                            deck.GameObjects.Add(addingcard);
                        }
                    }
                    
                }
                //덱 셔플
                shuffleZoneCardObjectOrder(deck);

                gameObjects.Add(battlefield);
                gameObjects.Add(graveyard);
                gameObjects.Add(deck);
                gameObjects.Add(handzone);

                player.Hand = handzone;
                player.Battlefield = battlefield;
                player.Graveyard = graveyard;
                player.Deck = deck;
            }

            //첫 시퀀스
            sequences =
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