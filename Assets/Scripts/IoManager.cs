using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IoManager : MonoBehaviour
{
    private RaycastHit2D hit;
    public GameObject currentIoTarget;

    private static IoManager m_instance;


    // Start is called before the first frame update
    void Awake()
    {
        m_instance = this;
    }

    public static IoManager instance{
        get{
            if(m_instance == null)
            {
                GameObject go = new GameObject("IoManager");
                go.AddComponent<IoManager>();
            }
 
            return m_instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
        if(hit.collider != null){
            currentIoTarget = hit.collider.gameObject;
        }else{
            currentIoTarget = null;
        }
    }
}
