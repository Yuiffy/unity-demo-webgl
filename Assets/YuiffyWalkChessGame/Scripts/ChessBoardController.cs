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
    private float xBlockRange;
    private float yBlockRange;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        realSize = GetComponent<Renderer>().bounds.size;
        xBlockRange = (realSize.x - 2 * edgeWidth) / xMax;
        yBlockRange = (realSize.z - 2 * edgeWidth) / yMax;
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
    public Vector3 GetChessPosition(int x, int y){
        Vector3 boardPos = tf.position;
        double chessX = boardPos.x - realSize.x / 2 + edgeWidth + (xBlockRange * (x+0.5));
        double chessY = boardPos.z - realSize.z / 2 + edgeWidth + (yBlockRange * (y + 0.5));
        return new Vector3((float)chessX, 1.0f, (float)chessY);
    }

    public float GetBlockRange() {
        return Math.Max(xBlockRange, yBlockRange);
    }
}