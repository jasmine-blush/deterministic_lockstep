using System;

namespace GameClient {
    class FramesPerSecondCounter {
        public float CurrentGameTime = 0; //Current time since last full second


        private float _averageFPS = 0; //averageFPS since beginning of game loop
        private float _currentSum = 0; //Time since last full second, in a format for FPS calculation
        private float _currentCount = 0; //Amount of frames since last full second

        #region Important Methods

        //This calculates a new average FPS after every full second passed
        public void Update(float gameTime) {
            _currentSum += 1000f / gameTime;
            _currentCount++;
            CurrentGameTime += gameTime;
            if(CurrentGameTime >= 1000f) {
                _averageFPS = _currentSum / _currentCount;
                _currentSum = 0;
                _currentCount = 0;
                CurrentGameTime = 0;
            }
        }

        #endregion
        #region Setter/Getters

        public String GetFPS() {
            return ((int)_averageFPS).ToString();
        }

        #endregion
    }
}
