using SFML.Graphics;
using System;

namespace GameClient.Managers {
    class FontManager {
        //-----------------------------------------
        //Singleton
        private static readonly object _lck = new object();
        private static FontManager _instance;
        public static FontManager Instance {
            get {
                if(_instance == null) {
                    lock(_lck) {
                        if(_instance == null) {
                            _instance = new FontManager();
                        }
                    }
                }
                return _instance;
            }
        }
        //-----------------------------------------

        //All needed fonts will be defined like this:
        public Font Arial;

        private FontManager() {
            Arial = new Font(@"Assets\arial.ttf");
        }

        #region Important Methods

        public void Dispose() {
            Arial.Dispose();
        }

        #endregion
    }
}
