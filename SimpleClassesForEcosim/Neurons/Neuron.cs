using System;

namespace Neurons;

public readonly struct Neuron
{
    public readonly ushort Id;
    public readonly NeuronType Type;
    public readonly byte SubType;
    public readonly byte MaxInputs;
    public readonly byte MaxOutputs;
    public readonly short? MemoryNeuronId;
    public readonly float SignalOutput;

    public Neuron
        (ushort id, NeuronType type, byte subType, byte maxInputs, byte maxOutputs, short? memoryNeuronId, float signalOutput)
    {
        Id = id;
        Type = type;
        SubType = subType;
        MaxInputs = maxInputs;
        MaxOutputs = maxOutputs;
        SignalOutput = Math.Abs(signalOutput);
            
        if(memoryNeuronId != null)
        {
            MemoryNeuronId = memoryNeuronId;
        }
    }
}
