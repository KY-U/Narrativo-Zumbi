using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class XML_reading : MonoBehaviour
{
    [SerializeField] private TextAsset xmlFile;
    [SerializeField] private Text UI;

    // Start is called before the first frame update
    void Start()
    {
        string data = xmlFile.text;
        parseFile(data);
    }

    
    void parseFile(string xmlData){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData));

        string xmlPath = "//cena1";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPath);
        foreach(XmlNode Node in myNodeList){
            XmlNode text1 = Node.FirstChild;
            XmlNode text2 = text1.NextSibling;

            string totalValue = "Teste: " + text1.InnerXml + "\n" + text2.InnerXml + "\n\n";
            UI.text = totalValue;
        }
    }
}