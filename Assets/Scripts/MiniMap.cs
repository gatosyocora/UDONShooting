
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// CanvasにMiniMapを作成する 
/// 
/// Sceneに必要なもの
/// * Playerが持つまたは追従するGameObject4つ
/// * Playerの位置を示すCanvasアイテムを4つ持つCanvas
/// </summary>
public class MiniMap : UdonSharpBehaviour
{
    /// <summary>
    /// Player1が持つまたは追従するオブジェクト
    /// </summary>
    [SerializeField]
    private Transform player1Controller;

    /// <summary>
    /// Player2が持つまたは追従するオブジェクト
    /// </summary>
    [SerializeField]
    private Transform player2Controller;

    /// <summary>
    /// Player3が持つまたは追従するオブジェクト
    /// </summary>
    [SerializeField]
    private Transform player3Controller;

    /// <summary>
    /// Player4が持つまたは追従するオブジェクト
    /// </summary>
    [SerializeField]
    private Transform player4Controller;

    /// <summary>
    /// Player1の位置を示すCanvasオブジェクト
    /// </summary>
    [SerializeField]
    private RectTransform player1;

    /// <summary>
    /// Player2の位置を示すCanvasオブジェクト
    /// </summary>
    [SerializeField]
    private RectTransform player2;

    /// <summary>
    /// Player3の位置を示すCanvasオブジェクト
    /// </summary>
    [SerializeField]
    private RectTransform player3;

    /// <summary>
    /// Player4の位置を示すCanvasオブジェクト
    /// </summary>
    [SerializeField]
    private RectTransform player4;

    private void Update()
    {
        player1.localPosition = new Vector3(player1Controller.position.x, player1Controller.position.z, player1.localPosition.z);
        player2.localPosition = new Vector3(player2Controller.position.x, player2Controller.position.z, player2.localPosition.z);
        player3.localPosition = new Vector3(player3Controller.position.x, player3Controller.position.z, player3.localPosition.z);
        player4.localPosition = new Vector3(player4Controller.position.x, player4Controller.position.z, player4.localPosition.z);
    }
}
