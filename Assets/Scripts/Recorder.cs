
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// オブジェクトの動きを記録および再生する
/// 
/// 注意事項
/// * targetのオブジェクトにはSynchronizePositionにチェックがついたUdonBehaivourをつけておく
/// * 本コンポーネントはOwnerが頻繁に移動しない(Pickupしないなど)オブジェクトにつける
/// * 本コンポーネントを持つGameObjectのOwnerは常にインスタンスのMasterになる
/// * 記録時と再生時でOwnerが変わると記録したデータを再生する人は持っていないため反映されない
/// </summary>
public class Recorder : UdonSharpBehaviour
{
    /// <summary>
    /// 最大録画時間（秒）
    /// </summary>
    private const int RECORDSECOND = 60;

    private const int FRAMERATE = 90;

    /// <summary>
    /// 動きデータを入れておく配列
    /// </summary>
    private Vector3[] positionData = new Vector3[FRAMERATE * RECORDSECOND];
    private Quaternion[] rotationData = new Quaternion[FRAMERATE * RECORDSECOND];

    /// <summary>
    /// 動きデータの記録または再生している位置
    /// </summary>
    private int dataIndex = 0;

    /// <summary>
    /// 動きデータを記録および再生をするGameObject
    /// </summary>
    [SerializeField]
    private GameObject target;

    // TODO: 現状態みたいなEnumをつくってisRecordingとisRewindingをまとめる
    // 一方がtrueならその状態, どちらもfalseなら待ち状態

    /// <summary>
    /// 記録状態であるか
    /// </summary>
    private bool isRecording = false;

    /// <summary>
    /// 再生状態であるか
    /// </summary>
    private bool isRewinding = false;

    /// <summary>
    /// 現状態の表示用テキスト
    /// </summary>
    [UdonSynced]
    public string state;

    /// <summary>
    /// 現状態を表示するUIText
    /// </summary>
    [SerializeField]
    private Text stateText;

    private void Update()
    {
        // Master内だけで処理をする
        if (!Networking.LocalPlayer.IsOwner(this.gameObject))
            return;

        // 記録中であれば
        if (isRecording)
        {
            state = "Recording";
            // 記録上限を超えていれば記録しない
            if (dataIndex >= 90 * RECORDSECOND)
            {
                state = "Finish Recording";
                return;
            }

            // 現状態を記録する
            positionData[dataIndex] = target.transform.position;
            rotationData[dataIndex] = target.transform.rotation;
            dataIndex++;
        }
        // 再生状態であれば
        else if (isRewinding)
        {
            state = "Rewinding";
            // 再生できるデータがなければ待ち状態にする
            if (dataIndex <= 0)
            {
                state = "Waiting";
                return;
            }

            // 記録された状態を反映する
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
        // 状態表示テキストを更新する
        stateText.text = state;
    }

    /// <summary>
    /// 再生する(uGUIのButtonから実行する)
    /// </summary>
    public void Rewind()
    {
        // 誰がボタンを押しても再生できるように
        this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "StartRewindingInOwner");
    }

    /// <summary>
    /// 記録を開始する(uGUIのButtonから実行する)
    /// </summary>
    public void StartRecordingInOwner()
    {
        RemoveData();
        isRecording = true;
        isRewinding = false;
    }

    /// <summary>
    /// 記録を停止する(uGUIのButtonから実行する)
    /// </summary>
    public void StopRecordingInOwner()
    {
        isRecording = false;
    }

    /// <summary>
    /// 再生する(uGUIのButtonから実行する)
    /// </summary>
    public void StartRewindingInOwner()
    {
        // TODO: たぶんこのSetOwnerは不要
        Networking.SetOwner(Networking.LocalPlayer, target);
        isRecording = false;
        isRewinding = true;
    }

    /// <summary>
    /// すべてのデータを削除する
    /// </summary>
    public void RemoveData()
    {
        dataIndex = 0;
    }
}
