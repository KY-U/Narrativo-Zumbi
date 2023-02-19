using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using System.Xml;

/*
    pistola -> 0
    faca    -> 1
    taco    -> 2
*/
public class PlayerManager : MonoBehaviour{
    public string curScene;
    public string curBlock = "0";
    public int machucado = 0;
    public int armado = 0;
    public int deathCount = 0;

    // Salvar Status
    public void Salvar(){
        XmlDocument saveFile = new XmlDocument();

        XmlElement root = saveFile.CreateElement("Save");
        root.SetAttribute("filesave", "Save_01");

        XmlElement curSceneElement = saveFile.CreateElement("curScene");
        curSceneElement.InnerText = curScene;
        root.AppendChild(curSceneElement);

        XmlElement curBlockElement = saveFile.CreateElement("curBlock");
        curBlockElement.InnerText = curBlock;
        root.AppendChild(curBlockElement);

        XmlElement machucadoElement = saveFile.CreateElement("machucado");
        machucadoElement.InnerText = machucado.ToString();
        root.AppendChild(machucadoElement);

        XmlElement armadoElement = saveFile.CreateElement("armado");
        armadoElement.InnerText = armado.ToString();
        root.AppendChild(armadoElement);

        XmlElement deathCountElement = saveFile.CreateElement("deathCount");
        deathCountElement.InnerText = deathCount.ToString();
        root.AppendChild(deathCountElement);

        saveFile.AppendChild(root);
        saveFile.Save(Application.dataPath + "/ZRsave.text");
    }

    // Carregar Status
    public void Carregar(){
        if(File.Exists(Application.dataPath + "/ZRsave.text")){
            XmlDocument loadFile = new XmlDocument();
            loadFile.Load(Application.dataPath + "/ZRsave.text");

            XmlNode root = loadFile.SelectSingleNode("Save");
            if(root["curScene"] != null) curScene = root["curScene"].InnerXml;
            if(root["curBlock"] != null) curBlock = root["curBlock"].InnerXml;
            if(root["machucado"] != null) machucado = int.Parse(root["machucado"].InnerXml);
            if(root["armado"] != null) armado = int.Parse(root["armado"].InnerXml);
            if(root["deathCount"] != null) deathCount = int.Parse(root["deathCount"].InnerXml);
        }
    }

    // Sets
        // Machucado
        public void SetCurScene(string scene){
            curScene = scene;
        }

        public void SetCurBlock(string block){
            curBlock = block;
        }

        public void Ai(){ 
            machucado++; 
        }

        // Contador mortes
        public void Morreu(){ 
            deathCount++; 
        }

        // Adicionar/retirar arma
        public void GunControl(string g){
            int gun = int.Parse(g);
            if(gun > 0) armado |= 1 << gun;
            else armado &= ~(1 << -gun);
        }
}