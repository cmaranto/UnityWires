using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IoDialogManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text inputText;
    public TMP_Text outputText;
    public Slider valueSlider;
    public Button okButton;
    public Io io;
    // Start is called before the first frame update
    void Start()
    {
        okButton.onClick.AddListener(onOk);
        valueSlider.onValueChanged.AddListener(onValueSlider);     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setIo(Io _io){
        io = _io;
        nameInput.text = io.ioName;
        inputText.text = string.Format("In: {0}({1})", io.inputConnection ? io.inputConnection.ioName : "",io.inputValueInt);
        outputText.text = string.Format("Out: {0}({1})", io.outputConnection ? io.outputConnection.ioName : "",io.inputValueInt);
        if(!io.allowInput){
            valueSlider.gameObject.SetActive(true);
            valueSlider.SetValueWithoutNotify(io.inputValue ? 1 : 0);
        }else{
            valueSlider.gameObject.SetActive(false);
        }
    }

    void onOk(){
        io.ioName = nameInput.text;
        if(!io.allowInput)io.inputValue = valueSlider.value == 0 ? false : true;
        GameObject.Destroy(this.gameObject);
    }

    void onValueSlider(float value){
        io.inputValue = value > 0 ? true : false;
        setIo(io);
    }
}
