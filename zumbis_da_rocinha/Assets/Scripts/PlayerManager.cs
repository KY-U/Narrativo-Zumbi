using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    pistola -> 1
    faca    -> 2
    taco    -> 3
*/
public class PlayerManager : MonoBehaviour{
    public int curScene;
    public int machucado = 0;
    public int armado = 0;
    public int deathCount = 0;

    private void Start() {
        //Carregar Status
    }

    // Salvar Status
    // Carregar Status

    // Sets
        // Machucado
        public void Ai(){ 
            machucado++; 
        }
        // Contador mortes
        public void Morreu(){ deathCount++; }
        // Adicionar/retirar arma
        public void GunControl(string g){
            int gun = int.Parse(g);
            if(gun > 0) armado |= 1 << gun;
            else armado &= ~(1 << -gun);
        }
}