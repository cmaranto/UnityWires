using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IoScript : MonoBehaviour
{
    private bool pickedUp = false;
    private bool wiring = false;

    private GameObject wire;
    private LineRenderer wireRenderer;
    public Color wireNotConnectedColor = Color.red;
    public Color wireConnectedColor = Color.green;
    private Color wireColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pickedUp)
        {
            Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.SetPositionAndRotation(
                cameraPos,
                Quaternion.identity
            );
            if (Input.GetMouseButtonUp(1)) pickedUp = false;
        }

        if (wiring)
        {
            drawWire(gameObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition),
                IoManager.instance.currentIoTarget == null ? wireNotConnectedColor : wireConnectedColor);
            if (Input.GetMouseButtonUp(0)){
                wiring = false;
                GameObject.Destroy(wire);
            }
        }

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !wiring) onStartWiring();
        if (Input.GetMouseButtonDown(1) && !pickedUp) pickedUp = true;
        
    }

    void onStartWiring()
    {
        wire = new GameObject();
        wire.transform.position = Vector3.ProjectOnPlane(gameObject.transform.position,Vector3.forward);
        wire.AddComponent<LineRenderer>();

        wireRenderer = wire.GetComponent<LineRenderer>();
        wireRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        wireRenderer.startWidth = 0.1f;
        wireRenderer.endWidth = 0.1f;
        wireRenderer.startColor = wireNotConnectedColor;
        wireRenderer.endColor = wireNotConnectedColor;
        wiring = true;
    }

    void drawWire(Vector3 start, Vector3 end, Color color)
    {
        wireRenderer.startColor = color;
        wireRenderer.endColor = color;

        wireRenderer.SetPosition(0, Vector3.ProjectOnPlane(start,Vector3.forward));
        wireRenderer.SetPosition(1, Vector3.ProjectOnPlane(end,Vector3.forward));
    }
}
