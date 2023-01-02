using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemManager{
    //public Animator animator;
    public GameObject[] itens;

    public void ItemCue(string item){
        int pos = int.Parse(item);
        if(pos >= 0 && pos < itens.Length){
            itens[pos].SetActive(true);
        }
    }
}
