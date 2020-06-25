
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class CheatDice : UdonSharpBehaviour
{
    public Rigidbody rigidbody;

    public Toggle cheatToggle;
    public Text ownerText;

    private bool isPickupping = false;

    [SerializeField]
    private UdonBehaviour recorder;

    private void Update()
    {
        ownerText.text = Networking.GetOwner(this.gameObject).displayName;

        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject) && !isPickupping)
        {
            // ÉTÉCÉRÉçÇ™é~Ç‹Ç¡ÇΩÇ∆Ç´ÇÃèàóù
            if (rigidbody.velocity == Vector3.zero) 
            {
                if (cheatToggle.isOn && Vector3.Dot(this.transform.forward, Vector3.up) < 0.6f)
                {
                    float x = Random.Range(0f, 1f);
                    float y = Random.Range(0f, 1f);
                    float z = Random.Range(0f, 1f);

                    var forceAndTorque = new Vector3(x, y, z);

                    rigidbody.AddForce(forceAndTorque, ForceMode.Impulse);
                    rigidbody.AddTorque(forceAndTorque * Mathf.PI, ForceMode.Impulse);
                }
                else
                {
                    recorder.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StopRecordingInOwner");
                }
            }
        }
    }

    public override void OnPickup()
    {
        isPickupping = true;
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }

    public override void OnDrop()
    {
        isPickupping = false;
        recorder.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StartRecordingInOwner");
    }
}
