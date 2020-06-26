
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// プレイヤーのボーンにあわせてオブジェクトらを動かす
/// 
/// Sceneに必要なもの
/// * 6点のボーンに対応するオブジェクトを持つEmptyGameObject(本スクリプトをアタッチする)
///  - 6点のボーンに対応するオブジェクトそれぞれにSynchronize PositionにチェックをつけたUdonBehaivourをつける
/// </summary>
public class TrackingSwitch : UdonSharpBehaviour
{
    /// <summary>
    /// 追従させるオブジェクトらの親オブジェクト
    /// </summary>
    public Transform characterRoot;

    /// <summary>
    /// 追従させるオブジェクト
    /// </summary>
    public Transform headPoint;
    public Transform rhandPoint;
    public Transform lhandPoint;
    public Transform hipPoint;
    public Transform rfootPoint;
    public Transform lfootPoint;

    /// <summary>
    /// 追従させるかどうか
    /// </summary>
    private bool isTracking = false;

    /// <summary>
    /// 追従させるプレイヤー
    /// </summary>
    private VRCPlayerApi player;

    public override void Interact()
    {
        // 追従状態なら追従を解除する
        if (isTracking)
        {
            isTracking = false;
            return;
        }

        // 追従を開始する
        isTracking = true;

        // 追従するプレイヤーを切り替える
        player = Networking.LocalPlayer;
        if (!Networking.IsOwner(player, this.gameObject))
        {
            // すべてのOwnerを取得する
            Networking.SetOwner(player, this.gameObject);

            Networking.SetOwner(player, headPoint.gameObject);
            Networking.SetOwner(player, rhandPoint.gameObject);
            Networking.SetOwner(player, lhandPoint.gameObject);
            Networking.SetOwner(player, hipPoint.gameObject);
            Networking.SetOwner(player, rfootPoint.gameObject);
            Networking.SetOwner(player, lfootPoint.gameObject);
        }
    }

    private void LateUpdate()
    {
        // 追従するプレイヤーのLocalで処理をする
        if (isTracking)
        {
            // 追従オブジェクトと追従するプレイヤーとの場所の差分
            var distance = characterRoot.position - player.GetPosition();

            // ボーンの位置を反映させる
            headPoint.position = player.GetBonePosition(HumanBodyBones.Head) + distance;
            headPoint.rotation = player.GetBoneRotation(HumanBodyBones.Head);

            hipPoint.position = player.GetBonePosition(HumanBodyBones.Hips) + distance;
            hipPoint.rotation = player.GetBoneRotation(HumanBodyBones.Hips);

            lhandPoint.position = player.GetBonePosition(HumanBodyBones.LeftHand) + distance;
            lhandPoint.rotation = player.GetBoneRotation(HumanBodyBones.LeftHand);

            rhandPoint.position = player.GetBonePosition(HumanBodyBones.RightHand) + distance;
            rhandPoint.rotation = player.GetBoneRotation(HumanBodyBones.RightHand);

            lfootPoint.position = player.GetBonePosition(HumanBodyBones.LeftFoot) + distance;
            lfootPoint.rotation = player.GetBoneRotation(HumanBodyBones.LeftFoot);

            rfootPoint.position = player.GetBonePosition(HumanBodyBones.RightFoot) + distance;
            rfootPoint.rotation = player.GetBoneRotation(HumanBodyBones.RightFoot);
        }
    }
}
