using System.Collections;
using UnityEngine;
using static MyUtil.CommonUtil;
using System.Collections.Generic;
using DataEntity;
using MyGameController;
using MyGameObject;
using UnityEngine.UI;

public class LocalShopController : MonoBehaviour {
    private List<GameObject> localPlayerShop = new List<GameObject> ();
    private GameObject shopItemsParent;
    private GameObject templateItem;
    private List<GameObject> itemsGameObj = new List<GameObject> ();
    public DataController dataController;
    public int localPlayerIndex;
    // public float edgeWidth = 15;
    void Start () {

    }

    public void Init () {
        shopItemsParent = GameObject.Find ("shopItems");
        templateItem = shopItemsParent.transform.Find ("Item").gameObject;
        Debug.Log ("LocalShopCtrl Init over", templateItem);
    }

    public void UpdateShopDisplay (List<GameObject> shop) {
        localPlayerShop = shop;
        ClearShopDisplay ();
        RectTransform templateRect = templateItem.GetComponent<RectTransform> ();
        float itemWidth = templateRect.sizeDelta.x;
        float fullWidth = shopItemsParent.transform.parent.GetComponent<RectTransform> ().sizeDelta.x;
        Vector3 startPos = templateRect.localPosition;
        float edgeWidth = ((fullWidth - itemWidth * shop.Count) / (shop.Count + 1));
        startPos.x = edgeWidth - (fullWidth / 2) + itemWidth / 2;
        for (int i = 0; i < shop.Count; i++) {
            GameObject item = shop[i];
            ChessController chess = item.GetComponent<ChessController> ();
            GameObject newItem = GameObject.Instantiate (templateItem, shopItemsParent.transform);
            // Debug.Log ("Width" + itemWidth + "," + fullWidth);
            RectTransform rect = newItem.transform.GetComponent<RectTransform> ();
            rect.localScale = Vector3.one;
            rect.localPosition = startPos + new Vector3 (i * (itemWidth + edgeWidth), 0, 0);
            int j = i;
            newItem.GetComponent<Button> ().onClick.AddListener (() => BuyItem (j));
            newItem.SetActive (true);
            Transform label = newItem.transform.Find ("Item Label");
            Text text = label.GetComponent<Text> ();
            text.text = chess.name + " ☆"+chess.price;
            itemsGameObj.Add (newItem);
        }
    }

    void ClearShopDisplay () {
        foreach(GameObject obj in itemsGameObj){
            Destroy(obj);
        }
        itemsGameObj.Clear ();
    }

    // Update is called once per frameß
    void Update () {

    }

    void FixedUpdate () {

    }

    public void BuyItem (int i) {
        Debug.Log ("CLICK" + i);
        bool ret = dataController.OnePlayerBuyOneChess (localPlayerIndex, i);
        if (ret) {
            itemsGameObj[i].SetActive (false);
            DestroyImmediate (itemsGameObj[i], true);
        }
    }

    public void RefreshItems () {
        dataController.OnePlayerRefreshChess (localPlayerIndex);
    }
}