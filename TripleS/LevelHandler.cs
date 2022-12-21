using System;
using System.Linq;
using System.Collections.Generic;
using TripleS.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TiledCS;

namespace TripleS {

    /// <summary>
    /// Manages level system.
    /// </summary>
    public class LevelHandler {

        public bool Active { get; set; }
        public bool DrawLevel { get; set; }
        public bool BackCull { get; set; }

        public static int CurrentLevel { get; protected set; }
        public static int TileWidth { get; protected set; }
        public static int TileHeight { get; protected set; }
        public static int MapTileWidth { get; protected set; }
        public static int MapTileHeight { get; protected set; }
        public static int ScriptID { get; protected set; }
        public static Rectangle MapBounds { get; protected set; }

        public EntityManager EntityMan { get; protected set; }
        protected bool loaded;
        protected string levFilePath;
        protected string tilFilePath;
        protected string scrFilePath;
        protected List<Texture2D> tileTextures;
        protected TiledMap map;
        protected Dictionary<int, TiledTileset> tilesets;
        protected Type[] eTypes;

        protected string levName;
        protected string levFileName;
        protected int layerName;

        public LevelHandler(string levelFolderPath, string tileFolderPath, string scriptFolderPath, int layer)
        {
            levFilePath = levelFolderPath;
            tilFilePath = tileFolderPath;
            scrFilePath = scriptFolderPath;
            layerName = layer;

            Active = true;
            BackCull = true;
            DrawLevel = true;
        }

        public virtual void Initialize(int levelParamId, Type[] entityTypes)
        {
            CurrentLevel = levelParamId;
            eTypes = entityTypes;
        }

        public virtual void Load(ContentManager content)
        {
            if (Active)
            {
                var tileEntries = ParamLoader.GetMarkers(MarkerType.Entry, "tilesets");
                tileTextures = new List<Texture2D>();
                foreach (ParamMarker em in tileEntries)
                {
                    string tileLoc = (string)ParamLoader.GetParam("tilesets", em.EntryID, "location");
                    var tex = content.Load<Texture2D>($"{tilFilePath}/{tileLoc}");
                    tileTextures.Add(tex);
                }

                levName = (string)ParamLoader.GetParam("level", CurrentLevel, "name");
                ScriptID = (int)ParamLoader.GetParam("level", CurrentLevel, "script");
                levFileName = (string)ParamLoader.GetParam("level", CurrentLevel, "file");

                string mapPath = content.RootDirectory + "/" + levFilePath + "/" + levFileName + ".tmx";
                map = new TiledMap(mapPath);
                tilesets = map.GetTiledTilesets(content.RootDirectory + "/" + tilFilePath + "/");

                TileWidth = map.TileWidth;
                TileHeight = map.TileHeight;
                MapTileWidth = map.Width;
                MapTileHeight = map.Height;

                MapBounds = new Rectangle(0, 0, map.Width * TileWidth, map.Height * TileHeight);

                var objLayers = map.Layers.Where(x => x.type == TiledLayerType.ObjectLayer);
                EntityMan = new EntityManager(content, eTypes);
                EntityMan.Load(objLayers);

                ScriptManager.Init(content.RootDirectory + "/" + scrFilePath, ScriptID);

                loaded = true;
            }
        }

        public virtual void Update()
        {
            if (Active && loaded)
            {
                EntityMan.CallEnts(false);
                ScriptManager.ExecuteFunction("Update");
            }
        }

        public virtual void StandardDraw(Renderer renderer)
        {
            if (Active && DrawLevel && loaded)
            {
                EntityMan.CallEnts(true, renderer);

                float drawLayer = 0.9f;
                var tileLayers = map.Layers;
                foreach (var layer in tileLayers)
                {
                    drawLayer -= 0.1f;
                    for (int y = 0; y < layer.height; y++)
                    {
                        for (int x = 0; x < layer.width; x++)
                        {
                            var index = (y * layer.width) + x;
                            var gid = layer.data[index];
                            var tileX = x * map.TileWidth;
                            var tileY = y * map.TileHeight;

                            if (gid == 0)
                                continue;

                            var mapTileset = map.GetTiledMapTileset(gid);
                            var tileset = tilesets[mapTileset.firstgid];
                            var rect = map.GetSourceRect(mapTileset, tileset, gid);

                            var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                            var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);

                            bool cull = BackCull && destination.Intersects(renderer.View.ViewRect);
                            if (cull)
                            {
                                SpriteEffects effects = SpriteEffects.None;
                                if (map.IsTileFlippedHorizontal(layer, x, y))
                                {
                                    effects |= SpriteEffects.FlipHorizontally;
                                }
                                if (map.IsTileFlippedVertical(layer, x, y))
                                {
                                    effects |= SpriteEffects.FlipVertically;
                                }

                                var tex = DecideTexture(tileset.Name);
                                //spriteBatch.Draw(tex, destination.Location.ToVector2(), source, Color.White, 0f, Vector2.Zero, 1, effects, drawLayer);
                                renderer.BasicDraw(tex, destination.Location.ToVector2(), layerName, drawLayer, 1, 0, effects, Color.White, source);
                            }
                            else
                                continue;
                        }
                    }
                }
            }
        }

        private Texture2D DecideTexture(string name)
        {
            foreach(Texture2D tex in tileTextures)
            {
                if (name == tex.Name)
                    return tex;
            }
            return tileTextures[0];
        }

        public object GetTileProperty(int tilesetParamId, int tileId, string propertyName)
        {
            string tileLoc = (string)ParamLoader.GetParam("tilesets", tilesetParamId, "location");
            TiledTileset set = null;
            foreach(KeyValuePair<int, TiledTileset> keySet in tilesets)
            {
                if(keySet.Value.Name == tileLoc)
                {
                    set = keySet.Value;
                    break;
                }
            }

            foreach(TiledTile tile in set.Tiles)
            {
                if(tile.id == tileId)
                {
                    return GetProp(tile.properties, propertyName);
                }
            }
            return null;
        }

        private object GetProp(TiledProperty[] props, string name)
        {
            foreach (TiledProperty prop in props)
            {
                if (prop.name == name)
                {
                    switch (prop.type)
                    {
                        case TiledPropertyType.Int:
                            return int.Parse(prop.value);
                        case TiledPropertyType.Float:
                            return float.Parse(prop.value);
                        case TiledPropertyType.Bool:
                            return bool.Parse(prop.value);
                        case TiledPropertyType.String:
                            return prop.value;
                    }
                }      
            }
            return null;
        }
    }
}
