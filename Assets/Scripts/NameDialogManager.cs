using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameDialogManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button closeButton;
    public Button okButton;
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(onClose);
        okButton.onClick.AddListener(onOk);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onClose(){
        GameObject.Destroy(this.gameObject);
    }

    void onOk(){
        if(nameInput.text != ""){
            IoManager.instance.saveModule(nameInput.text);
            onClose();
        }        
    }   
}
