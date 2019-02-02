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
using Random = UnityEngine.Random;

public class DataController : MonoBehaviour {
    public List<PlayerInfo> players = new List<PlayerInfo> ();
    private List<ChessBoardController> boards = new List<ChessBoardController> ();
    private List<List<GameObject>> playerShop = new List<List<GameObject>> ();
    public bool onlineMode = false;
    private int playerShopItemCount = 5;
    void Start () {
        if (players.Count == 0) {
            PlayerInfo player = new PlayerInfo ("本地主机", PlayerType.LOCAL);
            players.Add (player);
            players.Add (new PlayerInfo ("电脑人", PlayerType.AI));
            players.Add (new PlayerInfo ("恶魔人", PlayerType.AI));
        }
        GenerateChessBoards (players);
        InitShop ();

        if (!onlineMode) {
            LocalInitEveryShop ();
        }

        LocalPlayerController localPlayerCtrl = this.gameObject.GetComponent<LocalPlayerController> ();
        GameFlowController flowCtrl = GetComponent<GameFlowController> ();
        localPlayerCtrl.DataReady (this);
        flowCtrl.DataReady(this);
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
        for (int i = 0; i < players.Count; i++) {
            PlayerInfo player = players[i];
            GameObject ground = GameObject.Instantiate (GroundPrefab);
            ground.transform.parent = GameObject.Find ("Grounds").transform;
            ground.transform.position = pos;
            pos += posAdd;
            ChessBoardController board = ground.GetComponent<ChessBoardController> ();
            board.ownerTeam = i;
            boards.Add (board);
        }
    }

    private string gameDataProjectFilePath = "/YuiffyWalkChessGame/Configs/chessData.json";
    Dictionary<string, dynamic> chessInfoDic;
    Dictionary<string, dynamic> shopCountInfo;
    Dictionary<string, GameObject> chessPrefabDic = new Dictionary<string, GameObject> ();
    Dictionary<string, int> shopCount;

    public List<List<GameObject>> PlayerShop {
        get => playerShop;
        set {
            playerShop = value;

            if (OnPlayerShopsChange != null)
                OnPlayerShopsChange (playerShop);
        }
    }
    public delegate void OnPlayerShopsChangeDelegate (List<List<GameObject>> newVal);
    public event OnPlayerShopsChangeDelegate OnPlayerShopsChange;

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

        //读规则
        if (gameData.ContainsKey ("rule")) {
            dynamic rule = gameData["rule"];
            if (rule.ContainsKey ("playerShopItemCount")) {
                playerShopItemCount = (int) rule["playerShopItemCount"];
            }
        }

        //读棋、店信息
        chessInfoDic = gameData["chess"];
        shopCountInfo = gameData["shop"];
        foreach (var chessInfo in chessInfoDic) {
            GameObject chessPrefab = GenerateChessPrefab (chessInfo.Key, chessInfo.Value);
            chessPrefabDic.Add (chessInfo.Key, chessPrefab);
        }
        shopCount = new Dictionary<string, int> ();
        foreach (var oneItem in shopCountInfo) {
            shopCount.Add (oneItem.Key, (int) oneItem.Value);
        }
    }

    private void LocalInitEveryShop () {
        PlayerShop.Clear ();
        foreach (PlayerInfo player in players)
            PlayerShop.Add (new List<GameObject> ());
        foreach (List<GameObject> onePlayerShop in PlayerShop) {
            PutRandomChessToPlayerShop (onePlayerShop, playerShopItemCount);
        }
    }

    private void PutRandomChessToPlayerShop (List<GameObject> onePlayerShop, int count) {
        List<string> ranItems = GetRandomChessInShop (count);
        foreach (string itemName in ranItems) {
            onePlayerShop.Add (chessPrefabDic[itemName]);
            Debug.Log ("put random chess to shop " + itemName + "," + chessPrefabDic[itemName].name);
            shopCount[itemName]--;
        }
        if (OnPlayerShopsChange != null)
            OnPlayerShopsChange (playerShop);
    }

    private void PutBackPlayerShopToPool (List<GameObject> onePlayerShop) {
        foreach (GameObject obj in onePlayerShop) {
            if (obj && obj.active == true) {
                ChessController chessCtrl = obj.GetComponent<ChessController> ();
                string keyName = chessCtrl.keyName;
                shopCount[keyName]++;
            }
        }
        onePlayerShop.Clear ();
        if (OnPlayerShopsChange != null)
            OnPlayerShopsChange (playerShop);
    }

    private List<string> GetRandomChessInShop (int count) {
        int sum = 0;
        List<KeyValuePair<string, int>> someSum = new List<KeyValuePair<string, int>> ();
        foreach (var one in shopCount)
            if (one.Value > 0) {
                sum += one.Value;
                someSum.Add (new KeyValuePair<string, int> (one.Key, sum));
            }
        List<string> ret = new List<string> ();

        for (int i = 0; i < count; i++) {
            int ran = Random.Range (0, sum);
            int tempSum = 0;
            int j = 0;
            while (tempSum <= ran) {
                tempSum += someSum[j].Value;
                j++;
            }
            // som 2 3
            // ran 0 1 2 3 4
            // j   1 1 2 2 2
            ret.Add (someSum[j - 1].Key);
            for (int k = j - 1; k < someSum.Count; k++)
                someSum[k] = new KeyValuePair<string, int> (someSum[k].Key, someSum[k].Value - 1);
        }

        return ret;
    }

    private GameObject GenerateChessPrefab (string keyName, dynamic data) {
        GameObject templatePrefab = Resources.Load ("Prefabs/Chess") as GameObject;
        GameObject prefab = Instantiate (templatePrefab);
        ChessController chessCtrl = prefab.GetComponent<ChessController> ();
        chessCtrl.keyName = keyName;
        chessCtrl.data = data;
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
            if (ui.ContainsKey ("name")) chessCtrl.name = ui["name"];
        }
        //TODO:...
        return prefab;
    }

    public void OnePlayerBuyOneChess (int playerIndex, int chessIndex) {
        List<GameObject> oneShop = PlayerShop[playerIndex];
        PutOneChessToBoardOutside (boards[playerIndex], oneShop[chessIndex]);
        oneShop[chessIndex].SetActive (false);
        // DestroyImmediate(oneShop[chessIndex], true);
    }

    private void PutOneChessToBoardOutside (ChessBoardController chessBoardController, GameObject gameObject) {
        chessBoardController.PutNewChess (gameObject);
    }

    public void OnePlayerRefreshChess (int playerIndex) {
        PutBackPlayerShopToPool (PlayerShop[playerIndex]);
        PutRandomChessToPlayerShop (PlayerShop[playerIndex], playerShopItemCount);
        Debug.Log ("Refresh over!" + playerIndex);
    }

    public void ArangeEnemyForBoards () {
        for (int i = 0; i < boards.Count; i++) {
            boards[i].enemyTeam = (i + boards.Count - 1) % boards.Count;
        }
        //TODO: 随机分配
    }
}