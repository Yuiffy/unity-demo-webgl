using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardController : MonoBehaviour
{
    public int xMax;
    public int yMax;
    public ChessController[] chesses;
    public float edgeWidth;

    private Transform tf;
    private Vector3 realSize;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        realSize = GetComponent<Renderer>().bounds.size;
        Debug.Log("board"+realSize);
        foreach (ChessController chess in chesses) {
            chess.BoardReady(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //棋用整数xy表示棋子位置，真实坐标是用x和z浮点数
    public Vector3 getChessPosition(int x, int y){
        Vector3 boardPos = tf.position;
        float xBlockRange = (realSize.x - 2 * edgeWidth) / xMax;
        float yBlockRange = (realSize.z - 2 * edgeWidth) / yMax;
        double chessX = boardPos.x - realSize.x / 2 + edgeWidth + (xBlockRange * (x+0.5));
        double chessY = boardPos.z - realSize.z / 2 + edgeWidth + (yBlockRange * (y + 0.5));
        return new Vector3((float)chessX, 1.0f, (float)chessY);
    }
}