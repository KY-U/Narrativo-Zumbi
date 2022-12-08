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
    [SerializeField] private PlayerManager Jogador; 

    public GameObject canvas;                       // Parente de todas as UI bases

    public GameObject prefabDialogo;                // Prefab da UI de dialogo
    private Queue<XmlNode> falas;

    public GameObject prefabEscolhas;               // Prefab da UI de escolhas
    public GameObject prefabBotao;                  // Prefab dos botões da UI de escolha

    // Para dinâmicas da caixa de nome
    private Vector3 pos1 = new Vector3(0,0,0);
    private Vector3 pos2;
    private Color original = new Color(0,0,0,0);
    private Color invisivel;
    
    void Start(){
        falas = new Queue<XmlNode>();
        xReader.definirBloco("0");
        xReader.LoadFile();
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
            //Instantiate(prefabEscolhas, canvas.transform);  // Instancia a UI de escolha
            CallEscolhas();
            return;
        }
        // Puxa um node da fila para ser utilizado
        XmlNode fala = falas.Dequeue();
        XmlNode nome = fala.FirstChild;
        XmlNode texto = nome.NextSibling;

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

        //Atualiza o trigger do "botao" que chama a proxima parte do dialogo 
        GameObject acelerar = GameObject.Find("dialogoUI(Clone)/Trigger");
        acelerar.GetComponent<Button>().onClick.RemoveAllListeners();
        acelerar.GetComponent<Button>().onClick.AddListener(delegate{acelerarTexto(caixaTexto, texto);});

        GameObject continuar = GameObject.Find("dialogoUI(Clone)/Continuar");
        continuar.GetComponent<Button>().onClick.RemoveAllListeners();
        continuar.GetComponent<Button>().onClick.AddListener(delegate{completarTexto(caixaTexto, texto);});

        StartCoroutine(LBLTyping(caixaTexto, texto));
    }

    private bool buttonPressed = false;
    public void completarTexto(GameObject caixa, XmlNode texto){
        StopAllCoroutines();
        if(caixa.GetComponent<TextMeshProUGUI>().text == texto.InnerXml){
            buttonPressed = false;
            CallNextDialogue();
        }
        else caixa.GetComponent<TextMeshProUGUI>().text = texto.InnerXml;
    }

    public void acelerarTexto(GameObject caixa, XmlNode texto){
        buttonPressed = true;
        if(caixa.GetComponent<TextMeshProUGUI>().text == texto.InnerXml){
            buttonPressed = false;
            CallNextDialogue();
        }
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
                if(estaSaudavel != null){
                    saudavel = (Jogador.machucado < int.Parse(estaSaudavel.InnerXml));
                }
                else saudavel = true;

                XmlNode estaArmado = escolha["estaArmado"];
                if(estaArmado != null){
                    arma = ((Jogador.armado & 1 << int.Parse(estaArmado.InnerXml)) != 0);
                }
                else arma = true;

                if(saudavel && arma){
                    // Cria um botao pra cada node da NodeList
                    GameObject botao = Instantiate(prefabBotao, GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame").transform);
                    
                    // Atualiza a posição da instância na UI
                    botao.transform.localPosition += new Vector3(0,i*(-30),0);

                    // Atualiza o texto do botao
                    botao.GetComponentInChildren<TextMeshProUGUI>().text = escolha["texto"].InnerXml;

                    // Testa as consequências da escolha
                    if(escolha["machucado"] != null)
                        botao.GetComponent<Button>().onClick.AddListener(Jogador.Ai);
                    if(escolha["armado"] != null)
                        botao.GetComponent<Button>().onClick.AddListener(delegate {Jogador.GunControl(escolha["armado"].InnerXml);});
                    
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
    public Animator transicao;
    private WaitForSeconds tempoTransicao = new WaitForSeconds(1.5f);
    IEnumerator ProximaCena(string c){
        Destroy(GameObject.Find("dialogoUI(Clone)"));
        Destroy(GameObject.Find("escolhasUI(Clone)"));

        transicao.SetTrigger("transition");
        yield return tempoTransicao;

        string cena = "Cena " + c;
        SceneManager.LoadScene(cena);
    }

    // Game Over
    void GameOver(){
        Jogador.Morreu();
        SceneManager.LoadScene("GameOver");
    }
}