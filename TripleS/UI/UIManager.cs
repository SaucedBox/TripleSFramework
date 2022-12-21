using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TripleS.UI {

    /// <summary>
    /// UI manager and renderer.
    /// </summary>
    public static class UIManager {

        private static Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        private static List<Element> elements = new List<Element>();
        private static ScreenSizer sizer = new ScreenSizer();
        public static float scale = 1;

        /// <summary>
        /// Draws all UI elements.
        /// </summary>
        public static void Draw(Renderer renderer)
        {
            sizer.view = renderer.View;
            int i = -1;
            foreach(Element element in elements)
            {
                i++;
                if (element.Visible)
                {
                    Texture2D tex = GetImage(element.Image);
                    Vector2 position = sizer.GetPos(element.Position, element.Anchor, element.Sizing);
                    SpriteFont font = GetFont(element.Font);
                    float teScale = (scale + element.Scale) / element.TextSize;
                    Vector2 textDims = font.MeasureString(element.Conents) * teScale;

                    if (element.Sizing == SizingStyle.Sretch)
                    {
                        int width = element.Width;
                        int height = element.Height;
                        if (element.Conents != "")
                        {
                            width = (int)(textDims.X + element.Padding * 2);
                            height = (int)(textDims.Y + element.Padding * 2);
                        }

                        position = sizer.ApplyOffset(element.Anchor, position, width, height);
                        elements[i].bounds = new Rectangle((int)position.X, (int)position.Y, width, height);
                        renderer.BasicDraw(tex, elements[i].bounds, 1, element.SubLayer / 10 + 0.01f, col: element.Color);
                    }
                    else
                    {
                        position = sizer.ApplyOffset(element.Anchor, position, element.Width, element.Height);
                        elements[i].bounds = new Rectangle((int)position.X, (int)position.Y, element.Width, element.Height);
                        renderer.BasicDraw(tex, position, 1, element.SubLayer / 10, col: element.Color);
                    }

                    if(element.Conents != "")
                    {
                        Vector2 textPos = sizer.GetTextPos(position, element.Padding, element.TextAlignment);
                        renderer.DrawText(font, element.Conents, textPos, 1, element.SubLayer / 10, col: element.TextColor, size: teScale);
                    }
                }
            }
        }

        /// <summary>
        /// Updates UI states.
        /// </summary>
        public static void Update(GameTime gameTime, Matrix view)
        {
            Vector2 mousePos = Vector2.Transform(GameInputs.GetMouse().Position.ToVector2(), Matrix.Invert(view));
            foreach (Element element in elements)
            {
                if (Physics.CollisionEngine.PointInRect(element.bounds, mousePos))
                {
                    if (GameInputs.GetMouse().LeftButton == ButtonState.Pressed)
                        element.State = ElementState.Pressed;
                    else
                        element.State = ElementState.Highlighted;
                }
                else
                    element.State = ElementState.None;

                element.Update(gameTime);
            }
        }

        public static void Load()
        {
            images.Add("solid", SSS.Square);
        }

        public static void AddElement(Element element)
        {
            elements.Add(element);
        }

        public static void AddImage(string id, string path, ContentManager content)
        {
            Texture2D tex = content.Load<Texture2D>(path);
            images.Add(id, tex);
        }

        public static void AddFont(string id, string path, ContentManager content)
        {
            SpriteFont tex = content.Load<SpriteFont>(path);
            fonts.Add(id, tex);
        }

        private static Texture2D GetImage(string id)
        {
            return images.Where(x => x.Key == id).First().Value;
        }

        public static SpriteFont GetFont(string id)
        {
            return fonts.Where(x => x.Key == id).First().Value;
        }

        public static Element GetElement(string id)
        {
            return elements.Where(x => x.ID == id).First();
        }
    }
}
