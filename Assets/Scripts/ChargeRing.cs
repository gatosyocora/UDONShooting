
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// JetControllerにエネルギーをチャージするリングを作成する
/// 
/// Sceneに必要なもの
/// * JetController.csがアタッチされたコントローラオブジェクト4つ
/// * リングに当たったときに発生するエフェクト用ParticleSystem
/// * リングが使用可能であることを示すエフェクト用ParticleSystem
/// </summary>
public class ChargeRing1 : UdonSharpBehaviour
{
    /// <summary>
    /// コントローラオブジェクト4つ
    /// </summary>
    public UdonBehaviour controller1;
    public UdonBehaviour controller2;
    public UdonBehaviour controller3;
    public UdonBehaviour controller4;

    /// <summary>
    /// リングに触れたときに発生するエフェクト
    /// </summary>
    [SerializeField]
    public ParticleSystem particle;

    /// <summary>
    /// 再チャージ可能になるまでの時間（秒）
    /// </summary>
    [SerializeField]
    private float recastSecond = 5f;

    /// <summary>
    /// 再チャージ可能になるまでの残り時間
    /// </summary>
    private float recastTime = 0;

    /// <summary>
    /// リングが使用可能であるか
    /// </summary>
    [UdonSynced]
    public bool isActive = true;

    /// <summary>
    /// リングがチャージ可能なことを示すエフェクト
    /// </summary>
    [SerializeField]
    private ParticleSystem chargeParticle;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに触れてリングが有効であれば
        if (other.GetType() == typeof(CharacterController) && isActive)
        {
            // 当たったプレイヤーのローカルで処理をおこなう
            // 当たったプレイヤーがどのコントローラを持っているか
            // 当たったプレイヤーが持つコントローラのエネルギーを満タンにする
            if (Networking.IsOwner(controller1.gameObject))
            {
                controller1.SendCustomEvent("ChargeEnergyToFull");
            }
            if (Networking.IsOwner(controller2.gameObject))
            {
                controller2.SendCustomEvent("ChargeEnergyToFull");
            }
            if (Networking.IsOwner(controller3.gameObject))
            {
                controller3.SendCustomEvent("ChargeEnergyToFull");
            }
            if (Networking.IsOwner(controller4.gameObject))
            {
                controller4.SendCustomEvent("ChargeEnergyToFull");
            }

            // リングにあたったエフェクトを全員に同期させるように発生させる
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayParticle");

            // インスタンスのMaster内だけで再チャージ処理をおこなう
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SetRecast");

            // 全員に同期させるようにチャージ可能であることを示すエフェクトを消す
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "InactiveChargeRing");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // TODO: Joinerのローカルでこれが動いているか怪しい

        // 入った人以外は処理をおこなわない
        if (!player.isLocal)
        {
            return;
        }

        // 無効であればリングエフェクトを消す
        if (!isActive)
        {
            InactiveChargeRing();
        }
    }

    private void Update()
    {
        // インスタンスのMaster内のみで処理を実行する
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            // リングが無効状態ならリキャスト時間を計算する
            if (!isActive)
            {
                recastTime -= Time.deltaTime;

                // 残り時間がないならリングを有効にする
                if (recastTime <= 0)
                {
                    isActive = true;

                    // 全員に同期するようにリングエフェクトを復活させる
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ActiveChargeRing");
                }
            }
        }
    }

    /// <summary>
    /// リングにあたったエフェクトを発生させる
    /// </summary>
    public void PlayParticle()
    {
        particle.Play();
    }

    /// <summary>
    /// チャージ可能であることを示すエフェクトを有効にする
    /// </summary>
    public void ActiveChargeRing()
    {
        chargeParticle.Play();
    }

    /// <summary>
    /// チャージ可能であることを示すエフェクトを無効にする
    /// </summary>
    public void InactiveChargeRing()
    {
        chargeParticle.Stop();
    }

    /// <summary>
    /// 再チャージ可能になるまでの時間計測を開始する
    /// </summary>
    public void SetRecast()
    {
        recastTime = recastSecond;
        isActive = false;
    }
}
