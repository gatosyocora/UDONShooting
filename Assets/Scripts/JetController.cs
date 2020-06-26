
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// 空を飛ぶためのコントローラオブジェクトを作成する
/// 
/// Sceneに必要なもの
/// * 子にカメラが入ったPickup可能なオブジェクト = コントローラオブジェクト(これに本スクリプトをアタッチ)
/// * カメラとコントローラ両方にynchronizePositionにチェックがついたUdonBehaivourをつける
/// 
/// 仕様
/// * トリガー押下でコントローラオブジェクトのz方向に空を飛ぶ
/// * エネルギーが存在し, これが切れると速力を失う
/// * 接地しているとエネルギーが回復する
/// * コントローラを持つプレイヤーの顔を追従するカメラがある
/// </summary>
public class JetController : UdonSharpBehaviour
{
    /// <summary>
    /// コントローラオブジェクトについているVRC_Pickup
    /// </summary>
    public VRC_Pickup pickup;

    /// <summary>
    /// 空を飛んでいるかどうか
    /// </summary>
    private bool isJetting = false;

    /// <summary>
    /// コントローラを持つことが可能かどうか
    /// </summary>
    private bool isPickupping = false;
    
    /// <summary>
    /// VRモードかどうか
    /// </summary>
    private bool useVR = false;

    /// <summary>
    /// コントローラを持つプレイヤーの名前を表示するUIText
    /// </summary>
    public Text playerNameText;

    /// <summary>
    /// コントローラを持つプレイヤーの名前
    /// </summary>
    [UdonSynced(UdonSyncMode.None)]
    public string playerName;

    #region Speed

    /// <summary>
    /// 移動速度を調整するUISlider
    /// </summary>
    public Slider speedSlider;

    /// <summary>
    /// 速度を表示するUIText
    /// </summary>
    public Text velocityText;

    #endregion

    #region DefaultState
    /// <summary>
    /// 初期状態(初期リセット用)
    /// </summary>
    private string defaultPlayerName;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    #endregion

    #region Energy

    /// <summary>
    /// エネルギー残量を表示するUISlider
    /// </summary>
    public Slider energyBar;

    /// <summary>
    /// コントローラのエネルギー残量
    /// </summary>
    private int energy;

    /// <summary>
    /// コントローラのエネルギーの最大値
    /// </summary>
    private const int MAX_ENERGY = 500;

    #endregion

    #region Boost

    /// <summary>
    /// ブースト時の速さ
    /// </summary>
    public float boostSpeed;

    #endregion

    #region HeadCamera

    /// <summary>
    /// プレイヤーの顔を追従させるカメラ
    /// </summary>
    public Transform playerMonitorCamera;

    /// <summary>
    /// プレイヤーとカメラとの距離
    /// </summary>
    private float cameraOffset = 1f;

    /// <summary>
    /// カメラが写っている状態か
    /// </summary>
    [UdonSynced(UdonSyncMode.None)]
    public bool isActiveCamera = false;

    /// <summary>
    /// Headボーンの向きを計算する用
    /// </summary>
    private Vector3 vector3z1 = new Vector3(0, 0, 1f);

    #endregion

    #region Debug

    /// <summary>
    /// デバッグ用表示
    /// </summary>
    public Text debugText;

    /// <summary>
    /// Ownerを表示するUIText
    /// </summary>
    public Text ownerNameText;

    #endregion

    private void Start()
    {
        // リセットのために初期状態を記録
        defaultPlayerName = playerName;
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;

        // 最初のエネルギー残量を計算
        energy = MAX_ENERGY;
        energyBar.value = energy / (MAX_ENERGY * 1f);
    }

    private void Update()
    {
        // Editor上でのエラー回避(Editor上では常にnullでエラーが出る)
        if (Networking.GetOwner(this.gameObject) != null)
        {
            // コントローラのOwner名を表示
            ownerNameText.text = Networking.GetOwner(this.gameObject).displayName;
        }

        // 自分が飛んでいる状態でエネルギー残量があれば加速
        if (isJetting && energy > 0)
        {
            // コントローラのz軸方向に速度を与える
            Networking.LocalPlayer.SetVelocity(this.transform.forward * speedSlider.value);
            
            // エネルギー残量を減らす
            energy--;

            // エネルギー残量を表示(少数にするために割る方または割られる方をfloat(少数)にする必要がある)
            energyBar.value = energy / (float)MAX_ENERGY;
        }

        // 自分が持っている状態なら
        if (isPickupping)
        {
            // 速度を表示(int型の数値を文字列にキャスト)
            velocityText.text = Networking.LocalPlayer.GetVelocity().ToString();

            // カメラがアクティブなら顔に追従させる
            if (isActiveCamera)
            {
                FollowCameraToHead();
            }

            // 自分自身が接地しているなら
            if (Networking.LocalPlayer.IsPlayerGrounded())
            {
                // エネルギーが最大値より低かったら回復させる
                if (energy < MAX_ENERGY)
                {
                    energy++;
                    energyBar.value = energy / (float)MAX_ENERGY;
                }
            } 
        }
        // 自分が持っていなかったら適当な表示する
        else
        {
            velocityText.text = "Velocity";
        }

        // 持っていてデスクトップユーザーなら
        if (isPickupping && !useVR)
        {
            // デスクトップユーザーだけは接地していなくいときにBキーでブーストできるように
            if (!Networking.LocalPlayer.IsPlayerGrounded() && Input.GetKeyDown(KeyCode.B))
            {
                Boost();
            }

            // デスクトップユーザーだけはCキーでカメラのオンオフができるように
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCamera();
            }
        }

        // デバッグ用表示
        debugText.text = Networking.GetOwner(playerMonitorCamera.gameObject).displayName;
    }

    // Updateの後に実行される毎フレーム処理
    private void LateUpdate()
    {
        // カメラのアクティブ状態を変更
        playerMonitorCamera.gameObject.SetActive(isActiveCamera);

        // プレイヤー名を表示
        // playerNameの取得はコントローラを持った人のローカルでおこなうが,
        // playerNameをUdonSyncさせているので表示内容が同期する
        playerNameText.text = playerName;
    }

    // コントローラを持った時の処理
    public override void OnPickup()
    {
        // 持った人がVRユーザーかどうか
        useVR = Networking.LocalPlayer.IsUserInVR();

        // Ownerを付与(独自で用意したメソッド)
        TakeOwnerships();

        // 持っている状態にする
        isPickupping = true;

        // 持った人の名前を取得
        playerName = Networking.LocalPlayer.displayName;

        // カメラを有効化
        isActiveCamera = true;
    }

    // コントローラを話したときの処理
    public override void OnDrop()
    {
        // 持っていない状態にする
        isPickupping = false;

        // 名前をデフォルト状態に戻す
        playerName = defaultPlayerName;

        // カメラの位置をリセット
        ResetPlayerCameraPosition();

        // カメラを無効化(負荷対策)
        isActiveCamera = false;
    }

    // トリガーを押し込んだときの処理
    public override void OnPickupUseDown()
    {
        // 飛んでいる状態にする
        isJetting = true;
    }

    // トリガーを離したときの処理
    public override void OnPickupUseUp()
    {
        // 飛んでいない状態にする
        isJetting = false;
    }

    private void Boost()
    {
        // ブースト速度にする
        // TODO: トリガーを押して通常速度でSetVelocityするまでブースト速度が維持されてしまう
        Networking.LocalPlayer.SetVelocity(this.transform.forward * boostSpeed);
    }

    private void ToggleCamera()
    {
        // カメラの状態を反転させる
        isActiveCamera = !isActiveCamera;

        // カメラが無効状態ならカメラ位置をリセット
        if (!isActiveCamera)
        {
            ResetPlayerCameraPosition();
        }
    }

    private void FollowCameraToHead()
    {
        // カメラをプレイヤーのHeadボーンの位置に追従させる

        // プレイヤーのHeadボーンの位置を取得
        var headPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.Head);

        // Headボーンのz軸の向きを取得
        var headForward = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.Head) * vector3z1;
        
        // z軸方向にoffsetをかける
        playerMonitorCamera.position = headPos + headForward * cameraOffset;
        
        // カメラがプレイヤーのほうを向くように回転させる
        playerMonitorCamera.LookAt(headPos);
    }

    private void ResetPlayerCameraPosition()
    {
        // カメラの位置をリセット
        // カメラはコントローラの子オブジェクトなのでコントローラ位置に移動する
        playerMonitorCamera.localPosition = Vector3.zero;
        playerMonitorCamera.localRotation = Quaternion.identity;

        // カメラの描画を移動先の風景に更新
        playerMonitorCamera.GetComponent<Camera>().Render();
    }

    public void Reset()
    {
        // コントローラ位置をリセット

        // 誰が持っているかわからないので全員がコントローラを離す処理を実行
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DropController");
        
        // 場所をリセット
        ResetPosition();
        // プレイヤー名をリセット
        playerName = defaultPlayerName;
    }

    private void ResetPosition()
    {
        // Ownerを移動させて初期位置に戻す
        TakeOwnerships();
        this.transform.position = defaultPosition;
        this.transform.rotation = defaultRotation;
    }

    private void TakeOwnerships()
    {
        // コントローラとカメラそれぞれでOwner出なければOwnerになる
        // (SetOwner実行から実際にOwnerになるまでラグがあるらしい)
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
        // コントローラを離す
        pickup.Drop();
        isPickupping = false;
    }

    public void ChargeEnergyToFull()
    {
        // エネルギーを満タン状態にする
        energy = MAX_ENERGY;
        energyBar.value = energy / (float)MAX_ENERGY;
    }
}
