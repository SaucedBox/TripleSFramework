using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TripleS.Lighting;

namespace TripleS {

    /// <summary>
    /// A drawing helper class that supports the lighting system.
    /// </summary>
    public class Renderer {

        public Effect CurrentEffect { get; protected set; }
        public SpriteBatch Batch { get; private set; }
        public DrawInfo Info { get; private set; }
        public GameView View { get; private set; }
        public LightingEngine Lighting { get; protected set; }
        public bool EnableLigthing { get; set; }
        public bool EnableBGEffects { get; set; }

        public Vector2 monitorResolution;
        public Vector2 gameResolution;

        public RenderTarget2D screenTarget1; 
        public RenderTarget2D screenTarget2;

        public Renderer(SpriteBatch spriteBatch, DrawInfo info, GameView port, int ml)
        {
            Batch = spriteBatch;
            Info = info;
            View = port;
            Lighting = new LightingEngine(ml);
        }

        public void BasicDraw(Texture2D texture, Vector2 position, int layer, float subLayer, float size = 1, float rot = 0, SpriteEffects mirror = SpriteEffects.None, Color? col = null, Rectangle? source = null, Vector2? rotSource = null, Effect effect = null)
        {
            if (!col.HasValue)
                col = Color.White;
            if (!rotSource.HasValue)
                rotSource = Vector2.Zero;

            if (CurrentEffect != effect)
            {
                CurrentEffect = effect;
                Batch.End();
                Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, CurrentEffect, View.MasterMatrix);
            }

            Batch.Draw(texture, position, source, col.Value, rot, rotSource.Value, size, mirror, GetProperLayer(layer, subLayer));
        }

        public void BasicDraw(Texture2D texture, Rectangle position, int layer, float subLayer, float rot = 0, SpriteEffects mirror = SpriteEffects.None, Color? col = null, Rectangle? source = null, Vector2? rotSource = null, Effect effect = null)
        {
            if (!col.HasValue)
                col = Color.White;
            if (!rotSource.HasValue)
                rotSource = Vector2.Zero;

            float width = (float)position.Width / (float)texture.Width;
            float height = (float)position.Height / (float)texture.Height;

            if (CurrentEffect != effect)
            {
                CurrentEffect = effect;
                Batch.End();
                Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, CurrentEffect, View.MasterMatrix);
            }
            
            Batch.Draw(texture, position.Location.ToVector2(), source, col.Value, 0, rotSource.Value, new Vector2(width, height), mirror, GetProperLayer(layer, subLayer));
        }

        public void DrawText(SpriteFont font, string text, Vector2 position, int layer, float subLayer, float size = 1, SpriteEffects effect = SpriteEffects.None, Color? col = null)
        {
            if (!col.HasValue)
                col = Color.White;
            Batch.DrawString(font, text, position, col.Value, 0, Vector2.Zero, size, effect, GetProperLayer(layer, subLayer));
        }

        /// <summary>
        /// Initiates first phase of drawing.
        /// </summary>
        public void PhaseOne()
        {
            CurrentEffect = null;
            Batch.GraphicsDevice.Clear(Color.Transparent);
            Batch.GraphicsDevice.SetRenderTarget(screenTarget1);

            Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, null, View.MasterMatrix);
        }

        /// <summary>
        /// Initiates second phase of drawing.
        /// </summary>
        public void PhaseTwo()
        {
            End();

            Batch.GraphicsDevice.SetRenderTarget(screenTarget2);

            Effect lf = EnableBGEffects ? Lighting.BackgroundEffect : null;
            Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, lf);
            Batch.Draw(screenTarget1, new Rectangle(0, 0, (int)gameResolution.X, (int)gameResolution.Y), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            End();

            CurrentEffect = null;
            Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, null, View.MasterMatrix);
        }

        /// <summary>
        /// Initiates third phase of drawing.
        /// </summary>
        public void PhaseThree()
        {
            End();

            Batch.GraphicsDevice.SetRenderTarget(null);

            Effect lf = EnableLigthing ? Lighting.LightingEffect : null;
            SetLightingParams();
            Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, lf);
            Batch.Draw(screenTarget2, new Rectangle(0, 0, (int)gameResolution.X, (int)gameResolution.Y), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            End();

            CurrentEffect = null;
            Begin(CurrentEffect);
        }

        /// <summary>
        /// Initiates last phase of drawing (ends everything).
        /// </summary>
        public void PhaseFour()
        {
            End();
        }

        public void Begin(Effect effect)
        {
            Batch.Begin(Info.SortMode, Info.Blend, Info.Sampler, Info.StencilState, Info.Rasterizer, effect, View.MasterMatrix);
        }

        public void End()
        {
            Batch.End();
        }

        /// <summary>
        /// Updates graphics device changes.
        /// </summary>
        public void UpdateGraphics()
        {
            monitorResolution = new Vector2();
            monitorResolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            monitorResolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            gameResolution = new Vector2();
            if (View.windowHeight != 0 && View.windowWidth != 0)
            {
                gameResolution.X = View.windowWidth;
                gameResolution.Y = View.windowHeight;
            }
            else
            {
                gameResolution.X = Batch.GraphicsDevice.Viewport.Width;
                gameResolution.Y = Batch.GraphicsDevice.Viewport.Height;
            }

            screenTarget1 = new RenderTarget2D(Batch.GraphicsDevice, (int)Batch.GraphicsDevice.Viewport.Width, (int)Batch.GraphicsDevice.Viewport.Height, false, Batch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            screenTarget2 = new RenderTarget2D(Batch.GraphicsDevice, (int)Batch.GraphicsDevice.Viewport.Width, (int)Batch.GraphicsDevice.Viewport.Height, false, Batch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        public void Load(ContentManager cm, string lightingPath)
        {
            UpdateGraphics();
            Lighting.Load(cm, lightingPath);
        }

        public void SetLightingParams()
        {
            Lighting.GetParams();
            Lighting.LightingEffect.Parameters["resolution"].SetValue(gameResolution);
            Lighting.LightingEffect.Parameters["cameraLocation"].SetValue(View.Position);
            Lighting.LightingEffect.Parameters["zoom"].SetValue(View.Zoom);
            Lighting.SetParams();
        }

        /// <summary>
        /// Gets draw layer.
        /// </summary>
        /// <returns>Draw layer</returns>
        public float GetProperLayer(int main, float sub)
        {
            var rSub = Math.Clamp(sub, 0, 1);
            var rMain = Math.Clamp(main, 0, 10);
            var real = (rMain / 10) + (rSub / 100);
            return Math.Clamp(real, 0, 1);
        }
    }
}
