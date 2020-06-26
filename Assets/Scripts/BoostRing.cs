
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 加速度をあたえるリングを作成する
/// 
/// Sceneに必要なもの
/// * IsTriggerのColliderがついたGameObject(本スクリプトをアタッチする)
/// * リングに当たったときに発生するエフェクト用ParticleSystem
/// </summary>
public class BoostRing : UdonSharpBehaviour
{
    /// <summary>
    /// ブースト後の速度
    /// </summary>
    public float boostVelocity;

    /// <summary>
    /// リングに触れたときに発生するエフェクト
    /// </summary>
    public ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーにあたったら(OnAvatarHit相当)
        if (other.GetType() == typeof(CharacterController))
        {
            // リングオブジェクトのz軸方向に速度を与える
            Networking.LocalPlayer.SetVelocity(Networking.LocalPlayer.GetVelocity() + this.transform.forward * boostVelocity);
            
            // 同期するようにリングエフェクトを発生
            // インスタンスにいるすべてのプレイヤーのLocalでパーティクルを発生させる処理をすれば発生タイミングは同期する
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayParticle");
        }
    }

    public void PlayParticle()
    {
        // リングエフェクトを発生
        particle.Play();
    }
}
