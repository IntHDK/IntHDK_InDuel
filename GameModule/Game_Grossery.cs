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

        }
        //1장을 드로우
        private void drawCard(GamePlayer player)
        {
            
        }
    }
}
