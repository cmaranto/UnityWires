using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent inputChangedEvent = new UnityEvent();
    public UnityEvent outputChangedEvent = new UnityEvent();


    private Io m_inputConnection;
    public Io inputConnection
    {
        get { return m_inputConnection; }
        set { 
            m_inputConnection = value;
            m_inputConnection.inputChangedEvent.AddListener(onInputChanged);
        }
    }

    private Io m_outputConnection;
    public Io outputConnection
    {
        get { return m_outputConnection; }
        set { m_outputConnection = value; }
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

    private bool m_inputValue = false;
    public bool inputValue{
        get{
            if(allowInput && inputConnection){
                return inputConnection.inputValue;
            }else{
                return m_inputValue;
            }
        }
        set{
            m_inputValue = value;
            inputChangedEvent.Invoke();
        }
    }

    public int inputValueInt{
        get{
            return inputValue ? 1 : 0;
        }
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

    void onInputChanged(){
        inputChangedEvent.Invoke();
    }

    void onOutputChanged(){
        outputChangedEvent.Invoke();
    }
}
