
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class VelocitySlider1 : UdonSharpBehaviour
{
    public Text velocityText;
    public Slider velocitySlider;
    private void Update()
    {
        velocityText.text = ((int)velocitySlider.value).ToString();
    }
}
