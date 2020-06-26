
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// スライダーの値を表示する
/// </summary>
public class VelocitySlider1 : UdonSharpBehaviour
{
    /// <summary>
    /// 値表示用のUIText
    /// </summary>
    public Text velocityText;

    /// <summary>
    /// 値を参照するUISlider
    /// </summary>
    public Slider velocitySlider;

    private void Update()
    {
        // UISliderの値をUITextに表示
        velocityText.text = ((int)velocitySlider.value).ToString();
    }
}
