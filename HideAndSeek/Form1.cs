using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public partial class Form1 : Form
    {
        public const int TILE_SIZE = 32*3*2;


        const int STARTING_DISTACE = 1;
        const int SPEED = 4;
        const int TIMER_INTERVAL = 10; //40
        int tileTypes = 0;

        int timePassed = 0;



        Label debugLog2 = new Label();
        Label debugLog1 = new Label();
        Panel leftPlayer = new Panel();
        Panel rightPlayer = new Panel();
        Label displayGameTime = new Label();

        Panel leftBlind = new Panel();
        Panel rightBlind = new Panel();
        Timer gameTimer = new Timer();
        Timer secondTimer = new Timer();

        private readonly Random _random = new Random();


        //customizable 

        int mapWidth = 6;
        int mapHeight = 6;

        //

        int gameTime = 0;

        bool leftKeyboard;
    
        bool wait = false;
        Tile[,] gameMap;
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
        Player[] players = new Player[2];

        public Form1()
        {
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e) {

           

            InitGame(mapWidth, mapHeight, true);
        }

        private void LoadGraphics()
        {
            /*
            debugLog1.Text = "dd";
            debugLog1.Location = new Point(0, 0);
            debugLog1.Height = 500;
            debugLog1.Width = 400;
            debugLog1.Visible = true;
            this.Controls.Add(debugLog1);*/

            displayGameTime.Location = new Point(this.Size.Width / 2 - 60, 0);
            displayGameTime.Font = new Font("Arial", 30);
            displayGameTime.AutoSize = true;
            displayGameTime.Anchor = AnchorStyles.Top;
            this.Controls.Add(displayGameTime);

            leftPlayer.Location = new Point(0, 0);
            leftPlayer.Width = this.Size.Width / 2;
            leftPlayer.Height = this.Size.Height;
            leftPlayer.Anchor = AnchorStyles.Left;
            this.Controls.Add(leftPlayer);

            rightPlayer.Location = new Point(this.Size.Width / 2, 0);
            rightPlayer.Width = this.Size.Width / 2;
            rightPlayer.Height = this.Size.Height;
            rightPlayer.Anchor = AnchorStyles.Right;
            this.Controls.Add(rightPlayer);

            

            foreach (Player player in players)
            {
                Panel side = rightPlayer;

                if (player.getOnLeft())
                {
                    side = leftPlayer;
                }

                Panel screen = new Panel();
                screen.Location = new Point(50, 50);
                screen.Width = TILE_SIZE * 3;
                screen.Height = TILE_SIZE * 3;
                screen.Tag = "screen";
                side.Controls.Add(screen);

                ProgressBar energy = new ProgressBar();
                energy.Location = new Point(50, TILE_SIZE * 3 + 50 + 20);
                energy.Width = 300;
                energy.Height = 40;
                energy.Tag = "energy";
                side.Controls.Add(energy);
            }           

            debugLog2.Text = "";
            debugLog2.Location = new Point(650, 500);
            debugLog2.Height = 500;
            debugLog2.Width = 450;
            debugLog2.BorderStyle = BorderStyle.Fixed3D;
            debugLog2.Font = new Font("Calibri", 10);
            debugLog2.Padding = new Padding(6);
            debugLog2.Visible = false;
            this.Controls.Add(debugLog2);

            
            leftBlind.Location = new Point(50, 50);
            leftBlind.Width = TILE_SIZE * 3;
            leftBlind.Height = TILE_SIZE * 3;
            leftBlind.BringToFront();
            leftBlind.BackColor = Color.Black;
            leftBlind.Visible = false;
            leftBlind.Anchor = AnchorStyles.Left;
            this.Controls.Add(leftBlind);

            rightBlind.Location = new Point(this.Size.Width / 2 + 50, 50);
            rightBlind.Width = TILE_SIZE * 3;
            rightBlind.Height = TILE_SIZE * 3;
            rightBlind.BringToFront();
            rightBlind.BackColor = Color.Black;
            rightBlind.Visible = false;
            rightBlind.Anchor = AnchorStyles.Right;
            this.Controls.Add(rightBlind);


            
            /*
            rightScreen.Location = new Point(this.Size.Width / 2 + 50, 50);
            rightScreen.Width = TILE_SIZE * 3 + TILE_SIZE;
            rightScreen.Height = TILE_SIZE * 3 + TILE_SIZE;
            rightScreen.Anchor = AnchorStyles.Right;
            this.Controls.Add(rightScreen);

           */




        }
        private void InitGame(int width, int height, bool leftPlayerHider)
        {
           
            //GENERATE BIOMS
            gameMap = new Tile[width, height];
            
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    Image biom = null;
                    int spriteId = _random.Next(0, tileTypes + 1 + 1);
                    //spriteId = testMap2[y, x];
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
                    
                    gameMap[y, x] = new Tile(biom, x , y);
             
                }
            }

            //INIT PLAYERS
            //if (players[0] == null) // if playing for the first time
            {
         
                players[0] = new Player(leftPlayerHider, true, 0, 0);    
                int x = _random.Next(STARTING_DISTACE, width - STARTING_DISTACE); //TODO expception
                int y = _random.Next(STARTING_DISTACE, height - STARTING_DISTACE);
                players[1] = new Player(!leftPlayerHider, false, 2, 2);


                gameTimer.Interval = TIMER_INTERVAL;
                gameTimer.Tick += new EventHandler(GameLoop);

                secondTimer.Interval = 1000;
                secondTimer.Tick += new EventHandler(SecondPassed);

            }
            gameTimer.Start();
            secondTimer.Start();

            this.Controls.Clear();
            LoadGraphics();

            // DRAW COINS


            // DRAW PLAYER      

            PictureBox p1 = createPictureBox(players[0], 1, 1, true);
            p1.Tag = "player1";
            p1.BringToFront();
            ((Panel)getComponentOnSide(true, "screen")).Controls.Add(p1);

            

            PictureBox p2 = createPictureBox(players[1], 1, 1, false);
            p2.Tag = "player2";
            p2.BringToFront();

            ((Panel)getComponentOnSide(false, "screen")).Controls.Add(p2);

            // DRAW TILES AROUND THE PLAYER   
            foreach (Player player in players)
            {
                Point mapPoint = new Point();
                for (int y = 0; y < 3; y++) // draw 3x3 tiles around the player
                {
                    for (int x = 0; x < 3; x++)
                    {
                        mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1); // -1 because player is in [1;1]
                        drawTile(player.getOnLeft(), x, y, gameMap[mapPoint.Y, mapPoint.X]); 
                    }
                    
                }

                drawBiomsAround(player);
   
            }

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


            Panel currentScreen = (Panel)getComponentOnSide(left, "screen");

            PictureBox biomPB = createPictureBox(tile, x ,y, left);

            int i = 0;
            foreach (Entity en in tile.entities)
            {
                if (en.GetType() == typeof(DebugText))
                {
                    debugLog2.Text += ((DebugText)en).label.Text + " -----  ";

                    //biomPB.Controls.Add(((DebugText)en).label);
                }
                else
                {
                    //biomPB.Controls.Add(createPictureBox(en, x, y, left));
                }
                
            }
           
            debugLog2.Text += biomPB.Name;
            currentScreen.Controls.Add(biomPB);
        }
        private void endAnimation(bool left)
        {

            Panel currentScreen = (Panel)getComponentOnSide(left, "screen");

            debugLog2.Text = "";
            debugLog1.Text = "";
            int i = 0;
            int j = 0;
            
            
            List<Control> toBeRemoved = new List<Control>();
            foreach (Control item in currentScreen.Controls.OfType<PictureBox>())
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

                    debugLog1.Text += x + " " + y + "       ";

                    j++;
                    if (x < 0 || x > TILE_SIZE*2  || y < 0 || y > TILE_SIZE*2) // delete tiles out of the grid
                    {
                        i++;
                        debugLog2.Text += item.Left + " " + item.Top + "   " + ((currentScreen.Width - TILE_SIZE + 1).ToString()) + " " + ((currentScreen.Height - TILE_SIZE + 1).ToString()) + "\n";
                        toBeRemoved.Add(item);
                    }
                }

            }
            debugLog2.Text += i;
            debugLog1.Text += j;
            foreach (Control item in toBeRemoved)
            {

                currentScreen.Controls.Remove(item);
                ((PictureBox)item).Dispose();

            }
            if (!left)
            {
                drawBiomsAround(players[1]);

            }
            else
            {
                drawBiomsAround(players[0]);

            }
         

        }

        private PictureBox createPictureBox(Entity entity, int x, int y, bool left)
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
                x = mapWidth + x;
            }
            else if (x > mapWidth - 1)
            {
                x = x - mapWidth;
            }

            if (y < 0)
            {
                y = mapHeight + y;
            }
            else if (y > mapHeight - 1)
            {
                y = y - mapHeight;
            }
            return new Point(x, y);
        }

        void drawBiomsAround(Player player)
        {
            /*
            draw bioms around like this (x = draw):
            x x x x x 
            x . . . x
            x . . . x
            x . . . x
            x x x x x
            */

            int playerX = player.posX;
            int playerY = player.posY;
           
            Point mapPoint = new Point();
       
            {
                int y = -1;
                for (int x = -1; x < 3 + 1; x++)
                {
                    
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, gameMap[mapPoint.Y, mapPoint.X]); 
                }
            }
      
            for (int y = 0; y < 3; y++)
            {
                for (int x = -1; x < 3 + 1; x+= 3 + 1)
                {
                
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, gameMap[mapPoint.Y, mapPoint.X]); 
                }
            }
        
            {
                int y = 3;
                for (int x = -1; x < 3 + 1; x++)
                {
              
                    mapPoint = OutOfBounds(player.posX + x - 1, player.posY + y - 1);
                    drawTile(player.getOnLeft(), x, y, gameMap[mapPoint.Y, mapPoint.X]);
                }
            }


         

        }
        private void spawnItem(bool left, int x, int y)
        {

        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
       
            leftKeyboard = false;
            if(e.KeyCode == Keys.Left)
            {
                Move(-1,0 , leftKeyboard);
          
            }
            else if(e.KeyCode == Keys.Right)
            {
                Move(1, 0, leftKeyboard);
           
            }
            else if (e.KeyCode == Keys.Up)
            {
                Move(0, -1, leftKeyboard);
         
            }
            else if (e.KeyCode == Keys.Down)
            {
                Move(0, 1, leftKeyboard);
                
            }
            else if (e.KeyCode == Keys.P)
            {
    
                PrimarySkill(leftKeyboard);
            }
            else if (e.KeyCode == Keys.O)
            {
                SecondarySkill(leftKeyboard);
            }
            else
            {
                leftKeyboard = true;

                if (e.KeyCode == Keys.A)
                {
                    Move(-1, 0, leftKeyboard);


                }
                else if (e.KeyCode == Keys.D)
                {
                    Move(1, 0, leftKeyboard);

                }
                else if (e.KeyCode == Keys.W)
                {
                    Move(0, -1, leftKeyboard);

                }
                else if (e.KeyCode == Keys.S)
                {
                    Move(0, 1, leftKeyboard);

                }
                else if (e.KeyCode == Keys.F)
                {
                    PrimarySkill(leftKeyboard);
                }
                else if (e.KeyCode == Keys.E)
                {
                    SecondarySkill(leftKeyboard);
                }


            }

           
        }

        private void Move(int deltaX, int deltaY, bool leftPlayer)
        {

            Player player = players[1];     
            if (leftPlayer)
            {
                player = players[0];
            }

            if (player.movingX != 0 || player.movingY != 0)
            {
                return;
            }       

            if(player.energy < 100)
            {
                return;
            }
            SetEnergy(player, 0);    
            secondTimer.Stop();
            secondTimer.Start();

            deltaX *= -1;
            deltaY *= -1;

            player.movingX = deltaX;
            player.movingY = deltaY;

        }

        private void SetEnergy(Player player, int ammout)
        {
            ProgressBar energyProgressBar = (ProgressBar)getComponentOnSide(player.getOnLeft(), "energy");
            player.energy = ammout;
            energyProgressBar.Value = player.energy;
            debugLog1.Text += "     "+ player.energy;
        }

        private void PrimarySkill(bool leftPlayer)
        {
 
            int id;
            if (leftPlayer)
            {
                id = 0;
            }
            else
            {
                id = 1;
            }

            if (players[id].getHider())
            {

            }
            else
            {
            
                if (players[(id + 1) % 2].posX == players[id].posX && players[(id + 1) % 2].posY == players[id].posY)
                {
            
                    EndGame(leftPlayer, "Seeker found the hider");
                    
                }
            }
        }

        private void SecondarySkill(bool leftPlayer)
        {
            int id;
            if (leftPlayer)
            {
                id = 0;
            }
            else
            {
                id = 1;
            }

            if (players[id].getHider())
            {

            }
            else
            {
              
            }

        }

        private void EndGame(bool leftWon, string reason = "")
        {
            gameTimer.Stop();
            secondTimer.Stop();
            gameTime = 0;
            this.Controls.Clear();
          

            Label winner = new Label();       
            winner.Location = new Point(50, 200);
            winner.Height = 500;
            winner.Width = 450;
            winner.BorderStyle = BorderStyle.Fixed3D;
            winner.Font = new Font("Calibri", 30);
            winner.Padding = new Padding(6);
          
          
            if (leftWon)
            {           
                winner.Text = "Left player won\n" + reason;
                players[0].gamesWon += 1;
            }
            else
            {
                winner.Text = "Right player won\n" + reason;
                players[1].gamesWon += 1;
            }

            Button restartBtn = new Button();
            restartBtn.Location = new Point(this.Width/2, this.Height/2);
            restartBtn.Text = "Restart";
            restartBtn.AutoSize = true;
            restartBtn.BackColor = Color.LightBlue;
            restartBtn.Padding = new Padding(6);
            restartBtn.Font = new Font("Calibri", 18);
            restartBtn.Click += new EventHandler(RestartGame);

            Button exitBtn = new Button();
            exitBtn.Location = new Point(this.Width / 2, (int)(this.Height / 1.7f));
            exitBtn.Text = "Exit";
            exitBtn.AutoSize = true;
            exitBtn.BackColor = Color.LightBlue;
            exitBtn.Padding = new Padding(6);
            exitBtn.Font = new Font("Calibri", 18);

            this.Controls.Add(winner);
            this.Controls.Add(exitBtn);
            this.Controls.Add(restartBtn);
        }
        private void CloseWindow(object sender, EventArgs e)
        {

        }
        private void RestartGame(object sender, EventArgs e)
        {
            players[0].hider = !players[0].hider;
            players[1].hider = !players[1].hider;
            InitGame(mapWidth, mapHeight, players[1].hider);
        }
        private void GameLoop(object sender, EventArgs e)
        {
            foreach (Player player in players)
            {
                bool left = player.getOnLeft();
                
                if(player.movingX != 0 || player.movingY != 0) // MOVING ANIMATION
                {

                    Panel currentScreen = (Panel)getComponentOnSide(left, "screen");

                    foreach (Control item in currentScreen.Controls.OfType<PictureBox>())
                    {
                        if((string)item.Tag != "player1" && (string)item.Tag != "player2")
                        {
                            ((PictureBox)item).Left += player.movingX * SPEED;
                            ((PictureBox)item).Top += player.movingY * SPEED;
                        }
                            
                        
                    }

                    player.distanceTraveled++;
    
                    if (player.distanceTraveled * SPEED >= TILE_SIZE) // end anim
                    {
                        Point newPos = OutOfBounds(player.posX + player.movingX * -1, player.posY + player.movingY * -1);
                        player.posX = newPos.X;
                        player.posY = newPos.Y;
                        player.distanceTraveled = 0;
                        player.movingX = 0;
                        player.movingY = 0;


                        endAnimation(player.getOnLeft());


                    }

                }              

            }
        }
        private void SecondPassed(object sender, EventArgs e)
        {
            gameTime++;
            displayGameTime.Text = gameTime.ToString();
            if (gameTime > 100)
            {
                if(players[0].getOnLeft() && players[0].getHider())
                {
                    EndGame(true, "Seeker havent find hider");
                }
                else
                {
                    EndGame(false, "Seeker havent find hider");
                }
                
            }
            foreach (Player player in players)
            {
                if (player.energy < 100)
                { 
                    SetEnergy(player, player.energy + player.energyRecovery);
                }
  
            }
        }
        private Control getComponentOnSide(bool left, string name)
        {
            Control component = new Control();

            if (left)
            {
                foreach (Control item in leftPlayer.Controls)
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
               
                foreach (Control item in rightPlayer.Controls)
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
  
    }
}
