using System;
using Microsoft.Xna.Framework;

namespace TripleS.Physics {

    public struct Collider {

        public int Width { get; }
        public int Height { get; }
        public float X { get; }
        public float Y { get; }
        public ColliderType Type { get; }

        public float Slope { get; }
        public float SlopeOffset { get; }
        public bool MiddleSlope { get; }

        public int Side { get; }
        public float SideOffset { get; }

        public Collider(Rectangle col)
        {
            X = col.X;
            Y = col.Y;
            Width = col.Width;
            Height = col.Height;
            Type = ColliderType.Rectangle;

            Slope = 0;
            SlopeOffset = 0;
            MiddleSlope = false;
            Side = 0;
            SideOffset = 0;
        }

        public Collider(Rectangle col, float m, float slopeOff, bool middleSlope)
        {
            X = col.X;
            Y = col.Y;
            Width = col.Width;
            Height = col.Height;
            Type = ColliderType.Slope;

            Slope = m;
            SlopeOffset = slopeOff;
            MiddleSlope = middleSlope;

            Side = 0;
            SideOffset = 0;
        }

        public Collider(Rectangle col, int side, float sOff)
        {
            X = col.X;
            Y = col.Y;
            Width = col.Width;
            Height = col.Height;
            Type = ColliderType.Face;

            Slope = 0;
            SlopeOffset = 0;
            MiddleSlope = false;

            Side = side;
            SideOffset = sOff;
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)X, (int)Y, Width, Height);
        }
    }

    public enum ColliderType {
        Rectangle,
        Slope,
        Face
    }

    public class Transform {
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 OldPosition { get; private set; }
        public float Weight { get; set; }
        public bool Friction { get; set; }
        public bool Gravity { get; set; }

        public Transform(Vector2 position, int width, int height, float weight, bool friction, bool gravity = true)
        {
            Position = position;
            Width = width;
            Height = height;
            Weight = weight;
            Velocity = Vector2.Zero;
            Friction = friction;
            Gravity = gravity;
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void ApplyVelocity()
        {
            OldPosition = Position;
            Position += Velocity;
        }

        public void ChangeVelocityX(float X)
        {
            Velocity = new Vector2(X, Velocity.Y);
        }

        public void ChangeVelocityY(float Y)
        {
            Velocity = new Vector2(Velocity.X, Y);
        }
    }
}
