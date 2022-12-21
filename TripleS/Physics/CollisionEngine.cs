using System;
using Microsoft.Xna.Framework;

namespace TripleS.Physics {

    /// <summary>
    /// A static helper class that can calculate 
    /// collision and physics with rectangles.
    /// Named "The Damscus Engine"
    /// </summary>
    public static class CollisionEngine {

        public static float wallThreshold = 1;
        public static float frictionMult;
        public static float slopeMax;
        public static float gravityF = 4;

        /// <summary>
        /// Gets the Y value of an intercepting plane.
        /// </summary>
        /// <param name="line">The line being calculated</param>
        /// <param name="intercept">Intersceting point</param>
        /// <param name="velocity">Velocity of intersecting point</param>
        /// <returns>Updated velocity of moving object</returns>
        public static Vector2 CollideLine(Line line, Rectangle intercept, Vector2 velocity, bool friction = false)
        {
            Vector2 outVel = velocity;

            Vector2[] points = new Vector2[4] { 
                new Vector2(intercept.Left, intercept.Top),
                new Vector2(intercept.Left, intercept.Bottom),
                new Vector2(intercept.Right, intercept.Top),
                new Vector2(intercept.Right, intercept.Bottom)
            };

            foreach(Vector2 v in points)
            {
                if(IsInterceptLine(line, v, velocity))
                {
                    var m = GetM(line);
                    if (line.Inversion == SlopeInversion.Bottom)
                        outVel.Y = 0;
                    else
                        outVel.Y = m;
                    if (!friction)
                        outVel.X = m;
                    return outVel;
                }
            }

            return velocity;
        }

        /// <summary>
        /// Detects if a point intersects a plane.
        /// </summary>
        /// <param name="line">The line being calculated</param>
        /// <param name="intercept">Intersceting point</param>
        /// <param name="velocity">Velocity of intersecting point</param>
        public static bool IsInterceptLine(Line line, Vector2 intercept, Vector2 velocity)
        {
            float m = GetM(line);
            float x = intercept.X - line.GetLeast().X;

            float b = line.GetLeastY();
            if (float.IsNegative(m))
                b = line.GetMostY();

            float y = m * x + b;

            if (x > line.GetMost().X - line.GetLeast().X + 1 || x <= -1)
                return false;
            else if ((line.Inversion == SlopeInversion.Bottom && intercept.Y + velocity.Y > y) || (line.Inversion == SlopeInversion.Top && intercept.Y + velocity.Y < y))
                return true;
            else
                return false;
        }

        /// <summary>
        /// The basic y = mx + b calculations.
        /// </summary>
        /// <param name="line">The line being calculated</param>
        /// <param name="intercept">Intersceting point</param>
        /// <returns>Y value of point</returns>
        public static float InterceptLineFundemental(Line line, Vector2 intercept)
        {
            float m = GetM(line);
            float x = intercept.X - line.GetLeast().X;

            float b = line.GetLeastY();
            if (float.IsNegative(m))
                b = line.GetMostY();

            float y = m * x + b;

            return y;
        }

        /// <summary>
        /// Gets the m value of a slope.
        /// </summary>
        public static float GetM(Line line)
        {
            return (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
        }

        public static bool TouchingLeft(Rectangle r1, Rectangle r2, float v)
        {
            return r1.Right + v > r2.Left &&
                    r1.Left < r2.Left &&
                    r1.Bottom > r2.Top &&
                    r1.Top < r2.Bottom;
        }

        public static bool TouchingRight(Rectangle r1, Rectangle r2, float v)
        {
            return r1.Left + v < r2.Right &&
                    r1.Right > r2.Right &&
                    r1.Bottom > r2.Top &&
                    r1.Top < r2.Bottom;
        }

        public static bool TouchingTop(Rectangle r1, Rectangle r2, float v)
        {
            return r1.Bottom + v > r2.Top &&
                    r1.Top < r2.Top &&
                    r1.Right > r2.Left &&
                    r1.Left < r2.Right;
        }

        public static bool TouchingBottom(Rectangle r1, Rectangle r2, float v)
        {
            return r1.Top + v < r2.Bottom &&
                    r1.Bottom > r2.Bottom &&
                    r1.Right > r2.Left &&
                    r1.Left < r2.Right;
        }

        /// <summary>
        /// Calculate collisions for two rectangles.
        /// </summary>
        /// <param name="geo">The static rectangle</param>
        /// <param name="intercept">The moving object</param>
        /// <param name="velocity">Velocity of moving object</param>
        /// <returns>Updated velocity of moving object</returns>
        public static Vector2 CollideRectangle(Rectangle geo, Rectangle intercept, Vector2 velocity)
        {
            Vector2 outVel = velocity;
            if (IsInterceptRecHor(intercept, geo, velocity.X))
                outVel.X = 0;
            if (IsInterceptRecVer(intercept, geo, velocity.Y))
                outVel.Y = 0;
            return outVel;
        }

        public static bool IsInterceptRecHor(Rectangle inter, Rectangle geo, float xVel)
        {
            return TouchingLeft(inter, geo, xVel) || TouchingRight(inter, geo, xVel);
        }

        public static bool IsInterceptRecVer(Rectangle geo, Rectangle intercept, float yVel)
        {
            return TouchingTop(geo, intercept, yVel) || TouchingBottom(geo, intercept, yVel);
        }

        /// <summary>
        /// Calculates velocity based on physics.
        /// Should be called before collision.
        /// </summary>
        /// <param name="velocity">Current velocity of object</param>
        /// <param name="weight">Weight of object</param>
        /// <param name="deltaTime">Delta time of game</param>
        /// <param name="gravity">Enable gravity</param>
        /// <param name="friction">Enable horizontal friction</param>
        /// <returns>Physical velocity</returns>
        public static Vector2 UpdateVelocity(Vector2 velocity, float weight, float deltaTime, bool gravity = true, bool friction = true)
        {
            Vector2 outVel = velocity;
            if(friction)
                outVel.X = (float)Math.Round(outVel.X /= weight / 100 + 1, 3, MidpointRounding.ToZero);
            if (gravity)
                outVel.Y += weight / gravityF * deltaTime;
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
