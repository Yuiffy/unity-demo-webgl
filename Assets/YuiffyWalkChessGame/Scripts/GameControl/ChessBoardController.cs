using System;
using System.Collections;
using System.Collections.Generic;
using MyGameObject;
using UnityEngine;
using static MyUtil.CommonUtil;

namespace MyGameController {
    public class ChessBoardController : MonoBehaviour {
        public int xMin = 0;
        public int yMin = 0;
        public int xMax = 8;
        public int yMax = 8;
        public List<ChessController> chesses;
        private List<GameObject> chessesBackup = new List<GameObject> ();
        public float edgeWidth;

        private Transform tf;
        private Vector3 realSize;
        private float xBlockRange;
        private float yBlockRange;
        private Dictionary<int, Dictionary<int, object>> map = new Dictionary<int, Dictionary<int, object>> ();
        // Start is called before the first frame update
        void Start () {
            tf = GetComponent<Transform> ();
            realSize = GetComponent<Renderer> ().bounds.size;
            xBlockRange = (realSize.x - 2 * edgeWidth) / xMax;
            yBlockRange = (realSize.z - 2 * edgeWidth) / yMax;
            Debug.Log ("board" + realSize);
            foreach (ChessController chess in chesses) {
                chess.BoardReady (this);
            }
        }

        // Update is called once per frame
        void Update () {

        }

        void FixedUpdate () {
            UpdateMap ();
        }

        //棋用整数xy表示棋子位置，真实坐标是用x和z浮点数
        public Vector3 GetChessPosition (int x, int y) {
            Vector3 boardPos = tf.position;
            double chessX = boardPos.x - realSize.x / 2 + edgeWidth + (xBlockRange * (x + 0.5));
            double chessY = boardPos.z - realSize.z / 2 + edgeWidth + (yBlockRange * (y + 0.5));
            return new Vector3 ((float) chessX, boardPos.y + 1.0f, (float) chessY);
        }

        public float GetBlockRange () {
            return Math.Max (xBlockRange, yBlockRange);
        }

        public Vector2Int GetNearestPosition (Vector3 pos) {
            Vector3 boardPos = tf.position;
            int chessXonBoard = (int) Math.Round ((pos.x - boardPos.x + realSize.x / 2 - edgeWidth) / xBlockRange - 0.5);
            int chessYonBoard = (int) Math.Round ((pos.z - boardPos.z + realSize.z / 2 - edgeWidth) / yBlockRange - 0.5);
            return new Vector2Int (chessXonBoard, chessYonBoard);
        }

        private void UpdateMap () {
            foreach (KeyValuePair<int, Dictionary<int, object>> pair in map) {
                pair.Value.Clear ();
            }
            foreach (ChessController chess in chesses) {
                int x = chess.x;
                int y = chess.y;
                if (!map.ContainsKey (x)) map.Add (x, new Dictionary<int, object> ());
                Dictionary<int, object> column = map[x];
                if (!column.ContainsKey (y))
                    column.Add (y, chess);
                else {
                    Debug.Log ("已经有棋子在" + x + "," + y + chess);
                }
                // map[x][y] = chess;
            }
        }

        public bool IsChessOnPosition (int x, int y) {
            if (map.ContainsKey (x) && map[x].ContainsKey (y)) {
                return true;
            } else return false;
        }

        private static Vector2Int[] goArr = { new Vector2Int (1, 0), new Vector2Int (-1, 0), new Vector2Int (0, 1), new Vector2Int (0, -1) };

        public enum FindPositionType {
            ANY,
            IN_BOARD,
            OUT_BOARD
        }
        public bool IsPositionOk (Vector2Int pos, FindPositionType type) {
            switch (type) {
                case FindPositionType.IN_BOARD:
                    if (pos.x >= xMin && pos.x < xMax && pos.y >= yMin && pos.y < yMax)
                        return true;
                    else return false;
                case FindPositionType.OUT_BOARD:
                    if (!(pos.x >= xMin && pos.x < xMax && pos.y >= yMin && pos.y < yMax))
                        return true;
                    else return false;
                default:
                    return true;
            }
        }
        public Vector2Int FindNearestEmptyPosition (int x, int y, FindPositionType type = FindPositionType.ANY) {
            Queue<Vector2Int> queue = new Queue<Vector2Int> ();
            queue.Enqueue (new Vector2Int (x, y));
            while (queue.Count > 0) {
                Vector2Int now = queue.Dequeue ();
                if (!IsChessOnPosition (now.x, now.y) && IsPositionOk (now, type)) return new Vector2Int (now.x, now.y);
                foreach (Vector2Int go in goArr) {
                    Vector2Int next = now + go;
                    if (!queue.Contains (next)) queue.Enqueue (next);
                }
            }
            return new Vector2Int (); //不懂怎样返回null
        }

        public bool IsInBoard (int x, int y) {
            return IsPositionOk (new Vector2Int (x, y), FindPositionType.IN_BOARD);
        }

        public void ReadyBattle () {
            chessesBackup = new List<GameObject> ();
            foreach (ChessController chess in chesses) {
                if (IsInBoard (chess.x, chess.y)) {
                    GameObject copyChess = Instantiate<GameObject> (chess.transform.gameObject, chess.transform.position, chess.transform.rotation);
                    copyChess.transform.parent = GameObject.Find ("Chesses").transform;
                    copyChess.SetActive (false);
                    chessesBackup.Add (copyChess);
                    chess.state = MyUtil.CommonUtil.ChessState.READY;
                }
            }
        }

        public void StartBattle () {
            foreach (ChessController chess in chesses) {
                if (chess.state == MyUtil.CommonUtil.ChessState.READY) {
                    chess.state = MyUtil.CommonUtil.ChessState.BATTLE;
                }
            }
        }

        public void StopBattle () {
            List<ChessController> newChess = new List<ChessController> ();
            //添加备份棋子
            foreach (GameObject chessObj in chessesBackup) {
                chessObj.SetActive (true);
                ChessController chess = chessObj.GetComponent<ChessController> ();
                chess.Init (this);
                newChess.Add (chess);
            }
            //添加未战斗棋子，删除战斗棋子
            foreach (ChessController chess in chesses) {
                if (chess == null) continue;
                if (chess.gameObject && chess.state == MyUtil.CommonUtil.ChessState.MANAGE) newChess.Add (chess);
                else chess.DestroySelf ();
            }
            Debug.Log (chesses.Count + "," + newChess.Count);
            chesses = newChess;
        }

        public void SetState (GameFlowState _state) {
            switch (_state) {
                case GameFlowState.MANAGE:
                    StopBattle ();
                    break;
                case GameFlowState.READY:
                    ReadyBattle ();
                    break;
                case GameFlowState.BATTLE:
                    StartBattle ();
                    break;
            }
        }

        public void PutNewChess (GameObject prefab) {
            Vector2Int pos = FindNearestEmptyPosition ((xMax + xMin) / 2, (yMax + yMin) / 2, FindPositionType.OUT_BOARD);
            GameObject newChessObj = Instantiate<GameObject> (prefab);
            newChessObj.transform.parent = GameObject.Find ("Chesses").transform;
            ChessController chess = newChessObj.GetComponent<ChessController> ();
            chess.state = MyUtil.CommonUtil.ChessState.READY;
            chess.x = pos.x;
            chess.y = pos.y;
            chess.state = ChessState.MANAGE;
            newChessObj.SetActive (true);
            chess.BoardReady (this);
            chesses.Add (chess);
        }
    }
}