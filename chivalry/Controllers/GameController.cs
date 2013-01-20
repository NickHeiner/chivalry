using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Controllers
{
    class GameController
    {
        internal static void onBoardSpaceClick(Game game, int rowIndex, int colIndex)
        {
            if (GameValidator.isValidMove(game, rowIndex, colIndex))
            {
                game.AddActiveMove(new Tuple<int, int>(rowIndex, colIndex));
            }
        }
    }
}
