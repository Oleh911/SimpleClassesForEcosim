using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Creatures;

namespace Neurons;

public abstract class MemoryNeuron
{
    public readonly short Id;

    public readonly MemoryNeuronType Subtype;

    public readonly byte MaxConnections;

    public abstract object StoredData { get;}

    public MemoryNeuron(short id, MemoryNeuronType subtype, byte maxConnections)
    {
        Id = id;
        Subtype = subtype;
        MaxConnections = maxConnections;
    }

    public abstract float Compare(object data);
}

class FloatMemoryNeuron : MemoryNeuron
{
    public override object StoredData 
    { 
        get => (float)_storedData;
    }

    private readonly float _storedData;

    public FloatMemoryNeuron(short id, byte maxConnections, float storedData):
        base(id, MemoryNeuronType.Float, maxConnections)
    {
        _storedData = storedData;
    }

    public override float Compare(object data)
    {
        if (data is float f)
        {
            float a = Math.Abs(_storedData);
            float b = Math.Abs(f);
           
            if (a == b)
            {
                return 2;
            }
            float max = Math.Max(a, b);
            
            return (1f - Math.Abs(a - b)/max) * 2f;
        }

        return 0f;
    }
}

class NumberMemoryNeuron : MemoryNeuron
{
    public override object StoredData => (short)_storedData;

    private readonly short _storedData;

    public NumberMemoryNeuron(short id, byte maxConnections, short storedData) :
        base(id, MemoryNeuronType.Number, maxConnections)
    {
        _storedData = storedData;
    }

    public override float Compare(object data)
    {
        if (data is short f)
        {
            float a = Math.Abs(_storedData);
            float b = Math.Abs(f);

            if (a == b)
            {
                return 2;
            }
            float max = Math.Max(a, b);

            return (1f - Math.Abs(a - b) / max) * 2f;
        }

        return 0f;
    }
}

class PheromoneMemoryNeuron : MemoryNeuron
{
    public override object StoredData => (short)_storedData;

    private readonly byte _storedData;

    public PheromoneMemoryNeuron(short id, byte maxConnections, byte storedData) :
        base(id, MemoryNeuronType.Number, maxConnections)
    {
        _storedData = storedData;
    }

    public override float Compare(object data)
    {
        if (data is byte f)
        {
            float a = Math.Abs(_storedData);
            float b = Math.Abs(f);

            if (a == b)
            {
                return 2;
            }
            float max = Math.Max(a, b);

            return (1f - Math.Abs(a - b) / max) * 2f;
        }

        return 0f;
    }
}

class DirectionMemoryNeuron : MemoryNeuron
{
    public override object StoredData => (short)_storedData;

    private readonly byte _storedData;

    public DirectionMemoryNeuron(short id, byte maxConnections, byte storedData) :
        base(id, MemoryNeuronType.Number, maxConnections)
    {
        _storedData = storedData;
    }

    public override float Compare(object data)
    {
        if (data is byte f)
        {
            float a = Math.Abs(_storedData);
            float b = Math.Abs(f);

            if (a == b)
            {
                return 2;
            }
            float max = Math.Max(a, b);

            return (1f - Math.Abs(a - b) / max) * 2f;
        }

        return 0f;
    }
}

class CoordinatesMemoryNeuron : MemoryNeuron
{
    public override object StoredData => (Coordinates)_storedData;

    private readonly Coordinates _storedData;

    public CoordinatesMemoryNeuron(short id, byte maxConnections, Coordinates storedData) :
        base(id, MemoryNeuronType.Number, maxConnections)
    {
        _storedData = storedData;
    }

    public override float Compare(object data)
    {
        if (data is Coordinates f)
        {
            Coordinates a = _storedData;
            Coordinates b = f;

            if (a.DistanceTo(b)==0)
            {
                return 2;
            }

            float max = Math.Max(Math.Abs(a.X), Math.Abs(b.X)) + Math.Max(Math.Abs(a.Y), Math.Abs(b.Y));

            return (1f - MathF.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)) / max) * 2f;
        }

        return 0f;
    }
}

class CreatureMemoryNeuron : MemoryNeuron
{
    public override object StoredData => (Coordinates)_storedData;

    private readonly Coordinates _storedData;

    public CreatureMemoryNeuron(short id, byte maxConnections, Coordinates storedData) :
        base(id, MemoryNeuronType.Number, maxConnections)
    {
        _storedData = storedData;
    }
    // todo придумати як імплементувати порівняння істот;
    public override float Compare(object data)
    {
        if (data is Creature f)
        {
            return 0f;
        }

        return 0f;
    }
}