using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IoManager : MonoBehaviour
{
    private static IoManager m_instance;
    public GameObject ioDialog;
    public TMP_InputField ioDialogNameInput;
    public Slider ioDialogValue;
    public Button ioDialogOkButton;
    public GameObject canvas;
    public Io currentIo;
    public Io currentIoTarget;
    private GameObject wire;
    private LineRenderer wireRenderer;
    public Color wireNotConnectedColor = Color.red;
    public Color wireConnectedColor = Color.green;
    private Color wireColor;

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;

        ioDialogOkButton.onClick.AddListener(onOkButtonClicked);
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
        if(currentIo){
            if(currentIo.wiring){
                if (Input.GetMouseButtonUp(0)){
                    currentIo.wiring = false;
                    GameObject.Destroy(wire);


                }else{
                    drawWire(currentIo.gameObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    currentIoTarget == null || currentIoTarget == currentIo ? wireNotConnectedColor : wireConnectedColor);
                }   
            }

            if(currentIo.moving){
                if(Input.GetMouseButtonUp(1)){
                    currentIo.moving = false;
                }else{
                    Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    currentIo.gameObject.transform.SetPositionAndRotation(cameraPos,Quaternion.identity);
                }
                
            }
        }
    }

    public void startWiring(){
        wire = new GameObject();
        wire.transform.position = Vector3.ProjectOnPlane(currentIo.gameObject.transform.position,Vector3.forward);
        wire.AddComponent<LineRenderer>();

        wireRenderer = wire.GetComponent<LineRenderer>();
        wireRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        wireRenderer.startWidth = 0.1f;
        wireRenderer.endWidth = 0.1f;
        wireRenderer.startColor = wireNotConnectedColor;
        wireRenderer.endColor = wireNotConnectedColor;
    }

    public void drawWire(Vector3 start, Vector3 end, Color color){
        wireRenderer.startColor = color;
        wireRenderer.endColor = color;

        wireRenderer.SetPosition(0, Vector3.ProjectOnPlane(start,Vector3.forward));
        wireRenderer.SetPosition(1, Vector3.ProjectOnPlane(end,Vector3.forward));
    }

    public void showDialog(){
        RectTransform CanvasRect=canvas.GetComponent<RectTransform>();
        RectTransform DialogRect=ioDialog.GetComponent<RectTransform>();
 
        Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(currentIo.gameObject.transform.position);
        Vector2 pos =  new Vector2(
        ((ViewportPosition.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
        ((ViewportPosition.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)) + 100);
        DialogRect.anchoredPosition = pos;

        ioDialogNameInput.SetTextWithoutNotify(currentIo.ioName);
        ioDialogValue.SetValueWithoutNotify(currentIo.value ? 1.0f : 0.0f);
        ioDialog.SetActive(true);
    }

    public void onOkButtonClicked(){
        currentIo.ioName = ioDialogNameInput.text;
        currentIo.value = ioDialogValue.value == 0.0f ? false : true;
        ioDialog.SetActive(false);
    }
}
