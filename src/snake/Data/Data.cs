using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysium.Foundation.Serpentis.Core.Domain;

namespace Snake.SaveData
{
    public class Data
    {
        private bool _headerWritten = false;

        public void SaveData(Snapshot snapshot, Direction currentDirection)
        {

            var snakeHead = snapshot.Snake[0];
            var apple = snapshot.Food;
            var grid = snapshot.Grid;

            var distance_between_snake_apple_X = apple.X - snakeHead.X;
            var distance_between_snake_apple_Y = apple.Y - snakeHead.Y;

            int closeLeftWall = snakeHead.X <= 1 ? 1 : 0;
            int closeRightWall = snakeHead.X >= grid.Width - 2 ? 1 : 0;
            int closeTopWall = snakeHead.Y <= 1 ? 1 : 0;
            int closeBottomWall = snakeHead.Y >= grid.Height - 2 ? 1 : 0;

            int action = currentDirection switch
            {
                Direction.Up => 0,
                Direction.Down => 1,
                Direction.Left => 2,
                Direction.Right => 3,
                _ => -1
            };


            using (StreamWriter writetext = new StreamWriter("dataset.csv"))
            {

                if (!_headerWritten)
                {
                    writetext.WriteLine("distanceX,distanceY,closeTopWall,closeBottomWall,closeLeftWall,closeRightWall,action");
                    _headerWritten = true;
                }
                writetext.WriteLine(
                    distance_between_snake_apple_X + "," +
                    distance_between_snake_apple_Y + "," +
                    closeTopWall + "," +
                    closeBottomWall + "," +
                    closeLeftWall + "," +
                    closeRightWall + "," +
                    action
                    );
            }

        }

    }
}
