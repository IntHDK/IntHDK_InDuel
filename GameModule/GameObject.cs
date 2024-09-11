using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    public interface GameObject
    {
        public GameObjectType ObjectType { get; }
        public List<GameParameter> Parameters { get; set; }
        public Guid ID { get; set; }

        public GameObject? Owner { get; set; }
    }
    public enum GameObjectType
    {
        Player,
        GameInfo,
        GameCardObject, 
        GameEffect,
        GameZone,
    }

    public class GameParameter
    {
        public GameParameterName Name { get; set; }
        public int Index { get; set; }
        public GameParameterValueType ValueType { get; set; }
        public object Value { get; set; }
    }
    public enum GameParameterValueType
    {
        //오브젝트가 가질 수 있는 파라메터 타입 종류 일람
        Number,
        Player
    }
    public enum GameParameterName
    {
        //오브젝트가 가질 수 있는 파라메터 이름 일람
    }
    //플레이어를 게임 상 오브젝트로 취급, GameZone들을 하위로 가짐
    public class GamePlayer : GameObject
    {
        public GameObjectType ObjectType
        {
            get
            {
                return GameObjectType.Player;
            }
        }
        public List<GameParameter> Parameters { get; set; } = [];
        public Guid ID { get; set; }
        public GameObject? Owner { get; set; }

        public List<DeckCardInfoRecord> StartingDeck { get; set; } = [];
        public int TurnOrder { get; set; }

        public GameZone Hand { get; set; }
        public GameZone Deck { get; set; }
        public GameZone Battlefield { get; set; }
        public GameZone Graveyard { get; set; }
    }
    //게임 영역 : 다른 GameObject를 하위로 가질 수 있음
    public class GameZone : GameObject
    {
        public GameObjectType ObjectType
        {
            get
            {
                return GameObjectType.GameZone;
            }
        }
        public List<GameParameter> Parameters { get; set; } = [];
        public Guid ID { get; set; }
        public GameObject? Owner { get; set; }
        public List<GameObject> GameObjects { get; set; } // 덱 같은 경우 인덱스 숫자가 낮은 쪽일수록 쌓여있는 뭉치 위에 있는 판정
    }
    //GameObject 중 '카드' 형태로 표현될 수 있는 기물들
    public class GameCardObject : GameObject
    {
        public GameObjectType ObjectType { get { return GameObjectType.GameCardObject; } }

        public List<GameParameter> Parameters { get; set; } = [];
        public Guid ID { get; set; }
        public GameObject? Owner { get; set; }

        public string CardCode { get; set; }
        public MetaGameCardInfo CardMetaInfo { get; set; }
        public bool IsFaceDown { get; set; } // 뒷면 상태. 덱, 패에 있는것도 뒷면으로 간주 (특정 플레이어 또는 전체 플레이어 입장에서는 보여서는 안됨)
        public List<GamePlayer> PlayersWhoViewAsFaceUp { get; set; } // 어떤 플레이어가 이 오브젝트의 세부 정체를 확인 가능한지 (FaceDown 상태에서 정체를 알 수 있는 플레이어)

        public string Name
        {
            get
            {
                if (CardMetaInfo != null)
                {
                    return CardMetaInfo.CardFunctionallyCode;
                }
                return "";
            }
        }
        public int Level
        {
            get
            {
                if (CardMetaInfo != null)
                {
                    return CardMetaInfo.Level;
                }
                return 0;
            }
        }
        public int Power
        {
            get
            {
                //HQ와 Unit만 Power를 가질 수 있음
                if (CardMetaInfo.Type == GameCardType.HQ ||
                    CardMetaInfo.Type == GameCardType.Unit)
                {
                    return CardMetaInfo.Power;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class GameEffect : GameObject
    {
        public GameObjectType ObjectType { get { return GameObjectType.GameEffect; } }

        public List<GameParameter> Parameters { get; set; } = [];
        public Guid ID { get; set; }
        public GameObject? Owner { get; set; }

        public MetaEffectCode EffectCode { get; set; }
    }
    public enum GameCardObjectType
    {
        HQ,
        Unit,
        Command,
    }
    public class DeckCardInfoRecord
    {
        public string CardCode { get; set; }
        public int Quantity { get; set; }
    }

    public enum GameCardType
    {
        HQ,
        Unit,
        Command
    }
    public enum GameCardSubType
    {

    }
}
