using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(GameObject.Find("Tela de jogo"));
    }

    public void QuitToMenu(){
        SceneManager.LoadScene("Menu");
    }
}
