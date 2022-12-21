using TripleS.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TripleS {

    /// <summary>
    /// Basic methods and setup for game.
    /// </summary>
    public static class SSS {

        public static Game Game { get; private set; }
        public static string GameName { get; private set; }

        public static Texture2D Square
        { 
            get 
            {
                if (Game != null)
                {
                    Texture2D tex = new Texture2D(Game.GraphicsDevice, 1, 1);
                    tex.SetData(new Color[] { new Color(1, 1, 1, 1f) });
                    return tex;
                }
                return null;
            }
            private set { }
        }

        public static void Init(Game game, string name, int maxSaveSlots)
        {
            Game = game;
            GameStateManager.maxSaves = maxSaveSlots;
            GameName = name;
            GameStateManager.States = new Dictionary<int, int>();
            ParamLoader.Initialize(game);
            UIManager.Load();
        }

        /// <summary>
        /// Basic, default draw info.
        /// </summary>
        public static DrawInfo DefaultInfo() 
        {
            return new DrawInfo(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        }
    }
}
