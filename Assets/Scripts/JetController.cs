
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class JetController : UdonSharpBehaviour
{
    public Slider speedSlider;
    public float boostSpeed;

    public VRC_Pickup pickup;

    private bool isJetting = false;
    private bool isPickupping = false;
    
    private bool useVR = false;

    public Transform playerMonitorCamera;
    private float cameraOffset = 1f;

    [UdonSynced(UdonSyncMode.None)]
    public bool isActiveCamera = false;

    public Text playerNameText;
    public Text velocityText;

    private Vector3 vector3z1 = new Vector3(0, 0, 1f);

    [UdonSynced(UdonSyncMode.None)]
    public string playerName;

    public Text ownerNameText;

    private string defaultPlayerName;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    public Text debugText;

    public Slider energyBar;
    private int energy;
    private const int MAX_ENERGY = 500;

    private void Start()
    {
        defaultPlayerName = playerName;
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;
        energy = MAX_ENERGY;
        energyBar.value = energy / (MAX_ENERGY * 1f);
    }

    private void Update()
    {
        if (Networking.GetOwner(this.gameObject) != null)
        {
            ownerNameText.text = Networking.GetOwner(this.gameObject).displayName;
        }

        if (isJetting && energy > 0)
        {
            Networking.LocalPlayer.SetVelocity(this.transform.forward * speedSlider.value);
            energy--;
            energyBar.value = energy / (MAX_ENERGY * 1f);
        }

        if (isPickupping)
        {
            velocityText.text = Networking.LocalPlayer.GetVelocity().ToString();

            if (isActiveCamera)
            {
                FollowCameraToHead();
            }

            if (Networking.LocalPlayer.IsPlayerGrounded())
            {
                if (energy < MAX_ENERGY)
                {
                    energy++;
                    energyBar.value = energy / (MAX_ENERGY * 1f);
                }
            } 
        }
        else
        {
            velocityText.text = "Velocity";
        }

        if (isPickupping && !useVR)
        {
            if (!Networking.LocalPlayer.IsPlayerGrounded() && Input.GetKeyDown(KeyCode.B))
            {
                Boost();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCamera();
            }
        }

        debugText.text = Networking.GetOwner(playerMonitorCamera.gameObject).displayName;
    }

    private void LateUpdate()
    {
        playerMonitorCamera.gameObject.SetActive(isActiveCamera);
        playerNameText.text = playerName;
    }

    public override void OnPickup()
    {
        useVR = Networking.LocalPlayer.IsUserInVR();

        TakeOwnerships();

        isPickupping = true;

        playerName = Networking.LocalPlayer.displayName;
        isActiveCamera = true;
    }

    public override void OnDrop()
    {
        isPickupping = false;

        playerName = defaultPlayerName;

        ResetPlayerCameraPosition();
        isActiveCamera = false;
    }

    public override void OnPickupUseDown()
    {
        isJetting = true;
    }

    public override void OnPickupUseUp()
    {
        isJetting = false;
    }

    private void Boost()
    {
        Networking.LocalPlayer.SetVelocity(this.transform.forward * boostSpeed);
    }

    private void ToggleCamera()
    {
        if (isActiveCamera)
        {
            ResetPlayerCameraPosition();
        }
        isActiveCamera = !isActiveCamera;
    }

    private void FollowCameraToHead()
    {
        var headPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.Head);
        var headForward = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.Head) * vector3z1;
        playerMonitorCamera.position = headPos + headForward * cameraOffset;
        playerMonitorCamera.LookAt(headPos);
    }

    private void ResetPlayerCameraPosition()
    {
        playerMonitorCamera.localPosition = Vector3.zero;
        playerMonitorCamera.localRotation = Quaternion.identity;
        playerMonitorCamera.GetComponent<Camera>().Render();
    }

    public void Reset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DropController");
        ResetPosition();
        playerName = defaultPlayerName;
    }

    private void ResetPosition()
    {
        TakeOwnerships();
        this.transform.position = defaultPosition;
        this.transform.rotation = defaultRotation;
    }

    private void TakeOwnerships()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        if (!Networking.IsOwner(Networking.LocalPlayer, playerMonitorCamera.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, playerMonitorCamera.gameObject);
        }
    }

    public void DropController()
    {
        pickup.Drop();
        isPickupping = false;
    }

    public void ChargeEnergyToFull()
    {
        energy = MAX_ENERGY;
        energyBar.value = energy / (MAX_ENERGY * 1f);
    }
}
