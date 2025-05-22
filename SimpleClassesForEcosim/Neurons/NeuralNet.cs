using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Creatures;

namespace Neurons;

public class NeuralNet
{
    private readonly Neuron[] _neurons;
    private readonly Synapse[] _synapses;
    private readonly MemoryNeuron[] _memory;

    public NeuralNet(Neuron[] neurons, Synapse[] synapses, MemoryNeuron[] memory)
    {
        _neurons = neurons;
        _synapses = synapses;
        _memory = memory;
    }

    public CreatureAction BrainCalculateAction(Map map, Creature creature)
    {
        Dictionary<ushort, short> memoryBindings = new();

        foreach (var neuron in _neurons)
        {
            if (neuron.MemoryNeuronId.HasValue)
            {
                memoryBindings[neuron.Id] = neuron.MemoryNeuronId.Value;
            }
        }

        // 2. Обробка Sensor нейронів  
        foreach (Neuron sensor in _neurons.Where(x => x.Type == NeuronType.Sensor))
        {
            float signal = 0;
            if (memoryBindings.TryGetValue(sensor.Id, out var memoryId))
            {
                signal = ProcessSensor(sensor.SubType, memoryId, map, creature);
            }
            else
            {
                signal = ProcessSensor(sensor.SubType, null, map, creature);
            }

            foreach (Synapse synaps in _synapses.Where(x => x.SourceId == sensor.Id))
            {
                synaps.UpdateWeight(signal);
            }
        }

        // 3. Обробка Inner нейронів
        foreach (Neuron inner in _neurons.Where(x => x.Type == NeuronType.Inner))
        {
            float signalIn = 0;
            float signalOut = 0;

            foreach (Synapse synaps in _synapses.Where(x => x.SourceId == inner.Id))
            {
                signalIn += synaps.Pop();
            }

            if (memoryBindings.TryGetValue(inner.Id, out var memoryId))
            {
                signalOut = ProcessInner(signalIn, inner.SubType, memoryId);
            }
            else
            {
                signalOut = ProcessInner(signalIn, inner.SubType, null);
            }

            foreach (Synapse synaps in _synapses.Where(x => x.SourceId == inner.Id))
            {
                synaps.UpdateWeight(signalOut);
            }
        }

        ActionNeuronType bestAction = ActionNeuronType.Stay;
        float maxSignal = float.MinValue;
        MemoryNeuron? selectedMemory = null;

        foreach (var action in _neurons.Where(x => x.Type == NeuronType.Action))
        {
            float signal = 0;

            foreach (Synapse synaps in _synapses.Where(x => x.SourceId == action.Id))
            {
                signal += synaps.Pop();
            }

            if (signal > maxSignal)
            {
                maxSignal = signal;
                bestAction = (ActionNeuronType)action.SubType;

                if (memoryBindings.TryGetValue(action.Id, out var memoryId))
                {
                    selectedMemory = _memory.FirstOrDefault(m => m.Id == memoryId);
                }
                else
                {
                    selectedMemory = null;
                }
            }
        }

        return new CreatureAction(
            creature.Id,
            bestAction,
            creature.Speed,
            selectedMemory
        );
    }


    private float ProcessSensor(byte subType,Neuron sensor, short? memoryNeuronId, Map map, Creatures.Creature creature)
    {
        MemoryNeuron? memoryNeuron = null;

        if(memoryNeuronId != null)
        {
            memoryNeuron = _memory.FirstOrDefault(x => x.Id == memoryNeuronId);
        }

        return (SensorNeuronType)subType switch
        {
            SensorNeuronType.FeelFood => Sensor_FeelFood(sensor, memoryNeuron, map, creature),
            SensorNeuronType.FeelObstacle => Sensor_Obstacle(sensor, memoryNeuron, map, creature),
            SensorNeuronType.BiomassLevel => Sensor_BiomassLevel(sensor, memoryNeuron, creature),
            SensorNeuronType.EnergyLevel => Sensor_EnergyLevel(sensor, memoryNeuron, creature),
            SensorNeuronType.SeeCreatureAhead => Sensor_SeeCreatureAhead(sensor, memoryNeuron, map, creature),
            SensorNeuronType.Age => Sensor_CreatureAge(sensor, memoryNeuron, creature),
            SensorNeuronType.SmellFeromon => Sensor_SmellPheromone(sensor,memoryNeuron, map, creature),
            SensorNeuronType.Distance => Sensor_DistanceToTarget(sensor, creature, memoryNeuron),
            SensorNeuronType.GotAttacked => Sensor_GotAttacked(creature),
            SensorNeuronType.CreatureHealth => Sensor_CreatureHealth(creature),
            SensorNeuronType.TargetHealth => SenseTargetHealth(creature, memoryNeuron),
            SensorNeuronType.TargetEnergy => SenseTargetEnergy(creature, memoryNeuron),
            _ => 0f,
        };
    }


    private float ProcessInner(float signal, byte subType, short? memoryNeuronId)
    {
        return subType switch
        {
            0 => SenseFeelFood(),
            1 => SenseTouchWall(),
            2 => SenseHungerLevel(),
            // ...
            _ => 0f
        };
    }

    #region<SensorMethods>
    private float Sensor_FeelFood(Neuron sensor, MemoryNeuron? memoryNeuron, Map map, Creature creature)
    {
        byte radius = creature.CreatureBody.ChemicalReceptors.Distance;
        float multiplier = sensor.SignalOutput;
        float totalAmount = 0f;
        Coordinates originPos = creature.Position;

        if (memoryNeuron == null)
        {
            totalAmount = map.FoodInRadius(originPos, radius);

            return totalAmount * multiplier;
        }

        switch (memoryNeuron.Subtype)
        {
            case MemoryNeuronType.Direction:
                if ((Directions)memoryNeuron.StoredData is Directions dir && dir != Directions.None)
                {
                    Coordinates targetPos = originPos.OffsetByDirection(dir);
                    Biomaterial food = map.GetFoodByPosition(targetPos);

                    return food.Biomass * multiplier; 
                }

                return 0f;

            case MemoryNeuronType.Number:
                float amount = map.BiomatirealsNumInRadius(originPos, radius);

                return amount * memoryNeuron.Compare(amount);

            case MemoryNeuronType.Coordinates:
                if ((Coordinates)memoryNeuron.StoredData is Coordinates coords &&
                    originPos.DistanceTo(coords) <= radius)
                {
                    float biomass = map.GetFoodByPosition(coords).Biomass;

                    if (biomass > 0)
                    {
                        return biomass * memoryNeuron.Compare(biomass);
                    }
                }

                return 0f;

            case MemoryNeuronType.Float:
                //середнє значення біомаси в радіусі порівнюємо з memory
                float foodAmount = map.FoodInRadius(originPos, radius);
                byte biomaterialNum = map.BiomatirealsNumInRadius(originPos, radius);

                if (biomaterialNum == 0)
                {
                    return 0f;
                }

                float avg = foodAmount / biomaterialNum;

                return avg * memoryNeuron.Compare(avg);

            default:
                return 0f;
        }
    }

    private float Sensor_Obstacle(Neuron sensor, MemoryNeuron? memoryNeuron, Map map, Creature creature)
    {
        byte radius = creature.CreatureBody.ChemicalReceptors.Distance;
        float multiplier = sensor.SignalOutput;
        Coordinates origin = creature.Position;

        if (memoryNeuron == null)
        {
            int count = 0;

            foreach (var offset in Coordinates.GetOffsetsInRadius(radius))
            {
                Coordinates pos = origin + offset;
                if (map.IsObstacleAt(pos))
                {
                    count++;
                }
            }

            return count * multiplier;
        }

        switch (memoryNeuron.Subtype)
        {
            case MemoryNeuronType.Number:
                int obstacleCount = 0;
                foreach (var offset in Coordinates.GetOffsetsInRadius(radius))
                {
                    Coordinates pos = origin + offset;
                    if (map.IsObstacleAt(pos))
                    {
                        obstacleCount++;
                    }
                }

                return multiplier * memoryNeuron.Compare(obstacleCount);

            case MemoryNeuronType.Direction:
                if ((Directions)memoryNeuron.StoredData is Directions dir && dir != Directions.None)
                {
                    Coordinates target = origin.OffsetByDirection(dir);

                    return map.IsObstacleAt(target) ? multiplier * 2f : 0f;
                }

                return 0f;

            default:
                return 0f;
        }
    }

    private float Sensor_BiomassLevel(Neuron sensor, MemoryNeuron? memoryNeuron, Creature creature)
    {
        float currentBiomass = creature.CreatureBody.DigestiveCavity.BiomassAmount;
        float multiplier = sensor.SignalOutput;

        if (memoryNeuron == null)
        {
            return currentBiomass * multiplier;
        }


        if (memoryNeuron.Subtype == MemoryNeuronType.Float && (float)memoryNeuron.StoredData is float storedBiomass)
        {

            return multiplier * memoryNeuron.Compare(currentBiomass);
        }

        return 0f;
    }

    private float Sensor_EnergyLevel(Neuron sensor, MemoryNeuron? memoryNeuron, Creature creature)
    {
        float currentEnergy = creature.Energy;
        float multiplier = sensor.SignalOutput;

        if (memoryNeuron == null)
        {
            return currentEnergy * multiplier;
        }

        if (memoryNeuron.Subtype == MemoryNeuronType.Float && memoryNeuron.StoredData is float storedEnergy)
        {
            return multiplier * memoryNeuron.Compare(currentEnergy);
        }

        return 0f;
    }

    private float Sensor_SeeCreatureAhead(Neuron sensor, MemoryNeuron? memoryNeuron, Map map, Creature creature)
    {
        Directions direction = creature.lastDirection;
        Coordinates originalPosition = creature.Position;
        float multiplier = sensor.SignalOutput;

        if(memoryNeuron == null)
        {
            Coordinates targetPos = originalPosition + originalPosition.OffsetByDirection(direction);

            if (!map.CreatureExist(targetPos))
            {
                return 0f;
            }

            return multiplier;
        }

        switch (memoryNeuron.Subtype)
        {
            case MemoryNeuronType.Direction:
                if ((Directions)memoryNeuron.StoredData is Directions storedDir)
                {
                    Coordinates targetPos = originalPosition + originalPosition.OffsetByDirection(storedDir)
                    if (map.CreatureExist(targetPos))
                    {
                        return multiplier * 2f;
                    }
                }
                return 0f;

            default:
                return 0f;
        }
    }

    private float Sensor_CreatureAge(Neuron sensor, MemoryNeuron? memoryNeuron, Creature creature)
    {
        short age = creature.Age;
        float multiplier = sensor.SignalOutput;

        if (memoryNeuron == null)
        {
            return age * multiplier;
        }

        switch (memoryNeuron.Subtype)
        {
            case MemoryNeuronType.Float :
                return multiplier * memoryNeuron.Compare(age);

            case MemoryNeuronType.Number:
                return multiplier * memoryNeuron.Compare(age);

            default:
                return 0f;
        }
    }

    private float Sensor_SmellPheromone(Neuron sensor, MemoryNeuron? memoryNeuron, Map map, Creature creature)
    {
        float multiplier = sensor.SignalOutput;
        byte radius = creature.CreatureBody.ChemicalReceptors.Distance;

        float totalSignal = 0f;
        Coordinates originCoordinates = creature.Position;

        foreach (var offset in Coordinates.GetOffsetsInRadius(radius))
        {
            Coordinates pos = originCoordinates + offset;
            if (map.IsOutOfBounds(pos))
            {
                continue;
            }

            float? pheromone = map.GetPheromoneValueAt(pos);
            if (pheromone == null || pheromone <= 0)
            {
                continue;
            }

            if (memoryNeuron == null)
            {
                totalSignal += pheromone.Value;
            }
            else
            {
                switch (memoryNeuron.Subtype)
                {
                    case MemoryNeuronType.Direction:
                        Directions offsetDirection = Coordinates.GetDirectionFromOffset(offset);
                        if (memoryNeuron.Compare(offsetDirection))
                            totalSignal += pheromone.Value;
                        break;

                    case MemoryNeuronType.Float:
                    case MemoryNeuronType.Number:
                        totalSignal += pheromone.Value * memoryNeuron.Compare(pheromone.Value);
                        break;

                    default:
                        break;
                }
            }
        }

        return totalSignal * multiplier;
    }



    #endregion

    public float GetBrainVolume()
    {
        return _neurons.Length * 0.1f + _memory.Length * 0.2f + _synapses.Length * 0.01f;
    }

    public float BrainConsumeEnergy()
    {
        return _neurons.Length * 0.05f + _memory.Length * 0.1f + _synapses.Length * 0.02f;
    }

}


