
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// ワイヤーフックコントローラを作成する
/// 
/// Sceneに必要なもの
/// * LineRendererとVRC_PickupがついたGameObejct(本スクリプトをアタッチする)
///  - 子オブジェクトにEmptyObjectをおく = defaultHookPoint
/// * フック部分のGameObject = movePoint
/// </summary>
public class HookController : UdonSharpBehaviour
{
    /// <summary>
    /// 移動先の場所を示すGameObject
    /// </summary>
    [SerializeField] 
    private GameObject movePoint;

    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField]
    private float speed;

    /// <summary>
    /// ワイヤーに見立てたLineRenderer
    /// </summary>
    [SerializeField]
    private LineRenderer line;

    /// <summary>
    /// ワイヤーを発射していないときのmovePointの場所
    /// </summary>
    [SerializeField]
    private Transform defaultHookPoint;

    /// <summary>
    /// コントローラを持っているかどうか
    /// </summary>
    private bool isPickupping = false;

    /// <summary>
    /// フックが壁についているかどうか
    /// </summary>
    [UdonSynced]
    private bool isHooking = false;

    /// <summary>
    /// コントローラの入力用
    /// </summary>
    private const string L_TOUCHPAD_CLICK = "Oculus_CrossPlatform_PrimaryThumbstick";
    private const string R_TOUCHPAD_CLICK = "Oculus_CrossPlatform_SecondaryThumbstick";
    private const string L_VERTICAL = "Vertical";
    private const string R_VERTICAL = "Oculus_CrossPlatform_SecondaryThumbstickVertical";

    /// <summary>
    /// 入力を可視化する用のGameObject
    /// </summary>
    [SerializeField]
    private GameObject upSign;
    [SerializeField]
    private GameObject downSign;

    /// <summary>
    /// コントローラについたVRC_Pickup
    /// </summary>
    [SerializeField]
    private VRC_Pickup pickup;

    /// <summary>
    /// コントローラの初期位置
    /// </summary>
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        // 初期位置を記録
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;
    }

    public override void OnPickup()
    {
        // コントローラとフックオブジェクトのOwnerを取得
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        Networking.SetOwner(Networking.LocalPlayer, movePoint);

        // 持っている状態にする
        isPickupping = true;
    }

    public override void OnDrop()
    {
        // 持っていない状態にする
        isPickupping = false;
    }

    /// <summary>
    /// コントローラのトリガーを押したときの処理
    /// </summary>
    public override void OnPickupUseDown()
    {
        // コントローラオブジェクトのz軸方向にRayを飛ばす
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Rayがあたった場所にフックをつける
            movePoint.transform.position = hit.point;

            // フックがついた状態にする
            isHooking = true;
        }
    }

    private void Update()
    {
        // コントローラを持っていてフックがついた状態なら
        if (isPickupping && isHooking)
        {
            // 持っている手が左手なら左手コントローラの入力をみる
            // 左手コントローラのタッチパッドの右側が押されているなら
            // Input.GetAxis(L_VERTICAL) > 0 : タッチパッドの右側, < 0 : 左側が触れられている
            // Input.GetButton(L_TOUCHPAD_CLICK) : タッチパッドが押し込まれている
            if (pickup.currentHand == VRC_Pickup.PickupHand.Left &&
                Input.GetButton(L_TOUCHPAD_CLICK) && Input.GetAxis(L_VERTICAL) > 0)
            {
                PressUp();
            }

            // 持っている手が右手なら右手コントローラの入力を見る
            // 右手コントローラのタッチパッドの右側が押されているなら
            if (pickup.currentHand == VRC_Pickup.PickupHand.Right &&
                Input.GetButton(R_TOUCHPAD_CLICK) && Input.GetAxis(R_VERTICAL) > 0)
            {
                PressUp();
            }

            // Rキーが押されているなら
            if (Input.GetKey(KeyCode.R))
            {
                PressUp();
            }

            // 持っている手が左手なら左手コントローラの入力をみる
            // 左手コントローラのタッチパッドの左側が押されているなら
            if (pickup.currentHand == VRC_Pickup.PickupHand.Left &&
                Input.GetButton(L_TOUCHPAD_CLICK) && Input.GetAxis(L_VERTICAL) < 0)
            {
                PressDown();
            }

            // 持っている手が右手なら右手コントローラの入力をみる
            // 右手コントローラのタッチパッドの左側が押されているなら
            if (pickup.currentHand == VRC_Pickup.PickupHand.Right &&
                Input.GetButton(R_TOUCHPAD_CLICK) && Input.GetAxis(R_VERTICAL) < 0)
            {
                PressDown();
            }

            // Tキーが押されているなら
            if (Input.GetKey(KeyCode.T))
            {
                PressDown();
            }
        }

        // コントローラを持っているがフックがささっていない
        if (isPickupping && !isHooking)
        {
            // フックオブジェクトの位置をデフォルト位置にする
            movePoint.transform.position = defaultHookPoint.position;
        }

        // コントローラオブジェクトとフックオブジェクトをワイヤーでつなぐ
        line.SetPosition(0, transform.position);
        line.SetPosition(1, movePoint.transform.position);
    }

    private void PressUp()
    {
        // ワイヤーをまく処理(プレイヤーがフックに近づいていく)
        // 表示用
        upSign.SetActive(true);
        downSign.SetActive(false);
        // コントローラからフックへの方向ベクトルを取得する
        var moveVector = movePoint.transform.position - transform.position;
        // フックに向けて移動する速度をプレイヤーに与える
        Networking.LocalPlayer.SetVelocity(moveVector * speed);
    }

    private void PressDown()
    {
        // フックを切り離す処理（フックがコントローラに戻ってくる）
        // 表示用
        upSign.SetActive(false);
        downSign.SetActive(true);
        isHooking = false;
    }

    /// <summary>
    /// コントローラの位置をリセットする(UIButtonで実行)
    /// </summary>
    public void ResetPosition()
    {
        // OwnerでなければOwnerになる
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        // 元の位置に戻す
        this.transform.position = defaultPosition;
        this.transform.rotation = defaultRotation;

        // フックがささっていない状態にする
        isHooking = false;
    }
}
