using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseButton : MonoBehaviour
{
    Transform innerWindow;
    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        innerWindow = transform.Find("InnerWindow");
        SetOpen(isOpen);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOpen(bool _isOpen) {
        isOpen = _isOpen;
        innerWindow.gameObject.SetActive(isOpen);
    }

    public void switchOpen(){
        isOpen = !isOpen;
        innerWindow.gameObject.SetActive(isOpen);
    }
}
