using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;

[System.Serializable]
public class XML_reading {
    [SerializeField] TextAsset xmlFile;
    public XmlDocument xmlDoc;

    public void LoadFile(){ 
        string data = xmlFile.text;
        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));
    }

    public XmlNodeList ParseDialogo(string bloco){
        string xmlPath = "//cena1/bloco[@s='"+ bloco +"']/dialogos/dialogo";
        XmlNodeList dialogos = xmlDoc.SelectNodes(xmlPath);
        return dialogos;
    }
}