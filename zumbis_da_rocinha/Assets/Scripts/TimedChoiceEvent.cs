using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedChoiceEvent : MonoBehaviour
{
    private TextManager textManager;
    private PlayerManager playerManager;
    private Image timebar;

    public float maxTime;
    private float curTime;

    private float count;
    private WaitForSeconds timer;

    private void Awake(){
        textManager = GameObject.Find("TextManager").GetComponent<TextManager>();
        playerManager = GameObject.Find("Jogador").GetComponent<PlayerManager>();
        timebar = GetComponent<Image>();

        count = (maxTime - 0.5f * playerManager.machucado) / 100f;

        if(textManager != null){
            timer = new WaitForSeconds(count);
            timebar.color = Color.green;
            StartCoroutine(TimerEvent());
        }
    }

    private void Update() {
        this.GetComponent<Image>().fillAmount =  curTime / maxTime;
    }

    IEnumerator TimerEvent(){
        for(curTime = maxTime; curTime >= 0; curTime -= count){
            if(curTime < maxTime/2 && curTime > maxTime/10) timebar.color = Color.yellow;
            else if (curTime <= 1f) timebar.color = Color.red;
            yield return timer;
        }

        textManager.GameOver();
    }

}
