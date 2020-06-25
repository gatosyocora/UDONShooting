
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class EmoteSwitchButton : UdonSharpBehaviour
{
    private GameObject emoteSwitchObj;
    private Animator emoteSwitchAnimator;
    private bool isPlayingAnimator = false;
    private int frameCount = 0;

    public Text debugText;

    public override void Interact()
    {
        emoteSwitchObj = GameObject.Find("Object");

        if (emoteSwitchObj == null)
        {
            debugText.text = "null";
            return;
        }

        frameCount = 0;
        isPlayingAnimator = !isPlayingAnimator;

        debugText.text = emoteSwitchObj.transform.parent.parent.name+"/"+emoteSwitchObj.transform.parent.name + "/" + emoteSwitchObj.name;
    }

    private void LateUpdate()
    {
        if (isPlayingAnimator)
        {
            emoteSwitchObj.SetActive(false);
        }
        
    }

    /*private void Update()
    {
        if (isPlayingAnimator)
        {
            frameCount++;

            if (frameCount < 10)
            {
                emoteSwitchObj.SetActive(false);
            }
            else
            {
                emoteSwitchObj.SetActive(true);
            }

            if (frameCount < 17)
            {
                emoteSwitchAnimator.enabled = true;
            }
            else
            {
                emoteSwitchAnimator.enabled = false;
                frameCount = 0;
                isPlayingAnimator = false;
            }
        }
    }*/
}
