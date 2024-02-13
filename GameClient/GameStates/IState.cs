namespace GameClient.GameStates {
    interface IState {
        void Initialize();
        void LoadContent();
        void Update(float gameTime);
        void Draw();
        void UnloadContent();
    }
}
