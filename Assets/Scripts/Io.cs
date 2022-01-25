using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Io : MonoBehaviour
{
    private bool m_allowInput = true;
    public bool allowInput{
        get{return m_allowInput;}
        set{m_allowInput = value;}
    }
    private bool m_allowOutput = true;
    public bool allowOutput{
        get{return m_allowOutput;}
        set{m_allowOutput = value;}
    }


    private Io m_inputConnection;
    public Io inputConnection
    {
        get { return m_inputConnection; }
        set { m_inputConnection = value; }
    }

    private Io m_outputConnection;
    public Io outputConnection
    {
        get { return m_outputConnection; }
        set { m_outputConnection = value; }
    }

    public TMP_Text nameText;
    private string m_ioName;
    public string ioName
    {
        get { return m_ioName; }
        set
        {
            m_ioName = value;
            name = value;      
            updateNameText();      
        }
    }

    private bool m_inputValue = false;
    public bool inputValue{
        get{
            if(allowInput && inputConnection){
                return inputConnection.inputValue;
            }else{
                return m_inputValue;
            }
        }
        set{m_inputValue = value;}
    }

    public int inputValueInt{
        get{
            return inputValue ? 1 : 0;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        ioName = name;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void updateNameText(){
        nameText.SetText(m_ioName[0].ToString());
    }

    void OnMouseOver(){
        if(allowInput)IoManager.instance.currentIoTarget = this;

        if(Input.GetMouseButtonDown(1)){
            onRightClick();
        }
        if(Input.GetMouseButtonDown(0)){
            onLeftClick();
        }
    }

    void OnMouseExit(){
        if(allowInput)IoManager.instance.currentIoTarget = null;
    }

    void onRightClick(){
        IoManager.instance.showIoDialog(this);
    }

    void onLeftClick(){
        if(allowOutput && !IoManager.instance.wiring){
            IoManager.instance.currentIo = this;
            IoManager.instance.startWiring();
        }
    }
}
