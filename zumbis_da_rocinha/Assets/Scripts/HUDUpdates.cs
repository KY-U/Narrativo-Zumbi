using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUpdates : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    public Sprite coracaoVazio;
    public Sprite coracaoCheio;
    public Image[] hearts;

    public void UpdateUI(){
        for(int i = 0; i < pm.machucado; i++){
            hearts[pm.machucado - 1 - i].sprite = coracaoVazio;
        }
    }

    public void RenewUI(){
        for(int i = 0; i < 3; i++){
            hearts[i].sprite = coracaoCheio;
        }
        UpdateUI();
    }
}
