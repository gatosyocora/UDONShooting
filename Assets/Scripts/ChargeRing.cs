
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// JetController�ɃG�l���M�[���`���[�W���郊���O���쐬����
/// 
/// Scene�ɕK�v�Ȃ���
/// * JetController.cs���A�^�b�`���ꂽ�R���g���[���I�u�W�F�N�g4��
/// * �����O�ɓ��������Ƃ��ɔ�������G�t�F�N�g�pParticleSystem
/// * �����O���g�p�\�ł��邱�Ƃ������G�t�F�N�g�pParticleSystem
/// </summary>
public class ChargeRing1 : UdonSharpBehaviour
{
    /// <summary>
    /// �R���g���[���I�u�W�F�N�g4��
    /// </summary>
    public UdonBehaviour controller1;
    public UdonBehaviour controller2;
    public UdonBehaviour controller3;
    public UdonBehaviour controller4;

    /// <summary>
    /// �����O�ɐG�ꂽ�Ƃ��ɔ�������G�t�F�N�g
    /// </summary>
    [SerializeField]
    public ParticleSystem particle;

    /// <summary>
    /// �ă`���[�W�\�ɂȂ�܂ł̎��ԁi�b�j
    /// </summary>
    [SerializeField]
    private float recastSecond = 5f;

    /// <summary>
    /// �ă`���[�W�\�ɂȂ�܂ł̎c�莞��
    /// </summary>
    private float recastTime = 0;

    /// <summary>
    /// �����O���g�p�\�ł��邩
    /// </summary>
    [UdonSynced]
    public bool isActive = true;

    /// <summary>
    /// �����O���`���[�W�\�Ȃ��Ƃ������G�t�F�N�g
    /// </summary>
    [SerializeField]
    private ParticleSystem chargeParticle;

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ɐG��ă����O���L���ł����
        if (other.GetType() == typeof(CharacterController) && isActive)
        {
            // ���������v���C���[�̃��[�J���ŏ����������Ȃ�
            // ���������v���C���[���ǂ̃R���g���[���������Ă��邩
            // ���������v���C���[�����R���g���[���̃G�l���M�[�𖞃^���ɂ���
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

            // �����O�ɂ��������G�t�F�N�g��S���ɓ���������悤�ɔ���������
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayParticle");

            // �C���X�^���X��Master�������ōă`���[�W�����������Ȃ�
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SetRecast");

            // �S���ɓ���������悤�Ƀ`���[�W�\�ł��邱�Ƃ������G�t�F�N�g������
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "InactiveChargeRing");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // TODO: Joiner�̃��[�J���ł��ꂪ�����Ă��邩������

        // �������l�ȊO�͏����������Ȃ�Ȃ�
        if (!player.isLocal)
        {
            return;
        }

        // �����ł���΃����O�G�t�F�N�g������
        if (!isActive)
        {
            InactiveChargeRing();
        }
    }

    private void Update()
    {
        // �C���X�^���X��Master���݂̂ŏ��������s����
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            // �����O��������ԂȂ烊�L���X�g���Ԃ��v�Z����
            if (!isActive)
            {
                recastTime -= Time.deltaTime;

                // �c�莞�Ԃ��Ȃ��Ȃ烊���O��L���ɂ���
                if (recastTime <= 0)
                {
                    isActive = true;

                    // �S���ɓ�������悤�Ƀ����O�G�t�F�N�g�𕜊�������
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ActiveChargeRing");
                }
            }
        }
    }

    /// <summary>
    /// �����O�ɂ��������G�t�F�N�g�𔭐�������
    /// </summary>
    public void PlayParticle()
    {
        particle.Play();
    }

    /// <summary>
    /// �`���[�W�\�ł��邱�Ƃ������G�t�F�N�g��L���ɂ���
    /// </summary>
    public void ActiveChargeRing()
    {
        chargeParticle.Play();
    }

    /// <summary>
    /// �`���[�W�\�ł��邱�Ƃ������G�t�F�N�g�𖳌��ɂ���
    /// </summary>
    public void InactiveChargeRing()
    {
        chargeParticle.Stop();
    }

    /// <summary>
    /// �ă`���[�W�\�ɂȂ�܂ł̎��Ԍv�����J�n����
    /// </summary>
    public void SetRecast()
    {
        recastTime = recastSecond;
        isActive = false;
    }
}
