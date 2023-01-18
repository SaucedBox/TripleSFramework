using System;
using Microsoft.Xna.Framework;

namespace TripleS.Physics {

    /// <summary>
    /// A static helper class that can calculate 
    /// collision and physics with rectangles.
    /// Named "The Damscus Engine"
    /// </summary>
    public static class CollisionEngine {

        public static float gravityF = 4;

        public static bool CollideSlope(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            float x = (!col.MiddleSlope ? (col.Slope < 0 ? obj.Position.X + obj.Width : obj.Position.X) : obj.Position.X + obj.Width / 2) - col.X;
            float y = col.Slope * x + (col.Y + col.Height * col.SlopeOffset);

            oPos = new Vector2(obj.Position.X, y - obj.Height);
            oVel = new Vector2(obj.Friction ? col.Slope : obj.Velocity.X, 0);

            if (Math.Floor(x) > col.Width || Math.Floor(x) < 0 || obj.Position.Y + obj.Height > col.Y + col.Height + 1)
                return false;
            else if (obj.Position.Y + obj.Height + obj.Velocity.Y > y)
            {
                if (y < col.Y)
                    oPos = new Vector2(obj.Position.X, col.Y - obj.Height);
                return true;
            }
            else
                return false;
        }

        public static bool CollideTop(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = new Vector2(obj.Position.X, col.Y - obj.Height);
            oVel = new Vector2(obj.Velocity.X, 0);

            if (obj.Position.Y + obj.Velocity.Y + obj.Height > col.Y && obj.OldPosition.Y + obj.Height <= col.Y
                && obj.Position.X - col.X + obj.Width > 0 && obj.Position.X - col.X < col.Width)
                return true;
            return false;
        }

        public static bool CollideBottom(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = new Vector2(obj.Position.X, col.Y + col.Height);
            oVel = new Vector2(obj.Velocity.X, 0);

            if (obj.Position.Y + obj.Velocity.Y < col.Y + col.Height && obj.OldPosition.Y >= col.Y + col.Height
                && obj.Position.X - col.X + obj.Width > 0 && obj.Position.X - col.X < col.Width)
                return true;
            return false;
        }

        public static bool CollideLeft(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = new Vector2(col.X - obj.Width, obj.Position.Y);
            oVel = new Vector2(0, obj.Velocity.Y);

            if (obj.Position.X + obj.Velocity.X + obj.Width > col.X && obj.OldPosition.X + obj.Width <= col.X
                && obj.Position.Y - col.Y + obj.Height > 0 && obj.Position.Y - col.Y < col.Height)
                return true;
            return false;
        }

        public static bool CollideRight(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = new Vector2(col.X + col.Width, obj.Position.Y);
            oVel = new Vector2(0, obj.Velocity.Y);

            if (obj.Position.X + obj.Velocity.X < col.X + col.Width && obj.OldPosition.X >= col.X + col.Width
                && obj.Position.Y - col.Y + obj.Height > 0 && obj.Position.Y - col.Y < col.Height)
                return true;
            return false;
        }

        public static bool CollideFace(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = obj.Position;
            oVel = obj.Velocity;

            switch (col.Side)
            {
                case 0:
                    return CollideTop(col, obj, out oPos, out oVel);
                case 1:
                    return CollideBottom(col, obj, out oPos, out oVel);
                case 2:
                    return CollideLeft(col, obj, out oPos, out oVel);
                case 3:
                    return CollideRight(col, obj, out oPos, out oVel);
            }

            return false;
        }

        public static bool CollideRect(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            if (CollideTop(col, obj, out oPos, out oVel))
                return true;
            else if (CollideBottom(col, obj, out oPos, out oVel))
                return true;
            else if (CollideLeft(col, obj, out oPos, out oVel))
                return true;
            else if (CollideRight(col, obj, out oPos, out oVel))
                return true;
            else
                return false;
        }

        public static bool CollideAll(Collider col, Transform obj, out Vector2 oPos, out Vector2 oVel)
        {
            oPos = obj.Position;
            oVel = obj.Velocity;

            switch (col.Type)
            {
                case ColliderType.Rectangle:
                    return CollideRect(col, obj, out oPos, out oVel);
                case ColliderType.Face:
                    return CollideFace(col, obj, out oPos, out oVel);
                case ColliderType.Slope:
                    return CollideSlope(col, obj, out oPos, out oVel);
            }

            return false;
        }

        /// <summary>
        /// Calculates velocity based on physics.
        /// Should be called before collision.
        /// </summary>
        /// <param name="deltaTime">Delta time of game</param>
        /// <returns>Physical velocity</returns>
        public static Vector2 UpdateVelocity(Transform obj, float deltaTime)
        {
            Vector2 outVel = obj.Velocity;
            if (obj.Friction)
                outVel.X = (float)Math.Round(outVel.X /= obj.Weight / 100 + 1, 3, MidpointRounding.ToZero);
            if (obj.Gravity)
                outVel.Y += obj.Weight / gravityF * deltaTime;
            return outVel;
        }

        public static bool PointInRect(Rectangle bounds, Vector2 point)
        {
            return point.X > bounds.Left &&
                point.X < bounds.Right &&
                point.Y > bounds.Top &&
                point.Y < bounds.Bottom;
        }
    }
}
