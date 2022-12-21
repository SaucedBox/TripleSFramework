using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TiledCS;

namespace TripleS.Scripting {
    public class EntityManager {

        private ContentManager content;
        private Type[] entityTypes;
        private List<Entity> entities;

        public EntityManager(ContentManager cm, Type[] types)
        {
            content = cm;
            entityTypes = types;
        }

        public void Load(IEnumerable<TiledLayer> objectLayers)
        {
            entities = new List<Entity>();
            foreach(TiledLayer layer in objectLayers)
            {
                foreach(TiledObject obj in layer.objects)
                {
                    dynamic ent = null;
                    Type usingType = null;
                    foreach(Type type in entityTypes)
                    {
                        var tempEnt = (Entity)Activator.CreateInstance(type);
                        if (tempEnt.ID == obj.@class)
                        {
                            usingType = type;
                            ent = tempEnt;
                        }
                    }

                    ent.Properties = new List<EntityProp>();
                    foreach(DefaultProp parentProp in ent.DefaultProperties)
                    {
                        var mathcingProp = obj.properties.Where(x => x.name == parentProp.ID && x.type == parentProp.Type).First();
                        ent.Properties.Add(new EntityProp(parentProp, mathcingProp.value));
                    }

                    Vector2 position = new Vector2(obj.x, obj.y);
                    Rectangle? bounds = null;

                    if (obj.point == null)
                        bounds = new Rectangle((int)position.X, (int)position.Y, (int)obj.width, (int)obj.height);

                    ent.Position = position;
                    ent.Bounds = bounds;
                    ent.Name = obj.name;

                    entities.Add(ent);
                }
            }
            if(entities != null && entities.Count > 0)
            {
                foreach(Entity ent in entities)
                {
                    ent.Init();
                    ent.Load(content);
                }
            }
        }

        public void CallEnts(bool draw, Renderer ren = null)
        {
            foreach(Entity ent in entities)
            {
                if (draw && ren != null)
                {
                    ent.Draw(ren);
                }
                else
                {
                    ent.Update();
                }
            }
        }

        public Entity GetEnt(string name)
        {
            if (entities != null && entities.Count > 0)
            {
                foreach (Entity ent in entities)
                {
                    if (ent.Name == name)
                        return ent;
                }
            }
            return null;
        }

        public EntityProp GetEntityProperty(string entName, string propName)
        {
            if (entities != null && entities.Count > 0)
            {
                Entity ent = GetEnt(entName);
                if(ent != null)
                {
                    foreach(EntityProp prop in ent.Properties)
                    {
                        if (prop.ParentProp.ID == propName)
                            return prop;
                    }
                }
            }
            return new EntityProp();
        }
    }
}
