using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    private Queue<XmlNode> falas;

    public string bloco = "0";
    [SerializeField] private XML_reading xReader;
    [SerializeField] private Text CaixaNome;
    [SerializeField] private Text CaixaTexto;

    void Start(){
        falas = new Queue<XmlNode>();
        LoadDialogue(bloco);
    }

    public void LoadDialogue(string bloco){
        xReader.LoadFile();
        XmlNodeList dialogos = xReader.ParseDialogo(bloco);
        foreach(XmlNode dialogo in dialogos){
            falas.Enqueue(dialogo);
        }

        CallNextDialogue();
    }

    public void CallNextDialogue(){
        if(falas.Count == 0) return;// Inserir chamada de escolhas aqui depois

        Debug.Log(falas.Count.ToString());
        XmlNode fala = falas.Dequeue();

        XmlNode nome = fala.FirstChild;
        XmlNode texto = nome.NextSibling;

        CaixaNome.text = nome.InnerXml;
        StopAllCoroutines();
        StartCoroutine(LBLTyping(texto));
    }

    IEnumerator LBLTyping(XmlNode texto){
        CaixaTexto.text = "";
        foreach(char letra in texto.InnerXml.ToCharArray()){
            CaixaTexto.text += letra;
            yield return null;
        }
    }
}
