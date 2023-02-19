//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour {
    [Header ("XML")]
    [SerializeField] private XML_reading xReader;       // Instanciação da classe que lê o XML
    private Queue<XmlNode> falas;

    [Header ("UI")]
    public GameObject canvas;                           // Parente de todas as UI bases
    public GameObject prefabDialogo;                    // Prefab da UI de dialogo
    public GameObject prefabEscolhas;                   // Prefab da UI de escolhas
    public GameObject prefabBotao;                      // Prefab dos botões da UI de escolha

    [Header ("Scene Dynamics")]
    [SerializeField] private AudioManager  audioCues;   // Deixas de audio
    [SerializeField] private ItemManager   itemCues;    // Deixas de item
    
    [Header ("Player")]
    [SerializeField] private PlayerManager Jogador; 

    [Header ("Animator")]
    public Animator transicao;

    // Para dinâmicas da caixa de nome
    private Vector3 pos1 = new Vector3(0,0,0);
    private Vector3 pos2;
    private Color original = new Color(0,0,0,0);
    private Color invisivel;

    void Start(){
        falas = new Queue<XmlNode>();
        xReader.definirBloco("0");
        xReader.LoadFile();

        Jogador.Carregar();

        LoadDialogue(); // Chama a primeira execução de bloco
    }

    public void LoadDialogue(){
        foreach(XmlNode dialogo in xReader.ParseDialogo()){
            falas.Enqueue(dialogo); // Adiciona todos os nodes na fila
        }

        Destroy(GameObject.Find("escolhasUI(Clone)"));  // Limpa a tela se necessário
        Instantiate(prefabDialogo, canvas.transform);   // Instancia a UI de diálogo

        CallNextDialogue();
    }

    public void CallNextDialogue(){
        // i.e. quando acabar o diálogo
        if(falas.Count == 0){
            StopAllCoroutines();
            Destroy(GameObject.Find("dialogoUI(Clone)"));   // Limpa a tela
            CallEscolhas();
            return;
        }
        // Puxa um node da fila para ser utilizado
        XmlNode fala = falas.Dequeue();
        XmlNode nome = fala.FirstChild;
        XmlNode texto = nome.NextSibling;
        XmlNode som = fala["som"];
        XmlNode item = fala["item"];

        if(som != null)
            audioCues.PlayCue(som.InnerXml);

        if(item != null)
            itemCues.ItemCue(item.InnerXml);

        // Define as posições da caixa de nome
        GameObject caixaNome = GameObject.Find("dialogoUI(Clone)/CaixaNomeFrame");
        if(pos1.x == 0){
            pos1 = caixaNome.transform.localPosition;
            pos2 = caixaNome.transform.localPosition - new Vector3(2*caixaNome.transform.localPosition.x,0,0);
        }

        // Para a caixa de nome ficar invisivel quando for o narrador falando
        if(original.r == 0){
            original = caixaNome.GetComponent<Image>().color;
            invisivel = new Color(original.r, original.g, original.b, 0f);
        }

        // Atualiza a caixa de nome da personagem
        if(nome.InnerXml == "..."){
            caixaNome.GetComponent<Image>().color = invisivel;
            caixaNome.transform.localPosition = pos2;
            caixaNome.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else if(caixaNome.GetComponentInChildren<TextMeshProUGUI>().text != nome.InnerXml){
            caixaNome.GetComponent<Image>().color = original;
            if(nome.InnerXml == "José Carlos" || caixaNome.transform.localPosition == pos2)
                caixaNome.transform.localPosition = pos1;
            else if(caixaNome.transform.localPosition == pos1)
                caixaNome.transform.localPosition = pos2;
            caixaNome.GetComponentInChildren<TextMeshProUGUI>().text = nome.InnerXml;
        }

        // Atualiza a caixa de texto
        // Cool Dynamic with gradual texting wow
        GameObject caixaTexto = GameObject.Find("dialogoUI(Clone)/CaixaDialogoFrame/CaixaDialogo");
        caixaTexto.GetComponent<TextMeshProUGUI>().text = "";

        // Atualiza os botões que controlam o fluxo do diálogo
            // Termina texto imediatamente 
            GameObject acelerar = GameObject.Find("dialogoUI(Clone)/Trigger");
            acelerar.GetComponent<Button>().onClick.RemoveAllListeners();
            acelerar.GetComponent<Button>().onClick.AddListener(delegate{acelerarTexto(caixaTexto, texto);});

            // Acelera o texto
            GameObject continuar = GameObject.Find("dialogoUI(Clone)/Continuar");
            continuar.GetComponent<Button>().onClick.RemoveAllListeners();
            continuar.GetComponent<Button>().onClick.AddListener(delegate{completarTexto(caixaTexto, texto);});

        // Inicia a mostragem do texto
        // Cool printing dynamic wow
        StartCoroutine(LBLTyping(caixaTexto, texto));
    }

    // Função do botão Trigger
    public void acelerarTexto(GameObject caixa, XmlNode texto){
        buttonPressed = true;
        if(caixa.GetComponent<TextMeshProUGUI>().text == texto.InnerXml){
            buttonPressed = false;
            CallNextDialogue();
        }
    }

    // Função do botão Continuar
    private bool buttonPressed = false;
    public void completarTexto(GameObject caixa, XmlNode texto){
        StopAllCoroutines();
        if(caixa.GetComponent<TextMeshProUGUI>().text == texto.InnerXml){
            buttonPressed = false;
            CallNextDialogue();
        }
        else caixa.GetComponent<TextMeshProUGUI>().text = texto.InnerXml;
    }

    // sub rotina para imprimir o texto letra por letra
    WaitForSeconds fastDelay = new WaitForSeconds(0.01f);
    WaitForSeconds slowDelay = new WaitForSeconds(0.05f);
    IEnumerator LBLTyping(GameObject caixa, XmlNode texto){
        foreach(char letra in texto.InnerXml.ToCharArray()){
            caixa.GetComponent<TextMeshProUGUI>().text += letra;
            if(buttonPressed) yield return fastDelay;
            else yield return slowDelay;
        }
    }

    public void CallEscolhas(){
        int i = 0;

        XmlNodeList escolhas = xReader.ParseEscolhas();
        if(escolhas.Count != 0){
            Instantiate(prefabEscolhas, canvas.transform);  // Instancia a UI de escolha

            // Atualiza a caixa de resumo da escolha
            GameObject resumoEscolha = GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame/ResumoEscolha");
            resumoEscolha.GetComponentInChildren<TextMeshProUGUI>().text = xReader.ParseResumo();

            foreach(XmlNode escolha in escolhas){
                // Testa se a escolha deveria aparecer
                bool saudavel;
                bool arma;

                XmlNode estaSaudavel = escolha["estaSaudavel"];
                if(estaSaudavel != null)
                    saudavel = (Jogador.machucado < int.Parse(estaSaudavel.InnerXml));
                else saudavel = true;

                XmlNode estaArmado = escolha["estaArmado"];
                if(estaArmado != null){
                    int temArma = int.Parse(estaArmado.InnerXml);
                    arma = ((Jogador.armado & 1 << System.Math.Abs(temArma)) != 0);
                    Debug.Log(arma);
                    if(temArma < 0) arma = !arma;
                    Debug.Log(arma);
                }
                else arma = true;

                if(saudavel && arma){
                    // Cria um botao pra cada node da NodeList
                    GameObject botao = Instantiate(prefabBotao, GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame").transform);
                    
                    // Atualiza a posição da instância na UI
                    botao.transform.localPosition += new Vector3(0,i*(-30),0);

                    // Atualiza o texto do botao
                    botao.GetComponentInChildren<TextMeshProUGUI>().text = escolha["texto"].InnerXml;
                    botao.GetComponent<Button>().onClick.AddListener(Jogador.Salvar);

                    // Testa as consequências da escolha
                    if(escolha["machucado"] != null)
                        botao.GetComponent<Button>().onClick.AddListener(Jogador.Ai);
                    if(escolha["armado"] != null){
                        Debug.Log("achou a tag");
                        botao.GetComponent<Button>().onClick.AddListener(delegate {Jogador.GunControl(escolha["armado"].InnerXml);});
                    }

                    // Atualiza o caminho do botão
                    if(escolha["paraBloco"] != null)
                        botao.GetComponent<Button>().onClick.AddListener(delegate {ProximoBloco(escolha["paraBloco"].InnerXml); });
                    
                    i++;
                }
            }
        }
        else if(escolhas.Count == 0){
            XmlNode transicao = xReader.ParseTransicao();
            if(transicao["gameOver"] != null)
                GameOver();
            else if(transicao["paraCena"] != null)
                StartCoroutine(ProximaCena(transicao["paraCena"].InnerXml));
        }
    }

    // Proximo bloco de dialogo
    void ProximoBloco(string bloco){
        xReader.definirBloco(bloco);
        LoadDialogue();
    }

    // Proxima cena
    private WaitForSeconds tempoTransicao = new WaitForSeconds(1.5f);
    IEnumerator ProximaCena(string c){
        Destroy(GameObject.Find("dialogoUI(Clone)"));
        Destroy(GameObject.Find("escolhasUI(Clone)"));

        transicao.SetTrigger("transition");
        yield return tempoTransicao;

        if(c != "Fim"){
            Jogador.SetCurScene(c);
            Jogador.Salvar();
        }
        
        string cena = "Cena " + c;
        SceneManager.LoadScene(cena);
    }

    // Game Over
    void GameOver(){
        Jogador.Morreu();
        SceneManager.LoadScene("GameOver");
    }
}