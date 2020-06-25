
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerMods : UdonSharpBehaviour
{
    [SerializeField]
    private float jumpImpulse = 3f;
    
    [SerializeField]
    private float walkSpeed = 2f;
    
    [SerializeField]
    private  float runSpeed = 4f;

    [SerializeField]
    private  float gravityStrength = 1.0f;

    private void Start()
    {
        var player = Networking.LocalPlayer;

        if (player == null) return;

        player.SetJumpImpulse(jumpImpulse);
        player.SetWalkSpeed(walkSpeed);
        player.SetRunSpeed(runSpeed);
        player.SetGravityStrength(gravityStrength);
    }
}
