using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemManager{
    //public Animator animator;
    public GameObject[] itens;

    public void ItemCue(string item){
        int pos = int.Parse(item);

        // Isso significa que não dá pra limpar um item da tela da posição 0
        // Resolver do jeito macaco e NÂO COLOCAR UM ITEM QUE PRECISA DISSO NO 0
        if(pos < 0) itens[-pos].SetActive(false);

        if(pos >= 0 && pos < itens.Length){
            itens[pos].SetActive(true);
        }
    }
}
