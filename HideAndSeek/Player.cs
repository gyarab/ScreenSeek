using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideAndSeek
{
    public class Player : Entity
    {
        private Form1 control;
        public Timer timer = new Timer();
        public bool hider;
        public bool onLeft;
    
        public int movingX, movingY = 0;
        public int distanceTraveled = 0;
        public int gamesWon = 0;
        public int energy;
        public int energyRecovery;
        public int lives;
        public Player(bool hider, bool onLeft, Form1 control)
        {
            this.hider = hider;
            this.onLeft = onLeft;
            this.control = control;

            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(SecondPassed);

        }
        public void InitPlayer(bool hider, int x, int y)
        {
            if (this.onLeft)
            {
                this.sprite = Image.FromFile(@"..\..\Images\BBB_idle_back.png");
            }
            else
            {
                this.sprite = Image.FromFile(@"..\..\Images\BBB_idle_front.png");
            }
            this.width = Form1.TILE_SIZE / 4;
            this.height = Form1.TILE_SIZE / 4;
            this.posX = x;
            this.posY = y;
            this.energy = 0;
            this.lives = GameSettings.seekerLives;

            if (hider)
            {
                this.energyRecovery = GameSettings.hiderRecovery;
            }
            else
            {
                this.energyRecovery = GameSettings.seekerRecovery;
            }
            this.hider = hider;

        }

        private void SecondPassed(object sender, EventArgs e)
        {

            if (this.energy < 100)
            {
                this.energy += this.energyRecovery;
                control.SetEnergy(this);
            }

        }
        public void StartMoving(int deltaX, int deltaY)
        {

            if (this.movingX != 0 || this.movingY != 0)
            {
                return;
            }

            if (this.energy < 100)
            {
                return;
            }
            this.energy = 0;

            deltaX *= -1;
            deltaY *= -1;

            this.movingX = deltaX;
            this.movingY = deltaY;

        }

        

        public void UsePrimarySkill()
        {
            int otherPlayer;
            if (this.onLeft)
            {
                otherPlayer = 1;
            }
            else
            {
                otherPlayer = 0;
            }
            if (this.hider)
            {

            }
            else
            {

                if (control.players[otherPlayer].posX == this.posX && control.players[otherPlayer].posY == this.posY)
                {

                    control.EndGame(this.onLeft, "Seeker found the hider");

                }
                else
                {
                    this.lives--;
                    control.lives.Text = "Lives: " + this.lives.ToString();
                    if (this.lives <= 0)
                    {

                        control.EndGame(this.onLeft, "Seeker ran out of lives");
                    }

                }
            }
        }

        public void UseSecondarySkill()
        {
            int otherPlayer;
            if (this.onLeft)
            {
                otherPlayer = 1;
            }
            else
            {
                otherPlayer = 0;
            }
            
            if (this.hider)
            {

            }
            else
            {

                
            }
        }
        public bool getOnLeft()
        {
            return this.onLeft;
        }

        public bool getHider()
        {
            return this.hider;
        }

        public int getX()
        {
            return this.posX;
        }

        public int getY()
        {
            return this.posY;
        }
        public void setX(int x)
        {
            this.posX = x;
        }

        public void setY(int y)
        {
            this.posY = y;
        }


    }
}
