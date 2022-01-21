using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    public Button m_button;
    public GameObject IoPrefab;
    // Start is called before the first frame update
    void Start()
    {
        m_button.onClick.AddListener(onReleased);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onReleased(){
        Instantiate(IoPrefab, new Vector3(0,0,0),Quaternion.identity);
    }

       
}
