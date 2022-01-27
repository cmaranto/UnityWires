using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Io : MonoBehaviour
{

    public enum Type{
        Input,
        Output,
        StaticInput,
        StaticOutput
    }
    private Type m_type = Type.Input;
    public Type type{
        get{return m_type;}
        set{m_type = value;}
    }

    private Io m_connection;
    public Io connection
    {
        get { return m_connection; }
        set { 
            m_connection = value;
            if(type == Type.Input || type == Type.StaticOutput){
                if(m_connection)m_connection.valueChangedEvent.AddListener(onValueChanged);
            }
            onValueChanged();
        }
    }

    public TMP_Text nameText;
    private string m_ioName = "I/O";
    public string ioName
    {
        get { return m_ioName; }
        set
        {
            m_ioName = value; 
            updateNameText();      
        }
    }

    public UnityEvent valueChangedEvent = new UnityEvent();
    private bool m_value = false;
    public bool value{
        get{
            if((type == Type.Input || type == Type.StaticOutput)){
                return connection ? connection.value : false;
            }else{
                return m_value;
            }
        }
        set{
            if(type == Type.StaticInput || type == Type.Output){
                m_value = value;
                onValueChanged();
            }
        }
    }

    public override string ToString()
    {
        return string.Format("{0}({1})",ioName,value);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void updateNameText(){
        nameText.SetText(m_ioName[0].ToString());
    }

    void OnMouseOver(){
        if(type == Type.StaticOutput || type == Type.Input)IoManager.instance.currentIoTarget = this;

        if(Input.GetMouseButtonDown(1)){
            onRightClick();
        }
        if(Input.GetMouseButtonDown(0)){
            onLeftClick();
        }
    }

    void OnMouseExit(){
        if(type == Type.StaticOutput || type == Type.Input)IoManager.instance.currentIoTarget = null;
    }

    void onRightClick(){
        IoManager.instance.showIoDialog(this);
    }

    void onLeftClick(){
        if((type == Type.StaticInput || type == Type.Output) && !IoManager.instance.wiring){
            IoManager.instance.currentIo = this;
            IoManager.instance.startWiring();
        }
    }

    void onValueChanged(){
        valueChangedEvent.Invoke();
    }
}
