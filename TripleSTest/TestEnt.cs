using System;
using Microsoft.Xna.Framework.Graphics;
using TripleS.Scripting;
using TiledCS;
using TripleS;
using Microsoft.Xna.Framework.Content;

namespace TripleSTest {
    public class TestEnt : Entity {

        public override DefaultProp[] DefaultProperties { get; protected set; }
        public override string ID { get; protected set; }

        Texture2D tex;

        public TestEnt()
        {
            ID = "pp";
            DefaultProperties = new DefaultProp[1] {
                new DefaultProp("test", TiledPropertyType.String)
        };
        }

        public override void Load(ContentManager content)
        {
            tex = content.Load<Texture2D>("texture1");
        }

        public override void Draw(Renderer renderer)
        {
            renderer.BasicDraw(tex, Bounds.Value, 5, 0);
        }
    }
}
