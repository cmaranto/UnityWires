using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
public class TruthTable
{
    private int m_inputCount = 0;
    public int inputCount{
        get{return m_inputCount;}
    }

    private int m_outputCount = 0;
    public int outputCount{
        get{return m_outputCount;}
    }

    public int outputMax{
        get{return (int)Math.Pow(2,outputCount);}
    }

    public int inputMax{
        get{return (int)Math.Pow(2,inputCount);}
    }

    
    private Dictionary<int,int> m_map = new Dictionary<int, int>();

    public TruthTable(int inputs, int outputs){
        m_inputCount = inputs;
        m_outputCount = outputs;
        for(int i = 0; i < inputMax; i++){
            m_map[i] = 0;
        }
    }

    public bool map(int input, int output){
        if(input < 0 || input >= inputMax){
            return false;
        }
        if(output < 0 || input >= outputMax){
            return false;
        }

        m_map[input] = output;
        return true;
    }

    public bool map(BitVector32 input, BitVector32 output){
        return map(input.Data,output.Data);
    }

    int output(int input){
        return m_map[input];
    }

    BitVector32 output(BitVector32 input){
        return new BitVector32(output(input.Data));
    }
}
