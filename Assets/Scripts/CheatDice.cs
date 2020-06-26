
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// 常に特定の面を向くサイコロを作成する
/// 
/// Scene上に必要なもの
/// * 6面のCubeオブジェクト(これに本スクリプトをアタッチ)
/// - VRC_PickupとRigidbodyがアタッチされている
/// - SynchronizePositionにチェックをつける
/// 
/// Recorderと連携させて, 手を離してから止まるまでの挙動を記録させている
/// </summary>
public class CheatDice : UdonSharpBehaviour
{
    /// <summary>
    /// サイコロオブジェクトのRigidbody
    /// </summary>
    public Rigidbody rigidbody;

    /// <summary>
    /// チートを使うかどうかのUIToggle
    /// </summary>
    public Toggle cheatToggle;

    /// <summary>
    /// チートを使える人の名前を表示するUIText
    /// </summary>
    public Text ownerText;

    // サイコロを持っているかどうか
    private bool isPickupping = false;

    /// <summary>
    /// サイコロの動きを記録するRecorder
    /// </summary>
    [SerializeField]
    private UdonBehaviour recorder;

    private void Update()
    {
        // Ownerを表示
        // 全員のLocalでOwner名を取得して表示処理をしているため同期する(Ownerは必ず1人)
        ownerText.text = Networking.GetOwner(this.gameObject).displayName;

        // サイコロを持っていない時にOwnerのLocalのみで処理をする
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject) && !isPickupping)
        {
            // サイコロが止まったときの処理
            if (rigidbody.velocity == Vector3.zero) 
            {
                // チートを使用する状態で, サイコロのz軸が上を向いていなければ
                // Vector3.Dot(this.transform.forward, Vector3.up) == 1.0fなら完全に上を向いている
                // -1.0:逆向き, 1.0:同じ向き 0.0:直交
                if (cheatToggle.isOn && Vector3.Dot(this.transform.forward, Vector3.up) < 0.6f)
                {
                    float x = Random.Range(0f, 1f);
                    float y = Random.Range(0f, 1f);
                    float z = Random.Range(0f, 1f);

                    var forceAndTorque = new Vector3(x, y, z);

                    // 一瞬だけランダムな方向に速度と回転を与える
                    rigidbody.AddForce(forceAndTorque, ForceMode.Impulse);
                    rigidbody.AddTorque(forceAndTorque * Mathf.PI, ForceMode.Impulse);
                }
                else
                {
                    // 上を向いているなら記録を停止
                    recorder.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StopRecordingInOwner");
                }
            }
        }
    }

    public override void OnPickup()
    {
        // 持っている状態にしてOwnerを取得
        isPickupping = true;
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }

    public override void OnDrop()
    {
        // 持っていない状態にして記録を開始
        isPickupping = false;
        recorder.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StartRecordingInOwner");
    }
}
