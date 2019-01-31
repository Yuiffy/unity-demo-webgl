using UnityEngine;
using System.Collections;
using static MyUtil.CommonUtil;
using MyGameController;
using Player;

public class GameFlowController : MonoBehaviour
{
    private GameFlowState state = GameFlowState.MANAGE;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToNextState()
    {
        switch (state)
        {
            case GameFlowState.MANAGE:
                ToState(GameFlowState.READY);
                break;
            case GameFlowState.READY:
                ToState(GameFlowState.BATTLE);
                break;
            case GameFlowState.BATTLE:
                ToState(GameFlowState.MANAGE);
                break;
        }
    }

    public void ToState(GameFlowState _state){
        state = _state;
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        foreach(GameObject ground in grounds){
            ChessBoardController board = ground.GetComponent<ChessBoardController>();
            board.SetState(state);
        }

        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.SetState(state);
    }
}
