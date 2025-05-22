using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurons;

public class Synapse
{
    public readonly ushort SourceId;// Отримує сигнали від
    public readonly ushort TargetId;// Відправляє сигнали до
    public float Weight { get; private set; }// Накопичений сигнал
    public readonly float Threshold;// Поріг за якого сигнал не передається в нейрони
    public readonly float Length;// Значення (0;1] Перед передачею сигналу значення множиться на 1-Length

    public Synapse(ushort sourceId, ushort targetId, float weight, float threshold, float length)
    {
        SourceId = sourceId;
        TargetId = targetId;
        Weight = weight;
        Threshold = threshold;
        Length = length;
    }

    // Метод для передачі сигналу від нейрона до синапса
    public void UpdateWeight(float signal) 
    {
        Weight += signal;
    }

    // Метод для передачі сигналу від синапса до нейрону
    public float Pop()
    {
        if (Weight < Threshold)
        {
            return 0;
        }

        float result = Weight * (1-Math.Abs(Length));
        Weight = 0;

        return result;
    }
}