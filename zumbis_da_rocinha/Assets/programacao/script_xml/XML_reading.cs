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
    public XmlDocument xmlDoc;
    private string xPath;

    // Carrega o arquivo em uma string e le o documento a partir dela
    public void LoadFile(){ 
        string data = xmlFile.text;
        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));
    }

    // Retorna uma lista com todos os nodes de dialogo de determinado bloco
    public XmlNodeList ParseDialogo(string bloco){
        xPath = "//cena/bloco[@s='"+ bloco +"']/dialogos/dialogo";
        return xmlDoc.SelectNodes(xPath);;
    }

    // Retorna o texto do node resumo (textinho da UI de escolha)
    public string ParseResumo(string bloco){
        xPath = "//cena/bloco[@s='"+ bloco +"']/escolhas/resumo";
        return xmlDoc.SelectNodes(xPath)[0].InnerXml;
    }

    // Retorna uma lista com os nodes de escolha de determinado bloco
    public XmlNodeList ParseEscolhas(string bloco){
        xPath = "//cena/bloco[@s='"+ bloco +"']/escolhas/escolha";
        return xmlDoc.SelectNodes(xPath);
    }

}