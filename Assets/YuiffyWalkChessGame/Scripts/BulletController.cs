using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommonUtil;

public class BulletController : MonoBehaviour
{
    public GameObject aim;
    public int speed = 500;
    public AtkAttr atkAttr;
    public GameObject fromObj;
    public Color color;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttr(Color _color) {
        color = _color;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.startColor = color;
        //trail.endColor = color;
        Gradient colorGradient = trail.colorGradient;
        GradientColorKey[] colorKeys = colorGradient.colorKeys;
        colorKeys[0].color = color;
        colorKeys[1].color = color;
        GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
        trail.colorGradient.SetKeys(colorKeys, alphaKeys);
        Light light = GetComponent<Light>();
        light.color = color;
        GetComponent<MeshRenderer>().material.color = color;
    }

    void FixedUpdate()
    {
        Vector3 aimVelo = rb.velocity;
        if (!aim || !aim.gameObject)
        {
            Destroy(this.gameObject, 5.0f);
        }
        else {
            aimVelo = aim.transform.position - transform.position;
        }

        // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
        Vector3 movement = (aimVelo).normalized;

        // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
        // multiplying it by 'speed' - our public player speed that appears in the inspector
        rb.AddForce(movement * speed/100);
    }

    void OnTriggerEnter(Collider other)
    {
        // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
        Debug.Log("face:"+ other);
        if (other.gameObject == aim)
        {
            ChessController aimChess = other.gameObject.GetComponent<ChessController>();
            aimChess.BeAttacked(atkAttr, fromObj);
            Destroy(this.gameObject);
        }
    }
}
