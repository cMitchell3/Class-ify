using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;



namespace Com.CS.Classify
{
    public class MasterClientKickHandler : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        
        public void OnEvent(EventData photonEvent)
        {
            Debug.Log($"Received event with code: {photonEvent.Code}");
            if (photonEvent.Code == KickPlayerHandler.KickRequestEventCode && PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"Code matches event");
                object[] data = (object[])photonEvent.CustomData;
                int targetActorNumber = (int)data[0];

                Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber);
                if (targetPlayer != null)
                {
                    PhotonNetwork.CloseConnection(targetPlayer);
                    Debug.Log($"Player {targetPlayer.NickName} has been kicked from the room.");
                }
            }
        }

        public override void OnEnable()
        {
            Debug.Log("Masteclient kick player enabled");
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }



    }
}
