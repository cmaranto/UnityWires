using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IoManager : MonoBehaviour
{
    public GameObject ioPrefab;
    public GameObject moduleBase;
    public GameObject ioDialogPrefab;
    public GameObject modulePrefab;
    public GameObject moduleDialogPrefab;
    public GameObject nameDialogPrefab;
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
    private List<IoModuleData> m_moduleData = new List<IoModuleData>();
    private Dictionary<GameObject,GameObject> m_dialogs = new Dictionary<GameObject,GameObject>();

    public Button addButton;
    public TMP_Dropdown addOption;
    public Button saveButton;
    public Button clearButton;
    public bool saving = false;
    public Toggle infoToggle;

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
        loadModules();
        
        addButton.onClick.AddListener(onAddClick);
        saveButton.onClick.AddListener(onSaveClick);
        clearButton.onClick.AddListener(clear);
        infoToggle.onValueChanged.AddListener(onInfoToggle);

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

    void addModuleData(IoModuleData _data){
        m_moduleData.Add(_data);
        addOption.AddOptions(new List<string>{_data.ioName});
        saveModules();
    }

    void loadModules(){
        string modFilename = Application.persistentDataPath + "/modules.json";
        if(File.Exists(modFilename)){
            IoModuleDataList l = JsonUtility.FromJson<IoModuleDataList>(File.ReadAllText(modFilename));
            foreach(IoModuleData d in l.imdList){
                addModuleData(d);
            }
        }else{
            //load defaults
            addModuleData(new IoModuleData("NAND",TruthTable.createNand()));     
        }
    }

    void saveModules(){
        string modFilename = Application.persistentDataPath + "/modules.json";
        File.WriteAllText(modFilename,JsonUtility.ToJson(new IoModuleDataList(m_moduleData),true));
    }

    void onInfoToggle(bool on){

    }

    void onAddClick(){
        switch(addOption.value){
            case 0://input
                createInput();
                break;
            case 1://output
                createOutput();
                break;
            default://modules
                createModule(addOption.value - 2);
                break;
        }
    }

    void onSaveClick(){
        
        showNameDialog();
    }

    public void createInput()
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io input = go.GetComponent<Io>();
        input.type = Io.Type.StaticInput;
        input.ioName = "Input";
        input.valueChangedEvent.AddListener(onInputChanged);
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
    }

    void onInputChanged(){
        if(!saving)updateWires();
    }

    public void createModule(int idx)
    {
        if(m_moduleData.Count > idx){
            IoModule module;
            GameObject go;
            go = Instantiate(modulePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            module = go.GetComponent<IoModule>();
            module.init(m_moduleData[idx]);
            module.outputEvent.AddListener(updateWires);
            m_modules.Add(module);
        }
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
        start.z = 1;
        stop.z =1;
        wR.SetPosition(0, start);
        wR.SetPosition(1, stop);
        return w;
    }

    public void stopWiring()
    {
        wiring = false;
        GameObject.Destroy(wire);

        if (currentIoTarget){
            currentIoTarget.incomingConnection = currentIo;
            currentIo.addOutgoingConnection(currentIoTarget);
        }

        currentIo = null;
        updateWires();
    }

    void updateNewWire(Vector3 start, Vector3 end, Color color)
    {
        wireRenderer.startColor = color;
        wireRenderer.endColor = color;

        start.z = 1;
        end.z = 1;
        wireRenderer.SetPosition(0, start);
        wireRenderer.SetPosition(1, end);
    }

    public void updateWires()
    {

        clearWires();
        foreach (Io input in m_inputs)
        {
            foreach(Io o in input.outgoingConnections){
                wires.Add(createWire(input.gameObject.transform.position,
                    o.gameObject.transform.position,
                    input.value ? wireOnColor : wireOffColor));
            }
        }

        foreach (IoModule module in m_modules)
        {
            foreach (Io output in module.outputs)
            {
                foreach(Io o in output.outgoingConnections){
                    wires.Add(createWire(output.gameObject.transform.position,
                        o.gameObject.transform.position,
                        output.value ? wireOnColor : wireOffColor));
                }
            }
        }
    }

    public void clearWires(){
        foreach (GameObject obj in wires)
        {
            GameObject.Destroy(obj);
        }
        wires.Clear();
    }

    public void clear(){
        foreach(Io io in m_inputs){
            GameObject.Destroy(io.gameObject);
        }
        m_inputs.Clear();
        foreach(Io io in m_outputs){
            GameObject.Destroy(io.gameObject);
        }
        m_outputs.Clear();
        foreach(IoModule mod in m_modules){
            GameObject.Destroy(mod.gameObject);
        }
        m_modules.Clear();
        clearWires();
    }

    void removeDialog(GameObject obj){
        if(m_dialogs.ContainsKey(obj)){
            GameObject.Destroy(m_dialogs[obj]);
            m_dialogs.Remove(obj);
        }
    }

    void clearDialogs(){
        foreach(KeyValuePair<GameObject,GameObject> kvp in m_dialogs){
            GameObject.Destroy(kvp.Value);
        }
        m_dialogs.Clear();
    }

    public void showIoDialog(Io io)
    {
        if(m_dialogs.ContainsKey(io.gameObject))return;
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
        manager.closeEvent.AddListener(() => {
            removeDialog(io.gameObject);
        });
        m_dialogs.Add(io.gameObject,dialog);
    }

    public void showNameDialog(){
        GameObject dialog = Instantiate(nameDialogPrefab, Vector3.zero, Quaternion.identity);
        dialog.transform.SetParent(canvas.transform, false);
    }

    public void saveModule(string name){
        saving = true;
        List<string> inputNames = new List<string>(m_inputs.Count);
        foreach(Io input in m_inputs){
            inputNames.Add(input.ioName);
        }
        List<string> outputNames = new List<string>(m_outputs.Count);
        foreach(Io output in m_outputs){
            outputNames.Add(output.ioName);
        }
        TruthTable tt = new TruthTable(inputNames,outputNames);
        IntBits ins = new IntBits(m_inputs.Count);
        IntBits outs = new IntBits(m_outputs.Count);
        for(int i = 0; i < tt.inputMax; ++i){
            ins.value = i;
            for(int y = 0; y < ins.count; ++y){
                m_inputs[y].value = ins[y];
            }
            for(int z = 0; z < outs.count; ++z){
                outs[z] = m_outputs[z].value;
            }
            tt.map(ins,outs);
        }


        //get module name here
        IoModuleData data = new IoModuleData(name,tt);
        addModuleData(data);

        saving = false;
        clear();
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
        manager.closeEvent.AddListener(() => {
            removeDialog(module.gameObject);
        });
        m_dialogs.Add(module.gameObject,dialog);
    }

    public void removeIo(Io io){
        if(io.type == Io.Type.StaticInput){
            io.clearOutgoingConnections();
            m_inputs.Remove(io);
            GameObject.Destroy(io.gameObject);
        }else if(io.type == Io.Type.StaticOutput){
            if(io.incomingConnection)io.incomingConnection.removeOutgoingConnection(io);
            m_outputs.Remove(io);
            GameObject.Destroy(io.gameObject);
        }
        updateWires();
    }

    public void removeModule(IoModule module){
        module.removeConnections();
        m_modules.Remove(module);
        GameObject.Destroy(module.gameObject);
        updateWires();
    }
}
