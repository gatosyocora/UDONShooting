
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// プレイヤーに属性を設定する
/// </summary>
public class PlayerMods : UdonSharpBehaviour
{
    /// <summary>
    /// ジャンプ速度
    /// </summary>
    [SerializeField]
    private float jumpImpulse = 3f;
    
    /// <summary>
    /// 歩く速度
    /// </summary>
    [SerializeField]
    private float walkSpeed = 2f;
    
    /// <summary>
    /// 走る速度
    /// </summary>
    [SerializeField]
    private  float runSpeed = 4f;

    /// <summary>
    /// 重力の強さ
    /// </summary>
    [SerializeField]
    private  float gravityStrength = 1.0f;

    private void Start()
    {
        var player = Networking.LocalPlayer;

        // Editor上でのエラー回避(Editor上では常にnullでエラーが出る)
        if (player == null) return;

        // プレイヤーに属性を設定
        player.SetJumpImpulse(jumpImpulse);
        player.SetWalkSpeed(walkSpeed);
        player.SetRunSpeed(runSpeed);
        player.SetGravityStrength(gravityStrength);
    }
}
