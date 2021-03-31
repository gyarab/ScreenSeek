using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public partial class Form1 : Form
    {
        public static int TILE_SIZE;
        public Player[] players = new Player[2];
        public Game Game;

       
        const int STARTING_DISTACE = 3;
        const int TIMER_INTERVAL = 40; 

        int timePassed = 0;

      

        Label debugLog2 = new Label();
        Label debugLog1 = new Label();
        Panel leftPanel = new Panel();
        Panel rightPanel = new Panel();
        Label displayGameTime = new Label();

        Timer everyFrame = new Timer();


        public Label lives = new Label();

        private readonly Random _random = new Random();

        public Form1()
        {
            TILE_SIZE = (int) (32 * 3 * 2 * GameSettings.zoom);
            InitializeComponent();

            // user cant eneter number higher than number of images for bioms
            tileTypesSetting.Maximum = Directory.GetFiles(@"..\..\Images\Bioms\", "*.*", SearchOption.AllDirectories).Length;

        }

        private void Form1_Load(object sender, EventArgs e) {


         
            everyFrame.Interval = TIMER_INTERVAL;
            everyFrame.Tick += new EventHandler(Animations);

            



        }

        private void Init()
        {
            this.Controls.Clear();
            TILE_SIZE = (int)(32 * 3 * 2 * GameSettings.zoom);
            players[0] = new Player(true, true, this);
            players[1] = new Player(false, false, this);

            CreateGame();
        }

        private void CreateGame()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            // CREATE PLAYERS
            {
                int x, y;
                if (STARTING_DISTACE < GameSettings.mapWidth - STARTING_DISTACE) // Try to place players far from each other if possible
                {
                    x = _random.Next(STARTING_DISTACE, GameSettings.mapWidth - STARTING_DISTACE);
                    y = _random.Next(STARTING_DISTACE, GameSettings.mapHeigth - STARTING_DISTACE);
                }
                else
                {
                    x = _random.Next(0, GameSettings.mapWidth);
                    y = _random.Next(0, GameSettings.mapHeigth);
                }
                
                Player player1 = players[0];
                Player player2 = players[1];
                player1.InitPlayer(!player1.hider, 0, 0);
                player2.InitPlayer(!player2.hider, x, y);
            }

            // CREATE MAP
            Game = new Game(GameSettings.mapWidth, GameSettings.mapHeigth, GameSettings.tileTypes, this);



            // DRAW MAP AND UI
            
            this.Controls.Clear();
            LoadGraphics();

            
            // DRAW PLAYER      

            PictureBox p1 = createPictureBox(players[0], 1, 1);
            p1.Tag = "player1";
            p1.BringToFront();
            ((Panel)getComponentOnSide(true, "mapScreen")).Controls.Add(p1);



            PictureBox p2 = createPictureBox(players[1], 1, 1);
            p2.Tag = "player2";
            p2.BringToFront();
            ((Panel)getComponentOnSide(false, "mapScreen")).Controls.Add(p2);

            // DRAW TILES AROUND THE PLAYER   
            foreach (Player player in players)
            {
                Point mapPoint;
                for (int y = 0; y < 3; y++) // draw 3x3 tiles around the player
                {
                    for (int x = 0; x < 3; x++)
                    {
                        mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1); // -1 because player is in [1;1]
                        drawTile(player.onLeft, x, y, Game.gameMap[mapPoint.Y, mapPoint.X]);
                    }

                }

                drawBiomsAround(player);

            }

            // START TIMERS
            everyFrame.Start();
            foreach (Player player in players)
            {
                player.timer.Start();
            }

            this.Focus(); // Defocus from any previous control
            this.KeyPreview = true;  // Activate functions like KeyPressed

        }

        private void LoadGraphics()
        {

            displayGameTime.Location = new Point(this.Size.Width / 2, 0);
            displayGameTime.Font = new Font("Arial", 30 * GameSettings.zoom);
            displayGameTime.AutoSize = true;
            displayGameTime.Anchor = AnchorStyles.Top;
            this.Controls.Add(displayGameTime);

            leftPanel.Location = new Point(0, 0);
            leftPanel.Width = this.Size.Width / 2;
            leftPanel.Height = this.Size.Height;
            leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(leftPanel);

            rightPanel.Location = new Point(this.Size.Width / 2, 0);
            rightPanel.Width = this.Size.Width / 2;
            rightPanel.Height = this.Size.Height;
            rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(rightPanel);
            

            leftPanel.Controls.Clear();
            rightPanel.Controls.Clear();

            foreach (Player player in players)
            {
                Panel side;

                if (player.onLeft)
                {
                    side = leftPanel;
                }
                else
                {
                    side = rightPanel;
                }

                Panel mapScreen = new Panel();
                mapScreen.Location = new Point(50, 50);
                mapScreen.Width = TILE_SIZE * 3;
                mapScreen.Height = TILE_SIZE * 3;
                mapScreen.Tag = "mapScreen";
                side.Controls.Add(mapScreen);

                ProgressBar energy = new ProgressBar();
                energy.Location = new Point(50, TILE_SIZE * 3 + 50 + 20);
                energy.Width = (int) (300 * GameSettings.zoom);
                energy.Height = (int) (40 * GameSettings.zoom);
                energy.Tag = "energy";
                side.Controls.Add(energy);

                if (player.hider)
                {
                    
                }
                else
                {                 
                    lives.Location = new Point(50, 2);
                    lives.Text = "Lives: " + GameSettings.seekerLives;
                    lives.Font = new Font("Arial", 20 * GameSettings.zoom);
                    lives.Anchor = AnchorStyles.Top;
                    lives.AutoSize = true;
                    side.Controls.Add(lives);
                }
            }           
            /*
            debugLog2.Text = "";
            debugLog2.Location = new Point(650, 500);
            debugLog2.Height = 500;
            debugLog2.Width = 450;
            debugLog2.Visible = true;
            this.Controls.Add(debugLog2);*/

        }
       
        private Image resize(Image image, int targetWidth, int targetHeight)
        {
            var resizedImage = new Bitmap(targetWidth, targetHeight);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                var attributes = new ImageAttributes();
                attributes.SetWrapMode(WrapMode.TileFlipXY);

                var destination = new Rectangle(0, 0, targetWidth, targetHeight);
                graphics.DrawImage(image, destination, 0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel, attributes);
            }

            return resizedImage;
        }
   
        private void drawTile(bool left, int x, int y, Tile tile)
        {
            Panel currentScreen = (Panel)getComponentOnSide(left, "mapScreen");

            PictureBox biomPB = createPictureBox(tile, x ,y);

            currentScreen.Controls.Add(biomPB);
        }
        private void endAnimation(Player player)
        {
            // Move player to the coordinates where he was moving
            Point newPos = OutOfBounds(player.posX + player.movingX * -1, player.posY + player.movingY * -1); 
            player.posX = newPos.X;
            player.posY = newPos.Y;
            player.distanceTraveled = 0;
            player.movingX = 0;
            player.movingY = 0;


            Panel currentScreen = (Panel)getComponentOnSide(player.onLeft, "mapScreen");

            List<Control> toBeRemoved = new List<Control>();
            foreach (Control item in currentScreen.Controls.OfType<PictureBox>()) // delete every tile out of the 3x3 grid
            {
                if ((string)item.Tag != "player1" && (string)item.Tag != "player2")
                {
                    int x = ((PictureBox)item).Left;
                    int y = ((PictureBox)item).Top;
                    if (true) // snap tiles to the grid
                    {
                        if (x >= -TILE_SIZE / 2 && x <= TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = 0;
                        }
                        else if (x >= TILE_SIZE / 2 && x <= TILE_SIZE + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = TILE_SIZE;
                        }
                        else if (x >= TILE_SIZE + TILE_SIZE / 2 && x <= TILE_SIZE * 2 + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Left = TILE_SIZE * 2;
                        }

                        if (y >= -TILE_SIZE / 2 && y <= TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = 0;
                        }
                        else if (y >= TILE_SIZE / 2 && y <= TILE_SIZE + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = TILE_SIZE;
                        }
                        else if (y >= TILE_SIZE + TILE_SIZE / 2 && y <= TILE_SIZE * 2 + TILE_SIZE / 2)
                        {
                            ((PictureBox)item).Top = TILE_SIZE * 2;
                        }

                        x = ((PictureBox)item).Left;
                        y = ((PictureBox)item).Top;
                    }
          
                    if (x < 0 || x > TILE_SIZE*2  || y < 0 || y > TILE_SIZE*2) // check if the tile is out of the grid
                    {
                        toBeRemoved.Add(item);
                    }
                }

            }

            foreach (Control item in toBeRemoved)   // delete the tiles
            {
                currentScreen.Controls.Remove(item);
                ((PictureBox)item).Dispose();

            }

            drawBiomsAround(player);  // keep the tiles in 3x3 grid and draw new tiles outside the grid so that the scene is prepared


        }

        private PictureBox createPictureBox(Entity entity, int x, int y)
        {
            PictureBox pb = new PictureBox();       
            pb.Image = resize(entity.sprite, entity.width, entity.width);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Height = entity.height;
            pb.Width = entity.width;
            pb.Top = y * TILE_SIZE + TILE_SIZE / 2 - TILE_SIZE / (2 * TILE_SIZE / entity.height);
            pb.Left = x * TILE_SIZE + TILE_SIZE / 2 - TILE_SIZE / (2 * TILE_SIZE / entity.width);

            return pb;
        }

        public Point OutOfBounds(int x, int y)
        {
            if (x < 0)
            {
                x = GameSettings.mapWidth + x;
            }
            else if (x > GameSettings.mapWidth - 1)
            {
                x = x - GameSettings.mapWidth;
            }

            if (y < 0)
            {
                y = GameSettings.mapHeigth + y;
            }
            else if (y > GameSettings.mapHeigth - 1)
            {
                y = y - GameSettings.mapHeigth;
            }
            return new Point(x, y);
        }

        void drawBiomsAround(Player player)
        {
            /*
            draw bioms around like this:
        1.  x x x x x 
        2.  x . . . x   x = draw
        3.  x . P . x   . = keep
        4.  x . . . x   P = player
        5.  x x x x x
            */

            Point mapPoint;

            // line 1
            {
                int y = -1;
                for (int x = -1; x < 3 + 1; x++)
                {
                    
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.onLeft, x, y, Game.gameMap[mapPoint.Y, mapPoint.X]); 
                }
            }

            // line 2-4
            for (int y = 0; y < 3; y++) 
            {
                for (int x = -1; x < 3 + 1; x+= 3 + 1)
                {
                
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.onLeft, x, y, Game.gameMap[mapPoint.Y, mapPoint.X]); 
                }
            }
            // line 5
            {
                int y = 3;
                for (int x = -1; x < 3 + 1; x++)
                {
              
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.onLeft, x, y, Game.gameMap[mapPoint.Y, mapPoint.X]);
                }
            }

        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (Game.running)
            {
                Player player = players[1];
                if(e.KeyCode == Keys.Left)
                {
                    player.StartMoving(-1,0);
          
                }
                else if(e.KeyCode == Keys.Right)
                {
                    player.StartMoving(1, 0);
           
                }
                else if (e.KeyCode == Keys.Up)
                {
                    player.StartMoving(0, -1);
         
                }
                else if (e.KeyCode == Keys.Down)
                {
                    player.StartMoving(0, 1);
                
                }
                else if (e.KeyCode == Keys.P)
                {

                    player.UsePrimarySkill();
                }
                else if (e.KeyCode == Keys.O)
                {
                    player.UseSecondarySkill();
                }
                else
                {
                    player = players[0];

                    if (e.KeyCode == Keys.A)
                    {
                        player.StartMoving(-1, 0);


                    }
                    else if (e.KeyCode == Keys.D)
                    {
                        player.StartMoving(1, 0);

                    }
                    else if (e.KeyCode == Keys.W)
                    {
                        player.StartMoving(0, -1);

                    }
                    else if (e.KeyCode == Keys.S)
                    {
                        player.StartMoving(0, 1);

                    }
                    else if (e.KeyCode == Keys.F)
                    {
                        player.UsePrimarySkill();
                    }
                    else if (e.KeyCode == Keys.E)
                    {
                        player.UseSecondarySkill();
                    }


                }
            }

        }

        public void SetEnergy(Player player)
        {
            ProgressBar energyProgressBar = (ProgressBar)getComponentOnSide(player.onLeft, "energy");
           
            energyProgressBar.Value = Math.Min(player.energy, 100); // if players energy > 100, set value to 100

        }


        public void EndGame(bool leftWon, string reason = "")
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            Game.running = false;
            Game.gameTimer.Stop();
            everyFrame.Stop();
            foreach (Player player in players)
            {
                player.timer.Stop();
            }
            this.Controls.Clear();
          
            
            Label winner = new Label();
            winner.Location = new Point(this.Size.Width / 2, this.Size.Height / 4);
            winner.AutoSize = true;
            winner.Font = new Font("Calibri", 30 * GameSettings.zoom);

            if (leftWon)
            {
                winner.Text = "Left player won:\n" + reason;
                players[0].gamesWon += 1;
            }
            else
            {
                winner.Text = "Right player won:\n" + reason;
                players[1].gamesWon += 1;
            }

            Label leftScore = new Label();
            leftScore.AutoSize = true;
            leftScore.Anchor = AnchorStyles.Left;
            leftScore.Font = new Font("Calibri", 30 * GameSettings.zoom);
            leftScore.Text = "Score: " + players[0].gamesWon;

            Label rightScore = new Label();
            rightScore.AutoSize = true;
            rightScore.Anchor = AnchorStyles.Right;
            rightScore.Left = this.Size.Width - 200;
            rightScore.Font = new Font("Calibri", 30 * GameSettings.zoom);
            rightScore.Text = "Score: " + players[1].gamesWon;

            Button nextRndBtn = new Button();
            nextRndBtn.Location = new Point(this.Width/2, this.Height/2);
            nextRndBtn.Text = "Next round";
            nextRndBtn.AutoSize = true;
            nextRndBtn.Font = new Font("Calibri", 18 * GameSettings.zoom);
            nextRndBtn.Click += new EventHandler(NextRound);

            Button restartBtn = new Button();
            restartBtn.Location = new Point(this.Width / 2, (int)(this.Height / 1.7f));
            restartBtn.Text = "Restart";
            restartBtn.AutoSize = true;
            restartBtn.Font = new Font("Calibri", 18 * GameSettings.zoom);
            restartBtn.Click += new EventHandler(RestartGame);

            Button exitBtn = new Button();
            exitBtn.Location = new Point(this.Width / 2, (int)(this.Height / 1.5f));
            exitBtn.Text = "Exit";
            exitBtn.AutoSize = true;
            exitBtn.Font = new Font("Calibri", 18 * GameSettings.zoom);
            exitBtn.Click += new EventHandler(ExitGame);


            this.Controls.Add(winner);
            this.Controls.Add(leftScore);
            this.Controls.Add(rightScore);
            this.Controls.Add(exitBtn);
            this.Controls.Add(nextRndBtn);
            this.Controls.Add(restartBtn);
        }


        private void ExitGame(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void RestartGame(object sender, EventArgs e)
        {
            Application.Restart();
         
        }
        private void NextRound(object sender, EventArgs e)
        {
                this.Controls.Clear();
                CreateGame();
        }
        private void Animations(object sender, EventArgs e) // runs every few milliseconds
        {
            foreach (Player player in players)
            {
   
                if(player.movingX != 0 || player.movingY != 0) // if player is moving, start moving animation:
                {

                    Panel currentScreen = (Panel)getComponentOnSide(player.onLeft, "mapScreen");

                    foreach (Control item in currentScreen.Controls.OfType<PictureBox>())  // move all tiles oposite direction the player is moving by little
                    {
                        if((string)item.Tag != "player1" && (string)item.Tag != "player2")
                        {
                            ((PictureBox)item).Left += player.movingX * GameSettings.speed;
                            ((PictureBox)item).Top += player.movingY * GameSettings.speed;
                        }
                            
                        
                    }

                    player.distanceTraveled++;
    
                    if (player.distanceTraveled * GameSettings.speed >= TILE_SIZE) // end animation
                    {                     
                        endAnimation(player);
                    }

                }              

            }
        }
        public void UpdateGameTime(int time)
        {
            displayGameTime.Text = time.ToString();
        
        }

        public void TimeRanOut()
        {
            bool leftWon;
            if (players[0].getHider())
            {
                leftWon = true;
            }
            else
            {
                leftWon = false;
            }
            EndGame(leftWon, "Seeker havent find hider");

        }
        private Control getComponentOnSide(bool left, string name)
        {
            Control component = new Control();

            if (left)
            {
                foreach (Control item in leftPanel.Controls)
                {
                    if ((string)item.Tag == name)
                    {
                       
                        component = item;
                        break;
                    }
                   
                }
            }
            else
            {
                foreach (Control item in rightPanel.Controls)
                {
                    if ((string)item.Tag == name)
                    {
                      
                        component = item;
                        break;
                    }
                }
            }
            return component;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Init();
        }
      
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           
            GameSettings.mapWidth = (int)((NumericUpDown)(sender)).Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
         
            GameSettings.mapHeigth = (int)((NumericUpDown)(sender)).Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
          
            GameSettings.gameEndTime = (int)((NumericUpDown)(sender)).Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
           
            GameSettings.hiderRecovery = (int)((NumericUpDown)(sender)).Value;
        }
        
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
           
            GameSettings.seekerRecovery = (int)((NumericUpDown)(sender)).Value;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            GameSettings.zoom = (float)((NumericUpDown)(sender)).Value / 100f;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)((NumericUpDown)(sender)).Value;
            GameSettings.tileTypes = value;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged_1(object sender, EventArgs e)
        {
            GameSettings.speed = (int)((NumericUpDown)(sender)).Value;
        }
    }
}
