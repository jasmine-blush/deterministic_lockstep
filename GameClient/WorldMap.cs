using GameClient.Managers;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient {
    public struct WorldMap {
        public bool NeedsUpdate;

        public byte[,] WorldArray;
        public RenderTexture WorldRenderTexture;
        public Sprite DrawSprite;

        public WorldMap(byte[,] worldArray) {
            NeedsUpdate = true;

            WorldArray = worldArray;

            WorldRenderTexture = new RenderTexture((uint)(worldArray.GetLength(0)/2) * TileManager.TileSize, (uint)(worldArray.GetLength(0)/2) * TileManager.TileSize);
            WorldRenderTexture.Clear(Color.White);
            WorldRenderTexture.Display();

            DrawSprite = new Sprite();
            DrawSprite.Texture = WorldRenderTexture.Texture;
        }
    }
}
