using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDUpdates : MonoBehaviour
{
    [SerializeField] PlayerManager pm;

    public Sprite coracaoVazio, coracaoCheio;
    public Image[] hearts;

    [SerializeField] private Color defaultColor, shadowedColor;
    public Image[] inventory;

    public void UpdateUI(){
        for(int i = 0; i < pm.machucado; i++){
            hearts[pm.machucado - 1 - i].sprite = coracaoVazio;
        }

        for(int i = 0; i < 3; i++){
            if((pm.armado & (1 << (i+1))) != 0){
                inventory[i].color = defaultColor;
            }
            else inventory[i].color = shadowedColor;
        }
    }

    public void RenewUI(){
        for(int i = 0; i < 3; i++){
            hearts[i].sprite = coracaoCheio;
            inventory[i].color = shadowedColor;
        }

        UpdateUI();
    }
}
