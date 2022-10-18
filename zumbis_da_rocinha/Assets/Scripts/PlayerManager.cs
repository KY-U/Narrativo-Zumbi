using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    pistola -> 1
    faca    -> 2
    taco    -> 3
*/
public class PlayerManager : MonoBehaviour{
    int curScene;
    //int curDialogue;
    int machucado = 0;
    int armado = 0;
    int deathCount = 0;

    private void Start() {
        //Carregar Status
    }

    // Salvar Status
    // Carregar Status

    // Sets
        // Machucado
        int Ai(){ return machucado++; }
        // Contador mortes
        int Morreu(){ return deathCount++; }
        // Adicionar/retirar arma
        void GunControl(int gun){
            if(gun > 0) armado |= 1 << gun;
            else armado &= ~(1 << -gun);
        }
}
