using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
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

    //这里是使用Ray射线来控制物体移动，可是由于射线本身检测速率的限制，并不适合持续的跟踪移动。具体效果各位读者试试便知。
    private void RayMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                // Vector3 offset = Input.mousePosition;
                // hit.transform.position = new Vector3(hit.point.x, hit.point.y, hit.transform.position.z);
                if (oldHitObj)
                {
                    oldHitObj.transform.position += hit.point - oldMouse;
                }
                else {
                    oldHitObj = hit.transform.gameObject;
                }
                // Debug.DrawLine(ray.origin, hit.point);
            }
            oldMouse = hit.point;
        }
        else {
            oldHitObj = null;
        }


    }

}
