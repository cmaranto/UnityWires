using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Text;
using System;

[Serializable]
public class IoModuleData
{

    public IoModuleData(string name, TruthTable tt){
        ioName = name;
        truthTable = tt;
    }
    public string ioName;
    public TruthTable truthTable;

    public override string ToString()
    {
        return string.Format("{0}\n{1}",ioName,truthTable);
    }
}
public class IoModule : MonoBehaviour
{

    public TMP_Text nameText;
    public GameObject moduleBase;
    public GameObject ioPrefab;

    private List<Io> m_inputs;
    public ReadOnlyCollection<Io> inputs{
        get{return m_inputs.AsReadOnly();}
    }
    private List<Io> m_outputs;
    public ReadOnlyCollection<Io> outputs{
        get{return m_outputs.AsReadOnly();}
    }

    public UnityEvent outputEvent = new UnityEvent();

    private IoModuleData m_moduleData;
    public IoModuleData moduleData{
        get{return m_moduleData;}
    }

    // Start is called before the first frame update

    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnMouseDrag(){
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        gameObject.transform.SetPositionAndRotation(pos,Quaternion.identity);
        IoManager.instance.updateWires();
    }

    void OnMouseOver(){
        if(Input.GetMouseButtonDown(1)){
            OnRightClick();
        }
    }

    void OnRightClick(){
        IoManager.instance.showModuleDialog(this);
    }
    
    public void init(IoModuleData moduleData)
    {
        m_moduleData = moduleData;
        m_inputs = new List<Io>(m_moduleData.truthTable.inputCount);
        m_outputs = new List<Io>(m_moduleData.truthTable.outputCount);
        nameText.SetText(m_moduleData.ioName);     

        for (int i = 0; i < m_moduleData.truthTable.outputCount; i++)
        {
            createOutput(m_moduleData.truthTable.outputNames[i]);
        }
        for (int i = 0; i < m_moduleData.truthTable.inputCount; i++)
        {
            createInput(m_moduleData.truthTable.inputNames[i]);
        }
        setOutputs();
    }

    void setOutputs(){
        bool[] inputValues = new bool[m_moduleData.truthTable.inputCount];
        for(int i = 0; i < m_moduleData.truthTable.inputCount; ++i){
            inputValues[i] = m_inputs[i].value;
        }
        IntBits inputBits = new IntBits(inputValues);
        IntBits outputBits = new IntBits(m_moduleData.truthTable.output(inputBits).bits);
        for(int i = 0; i < m_moduleData.truthTable.outputCount; ++i){
            m_outputs[i].value = outputBits[m_moduleData.truthTable.outputCount - i - 1];
        }
        outputEvent.Invoke();
    }

    void onInputChanged(){
        setOutputs();
    }

    public void load(string filename)
    {

    }

    public void createInput(string ioName = "Input")
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io input = go.GetComponentInChildren<Io>();
        input.type = Io.Type.Input;
        input.ioName = ioName;
        input.valueChangedEvent.AddListener(onInputChanged);
        m_inputs.Add(input);
        align();
    }

    public void createOutput(string ioName = "Output")
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io output = go.GetComponentInChildren<Io>();
        output.type = Io.Type.Output;
        output.ioName = ioName;
        m_outputs.Add(output);
        align(false);
    }

    public void removeConnections(){
        foreach(Io io in m_inputs){
            if(io.incomingConnection){
                io.incomingConnection.removeOutgoingConnection(io);
            }
        }
        foreach(Io io in m_outputs){
            io.clearOutgoingConnections();
        }
    }

    void align(bool left = true)
    {
        Bounds baseBounds = gameObject.GetComponentInChildren<SpriteRenderer>().bounds;
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

    public string ioTableString(){
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0,-10}{1,10}\n","In","Out");
        sb.Append("--------------------\n");
        string cellFormat = "{0,-10}{1,10}\n"; 
        for(int i = 0; i < Math.Max(moduleData.truthTable.inputCount,moduleData.truthTable.outputCount); ++i){            
            sb.AppendFormat(cellFormat,i < moduleData.truthTable.inputCount ? inputs[i].ToString() : "          ",
                                        i < moduleData.truthTable.outputCount ? outputs[i].ToString() : "          ");
        }

        return sb.ToString();
    }
}
