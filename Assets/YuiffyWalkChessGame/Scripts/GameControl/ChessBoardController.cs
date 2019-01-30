using System;
using System.Collections;
using System.Collections.Generic;
using MyGameObject;
using UnityEngine;

namespace MyGameController
{
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
        private Dictionary<Vector2Int, object> map = new Dictionary<Vector2Int, object>();
        // Start is called before the first frame update
        void Start()
        {
            tf = GetComponent<Transform>();
            realSize = GetComponent<Renderer>().bounds.size;
            xBlockRange = (realSize.x - 2 * edgeWidth) / xMax;
            yBlockRange = (realSize.z - 2 * edgeWidth) / yMax;
            Debug.Log("board" + realSize);
            foreach (ChessController chess in chesses)
            {
                chess.BoardReady(this);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            UpdateMap();
        }

        //棋用整数xy表示棋子位置，真实坐标是用x和z浮点数
        public Vector3 GetChessPosition(int x, int y)
        {
            Vector3 boardPos = tf.position;
            double chessX = boardPos.x - realSize.x / 2 + edgeWidth + (xBlockRange * (x + 0.5));
            double chessY = boardPos.z - realSize.z / 2 + edgeWidth + (yBlockRange * (y + 0.5));
            return new Vector3((float)chessX, 1.0f, (float)chessY);
        }

        public float GetBlockRange()
        {
            return Math.Max(xBlockRange, yBlockRange);
        }

        public Vector2Int GetNearestPosition(Vector3 pos)
        {
            Vector3 boardPos = tf.position;
            int chessXonBoard = (int)Math.Round((pos.x - boardPos.x + realSize.x / 2 - edgeWidth) / xBlockRange - 0.5);
            int chessYonBoard = (int)Math.Round((pos.z - boardPos.z + realSize.z / 2 - edgeWidth) / yBlockRange - 0.5);
            return new Vector2Int(chessXonBoard, chessYonBoard);
        }

        private void UpdateMap()
        {
            map.Clear();
            foreach (ChessController chess in chesses)
            {
                Vector2Int v2 = new Vector2Int(chess.x, chess.y);
                if (map.ContainsKey(v2))
                    map.Add(v2, chess);
                else
                {
                    Debug.Log("已经有棋子在"+v2+chess);
                }
                // map[x][y] = chess;
            }
        }
    }
}
