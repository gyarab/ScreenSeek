using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public class Game
    {
        public Tile[,] gameMap;
        private Form1 control;
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

        public Timer gameTimer = new Timer();
    
        public int gameTime = 0;
        public bool running = true;
        private readonly Random _random = new Random();

        public Game(int width, int height, int tileTypes, Form1 control)
        {
            this.control = control;
            gameTimer.Interval = 1000;
            gameTimer.Tick += new EventHandler(EverySecond);
            InitGame(width, height, tileTypes);
        }

        private void InitGame(int width, int height, int tileTypes)
        {
            this.running = true;
            
            gameTimer.Start();

            gameMap = new Tile[width, height];
            string[] biomImages = Directory.GetFiles(@"..\..\Images\Bioms\", "*.*", SearchOption.AllDirectories); // stores list of all bioms images in the directory
            for (int y = 0; y < width; y++)   //GENERATE BIOMS
            {
                for (int x = 0; x < height; x++)
                {
                    Image biom = null;
                    int spriteId = _random.Next(0, tileTypes);
                 
                    biom = Image.FromFile(biomImages[spriteId]);
                

                    gameMap[y, x] = new Tile(biom, x, y);

                }
            }            

        }
        private void EverySecond(object sender, EventArgs e)
        {
            this.gameTime++;
            control.UpdateGameTime(GameSettings.gameEndTime - this.gameTime);
            if (this.gameTime > GameSettings.gameEndTime)
            {
                control.TimeRanOut();
            }

        }
    }

    
}
