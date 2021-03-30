using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public class Game
    {
        public Tile[,] gameMap;

        int[,] testMap1 = new int[6, 6] {
        { 0, 1, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 0, 1 },
        { 0, 1, 0, 1, 0, 1 }};
        int[,] testMap2 = new int[6, 6] {
        { 1, 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 0, 0 }};

        public int gameTime = 0;
        public bool running = true;
        private readonly Random _random = new Random();

        public Game(int width, int height, int tileTypes)
        {
            InitGame(width, height, tileTypes);
        }

        private void InitGame(int width, int height, int tileTypes)
        {
            this.running = true;
          
            gameMap = new Tile[width, height];

            for (int y = 0; y < width; y++)   //GENERATE BIOMS
            {
                for (int x = 0; x < height; x++)
                {
                    Image biom = null;
                    int spriteId = _random.Next(0, tileTypes);
                    switch (spriteId)
                    {
                        case 0:
                            {
                                biom = Image.FromFile(@"..\..\Images\grassTile8.png");
                                break;
                            }
                        case 1:
                            {
                                biom = Image.FromFile(@"..\..\Images\grassTile6.png");
                                break;
                            }
                    }

                    gameMap[y, x] = new Tile(biom, x, y);

                }
            }            

        }
    }

    
}
