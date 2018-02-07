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
    class ProgressBar
    {
        float min;
        float max;
        float current;
        Rectangle bounds;
        Rectangle bar;
        Texture2D background;
        Texture2D barTexture;

        public ProgressBar(float minimumValue, float maximumValue, Rectangle recBounds, Texture2D backgroundTex, Texture2D barTex)
        {
            min = minimumValue;
            max = maximumValue;
            current = maximumValue;
            bounds = recBounds;
            bar = recBounds;
            background = backgroundTex;
            barTexture = barTex;
        }

        public void Update()
        {
            // TODO: Adjust background texture to percentage of value
            bar.Width = (int)(bounds.Width * (current / max));
            //bar.Location = bounds.Location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, bounds, Color.White);
            spriteBatch.Draw(barTexture, bar, bounds, Color.White);
        }

        /// <summary>
        /// Set the value directly.
        /// </summary>
        /// <param name="value"> Value you want to pass in. </param>
        public void SetValue(float value)
        {
            if(value > max)
            {
                current = max;
            }
            else if(value < min)
            {
                current = min;
            }
            else
            {
                current = value;
            }
        }

        /// <summary>
        /// Let the bar figure out it's own percentage.
        /// </summary>
        /// <param name="percentage"> Give this a value from 0-1 </param>
        public void GivePercentage(float percentage)
        {
            if(percentage > 1)
            {
                current = max;
            }
            else if( percentage < 0)
            {
                current = min;
            }
            else
            {
                current = max * percentage;
            }
        }

        public void AssignTexture(Texture2D bgTex, Texture2D barTex)
        {
            background = bgTex;
            barTexture = barTex;
        }
    }
}
