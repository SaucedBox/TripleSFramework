﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TripleS {

    /// <summary>
    /// Game camera.
    /// </summary>
    public class GameView {

        public Matrix MasterMatrix { get; private set; }
        public Rectangle ViewRect { get; private set; }
        public Vector2 Position { get; private set; }
        public float Zoom { get; set; }
        public Vector2 TargetPosition { get; set; }
        public float Speed { get; set; }
        public bool Lerp { get; set; }

        private Vector2 cameraPos;
        public int viewPortWidth;
        public int viewPortHeight;
        public int windowWidth;
        public int windowHeight;

        public GameView()
        {
            Zoom = 1;
            Speed = 1;
            cameraPos = new Vector2();
            Lerp = true;
        }

        public void Update(GraphicsDevice device)
        {
            windowWidth = device.Viewport.Width;
            windowHeight = device.Viewport.Height;
            if (Lerp)
            {
                cameraPos.X = MathHelper.Lerp(cameraPos.X, -TargetPosition.X + (viewPortWidth / 2), Speed);
                cameraPos.Y = MathHelper.Lerp(cameraPos.Y, -TargetPosition.Y + (viewPortHeight / 2), Speed);
            }
            else
            {
                cameraPos.X = -TargetPosition.X;
                cameraPos.Y = -TargetPosition.Y;
            }

            MasterMatrix = Matrix.CreateTranslation(cameraPos.X, cameraPos.Y, 0) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));

            viewPortHeight = (int)(windowHeight / Zoom);
            viewPortWidth = (int)(windowWidth / Zoom);
            Position = new Vector2(-cameraPos.X, -cameraPos.Y);
            ViewRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(viewPortWidth + Zoom), (int)(viewPortHeight + Zoom));
        }
    }
}
