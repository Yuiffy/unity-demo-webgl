using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    public int x;
    public int y;
    public int atk;
    public int hp;
    private ChessBoardController board;

    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void BoardReady(ChessBoardController _board) {
        board = _board;
        tf = GetComponent<Transform>();
        Vector3 pos = board.getChessPosition(x, y);
        tf.SetPositionAndRotation(pos, tf.rotation);
        Debug.Log("?"+pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
