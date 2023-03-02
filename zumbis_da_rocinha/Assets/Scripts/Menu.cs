using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public PlayerManager Jogador;

    public void LoadScenes(string cena){
        SceneManager.LoadScene("Cena " + cena);
    }

    public void NewGame(){
        Jogador.LimparSave();
        LoadScenes(Jogador.curScene);
    }

    public void LoadSave(){
        Jogador.Carregar();
        LoadScenes(Jogador.curScene);
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void Quit(){
        Debug.Log("Saindo");
        Application.Quit();
    }
}
