using System.Collections;
using UnityEngine;
using static MyUtil.CommonUtil;
using MyGameController;
using Player;

public class GameFlowController : MonoBehaviour {
    private GameFlowState state = GameFlowState.MANAGE;
    public DataController dataController;
    // Use this for initialization
    void Start () {

    }
    public void DataReady (DataController dataCtrl) {
        dataController = dataCtrl;
    }
    // Update is called once per frame
    void Update () {

    }

    public void ToNextState () {
        switch (state) {
            case GameFlowState.MANAGE:
                ToState (GameFlowState.READY);
                break;
            case GameFlowState.READY:
                dataController.ArangeEnemyForBoards ();
                ToState (GameFlowState.BATTLE);
                break;
            case GameFlowState.BATTLE:
                ToState (GameFlowState.MANAGE);
                break;
        }
    }

    private int unfinishGroundCount;
    public void ToState (GameFlowState _state) {
        GameObject[] grounds = GameObject.FindGameObjectsWithTag ("Ground");
        state = _state;
        Debug.Log ("GameFlow ToState" + _state);
        switch (state) {
            case GameFlowState.MANAGE:

                break;
            case GameFlowState.READY:
                dataController.ArangeEnemyForBoards ();

                break;
            case GameFlowState.BATTLE:
                unfinishGroundCount = grounds.Length;
                break;
        }
        foreach (GameObject ground in grounds) {
            ChessBoardController board = ground.GetComponent<ChessBoardController> ();
            if (state == GameFlowState.BATTLE)
                board.OnBattleOver += BattleOverHandler;
            board.SetState (state);
        }

        PlayerController player = GameObject.Find ("Player").GetComponent<PlayerController> ();
        player.SetState (state);
    }

    private void BattleOverHandler (ChessBoardController board) {
        Debug.Log ("Battle Over Handler:" + unfinishGroundCount + "," + board.ownerTeam);
        if (unfinishGroundCount > 0) {
            unfinishGroundCount--;
            if (unfinishGroundCount <= 0) {
                ToNextState ();
            }
        }
    }
}