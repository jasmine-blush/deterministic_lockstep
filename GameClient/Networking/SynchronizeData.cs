using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient.Networking {
    public struct PositionData {
        public int HumanID;
        public int FrameID;
        public Vector2f Position;
    }

    /*Saves the position data of a server synchronization of all humans*/
    public static class SynchronizeData {
        private static readonly object _lck = new object();

        private static List<PositionData> _positionList = new List<PositionData>();

        public static void AddPositionData(PositionData positionData) {
            lock(_lck) {
                _positionList.Add(positionData);
            }
        }

        public static List<PositionData> AcquirePositionList() {
            Monitor.Enter(_lck);
            return _positionList;
        }

        public static void ReleasePositionList() {
            Monitor.Exit(_lck);
        }
    }
}
