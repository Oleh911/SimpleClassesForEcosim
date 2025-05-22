using System.Security.Claims;

namespace Neurons;

public enum NeuronType : byte
{
    Sensor,
    Memory,
    Inner,
    Action
}

public enum SensorNeuronType: byte
{
    FeelFood,
    FeelObstacle,
    FoodInRadius,
    BiomassLevel,
    EnergyLevel,
    SeeCreatureAhead,
    Age,
    SmellFeromon,
    Distance,
    GotAttacked,
    CreatureHealth,
    TargetHealth,
    TargetEnergy
    // ets...
}

public enum InnerNeuronType: byte
{
    Comparator,
    CounterNeuron,
    BasicRelay,
    ThresholdGate,
    Inverter,
    Oscillator,
    AND,
    XOR,
    OR,
    NOT,
    Delay
    // ����� ������ "��������� �������", "����������� �������", 
    // ets...
}

public enum ActionNeuronType: byte
{
    Stay,
    Move,
    DropFeromon,
    AttackByMouth,
    AttacByFlagella,
    AttacByAll,// ������� ����� ����㳿 �� 䳿 �� ��������
    Eat,
    Mate,
    GiveBirth,
    DepositEnergy
    // ets...
}

public enum MemoryNeuronType: byte
{
    Float,
    Number,
    Pheromone,
    Direction,
    Coordinates,
    Creature
    // ets...
}