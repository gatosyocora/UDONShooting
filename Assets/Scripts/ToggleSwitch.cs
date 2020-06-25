
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleSwitch : UdonSharpBehaviour
{
    public GameObject target;
    public override void Interact()
    {
        target.SetActive(!target.activeSelf);
    }
}
