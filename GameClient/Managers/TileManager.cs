using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace GameClient.Managers {
    /*Contains IDs of all possible tiles*/
    public enum Tiles {
        Dirt = 0,
        Grass = 1
    }

    class TileManager {
        //---------------------------
        //You can see this construct in multiple other classes. This is purely an implementation of the singleton pattern.
        private static readonly object _lck = new object();
        private static TileManager _instance;
        public static TileManager Instance {
            get {
                if(_instance == null) {
                    lock(_lck) {
                        if(_instance == null) {
                            _instance = new TileManager();
                        }
                    }
                }
                return _instance;
            }
        }
        //---------------------------

        public const int TileSize = 32;  //Every tile is 32x32 pixels in size

        public List<Sprite> _tiles;

        private TileManager() {
            _tiles = new List<Sprite>();
        }

        /*Loads all the textures and saves them in an accessible format*/
        public void LoadContent() {
            Texture tileMap = new Texture(@"Assets\Tiles.png");
            IntRect tileArea;
            for(int i=0; i <= (int)Tiles.Grass; i++) {
                tileArea = new IntRect(i * TileSize, 0, TileSize, TileSize);
                _tiles.Add(new Sprite(tileMap, tileArea));
            }
        }

        public Sprite GetTile(Tiles tileID) {
            return _tiles[(int)tileID];
        }

        public Sprite GetTile(int tileID) {
            return _tiles[tileID];
        }
    }
}
