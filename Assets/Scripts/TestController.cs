
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class TestController : UdonSharpBehaviour
{
    private const string L_HORIZONTAL = "Horizontal";
    private const string R_HORIZONTAL = "Oculus_CrossPlatform_SecondaryThumbstickHorizontal";

    private const string L_VERTICAL = "Vertical";
    private const string R_VERTICAL = "Oculus_CrossPlatform_SecondaryThumbstickVertical";

    private bool isPickupping = false;

    [SerializeField]
    private GameObject upSign;

    [SerializeField]
    private GameObject downSign;

    [SerializeField]
    private GameObject rightSign;

    [SerializeField]
    private GameObject leftSign;

    private void Update()
    {
        if (isPickupping)
        {
            if (Input.GetAxis(L_HORIZONTAL) < 0 || Input.GetAxis(R_HORIZONTAL) < 0)
            {
                leftSign.SetActive(true);
                rightSign.SetActive(false);
                upSign.SetActive(false);
                downSign.SetActive(false);
            }
            else if (Input.GetAxis(L_HORIZONTAL) > 0 || Input.GetAxis(R_HORIZONTAL) > 0)
            {
                leftSign.SetActive(false);
                rightSign.SetActive(true);
                upSign.SetActive(false);
                downSign.SetActive(false);
            }
            
            if (Input.GetAxis(L_VERTICAL) > 0 || Input.GetAxis(R_VERTICAL) > 0)
            {
                leftSign.SetActive(false);
                rightSign.SetActive(false);
                upSign.SetActive(true);
                downSign.SetActive(false);
            }
            else if (Input.GetAxis(L_VERTICAL) < 0 || Input.GetAxis(R_VERTICAL) < 0)
            {
                leftSign.SetActive(false);
                rightSign.SetActive(false);
                upSign.SetActive(false);
                downSign.SetActive(true);
            }
        }
    }

    public override void OnPickup()
    {
        isPickupping = true;
    }

    public override void OnDrop()
    {
        isPickupping = false;
    }
}
