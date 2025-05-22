using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Pheromone
{
    public readonly Coordinates Position;
    public float Strengh { get; private set; }
    public readonly byte Type;

    public Pheromone(Coordinates position, float strengh, byte type = 0)
    {
        Position = position;
        Strengh = strengh;
        Type = type;
    }

    public bool Evaporate(float amount)
    {
        Strengh -= amount;
        return Strengh > 0;
    }
}

