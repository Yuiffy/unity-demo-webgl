using System.Collections;
using System.Collections.Generic;
using DataEntity;
using MyGameObject;
using MyUtil;
using UnityEngine;

public class LocalPlayerController : MonoBehaviour {
    public static Plane BOARD_CHESS_PLAIN = new Plane (Vector3.up * 10f, Vector3.up * 1.33f);
    public int team = 0;
    public int layerMask = 1 << 8; //第8layer是棋在的layer
    private DataController dataController;
    private LocalShopController localShopCtrl;
    private PlayerInfo localPlayerInfo;
    private int localPlayerIndex = -1;
    public enum MOUSE_STATE {
        DRAGING,
        DRAG_NOTHING,
        NORMAL
    }

    // Start is called before the first frame update
    void Start () { }

    public void DataReady (DataController dataCtrl) {
        localShopCtrl = GetComponent<LocalShopController> ();
        dataController = dataCtrl;
        for (int i = 0; i < dataCtrl.players.Count; i++) {
            PlayerInfo player = dataCtrl.players[i];
            if (player.type == PlayerType.LOCAL) {
                localPlayerInfo = player;
                localPlayerIndex = i;
                break;
            }
        }
        localShopCtrl.dataController = dataController;
        localShopCtrl.localPlayerIndex = localPlayerIndex;
        if (localPlayerIndex != -1) {
            localShopCtrl.Init ();
            UpdateLocalPlayerShop (dataCtrl.PlayerShop);
            dataCtrl.OnPlayerShopsChange += PlayerShopsChangeHandler;
        }
    }

    void FixedUpdate () {
        RayMove ();
    }

    Vector3 oldMouse;
    GameObject oldHitObj;
    MOUSE_STATE mouseState = MOUSE_STATE.NORMAL;

    //这里是使用Ray射线来控制物体移动。
    private void RayMove () {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton (0)) {
            if (!oldHitObj) {
                if (mouseState == MOUSE_STATE.NORMAL) {

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                        GameObject hitObj = hit.transform.gameObject;
                        ChessController chess = hitObj.GetComponent<ChessController> ();
                        CommonUtil.ChessState state = chess.GetState ();
                        int chessTeam = chess.team;
                        if (state == CommonUtil.ChessState.MANAGE && chessTeam == team) {
                            mouseState = MOUSE_STATE.DRAGING;
                            oldHitObj = hitObj;
                        }
                    }
                    if (mouseState != MOUSE_STATE.DRAGING) mouseState = MOUSE_STATE.DRAG_NOTHING;
                } else {
                    //is drag nothing
                }
                // Vector3 offset = Input.mousePosition;
                // hit.transform.position = new Vector3(hit.point.x, hit.point.y, hit.transform.position.z);
                // Debug.DrawLine(ray.origin, hit.point);
            } else {
                float enter;
                bool banana = BOARD_CHESS_PLAIN.Raycast (ray, out enter);
                Vector3 point = ray.GetPoint (enter);
                oldHitObj.transform.position = point;
            }
            // oldMouse = hit.point;
        } else {
            if (oldHitObj) {
                ChessController chess = oldHitObj.GetComponent<ChessController> ();
                chess.NearBySitDown ();
                oldHitObj = null;
            }
            mouseState = MOUSE_STATE.NORMAL;
        }
    }

    void PlayerShopsChangeHandler (List<List<GameObject>> playerShops) {
        UpdateLocalPlayerShop (playerShops);
    }

    void UpdateLocalPlayerShop (List<List<GameObject>> playerShops) {
        localShopCtrl.UpdateShopDisplay (playerShops[localPlayerIndex]);
    }

}