using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Com.CS.Classify
{
    public class KickPlayerHandler : MonoBehaviourPunCallbacks
    {
        // Unique event code
        public const byte KickRequestEventCode = 1;
        public GameNetworkManager gameNetworkManager;

        public void RequestKickPlayer(string playerName)
        {
            Debug.Log("Playername in request kick is: " + playerName);
            Player targetPlayer = gameNetworkManager.GetPlayerByUsername(playerName);
            if (targetPlayer != null)
            {
                var content = new object[] { targetPlayer.ActorNumber };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                PhotonNetwork.RaiseEvent(KickRequestEventCode, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log($"Kick request sent for player {playerName}");
            }
            else
            {
                Debug.LogWarning($"Player with nickname {playerName} not found.");
            }
        }
    }
}
