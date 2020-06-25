
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class HookController : UdonSharpBehaviour
{
    [SerializeField] 
    private GameObject movePoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private LineRenderer line;

    [SerializeField]
    private Transform defaultHookPoint;

    private bool isPickupping = false;

    [UdonSynced]
    private bool isHooking = false;

    private const string L_TOUCHPAD_CLICK = "Oculus_CrossPlatform_PrimaryThumbstick";
    private const string R_TOUCHPAD_CLICK = "Oculus_CrossPlatform_SecondaryThumbstick";

    private const string L_VERTICAL = "Vertical";
    private const string R_VERTICAL = "Oculus_CrossPlatform_SecondaryThumbstickVertical";

    [SerializeField]
    private GameObject upSign;

    [SerializeField]
    private GameObject downSign;

    [SerializeField]
    private VRC_Pickup pickup;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        Networking.SetOwner(Networking.LocalPlayer, movePoint);
        isPickupping = true;
    }

    public override void OnDrop()
    {
        isPickupping = false;
    }

    public override void OnPickupUseDown()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            movePoint.transform.position = hit.point;
            isHooking = true;
        }
    }

    private void Update()
    {
        if (isPickupping && isHooking)
        {

            if (pickup.currentHand == VRC_Pickup.PickupHand.Left &&
                Input.GetButton(L_TOUCHPAD_CLICK) && Input.GetAxis(L_VERTICAL) > 0)
            {
                PressUp();
            }

            if (pickup.currentHand == VRC_Pickup.PickupHand.Right &&
                Input.GetButton(R_TOUCHPAD_CLICK) && Input.GetAxis(R_VERTICAL) > 0)
            {
                PressUp();
            }

            if (Input.GetKey(KeyCode.R))
            {
                PressUp();
            }

            if (pickup.currentHand == VRC_Pickup.PickupHand.Left &&
                Input.GetButton(L_TOUCHPAD_CLICK) && Input.GetAxis(L_VERTICAL) < 0)
            {
                PressDown();
            }

            if (pickup.currentHand == VRC_Pickup.PickupHand.Right &&
                Input.GetButton(R_TOUCHPAD_CLICK) && Input.GetAxis(R_VERTICAL) < 0)
            {
                PressDown();
            }

            if (Input.GetKey(KeyCode.T))
            {
                PressDown();
            }
        }

        if (isPickupping && !isHooking)
        {
            movePoint.transform.position = defaultHookPoint.position;
        }

        line.SetPosition(0, transform.position);
        line.SetPosition(1, movePoint.transform.position);
    }

    private void PressUp()
    {
        upSign.SetActive(true);
        downSign.SetActive(false);
        var moveVector = movePoint.transform.position - transform.position;
        Networking.LocalPlayer.SetVelocity(moveVector * speed);
    }

    private void PressDown()
    {
        upSign.SetActive(false);
        downSign.SetActive(true);
        isHooking = false;
    }

    public void ResetPosition()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        this.transform.position = defaultPosition;
        this.transform.rotation = defaultRotation;
        isHooking = false;
    }
}
