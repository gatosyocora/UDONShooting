
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// Canvas��MiniMap���쐬����
/// 
/// Scene�ɕK�v�Ȃ���
/// * Player�����܂��͒Ǐ]����GameObject4��
/// * Player�̈ʒu������Canvas�A�C�e����4����Canvas
/// </summary>
public class MiniMap : UdonSharpBehaviour
{
    /// <summary>
    /// Player1�����܂��͒Ǐ]����I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private Transform player1Controller;

    /// <summary>
    /// Player2�����܂��͒Ǐ]����I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private Transform player2Controller;

    /// <summary>
    /// Player3�����܂��͒Ǐ]����I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private Transform player3Controller;

    /// <summary>
    /// Player4�����܂��͒Ǐ]����I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private Transform player4Controller;

    /// <summary>
    /// Player1�̈ʒu������Canvas�I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private RectTransform player1;

    /// <summary>
    /// Player2�̈ʒu������Canvas�I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private RectTransform player2;

    /// <summary>
    /// Player3�̈ʒu������Canvas�I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private RectTransform player3;

    /// <summary>
    /// Player4�̈ʒu������Canvas�I�u�W�F�N�g
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
