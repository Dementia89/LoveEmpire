using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoveEmpire
{
    class Button
    {
        Texture2D tex;
        public Rectangle bounds;
        //Vector2 position;
        
        public Button(int height, int width, int xPos, int yPos)
        {
            bounds.Height = height;
            bounds.Width = width;
            //position.X = xPos;
            //position.Y = yPos;
            bounds.Location = new Point(xPos, yPos);
        }

        public void Update()
        {
            // TODO: Add Update Logic Here
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, bounds.Location.ToVector2(), Color.White);
        }

        public void AssignTexture(Texture2D image)
        {
            tex = image;
        }

        public bool Clicked(Vector2 clickSpot)
        {
            if (bounds.Contains(clickSpot))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
