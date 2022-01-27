using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class IoModuleDialogManager : MonoBehaviour
{
    public TMP_Text moduleNameText;
    public TMP_Text truthTableText;
    public TMP_Text ioText;
    public Button closeButton;
    public IoModule module;
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(onClose);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setModule(IoModule _module){
        module = _module;
        module.outputEvent.AddListener(updateIoText);
        moduleNameText.text = module.moduleData.ioName;
        truthTableText.text = module.moduleData.truthTable.ToString();
        updateIoText();
    }

    void updateIoText(){
        ioText.text = module.ioTableString();
    }

    void onClose(){
        GameObject.Destroy(this.gameObject);
    }
}
