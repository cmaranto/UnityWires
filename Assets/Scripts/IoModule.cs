using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IoModuleData
{

    public IoModuleData(string name, TruthTable tt){
        m_ioName = name;
        m_truthTable = tt;
    }
    private string m_ioName;
    public string ioName
    {
        get { return m_ioName; }
    }
    private TruthTable m_truthTable;
    public TruthTable truthTable
    {
        get { return m_truthTable; }
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

    private IoModuleData m_module;

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

    public void init(IoModuleData moduleData)
    {
        m_module = moduleData;
        m_inputs = new List<Io>(m_module.truthTable.inputCount);
        m_outputs = new List<Io>(m_module.truthTable.outputCount);
        nameText.SetText(m_module.ioName);     

        for (int i = 0; i < m_module.truthTable.outputCount; i++)
        {
            createOutput(m_module.truthTable.outputNames[i]);
        }
        for (int i = 0; i < m_module.truthTable.inputCount; i++)
        {
            createInput(m_module.truthTable.inputNames[i]);
        }
    }

    public void load(string filename)
    {

    }

    public void createInput(string ioName = "Input")
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io input = go.GetComponentInChildren<Io>();
        input.allowInput = true;
        input.allowOutput = false;
        input.ioName = ioName;
        m_inputs.Add(input);
        align();
    }

    public void createOutput(string ioName = "Output")
    {
        GameObject go = Instantiate(ioPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = gameObject.transform;
        Io output = go.GetComponentInChildren<Io>();
        output.allowInput = false;
        output.allowOutput = true;
        output.ioName = ioName;
        m_outputs.Add(output);
        align(false);
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
}
