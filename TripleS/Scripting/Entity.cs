using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TiledCS;

namespace TripleS.Scripting {

    /// <summary>
    /// Base class for a map entity.
    /// </summary>
    public abstract class Entity {

        public Vector2 Position { get; set; }
        public Rectangle? Bounds { get; set; }
        public string Name { get; set; }
        public List<EntityProp> Properties { get; set; }
        public bool Active { get; set; }

        public virtual DefaultProp[] DefaultProperties { get; protected set; }
        public virtual string ID { get; protected set; }

        public virtual void Init() { }
        public virtual void Load(ContentManager content) { }
        public virtual void Draw(Renderer renderer) { }
        public virtual void Update() { }
    }

    public struct EntityProp
    {
        public DefaultProp ParentProp { get; private set; }
        public string Value { get; set; }

        public EntityProp(DefaultProp parent, string value)
        {
            Value = value;
            ParentProp = parent;
        }

        public object GetValue()
        {
            object val = Value;
            switch (ParentProp.Type)
            {
                case TiledPropertyType.Int:
                    val = int.Parse(Value);
                    break;
                case TiledPropertyType.Float:
                    val = float.Parse(Value);
                    break;
                case TiledPropertyType.Bool:
                    val = bool.Parse(Value);
                    break;
            }
            return val;
        }
    }

    public struct DefaultProp
    {
        public string ID { get; set; }
        public TiledPropertyType Type { get; set; }

        public DefaultProp(string id, TiledPropertyType type)
        {
            ID = id;
            Type = type;
        }
    }
}
