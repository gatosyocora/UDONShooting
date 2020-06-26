
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// GameObjectのActiveをLocalで切り替えるスイッチをつくる
/// </summary>
public class ToggleSwitch : UdonSharpBehaviour
{
    /// <summary>
    /// Activeを切り替えるGameObject
    /// </summary>
    public GameObject target;

    public override void Interact()
    {
        // Active状態を反転させる
        target.SetActive(!target.activeSelf);
    }
}
