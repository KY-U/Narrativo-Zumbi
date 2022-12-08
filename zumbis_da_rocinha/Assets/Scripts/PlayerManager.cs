using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    pistola -> 0
    faca    -> 1
    taco    -> 2
*/
public class PlayerManager : MonoBehaviour{
    public int curScene;
    public int machucado = 0;
    public int armado = 0;
    public int deathCount = 0;

    // Salvar Status
    // Carregar Status

    // Sets
        // Machucado
        public void Ai(){ 
            machucado++; 
        }

        // Contador mortes
        public void Morreu(){ 
            deathCount++; 
        }

        // Adicionar/retirar arma
        public void GunControl(string g){
            int gun = int.Parse(g);
            if(gun > 0) armado |= 1 << gun;
            else armado &= ~(1 << -gun);

            Animator achouArma = GameObject.Find("animacaoArma/Canvas/arma").GetComponent<Animator>();
            achouArma.SetTrigger("ganhouArma");
        }
}