using System;
using Microsoft.Xna.Framework;

namespace TripleS.Lighting {

    /// <summary>
    /// Data for a point light.
    /// </summary>
    public struct LightData {

        public Vector3 Color { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public float Brightness { get; set; }

        public LightData(Vector2 pos, float radius, float brightness, Vector3 color)
        {
            Color = color;
            Position = pos;
            Radius = radius;
            Brightness = brightness;
        }
    }
}
