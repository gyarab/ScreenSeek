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
    class Tile : Entity
    {
        int spriteId;
        public List<Entity> entities = new List<Entity>();


        public Tile(Image biom, int x, int y)
        {
            this.posX = x;
            this.posY = y;
            this.sprite = biom;
            this.width = Form1.TILE_SIZE - 5;
            this.height = Form1.TILE_SIZE - 5;
            this.entities.Add(new DebugText(this.posX + " " + this.posY));
        }

       
    }
}
