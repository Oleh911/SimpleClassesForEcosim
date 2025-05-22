using Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct CreatureAction
{
    public int CreatureId;
    public ActionNeuronType Action;
    public float Speed;
    public MemoryNeuron? Memory;

    public CreatureAction(int creatureId, ActionNeuronType action, float speed, MemoryNeuron? memory = null)
    {
        CreatureId = creatureId;
        Action = action;
        Speed = speed;
        Memory = memory;
    }
}


