using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TripleS.UI {

    /// <summary>
    /// Base of UI element.
    /// </summary>
    public class Element {
        public string Conents { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }
        public Color TextColor { get; set; }
        public string Font { get; set; }
        public ElementState State { get; set; }
        public AnchorPoint Anchor { get; set; }
        public SizingStyle Sizing { get; set; }
        public TextPosition TextAlignment { get; set; }
        public string Image { get; set; }
        public string ID { get; set; }
        public bool Visible { get; set; }
        public int SubLayer { get; set; }
        public int Padding { get; set; }
        public float Scale { get; set; }
        public float TextSize { get; set; }

        public Rectangle bounds;

        public Element(string name)
        {
            bounds = new Rectangle();
            ID = name;
            Position = Vector2.Zero;
            Width = 1;
            Height = 1;
            Color = Color.White;
            TextColor = Color.Wheat;
            State = ElementState.None;
            Anchor = AnchorPoint.TopLeft;
            Sizing = SizingStyle.None;
            Visible = true;
            Image = "solid";
            Scale = 1;
            TextSize = 1;
            TextAlignment = TextPosition.Middle;
        }

        public virtual void Update(GameTime time) { }
    }

    public enum ElementState
    {
        None,
        Pressed,
        Highlighted,
        In,
        Out
    }

    public enum AnchorPoint
    {
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleLeft,
        Middle,
        MiddleRight,
        BottomLeft,
        BottomMiddle,
        BottomRight
    }

    public enum SizingStyle
    {
        Anchor,
        Sretch,
        None
    }

    public enum TextPosition
    {
        Left,
        Middle,
    }
}
