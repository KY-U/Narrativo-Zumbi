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

    private Vector3 pos1;
    private Vector3 pos2;
    
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
    public AudioSource source;
    public AudioClip clip;
    IEnumerator LBLTyping(XmlNode texto){
        GameObject caixaTexto = GameObject.Find("dialogoUI(Clone)/CaixaDialogoFrame/CaixaDialogo");
        caixaTexto.GetComponent<TextMeshProUGUI>().text = "";
        foreach(char letra in texto.InnerXml.ToCharArray()){
            source.Stop();
            caixaTexto.GetComponent<TextMeshProUGUI>().text += letra;
            source.PlayOneShot(clip);
            yield return null;
        }
    }

    public void CallEscolhas(){
        int i = 1;

        // Atualiza a caixa de resumo da escolha
        GameObject resumoEscolha = GameObject.Find("escolhasUI(Clone)/CaixaEscolhasFrame/ResumoEscolha");
        resumoEscolha.GetComponentInChildren<TextMeshProUGUI>().text = xReader.ParseResumo();

        XmlNodeList escolhas = xReader.ParseEscolhas();
        if(escolhas.Count == 0) GameOver();
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
                switch (i){
                    case 1:
                        break;
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
                botao.GetComponentInChildren<TextMeshProUGUI>().text = escolha["texto"].InnerXml;

                // Testa as consequências da escolha
                if(escolha["machucado"] != null)
                    botao.GetComponent<Button>().onClick.AddListener(Jogador.Ai);
                if(escolha["armado"] != null)
                    botao.GetComponent<Button>().onClick.AddListener(delegate {Jogador.GunControl(escolha["armado"].InnerXml);});
                
                // Atualiza o caminho do botão
                if(escolha["paraBloco"] != null)
                    botao.GetComponent<Button>().onClick.AddListener(delegate {ProximoBloco(escolha["paraBloco"].InnerXml); });
                else if(escolha["gameOver"] != null)
                    botao.GetComponent<Button>().onClick.AddListener(GameOver);
                else
                    botao.GetComponent<Button>().onClick.AddListener(delegate {ProximaCena(escolha["paraCena"].InnerXml); }); 
                
                i++;
            }
        }
    }

    // Proximo bloco de dialogo
    void ProximoBloco(string bloco){
        xReader.definirBloco(bloco);
        LoadDialogue();
    }

    // Proxima cena
    void ProximaCena(string cena){
        Jogador.curScene++;
        // Chama a animação de transição
        SceneManager.LoadScene(cena);
    }

    // Game Over
    void GameOver(){
        Jogador.Morreu();
        ProximaCena("GameOver");
    }
}