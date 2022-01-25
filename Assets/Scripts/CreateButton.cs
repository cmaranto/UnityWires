using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CreateButton : MonoBehaviour
{
    public Button createButton;
    public TMP_Dropdown createOption;
    // Start is called before the first frame update
    void Start()
    {
        createButton.onClick.AddListener(createButtonOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createButtonOnClick(){
        switch(createOption.value){
            case 0://input
                IoManager.instance.createInput();
                break;
            case 1://output
                IoManager.instance.createOutput();
                break;
        }
    }
}
