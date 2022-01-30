using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Text;
using System.Linq;
using UnityEngine;

public class IntBits
{
    bool[] m_bits;
    int m_count;
    public int count
    {
        get { return m_count; }
    }
    public bool[] bits
    {
        get { return m_bits; }
    }

    public bool this[int i]
    {
        get { return m_bits[i]; }
        set { m_bits[i] = value; }
    }

    int valueFromBits()
    {
        return Convert.ToInt32(ToString(),2);
    }

    void bitsFromValue(int value)
    {
        bool[] n = Convert.ToString(value, 2).PadLeft(count,'0').Select(s => s.Equals('1')).ToArray();
        for (int i = 0; i < count; ++i)
        {
            if (i >= n.Length)
            {
                m_bits[i] = false;
            }
            else
            {
                m_bits[i] = n[i];
            }
        }
    }

    public int value
    {
        get { return valueFromBits(); }
        set { bitsFromValue(value); }
    }

    public IntBits(int _count, int _value = 0)
    {
        m_count = _count;
        m_bits = new bool[_count];
        bitsFromValue(_value);
    }

    public IntBits(bool[] _bits)
    {
        m_bits = _bits;
        m_count = m_bits.Length;
    }

    public override string ToString()
    {
        string s = "";
        for(int i = 0; i < count; ++i){
            s += bits[i] ? "1" : "0";
        }
        return s;
    }



}
[Serializable]
public class TruthTable
{
    public List<string> inputNames;
    public List<string> outputNames;

    public int outputCount
    {
        get { return outputNames.Count; }
    }
    public int inputCount
    {
        get { return inputNames.Count; }
    }

    public int outputMax
    {
        get { return (int)Math.Pow(2, outputCount); }
    }

    public int inputMax
    {
        get { return (int)Math.Pow(2, inputCount); }
    }


    public List<int> outputMap;

    public TruthTable(List<string> _inputNames, List<string> _outputNames)
    {
        inputNames = _inputNames;
        outputNames = _outputNames;
        outputMap = new List<int>(inputMax);
        for (int i = 0; i < inputMax; i++)
        {
            outputMap.Add(0);
        }
    }

    public bool map(int input, int output)
    {
        if (input < 0 || input >= inputMax)
        {
            return false;
        }
        if (output < 0 || output >= outputMax)
        {
            return false;
        }

        outputMap[input] = output;
        return true;
    }

    public bool map(IntBits inputs, IntBits outputs)
    {        
        return map(inputs.value, outputs.value);
    }

    public int output(int input)
    {
        return outputMap[input];
    }

    public IntBits output(IntBits inputs)
    {
        return new IntBits(outputCount, output(inputs.value));
    }

    public static int boolToInt(bool val)
    {
        return val ? 1 : 0;
    }

    public static TruthTable createNand(){
        TruthTable tt = new TruthTable(new List<string>{"A","B"},new List<string>{"Output"});
        tt.map(0,1);
        tt.map(1,1);
        tt.map(2,1);
        tt.map(3,0);
        return tt;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        String cellFormat = "{0,-5}";
        //header   
        for (int i = 0; i < inputCount + outputCount; ++i)
        {
            sb.AppendFormat(cellFormat, i < inputCount ? inputNames[i] : outputNames[inputCount - i]);
        }
        sb.Append("\n");
        sb.Append("----------\n");
        //values
        for (int y = 0; y < inputMax; ++y)
        {
            IntBits inputBits = new IntBits(inputCount, y);
            IntBits outputBits = new IntBits(outputCount, output(inputBits.value));
            for (int i = 0; i < inputCount + outputCount; ++i)
            {
                sb.AppendFormat(cellFormat, i < inputCount ? boolToInt(inputBits[i])
                    : boolToInt(outputBits[inputCount - i]));
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }


}
