
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BoostRing : UdonSharpBehaviour
{
    public float boostVelocity;

    public ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(CharacterController))
        {
            Networking.LocalPlayer.SetVelocity(Networking.LocalPlayer.GetVelocity() + this.transform.forward * boostVelocity);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayParticle");
        }
    }

    public void PlayParticle()
    {
        particle.Play();
    }
}
