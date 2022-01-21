using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputScript : MonoBehaviour
{
    private bool pickedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pickedUp){
            Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.SetPositionAndRotation(
                new Vector3(cameraPos.x,cameraPos.y,1),
                Quaternion.identity
            );
        }
    }

    void OnMouseDown(){
        Debug.Log("PIX");
        pickedUp = true;
    }

    void OnMouseUp(){
        pickedUp = false;
    }
}
