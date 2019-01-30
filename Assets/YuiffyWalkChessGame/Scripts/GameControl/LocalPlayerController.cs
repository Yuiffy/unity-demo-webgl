using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using MyGameObject;

public class LocalPlayerController : MonoBehaviour
{
    public static Plane BOARD_CHESS_PLAIN = new Plane(Vector3.up * 10f, Vector3.up * 1.33f);
    public int team = 0;
    public int layerMask = 1 << 8;  //第8layer是棋在的layer
    public enum MOUSE_STATE
    {
        DRAGING,
        DRAG_NOTHING,
        NORMAL
    };
    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        RayMove();
    }

    Vector3 oldMouse;
    GameObject oldHitObj;
    MOUSE_STATE mouseState = MOUSE_STATE.NORMAL;

    //这里是使用Ray射线来控制物体移动。
    private void RayMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (!oldHitObj)
            {
                if (mouseState == MOUSE_STATE.NORMAL)
                {

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                    {
                        GameObject hitObj = hit.transform.gameObject;
                        ChessController chess = hitObj.GetComponent<ChessController>();
                        CommonUtil.ChessState state = chess.GetState();
                        int chessTeam = chess.team;
                        if (state == CommonUtil.ChessState.MANAGE && chessTeam == team)
                        {
                            mouseState = MOUSE_STATE.DRAGING;
                            oldHitObj = hitObj;
                        }
                    }
                    if (mouseState != MOUSE_STATE.DRAGING) mouseState = MOUSE_STATE.DRAG_NOTHING;
                }
                else
                {
                    //is drag nothing
                }
                // Vector3 offset = Input.mousePosition;
                // hit.transform.position = new Vector3(hit.point.x, hit.point.y, hit.transform.position.z);
                // Debug.DrawLine(ray.origin, hit.point);
            }
            else
            {
                float enter;
                bool banana = BOARD_CHESS_PLAIN.Raycast(ray, out enter);
                Vector3 point = ray.GetPoint(enter);
                oldHitObj.transform.position = point;
            }
            // oldMouse = hit.point;
        }
        else
        {
            if (oldHitObj)
            {
                ChessController chess = oldHitObj.GetComponent<ChessController>();
                chess.NearBySitDown();
                oldHitObj = null;
            }
            mouseState = MOUSE_STATE.NORMAL;
        }


    }

}
