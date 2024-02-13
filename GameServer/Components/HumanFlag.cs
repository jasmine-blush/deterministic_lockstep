using System.Net;

namespace GameServer.Components {
    public struct HumanFlag {
        public int HumanID;
        public IPAddress IPAddr;
        public int LastFrame;

        public HumanFlag(int humanID, IPAddress ipAddress) {
            HumanID = humanID;
            IPAddr = ipAddress;
            LastFrame = -1;
        }
    }
}
