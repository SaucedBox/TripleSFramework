using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TripleS.Lighting {

    /// <summary>
    /// Stores data for lighting.
    /// </summary>
    public class LightingEngine {

        public List<LightData> Lights { get; set; }
        public Effect LightingEffect { get; protected set; }
        public Effect BackgroundEffect { get; protected set; }
        public int MaxLights { get; }

        private static Vector2[] lightPos;
        private static Vector3[] lightCol;
        private static float[] brightness;
        private static float[] radius;

        private static EffectParameter _lightPos;
        private static EffectParameter _lightCol;
        private static EffectParameter _brightness;
        private static EffectParameter _radius;

        public LightingEngine(int ml)
        {
            MaxLights = ml;
            Lights = new List<LightData>(MaxLights);
            lightPos = new Vector2[MaxLights];
            lightCol = new Vector3[MaxLights];
            brightness = new float[MaxLights];
            radius = new float[MaxLights];
        }
        
        /// <summary>
        /// Loads effects.
        /// </summary>
        /// <param name="path">Path to main lighting effect</param>
        /// <param name="bg">Background effect (optional)</param>
        public void Load(ContentManager content, string path, string bg = "")
        {
            LightingEffect = content.Load<Effect>(path);
            if (bg != "")
                BackgroundEffect = content.Load<Effect>(bg);
        }

        /// <summary>
        /// Initiates parameters.
        /// </summary>
        public void GetParams()
        {
            _lightPos = LightingEffect.Parameters["lightPos"];
            _lightCol = LightingEffect.Parameters["lightCol"];
            _radius = LightingEffect.Parameters["radius"];
            _brightness = LightingEffect.Parameters["brightness"];

            for (int i = 0; i < MaxLights; i++)
            {
                LightData light = Lights[i];
                lightPos[i] = light.Position;
                lightCol[i] = light.Color;
                radius[i] = light.Radius;
                brightness[i] = light.Brightness;
            }
        }

        /// <summary>
        /// Applies parameters.
        /// </summary>
        public void SetParams()
        {
            _lightPos.SetValue(lightPos);
            _lightCol.SetValue(lightCol);
            _radius.SetValue(radius);
            _brightness.SetValue(brightness);
        }

        public void SetLightPos(int index, Vector2 pos)
        {
            var ol = Lights[index];
            Lights[index] = new LightData(pos, ol.Radius, ol.Brightness, ol.Color);
        }
        public void SetLightColor(int index, Vector3 col)
        {
            var ol = Lights[index];
            Lights[index] = new LightData(ol.Position, ol.Radius, ol.Brightness, col);
        }
        public void SetLightRadius(int index, float rad)
        {
            var ol = Lights[index];
            Lights[index] = new LightData(ol.Position, rad, ol.Brightness, ol.Color);
        }
        public void SetLightBright(int index, float bri)
        {
            var ol = Lights[index];
            Lights[index] = new LightData(ol.Position, ol.Radius, bri, ol.Color);
        }
    }
}
