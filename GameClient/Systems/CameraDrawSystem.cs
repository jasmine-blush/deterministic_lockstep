using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using GameClient.GameStates;
using GameClient.Managers;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Systems {
    public sealed class CameraDrawSystem : ISystem<World> {
        public bool IsEnabled { get; set; }

        private Entity _player;
        private Vector2f _screenFitTiles;
        private Vector2f _halfWindowSize;

        public CameraDrawSystem(Entity player) {
            _player = player;

            _screenFitTiles = new Vector2f((float)StateManager.Instance.Window.Size.X / TileManager.TileSize, (float)StateManager.Instance.Window.Size.Y / TileManager.TileSize);
            _halfWindowSize = new Vector2f((float)StateManager.Instance.Window.Size.X / 2f, (float)StateManager.Instance.Window.Size.Y / 2f);

            StateManager.Instance.Window.Resized += Window_Resized;
        }

        public void Update(World world) {
            /*If position of player changed, update world render texture*/
            if(GameState.Instance.WorldMap.NeedsUpdate) {
                RenderTexture worldRenderTexture = GameState.Instance.WorldMap.WorldRenderTexture;
                worldRenderTexture.Clear(Color.White);

                Vector2f cameraPosition = _player.Get<PositionComponent>().Position;  //Set camera position to player position

                int startIndex_x = (int)(cameraPosition.X / TileManager.TileSize) - (int)(_screenFitTiles.X / 2f);
                int startIndex_y = (int)(cameraPosition.Y / TileManager.TileSize) - (int)(_screenFitTiles.Y / 2f);

                int endIndex_x = (int)(startIndex_x + _screenFitTiles.X);
                int endIndex_y = (int)(startIndex_y + _screenFitTiles.Y);

                if(startIndex_x < 0) {
                    startIndex_x = 0;
                }
                if(startIndex_y < 0) {
                    startIndex_y = 0;
                }


                int worldSize = GameState.Instance.WorldMap.WorldArray.GetLength(0) - 1;

                if(endIndex_x > worldSize)
                    endIndex_x = worldSize;
                if(endIndex_y > worldSize)
                    endIndex_y = worldSize;

                Sprite currentDrawSprite;
                byte[,] worldArray = GameState.Instance.WorldMap.WorldArray;
                //for loop through the range from starting to end index
                for(int i = startIndex_y; i <= endIndex_y; i++) {
                    for(int j = startIndex_x; j <= endIndex_x; j++) {
                        currentDrawSprite = TileManager.Instance.GetTile(worldArray[i, j]);

                        float pos_x = ((j - startIndex_x) * TileManager.TileSize) - (cameraPosition.X % TileManager.TileSize);
                        float pos_y = ((i - startIndex_y) * TileManager.TileSize) - (cameraPosition.Y % TileManager.TileSize);
                        if(startIndex_x == 0) {
                            pos_x = ((j - startIndex_x) * TileManager.TileSize) - (cameraPosition.X - _halfWindowSize.X);
                        }
                        if(startIndex_y == 0) { //TODO: fix weird positioning issue (jump at upper edge and too far at lower edge)
                            pos_y = ((i - startIndex_y) * TileManager.TileSize) - (cameraPosition.Y - _halfWindowSize.Y);
                        }
                        currentDrawSprite.Position = new Vector2f(pos_x, pos_y);

                        worldRenderTexture.Draw(currentDrawSprite);
                    }
                }

                worldRenderTexture.Display();

                GameState.Instance.WorldMap.NeedsUpdate = false;
            }
            StateManager.Instance.Window.Draw(GameState.Instance.WorldMap.DrawSprite);
        }

        private void Window_Resized(object sender, SFML.Window.SizeEventArgs e) {
            _screenFitTiles = new Vector2f((float)StateManager.Instance.Window.Size.X / TileManager.TileSize, (float)StateManager.Instance.Window.Size.Y / TileManager.TileSize);
            _halfWindowSize = new Vector2f(StateManager.Instance.Window.Size.X / 2f, StateManager.Instance.Window.Size.Y / 2f);
        }

        public void Dispose() {

        }
    }
}
