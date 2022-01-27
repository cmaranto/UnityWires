using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class IoManager : MonoBehaviour
{
    public GameObject ioPrefab;
    public GameObject moduleBase;
    public GameObject ioDialogPrefab;
    public GameObject modulePrefab;
    public GameObject moduleDialogPrefab;
    public Canvas canvas;
    private static IoManager m_instance;
    private static List<Io> m_inputs = new List<Io>();
    private static List<Io> m_outputs = new List<Io>();

    public Io currentIoTarget = null;
    public Io currentIo = null;

    public bool wiring = false;
    public Color wireConnectedColor = Color.green;
    public Color wireNotConnectedColor = Color.red;
    public Color wireOnColor = Color.white;
    public Color wireOffColor = Color.gray;
    private GameObject wire = null;
    private List<GameObject> wires = new List<GameObject>();
    private LineRenderer wireRenderer = null;

    private List<IoModule> m_modules = new List<IoModule>();

    public static IoManager instance
    {
        get
        {
            return m_instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;

        createInput();
        createOutput();
    }

    // Update is called once per frame 
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (wiring) stopWiring();
        }
        if (wiring)
        {
            updateNewWire(currentIo.gameObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        currentIoTarget ? wireConnectedColor : wireNotConnectedColor);
        }
    }

    public void createInput()
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io input = go.GetComponent<Io>();
        input.type = Io.Type.StaticInput;
        input.ioName = "Input";
        input.valueChangedEvent.AddListener(updateWires);
        m_inputs.Add(input);
        align();
        updateWires();
    }

    public void createOutput()
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io output = go.GetComponent<Io>();
        output.type = Io.Type.StaticOutput;
        output.ioName = "Output";
        m_outputs.Add(output);
        align(false);
        updateWires();
    }

    public void createModule(int idx)
    {
        IoModule module;
        GameObject go;
        if (idx == 0)
        {
            //default on relay
            go = Instantiate(modulePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            module = go.GetComponent<IoModule>();
            TruthTable tt = new TruthTable(new List<string> { "A", "B" }, new List<string> { "O" });
            tt.map(0, 0);
            tt.map(1, 1);
            tt.map(2, 0);
            tt.map(3, 0);
            module.init(new IoModuleData("Relay(on)", tt));
            module.outputEvent.AddListener(updateWires);
        }
        else
        {
            //default off relay
            go = Instantiate(modulePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            module = go.GetComponent<IoModule>();
            TruthTable tt = new TruthTable(new List<string> { "A", "B" }, new List<string> { "O" });
            tt.map(0, 0);
            tt.map(1, 0);
            tt.map(2, 0);
            tt.map(3, 1);
            module.init(new IoModuleData("Relay(off)", tt));
            module.outputEvent.AddListener(updateWires);
        }
        m_modules.Add(module);
    }

    void align(bool left = true)
    {
        Bounds baseBounds = moduleBase.GetComponentInChildren<SpriteRenderer>().bounds;
        float xPos = moduleBase.transform.position.x - baseBounds.size.x / 2.0f;
        if (!left) xPos = Mathf.Abs(xPos);
        float count = left ? (float)m_inputs.Count + 1 : (float)m_outputs.Count + 1;
        float yInterval = baseBounds.size.y / count;
        int idx = 1;
        foreach (Io io in (left ? m_inputs : m_outputs))
        {
            Vector3 pos = new Vector3(xPos, baseBounds.size.y / 2 - yInterval * idx++, 0);
            io.gameObject.transform.SetPositionAndRotation(pos, Quaternion.identity);
        }
    }

    public void startWiring()
    {
        if (currentIo.connection)
        {
            currentIo.connection.connection = null;
            currentIo.connection = null;
        }

        wiring = true;
        wire = createWire(Vector3.zero, Vector3.zero, wireNotConnectedColor);
        wireRenderer = wire.GetComponentInChildren<LineRenderer>();
    }

    private GameObject createWire(Vector3 start, Vector3 stop, Color color)
    {
        GameObject w = new GameObject();
        w.transform.parent = gameObject.transform;
        LineRenderer wR = w.AddComponent<LineRenderer>();
        wR.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        wR.startWidth = 0.1f;
        wR.endWidth = 0.1f;
        wR.startColor = color;
        wR.endColor = color;
        wR.SetPosition(0, start);
        wR.SetPosition(1, stop);
        return w;
    }

    public void stopWiring()
    {
        wiring = false;
        GameObject.Destroy(wire);

        if (currentIoTarget) currentIoTarget.connection = currentIo;
        currentIo.connection = currentIoTarget;

        currentIo = null;
        updateWires();
    }

    void updateNewWire(Vector3 start, Vector3 end, Color color)
    {
        wireRenderer.startColor = color;
        wireRenderer.endColor = color;

        wireRenderer.SetPosition(0, Vector3.ProjectOnPlane(start, Vector3.forward));
        wireRenderer.SetPosition(1, Vector3.ProjectOnPlane(end, Vector3.forward));
    }

    public void updateWires()
    {
        foreach (GameObject obj in wires)
        {
            GameObject.Destroy(obj);
        }
        wires.Clear();

        foreach (Io input in m_inputs)
        {
            if (input.connection)
            {
                wires.Add(createWire(input.gameObject.transform.position,
                                        input.connection.gameObject.transform.position,
                                        input.value ? wireOnColor : wireOffColor));
            }
        }

        foreach (IoModule module in m_modules)
        {
            foreach (Io output in module.outputs)
            {
                if (output.connection)
                {
                    wires.Add(createWire(output.gameObject.transform.position,
                            output.connection.gameObject.transform.position,
                            output.value ? wireOnColor : wireOffColor));
                }
            }
        }
    }

    public void showIoDialog(Io io)
    {
        GameObject dialog = Instantiate(ioDialogPrefab, Vector3.zero, Quaternion.identity);
        dialog.transform.SetParent(canvas.transform, false);
        IoDialogManager manager = dialog.GetComponent<IoDialogManager>();
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();
        RectTransform DialogRect = dialog.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(io.gameObject.transform.position);
        Vector2 pos = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        DialogRect.anchoredPosition = pos;

        manager.setIo(io);
    }

    public void showModuleDialog(IoModule module)
    {
        GameObject dialog = Instantiate(moduleDialogPrefab, Vector3.zero, Quaternion.identity);
        dialog.transform.SetParent(canvas.transform, false);
        IoModuleDialogManager manager = dialog.GetComponent<IoModuleDialogManager>();
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();
        RectTransform DialogRect = dialog.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(module.gameObject.transform.position);
        Vector2 pos = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        DialogRect.anchoredPosition = pos;

        manager.setModule(module);
    }
}
