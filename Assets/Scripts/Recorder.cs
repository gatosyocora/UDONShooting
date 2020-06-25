
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// �I�u�W�F�N�g�̓������L�^����эĐ�����
/// 
/// ���ӎ���
/// * target�̃I�u�W�F�N�g�ɂ�SynchronizePosition�Ƀ`�F�b�N������UdonBehaivour�����Ă���
/// * �{�R���|�[�l���g��Owner���p�ɂɈړ����Ȃ�(Pickup���Ȃ��Ȃ�)�I�u�W�F�N�g�ɂ���
/// * �{�R���|�[�l���g������GameObject��Owner�͏�ɃC���X�^���X��Master�ɂȂ�
/// * �L�^���ƍĐ�����Owner���ς��ƋL�^�����f�[�^���Đ�����l�͎����Ă��Ȃ����ߔ��f����Ȃ�
/// </summary>
public class Recorder : UdonSharpBehaviour
{
    /// <summary>
    /// �ő�^�掞�ԁi�b�j
    /// </summary>
    private const int RECORDSECOND = 60;

    private const int FRAMERATE = 90;

    /// <summary>
    /// �����f�[�^�����Ă����z��
    /// </summary>
    private Vector3[] positionData = new Vector3[FRAMERATE * RECORDSECOND];
    private Quaternion[] rotationData = new Quaternion[FRAMERATE * RECORDSECOND];

    /// <summary>
    /// �����f�[�^�̋L�^�܂��͍Đ����Ă���ʒu
    /// </summary>
    private int dataIndex = 0;

    /// <summary>
    /// �����f�[�^���L�^����эĐ�������GameObject
    /// </summary>
    [SerializeField]
    private GameObject target;

    // TODO: ����Ԃ݂�����Enum��������isRecording��isRewinding���܂Ƃ߂�
    // �����true�Ȃ炻�̏��, �ǂ����false�Ȃ�҂����

    /// <summary>
    /// �L�^��Ԃł��邩
    /// </summary>
    private bool isRecording = false;

    /// <summary>
    /// �Đ���Ԃł��邩
    /// </summary>
    private bool isRewinding = false;

    /// <summary>
    /// ����Ԃ̕\���p�e�L�X�g
    /// </summary>
    [UdonSynced]
    public string state;

    /// <summary>
    /// ����Ԃ�\������UIText
    /// </summary>
    [SerializeField]
    private Text stateText;

    private void Update()
    {
        // Master�������ŏ���������
        if (!Networking.LocalPlayer.IsOwner(this.gameObject))
            return;

        // �L�^���ł����
        if (isRecording)
        {
            state = "Recording";
            // �L�^����𒴂��Ă���΋L�^���Ȃ�
            if (dataIndex >= 90 * RECORDSECOND)
            {
                state = "Finish Recording";
                return;
            }

            // ����Ԃ��L�^����
            positionData[dataIndex] = target.transform.position;
            rotationData[dataIndex] = target.transform.rotation;
            dataIndex++;
        }
        // �Đ���Ԃł����
        else if (isRewinding)
        {
            state = "Rewinding";
            // �Đ��ł���f�[�^���Ȃ���Α҂���Ԃɂ���
            if (dataIndex <= 0)
            {
                state = "Waiting";
                return;
            }

            // �L�^���ꂽ��Ԃ𔽉f����
            target.transform.position = positionData[dataIndex - 1];
            target.transform.rotation = rotationData[dataIndex - 1];
            dataIndex--;
        }
        else
        {
            state = "Waiting";
        }
    }

    private void LateUpdate()
    {
        // ��ԕ\���e�L�X�g���X�V����
        stateText.text = state;
    }

    /// <summary>
    /// �Đ�����(uGUI��Button������s����)
    /// </summary>
    public void Rewind()
    {
        // �N���{�^���������Ă��Đ��ł���悤��
        this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StartRewindingInOwner");
    }

    /// <summary>
    /// �L�^���J�n����(uGUI��Button������s����)
    /// </summary>
    public void StartRecordingInOwner()
    {
        RemoveData();
        isRecording = true;
        isRewinding = false;
    }

    /// <summary>
    /// �L�^���~����(uGUI��Button������s����)
    /// </summary>
    public void StopRecordingInOwner()
    {
        isRecording = false;
    }

    /// <summary>
    /// �Đ�����(uGUI��Button������s����)
    /// </summary>
    public void StartRewindingInOwner()
    {
        // TODO: ���Ԃ񂱂�SetOwner�͕s�v
        Networking.SetOwner(Networking.LocalPlayer, target);
        isRecording = false;
        isRewinding = true;
    }

    /// <summary>
    /// ���ׂẴf�[�^���폜����
    /// </summary>
    public void RemoveData()
    {
        dataIndex = 0;
    }
}
