using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    [SerializeField] private XML_reading xReader;   // Instanciação da classe que lê o XML
    public string blocoS = "0";

    public GameObject canvas;                       // Parente de todas as UI bases

    public GameObject prefabDialogo;                // Prefab da UI de dialogo
    private Queue<XmlNode> falas;

    public GameObject prefabEscolhas;               // Prefab da UI de escolhas
    public GameObject prefabBotao;                  // Prefab dos botões da UI de escolha

    private Vector3 pos1;
    private Vector3 pos2;
    
    void Start(){
        falas = new Queue<XmlNode>();
        LoadDialogue("0"); // Chama a primeira execução de bloco
    }

    public void LoadDialogue(string bloco){
        xReader.LoadFile();
        foreach(XmlNode dialogo in xReader.ParseDialogo(bloco)){
            falas.Enqueue(dialogo); // Adiciona todos os nodes na fila
        }

        blocoS = bloco;
        Destroy(GameObject.Find("escolhasUI(Clone)"));  // Limpa a tela se necessário
        Instantiate(prefabDialogo, canvas.transform);   // Instancia a UI de diálogo

        //Atualiza o trigger do "botao" que chama a proxima parte do dialogo 
        GameObject trigger = GameObject.Find("dialogoUI(Clone)/Trigger");
        trigger.GetComponent<Button>().onClick.AddListener(CallNextDialogue);

        CallNextDialogue();
    }

    public void CallNextDialogue(){
        // i.e. quando acabar o diálogo
        if(falas.Count == 0){
            Destroy(GameObject.Find("dialogoUI(Clone)"));   // Limpa a tela
            Instantiate(prefabEscolhas, canvas.transform);  // Instancia a UI de escolha
            CallEscolhas();
            return;
        }
        // Puxa um node da fila para ser utilizado
        XmlNode fala = falas.Dequeue();
        XmlNode nome = fala.FirstChild;
        XmlNode texto = nome.NextSibling;

        GameObject caixaNome = GameObject.Find("dialogoUI(Clone)/CaixaNomeFrame");
        pos1 = caixaNome.transform.localPosition;
        pos2 = caixaNome.transform.localPosition - new Vector3(2*caixaNome.transform.localPosition.x,0,0);

        // Atualiza a caixa de nome da personagem
        // Cool dynamic with position wow
        if(caixaNome.GetComponentInChildren<TextMeshProUGUI>().text != nome.InnerXml){
            if(nome.InnerXml == "..." || nome.InnerXml == "Protagonista")
                caixaNome.transform.localPosition = pos1;
            else if(caixaNome.transform.localPosition == pos1 && caixaNome.GetComponentInChildren<TextMeshProUGUI>().text !="")
                caixaNome.transform.localPosition = pos2;
            else if(caixaNome.transform.localPosition == pos2)
                caixaNome.transform.localPosition = pos1;
            caixaNome.GetComponentInChildren<TextMeshProUGUI>().text = nome.InnerXml;
        }

        // Atualiza a caixa de texto
        // Cool Dynamic with gradual texting wow
        StopAllCoroutines();
        StartCoroutine(LBLTyping(texto));
    }

    // sub rotina para imprimir o texto letra por letra
    IEnumerator LBLTyping(XmlNode texto){
        GameObject caixaTexto = GameObject.Find("dialogoUI(Clone)/CaixaDialogoFrame/CaixaDialogo");
        caixaTexto.GetComponent<TextMeshProUGUI>().text = "";
        foreach(char letra in texto.InnerXml.ToCharArray()){
            caixaTexto.GetComponent<TextMeshProUGUI>().text += letra;
            yield return null;
        }
    }

    public void CallEscolhas(){
        int i = 1;

        // Atualiza a caixa de resumo da escolha
        GameObject resumoEscolha = GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame/ResumoEscolha");
        resumoEscolha.GetComponentInChildren<TextMeshProUGUI>().text = xReader.ParseResumo(blocoS);

        foreach(XmlNode escolha in xReader.ParseEscolhas(blocoS)){
            // Cria um botao pra cada node da NodeList
            GameObject botao = Instantiate(prefabBotao, GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame").transform);
            // Atualiza a posição da instância na UI
            switch (i)
            {
                case 2:
                    botao.transform.localPosition += new Vector3(330,0,0);
                    break;
                case 3:
                    botao.transform.localPosition += new Vector3(0,-35,0);
                    break;
                case 4:
                    botao.transform.localPosition += new Vector3(330,-35,0);
                    break;
            }

            // Atualiza o texto do botao
            botao.GetComponentInChildren<TextMeshProUGUI>().text = escolha.InnerXml;

            // Atualiza o trigger do botao **
            string proximoBloco = (int.Parse(blocoS) + i).ToString();
            botao.GetComponent<Button>().onClick.AddListener(delegate{LoadDialogue(proximoBloco);});
            
            i++;
        }
    }

    // Tipos de triggers para os botões
    // Proximo bloco de dialogo
    void ProximoBloco(string bloco){
        LoadDialogue(bloco);
    }

    // Proxima cena
    void ProximaCena(string cena){
        // Chama a animação de transição
        SceneManager.LoadScene(cena);
    }

    // Game Over
    void GameOver(){
        ProximaCena("GameOver");
    }
}
