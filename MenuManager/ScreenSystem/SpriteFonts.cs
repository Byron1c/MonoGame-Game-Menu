using MenuManager.ScreenSystem;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MenuManager.ScreenSystem
{
    public class SpriteFonts
    {

        //put the fonts you want to load for the menu system in here:

        //public SpriteFont DetailsFont;
        //public SpriteFont FrameRateCounterFont;
        //public SpriteFont MenuSpriteFont;

        public List<SpriteFontExt> SpriteFontList;

        public SpriteFonts(ContentManager contentManager)
        {

            SpriteFontList = new List<SpriteFontExt>();

            AddFont(contentManager.Load<SpriteFont>("Fonts/frameRateCounter"), "FrameRateCounter") ;
            AddFont(contentManager.Load<SpriteFont>("Fonts/menu"), "MenuSprite");

        }

        public SpriteFont GetFont(string vName)
        {
            foreach (SpriteFontExt font in SpriteFontList)
            {
                if (vName == font.Name)
                {
                    return font.Font;
                }
            }

            return null;
        }

        public void AddFont(SpriteFont vFont, string vName)
        {
            SpriteFontExt newItem = new SpriteFontExt(vFont, vName);
            SpriteFontList.Add(newItem);
        }



        }
}