using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IoDialogManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text ioText;
    public Slider valueSlider;
    public Button closeButton;
    public Button deleteButton;
    public Io io;
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(onClose);
        deleteButton.onClick.AddListener(onDelete);
        valueSlider.onValueChanged.AddListener(onValueSlider);
        nameInput.onValueChanged.AddListener(onNameChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setIo(Io _io){
        io = _io;
        io.valueChangedEvent.AddListener(updateValue);
        nameInput.text = io.ioName;
        if(io.type == Io.Type.StaticInput){
            valueSlider.gameObject.SetActive(true);
            valueSlider.SetValueWithoutNotify(TruthTable.boolToInt(io.value));
        }else{
            valueSlider.gameObject.SetActive(false);
        }
        updateValue();
    }

    void updateValue(){
        ioText.SetText(string.Format("{0}->{1}->{2}",io.incomingConnection ? io.incomingConnection.ToString() : "None",io.ToString(),
                                                    io.outgoingConnections.Count > 0 ? string.Join(",",io.outgoingConnections) : "None"));
    }

    void onValueSlider(float value){
        if(io.type == Io.Type.StaticInput)io.value = value > 0 ? true : false;
    }

    void onNameChanged(string name){
        io.ioName = name;
        updateValue();
    }

    void onClose(){
        GameObject.Destroy(this.gameObject);
    }

    void onDelete(){
        IoManager.instance.removeIo(io);
        onClose();
    }
}
