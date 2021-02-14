using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace HideAndSeek
{
    
    abstract class Entity
    {
        public string name;
        public int posY, posX;
        public Image sprite;
        public int width, height;


    }

    class Coin : Entity
    {
        
        public Coin()
        {
            this.sprite = Image.FromFile(@"..\..\Images\blueberryBush.png");
        }
    }

    class DebugText : Entity
    {
        public Label label;
        public DebugText(string text)
        {
            this.label = new Label();
            this.label.Text = text;
        }
    }
}
