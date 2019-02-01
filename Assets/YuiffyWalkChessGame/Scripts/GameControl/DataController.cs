using System.Collections;
using UnityEngine;
using static MyUtil.CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using DataEntity;
using MyGameController;
using MyGameObject;
using Utf8Json;

public class DataController : MonoBehaviour {
    public List<PlayerInfo> players = new List<PlayerInfo> ();
    private List<ChessBoardController> boards = new List<ChessBoardController> ();
    public bool onlineMode = false;
    void Start () {
        if (players.Count == 0) {
            PlayerInfo player = new PlayerInfo ("本地主机", PlayerType.LOCAL);
            players.Add (player);
            players.Add (new PlayerInfo ("电脑人", PlayerType.AI));
            players.Add (new PlayerInfo ("恶魔人", PlayerType.AI));
        }
        GenerateChessBoards (players);
        InitShop ();
    }

    // Update is called once per frame
    void Update () {

    }

    void FixedUpdate () {

    }

    void GenerateChessBoards (List<PlayerInfo> players) {
        boards = new List<ChessBoardController> ();
        GameObject GroundPrefab = Resources.Load ("Prefabs/Ground") as GameObject;
        Vector3 groundSize = GroundPrefab.GetComponent<Renderer> ().bounds.size;
        Vector3 pos = Vector3.zero;
        Vector3 posAdd = new Vector3 (groundSize.x * 1.6f, -10, 15);
        foreach (PlayerInfo player in players) {
            GameObject ground = GameObject.Instantiate (GroundPrefab);
            ground.transform.parent = GameObject.Find ("Grounds").transform;
            ground.transform.position = pos;
            pos += posAdd;
            ChessBoardController board = ground.GetComponent<ChessBoardController> ();
            boards.Add (board);
        }
    }

    private string gameDataProjectFilePath = "/YuiffyWalkChessGame/Configs/chessData.json";
    Dictionary<string, dynamic> chessInfoDic;
    Dictionary<string, dynamic> shopCountInfo;
    Dictionary<string, GameObject> chessPrefabDic = new Dictionary<string, GameObject> ();
    Dictionary<string, int> shopCount;
    private void InitShop () {
        var gameData = new Dictionary<string, dynamic> ();
        if (!onlineMode) {
            string filePath = Application.dataPath + gameDataProjectFilePath;

            if (File.Exists (filePath)) {
                string dataAsJson = File.ReadAllText (filePath);
                // JsonValue value = JsonValue.Parse(@"{ ""name"": ""David"" }");
                gameData = JsonSerializer.Deserialize<dynamic> (dataAsJson);
                // Debug.Log ("json=" + dataAsJson);
            } else {
                Debug.Log ("No Json File!" + filePath);
            }
            Debug.Log ("gameData=" + JsonSerializer.PrettyPrint (JsonSerializer.Serialize (gameData)));
        } else {
            Debug.Log ("want online but NO ONLINE MODE!");
        }

        chessInfoDic = gameData["chess"];
        shopCountInfo = gameData["shop"];
        foreach (var chessInfo in chessInfoDic) {
            GameObject chessPrefab = GenerateChessPrefab (chessInfo.Value);
            chessPrefabDic.Add (chessInfo.Key, chessPrefab);
        }
        shopCount = new Dictionary<string, int>();
        foreach(var oneItem in shopCountInfo){
            shopCount.Add(oneItem.Key, (int)oneItem.Value);
        }
    }

    private GameObject GenerateChessPrefab (dynamic data) {
        GameObject prefab = Resources.Load ("Prefabs/Chess") as GameObject;
        ChessController chessCtrl = prefab.GetComponent<ChessController> ();
        if (data.ContainsKey ("atk")) chessCtrl.atk = (int) data["atk"];
        if (data.ContainsKey ("hp")) chessCtrl.maxHp = (int) data["hp"];
        if (data.ContainsKey ("ui")) {
            dynamic ui = data["ui"];
            if (ui.ContainsKey ("color")) {
                dynamic colorInfo = ui["color"];
                Color color = new Color ();
                switch (colorInfo) {
                    case "white":
                        color = Color.white;
                        break;
                    default:
                        ColorUtility.TryParseHtmlString (colorInfo, out color);
                        break;
                }
                chessCtrl.chessColor = color;
            }
        }
        //TODO:...
        return prefab;
    }
}