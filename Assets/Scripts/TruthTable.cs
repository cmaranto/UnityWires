using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Text;
using UnityEngine;
public class TruthTable
{
    private List<string> m_inputNames;
    public ReadOnlyCollection<string> inputNames{
        get{return m_inputNames.AsReadOnly();}
    }
    private List<string> m_outputNames;
    public ReadOnlyCollection<string> outputNames{
        get{return m_outputNames.AsReadOnly();}
    }

    public int outputCount{
        get{return m_outputNames.Count;}
    }
    public int inputCount{
        get{return m_inputNames.Count;}
    }

    public int outputMax{
        get{return (int)Math.Pow(2,outputCount);}
    }

    public int inputMax{
        get{return (int)Math.Pow(2,inputCount);}
    }

    
    private Dictionary<int,int> m_map = new Dictionary<int, int>();
    

    public TruthTable(List<string> inputNames, List<string> outputNames){
        m_inputNames = inputNames;
        m_outputNames = outputNames;
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

    public bool map(BitArray input, BitArray output){
        return map(bitsToInt(input),bitsToInt(output));
    }

    public int output(int input){
        return m_map[input];
    }

    public BitArray output(BitArray input){
        return intToBits(output(bitsToInt(input)));
    }

    public static int bitsToInt(BitArray bits){
        int [] array = new int[1];
        bits.CopyTo(array,0);
        return array[0];
    }

    public static BitArray intToBits(int val){
        return new BitArray(new int[1]{val});
    }

    public static int boolToInt(bool val){
        return val ? 1 : 0;
    }

    public static string ToBitString(BitArray bits)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < bits.Count; i++)
        {
            char c = bits[i] ? '1' : '0';
            sb.Append(c);
        }

        return sb.ToString();
    }

    public override string ToString(){
        StringBuilder sb = new StringBuilder();
        String cellFormat = "{0,-5}";
        //header   
        for(int i = 0; i < inputCount + outputCount; ++i){            
            sb.AppendFormat(cellFormat, i < inputCount ? inputNames[i] : outputNames[inputCount - i]);
        }
        sb.Append("\n");
        //values
        for(int y = 0; y < inputMax; ++y){
            BitArray inputBits = intToBits(y);
            BitArray outputBits = output(inputBits);
            for(int i = 0; i < inputCount + outputCount; ++i){
                int iIdx = inputCount - i - 1;
                int oIdx = outputCount - (i - inputCount) - 1;

                sb.AppendFormat(cellFormat, i < inputCount ? boolToInt(inputBits[iIdx])
                    : boolToInt(outputBits[oIdx]));
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }


}
