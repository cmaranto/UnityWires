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
    public Io io;
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(onClose);
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
        ioText.text = string.Format("{0}->{1}",io.ToString(),io.connection ? io.connection.ToString() : "No connection");
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
}
