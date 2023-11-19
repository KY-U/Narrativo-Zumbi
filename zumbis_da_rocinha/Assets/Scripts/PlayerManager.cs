using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using System.Xml;

/*
enum inventory{
    CLEAR_ALL,  //0
    PISTOLA,    //1
    FACA,       //2
    TACO        //3
}
*/

public class PlayerManager : MonoBehaviour{
    public string curScene = "1.0";
    public string curBlock = "0";
    public int machucado = 0;
    public int armado = 0;
    public int deathCount = 0;

    [SerializeField] private HUDUpdates hud;

    public void Start(){
        hud.RenewUI();
    }

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

    public void LimparSave(){
        SetCurScene("1.0");
        SetCurBlock("0");
        machucado = 0;
        armado = 0;
        deathCount = 0;
        Salvar();
    }

    // Sets
        public void SetCurScene(string scene){
            curScene = scene;
        }

        public void SetCurBlock(string block){
            curBlock = block;
        }

        // Machucado
        public void Ai(){ 
            machucado++;
            hud.UpdateUI();
            if(machucado > 3){
                Morreu();
            }
        }

        // Contador mortes
        public void Morreu(){ 
            deathCount++; 
            SceneManager.LoadScene("Game Over");
        }

        // Adicionar/retirar arma
        public void GunControl(string g){
            int gun = int.Parse(g);
            if(gun > 0) armado |= 1 << gun;
            else if(gun < 0) armado &= ~(1 << -gun);
            else{
                for(int i = 1; i < 4; i++) armado &= ~(1 << i);
            }
        }
}