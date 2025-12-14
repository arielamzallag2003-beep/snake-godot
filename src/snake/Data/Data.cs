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
        private string fileName = "dataset.txt";

        public void SaveData(Snapshot snapshot, Direction currentDirection)
        {
            var snakeHead = snapshot.Snake[0];
            var apple = snapshot.Food;
            var grid = snapshot.Grid;

            // detecte les murs proches
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

            if (action == -1) return;

            bool isSuicide = false;

            if (action == 0 && closeTopWall == 1) isSuicide = true;      // aller haut + Mur en haut
            if (action == 1 && closeBottomWall == 1) isSuicide = true;   // Veut aller bas + Mur en bas
            if (action == 2 && closeLeftWall == 1) isSuicide = true;     // Veut aller gauche + Mur gauche
            if (action == 3 && closeRightWall == 1) isSuicide = true;    // Veut aller droite + Mur droite

            // Si gameover on arrête tout, on n'écrit rien dans le fichier
            if (isSuicide)
            {
              
                return;
            }

           
            var distance_between_snake_apple_X = apple.X - snakeHead.X;
            var distance_between_snake_apple_Y = apple.Y - snakeHead.Y;

            int bodyUp = snapshot.Snake.Any(b => b.X == snakeHead.X && b.Y == snakeHead.Y - 1) ? 1 : 0;
            int bodyDown = snapshot.Snake.Any(b => b.X == snakeHead.X && b.Y == snakeHead.Y + 1) ? 1 : 0;
            int bodyLeft = snapshot.Snake.Any(b => b.X == snakeHead.X - 1 && b.Y == snakeHead.Y) ? 1 : 0;
            int bodyRight = snapshot.Snake.Any(b => b.X == snakeHead.X + 1 && b.Y == snakeHead.Y) ? 1 : 0;

            bool fileExists = File.Exists(fileName);

            using (StreamWriter writetext = new StreamWriter(fileName, append: true))
            {
                if (!fileExists)
                {
                    writetext.WriteLine("distanceX,distanceY,closeTopWall,closeBottomWall,closeLeftWall,closeRightWall,bodyUp,bodyDown,bodyLeft,bodyRight,action");
                }

                writetext.WriteLine(
                    $"{distance_between_snake_apple_X}," +
                    $"{distance_between_snake_apple_Y}," +
                    $"{closeTopWall}," +
                    $"{closeBottomWall}," +
                    $"{closeLeftWall}," +
                    $"{closeRightWall}," +
                    $"{bodyUp}," +
                    $"{bodyDown}," +
                    $"{bodyLeft}," +
                    $"{bodyRight}," +
                    $"{action}"
                );
            }
        }
    }
}

