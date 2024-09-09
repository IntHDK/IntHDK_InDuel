using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModule
{
    public partial class Game
    {
        private void shuffleZoneCardObjectOrder(GameZone Zone)
        {
            var shuffled = Zone.GameObjects.OrderBy(_ => randomizer.Next()).ToList();
            Zone.GameObjects = shuffled;
        }
        //N장을 드로우
        private void drawCards(GamePlayer player, int drawcount)
        {
            for (int i = 0; i < drawcount; i++)
            {
                drawCard(player);
            }
        }
        //1장을 드로우
        private void drawCard(GamePlayer player)
        {
            if (!player.Deck.GameObjects.Where(o => o.ObjectType == GameObjectType.GameCardObject).Any())
            {
                //덱에 카드가 없음
                //패배 효과를 대기 큐에
                gameStack.Enqueue(new GameEffect()
                {
                    ID = Guid.NewGuid(),
                    EffectCode = MetaEffectCode.GAMERULE_GAMELOSS_DRAWEMPTYDECK,
                    Owner = player,
                });
            }
            else
            {
                //카드가 있으니 맨 위 카드를 핸드로 옮겨줌
                foreach(var deckobj in player.Deck.GameObjects)
                {
                    //가장 최초로 나타나는 GameCardObject인걸 뽑아야함
                    if (deckobj is GameCardObject)
                    {
                        player.Hand.GameObjects.Add(deckobj);
                        player.Deck.GameObjects.Remove(deckobj);
                        break;
                    }
                }
            }
        }
    }
}
