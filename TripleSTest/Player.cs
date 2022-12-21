using System;
using TripleS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TripleSTest {
    public class Player : MinimalComponent {

        public Player(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            System.Diagnostics.Debug.Print($"");
        }
    }
}
