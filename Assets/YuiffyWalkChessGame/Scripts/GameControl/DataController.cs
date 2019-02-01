using System.Collections;
using UnityEngine;
using static MyUtil.CommonUtil;
using System.Collections.Generic;
using DataEntity;
using MyGameController;

public class DataController : MonoBehaviour {
    public List<PlayerInfo> players = new List<PlayerInfo> ();
    private List<ChessBoardController> boards = new List<ChessBoardController> ();
    void Start () {
        if (players.Count == 0) {
            PlayerInfo player = new PlayerInfo ("本地主机", PlayerType.LOCAL);
            players.Add(player);
            players.Add(new PlayerInfo ("电脑人", PlayerType.AI));
        }
        GenerateChessBoards (players);
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
        Vector3 posAdd = new Vector3(groundSize.x, 10, 10);
        foreach (PlayerInfo player in players) {
            GameObject ground = GameObject.Instantiate (GroundPrefab);
            ground.transform.parent = GameObject.Find ("Grounds").transform;
            ground.transform.position = pos;
            pos += posAdd;
            ChessBoardController board = ground.GetComponent<ChessBoardController> ();
            boards.Add (board);
        }
    }
}