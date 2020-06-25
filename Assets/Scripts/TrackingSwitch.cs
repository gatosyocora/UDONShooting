
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TrackingSwitch : UdonSharpBehaviour
{
    public Transform characterRoot;

    public Transform headPoint;
    public Transform rhandPoint;
    public Transform lhandPoint;
    public Transform hipPoint;
    public Transform rfootPoint;
    public Transform lfootPoint;

    private bool isTracking = false;

    private VRCPlayerApi player;

    public override void Interact()
    {
        if (isTracking)
        {
            isTracking = false;
            return;
        }

        isTracking = true;

        player = Networking.LocalPlayer;
        if (!Networking.IsOwner(player, this.gameObject))
        {
            Networking.SetOwner(player, this.gameObject);

            Networking.SetOwner(player, headPoint.gameObject);
            Networking.SetOwner(player, rhandPoint.gameObject);
            Networking.SetOwner(player, lhandPoint.gameObject);
            Networking.SetOwner(player, hipPoint.gameObject);
            Networking.SetOwner(player, rfootPoint.gameObject);
            Networking.SetOwner(player, lfootPoint.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (isTracking)
        {
            var distance = characterRoot.position - player.GetPosition();

            headPoint.position = player.GetBonePosition(HumanBodyBones.Head) + distance;
            headPoint.rotation = player.GetBoneRotation(HumanBodyBones.Head);

            hipPoint.position = player.GetBonePosition(HumanBodyBones.Hips) + distance;
            hipPoint.rotation = player.GetBoneRotation(HumanBodyBones.Hips);

            lhandPoint.position = player.GetBonePosition(HumanBodyBones.LeftHand) + distance;
            lhandPoint.rotation = player.GetBoneRotation(HumanBodyBones.LeftHand);

            rhandPoint.position = player.GetBonePosition(HumanBodyBones.RightHand) + distance;
            rhandPoint.rotation = player.GetBoneRotation(HumanBodyBones.RightHand);

            lfootPoint.position = player.GetBonePosition(HumanBodyBones.LeftFoot) + distance;
            lfootPoint.rotation = player.GetBoneRotation(HumanBodyBones.LeftFoot);

            rfootPoint.position = player.GetBonePosition(HumanBodyBones.RightFoot) + distance;
            rfootPoint.rotation = player.GetBoneRotation(HumanBodyBones.RightFoot);
        }
    }
}
