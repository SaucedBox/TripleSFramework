using System;
using Microsoft.Xna.Framework;

namespace TripleS.Physics {

    /// <summary>
    /// Data for a line collider.
    /// </summary>
    public struct Line {

        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }
        public Vector2 Second { get; private set; }
        public Vector2 Third { get; private set; }
        public Rectangle Bounds { get; private set; }
        public float Length { get; private set; }
        public SlopeInversion Inversion { get; private set; }

        public Line(Vector2 first, Vector2 last, SlopeInversion inversion)
        {
            Start = first;
            End = last;
            Second = new Vector2(Start.X, End.Y);
            Third = new Vector2(End.X, Start.Y);
            Inversion = inversion;

            var leastf = Math.Min(Start.X + Start.Y, End.X + End.Y);
            Vector2 least = End;
            Vector2 most = End;
            if (leastf == Start.X + Start.Y)
                least = Start;
            else
                most = Start;

            Bounds = new Rectangle((int)least.X, (int)least.Y, (int)(most.X - least.X) + 1, (int)(most.Y - least.Y) + 1);
            Length = (float)Math.Sqrt((most.X - least.X) + (most.Y - least.Y));
        }

        public Vector2 GetLeast()
        {
            var leastf = Math.Min(Start.X, End.X);
            Vector2 least = End;
            if (leastf == Start.X)
                least = Start;

            return least;
        }

        public Vector2 GetMost()
        {
            var mostf = Math.Max(Start.X, End.X);
            Vector2 most = End;
            if (mostf == Start.X)
                most = Start;

            return most;
        }

        public float GetMostY()
        {
            var mostf = Math.Max(Start.Y, End.Y);
            float most = End.Y;
            if (mostf == Start.Y)
                most = Start.Y;

            return most;
        }

        public float GetLeastY()
        {
            var leastf = Math.Min(Start.Y, End.Y);
            float least = End.Y;
            if (leastf == Start.Y)
                least = Start.Y;

            return least;
        }
    }

    public enum SlopeInversion
    {
        Top,
        Bottom
    }
}
