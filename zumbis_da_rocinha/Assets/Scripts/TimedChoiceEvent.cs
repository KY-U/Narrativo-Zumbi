using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedChoiceEvent : MonoBehaviour
{
    private TextManager textManager;

    public float maxTime;
    private float curTime;

    private float count;
    private WaitForSeconds timer;

    private void Awake(){
        textManager = GameObject.Find("TextManager").GetComponent<TextManager>();
        count = maxTime / 100f;

        if(textManager != null){
            timer = new WaitForSeconds(count);
            StartCoroutine(TimerEvent());
        }
    }

    private void Update() {
        this.GetComponent<Image>().fillAmount =  curTime / maxTime;
    }

    IEnumerator TimerEvent(){
        for(curTime = maxTime; curTime >= 0; curTime -= count){
            yield return timer;
        }

        textManager.GameOver();
    }

}
