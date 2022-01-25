using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Io : MonoBehaviour
{
    private string m_ioName = "New I/O";
    public string ioName{
        get{
            return m_ioName;
        }
        set{
            m_ioName = value;
            gameObject.name = value;
            if(nameText)nameText.SetText(value);
        }
    }

    public Color offColor = Color.gray;
    public Color onColor = Color.white;
    public SpriteRenderer circle;
    private bool m_value = false;
    public bool value{
        get{
            return m_value;
        }
        set{
            m_value = value;
            if(circle)circle.color = value ? onColor : offColor;
        }
    }
    public bool moving;
    public bool wiring;


    private float lastClickTime = 0;
    public float doubleClickSec = 0.2f;

    public TextMeshPro nameText;

    // Start is called before the first frame update
    void Start()
    {
        ioName = m_ioName;
        value = m_value;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnMouseOver()
    {
        IoManager.instance.currentIoTarget = this;
        if (Input.GetMouseButtonDown(0)){onLeftMouseDown();}
        if (Input.GetMouseButtonDown(1)){onRightMouseDown();}
    }

    void OnMouseExit(){
        IoManager.instance.currentIoTarget = null;
    }

    void onLeftMouseDown(){
        if(Time.time - lastClickTime <= doubleClickSec){
            onDoubleClick();
            return;
        }
        lastClickTime = Time.time;
        if(!wiring){
            wiring = true;
            IoManager.instance.currentIo = this;
            IoManager.instance.startWiring();
        }
    }
    void onRightMouseDown(){
        if(!moving){
            IoManager.instance.currentIo = this;
            moving=true;
        }
 
    }
    void onDoubleClick(){
        IoManager.instance.currentIo = this;
        IoManager.instance.showDialog();
    }
}
