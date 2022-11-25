using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;

// Classe não é MonoBehaviour, então funciona como um classe básica de C#
[System.Serializable]
public class XML_reading {
    [SerializeField] private TextAsset xmlFile; // Arquivo XML a ser lido
    private XmlDocument xmlDoc;
    public string bloco;
    string xPath;

    // Carrega o arquivo em uma string e le o documento a partir dela
    public void LoadFile(){ 
        string data = xmlFile.text;
        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));
    }

    public void definirBloco(string b){
        bloco = b; 
    }

    // Retorna uma lista com todos os nodes de dialogo de determinado bloco
    public XmlNodeList ParseDialogo(){
        xPath = "//cena/bloco[@s='"+ bloco +"']/dialogos/dialogo";
        return xmlDoc.SelectNodes(xPath);
    }

    // Retorna o texto do node resumo (textinho da UI de escolha) ******
    public string ParseResumo(){
        xPath = "//cena/bloco[@s='"+ bloco +"']/escolhas/resumo";
        XmlNode resumo = xmlDoc.SelectSingleNode(xPath);
        if(resumo != null) return resumo.InnerXml;
        else return "";
    }

    // Retorna uma lista com os nodes de escolha de determinado bloco
    public XmlNodeList ParseEscolhas(){
        xPath = "//cena/bloco[@s='"+ bloco +"']/escolhas/escolha";
        return xmlDoc.SelectNodes(xPath);
    }

    public XmlNode ParseTransicao(){
        xPath = "//cena/bloco[@s='"+ bloco +"']/transicao";
        return xmlDoc.SelectSingleNode(xPath);
    }
}