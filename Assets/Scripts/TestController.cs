
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// 入力テスト用のコントローラをつくる
/// </summary>
public class TestController : UdonSharpBehaviour
{
    /// <summary>
    /// コントローラ入力用
    /// </summary>
    private const string L_HORIZONTAL = "Horizontal";
    private const string R_HORIZONTAL = "Oculus_CrossPlatform_SecondaryThumbstickHorizontal";
    private const string L_VERTICAL = "Vertical";
    private const string R_VERTICAL = "Oculus_CrossPlatform_SecondaryThumbstickVertical";

    /// <summary>
    /// 持っているかどうか
    /// </summary>
    private bool isPickupping = false;

    /// <summary>
    /// 入力可視化用
    /// </summary>
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
        // 持っている状態であるとき
        if (isPickupping)
        {
            // 左右いずれかのコントローラの左側に触れているなら
            if (Input.GetAxis(L_HORIZONTAL) < 0 || Input.GetAxis(R_HORIZONTAL) < 0)
            {
                leftSign.SetActive(true);
                rightSign.SetActive(false);
                upSign.SetActive(false);
                downSign.SetActive(false);
            }
            // 左右いずれかのコントローラの右側に触れているなら
            else if (Input.GetAxis(L_HORIZONTAL) > 0 || Input.GetAxis(R_HORIZONTAL) > 0)
            {
                leftSign.SetActive(false);
                rightSign.SetActive(true);
                upSign.SetActive(false);
                downSign.SetActive(false);
            }

            // 左右いずれかのコントローラの上側に触れているなら
            if (Input.GetAxis(L_VERTICAL) > 0 || Input.GetAxis(R_VERTICAL) > 0)
            {
                leftSign.SetActive(false);
                rightSign.SetActive(false);
                upSign.SetActive(true);
                downSign.SetActive(false);
            }
            // 左右いずれかのコントローラの下側に触れているなら
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
        // 持っている状態にする
        isPickupping = true;
    }

    public override void OnDrop()
    {
        // 持っていない状態にする
        isPickupping = false;
    }
}
