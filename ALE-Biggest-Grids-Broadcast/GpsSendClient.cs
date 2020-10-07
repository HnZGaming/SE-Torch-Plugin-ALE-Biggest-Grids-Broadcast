using NLog;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.Screens.Helpers;
using Sandbox.ModAPI;

namespace ALE_Biggest_Grids_Broadcast
{
    public sealed class GpsSendClient
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly SetDictionary<long, string> _sentGpsPerPlayers;

        public GpsSendClient()
        {
            _sentGpsPerPlayers = new SetDictionary<long, string>();
        }

        public void AddOrModifyGps(long playerId, ref MyGps gps, bool playSoundIfNew)
        {
            var gpsCollection = MyAPIGateway.Session?.GPS as MyGpsCollection;
            if (gpsCollection == null) return;

            var gpsName = gps.DisplayName;

            if (_sentGpsPerPlayers.Contains(playerId, gpsName))
            {
                gpsCollection.SendModifyGps(playerId, gps);
                Log.Trace($"Updated existing GPS ({gpsName})");
            }
            else
            {
                gpsCollection.SendAddGps(playerId, ref gps, gps.EntityId, playSoundIfNew);
                _sentGpsPerPlayers.Add(playerId, gpsName);
                Log.Trace($"Sent new GPS ({gpsName})");
            }
        }

        public void ForgetGpsSentToPlayer(long player)
        {
            _sentGpsPerPlayers.Clear(player);
        }
    }
}