using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IoModule : MonoBehaviour
{
    private string m_ioName;
    public TMP_Text nameText;
    public GameObject moduleBase;
    
    public string ioName{
        get{return m_ioName;}
        set{
            m_ioName=value;
            gameObject.name = value;
            nameText.SetText(value);
        }
    }

    private TruthTable m_truthTable;
    public TruthTable truthTable{
        get{return m_truthTable;}
        set{m_truthTable = value;}
    }

    private int m_inputCount = 1;
    public int inputCount{
        get{return m_inputCount;}
        set{m_inputCount = value;}
    }
    private int m_outputCount = 1;
    public int outputCount{
        get{return m_outputCount;}
        set{m_outputCount = value;}
    }
    private List<Io> m_inputs;
    private List<Io> m_outputs;


    // Start is called before the first frame update

    public void init(int _inputCount, int _outputCount){
        inputCount = _inputCount;
        outputCount = _outputCount;
        m_truthTable = new TruthTable(inputCount,outputCount);
        m_inputs = new List<Io>(inputCount);
        m_outputs = new List<Io>(outputCount);
    }



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
