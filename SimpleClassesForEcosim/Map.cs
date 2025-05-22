using Creatures;
using Neurons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Map
{
    public readonly short Width;
    public readonly short Height;

    private List<Creature> _creaturesAlive;
    private List<Creature> _creaturesToDie;
    private List<Biomaterial> _biomaterials;
    private List<Pheromone> _pheromones;
    private Random _random;
    private int _step;
    private int _maxId;

    public Map(short width, short height)
    {
        Width = width;
        Height = height;

        _creaturesAlive = new();
        _creaturesToDie = new();
        _biomaterials = new();
        _pheromones = new();
        _random = GetRandom();
        _step = 1;
        _maxId = 0;
    }

    public void SimulationDoStep()
    {
        if(_step % 10 == 0)
        {
            _random = new Random();
        }

        var actions = new ConcurrentBag<CreatureAction>();

        Parallel.ForEach(_creaturesAlive, creature =>
        {
            var action = creature.CreatureBrain.BrainCalculateAction(this, creature);
            actions.Add(new CreatureAction(
                creature.Id,
                action.Action,
                creature.Speed,
                action.Memory
            ));
        });

        List<CreatureAction> ordered = actions
            .OrderByDescending(a => a.Speed)
            .ToList();

        foreach (CreatureAction action in ordered)
        {
            ExecuteAction(action);
        }

        ClearCreaturesToDie();
        
        ++_step;
    }

    public void ExecuteAction(CreatureAction action)
    {
        var creature = _creaturesAlive.FirstOrDefault(c => c.Id == action.CreatureId);
        if (creature == null) return;

        switch (action.Action)
        {
            case ActionNeuronType.Stay:
                // Можливо пасивне споживання енергії
                break;

            case ActionNeuronType.Move:
                break;

            case ActionNeuronType.DropFeromon:
                break;

            case ActionNeuronType.AttacByFlagella:
                break;

            case ActionNeuronType.AttackByMouth:
                break;

            case ActionNeuronType.AttacByAll:
                break;

            case ActionNeuronType.Eat:
                break;

            default:
                Action_Stay(creature);
                break;
        }

        //creature.IncreaseAge();

        if (creature.Age >= creature.LifeExpectation || creature.Energy <= 0)
        {
            KillCreature(creature);
        }
    }

    #region<HelpMethods>

    public float? GetPheromoneValueAt(Coordinates position)
    {
        Pheromone? pheromone = _pheromones.FirstOrDefault(x => x.Position == position);
        if (pheromone == null)
        {
            return null;
        }

        return pheromone?.Strengh;
    }


    public bool CreatureExist(Coordinates position)
    {
        return _creaturesAlive.Exists(x => x.Position == position);
    }

    public bool IsOutOfBounds(Coordinates pos)
    {
        return pos.X < 0 || pos.Y < 0 || pos.X >= Width || pos.Y >= Height;
    }

    public bool IsObstacleAt(Coordinates pos)
    {
        if (IsOutOfBounds(pos))
        {
            return true;
        }
        if (_creaturesAlive.Any(c => c.Position == pos))
        {
            return true;
        }

        return false;
    }

    public float FoodInRadius(Coordinates position, byte radius)
    {
        float totalAmount = 0;

        foreach (Biomaterial biomaterial in _biomaterials)
        {
            if (position.DistanceTo(biomaterial.Position) <= radius)
            {
                totalAmount += biomaterial.Biomass;
            }
        }

        return totalAmount;
    }

    public byte BiomatirealsNumInRadius(Coordinates position, byte radius)
    {
        byte number = (byte)_biomaterials
            .Where(x => x.Position.DistanceTo(position) <= radius)
            .Count();

        return number;
    }

    public Biomaterial GetFoodByPosition(Coordinates position)
    {
        return _biomaterials.FirstOrDefault(x => x.Position == position);
    }

    public int GenerateNewCreatureId()
    {
        ++_maxId;

        return _maxId;
    }

    public void AddNewCreature(Creature creature)
    {
        _creaturesAlive.Add(creature);
    }

    public void KillCreature(Creature creature)
    {
        _creaturesToDie.Add(creature);
        _creaturesAlive.Remove(creature);
    }

    public void ClearCreaturesToDie()
    {
        foreach (Creature creature in _creaturesToDie)
        {
            float biomass = creature.Die();
            AddBiomaterial(creature.Position, biomass);
        }
        _creaturesToDie.Clear();
    }

    public void AddBiomaterial(Coordinates position, float biomass)
    {
        if (biomass <= 0)
        {
            return;
        }

        if (_biomaterials.Exists(x => x.Position == position))
        {
            _biomaterials[_biomaterials.FindIndex(x => x.Position == position)]
                .ChangeBiomass(biomass);
        }
        else
        {
            _biomaterials.Add(new Biomaterial(position, biomass));
        }
    }

    public void RemoveBiomaterial(Biomaterial biomaterial)
    {
        _biomaterials.Remove(biomaterial);
    }

    public void AddPheromone(Pheromone pheromone)
    {
        _pheromones.Add(pheromone);
    }

    public void RemovePheromone(Pheromone pheromone)
    {
        _pheromones.Remove(pheromone);
    }

    private Random GetRandom()
    {
        return new Random();
    }

    #endregion

    #region <ActionMethods>

    private void Action_Stay(Creature creature)
    {
        // Істота нічого не робить — лише пасивне споживання енергії.
    }
    // додати валідацію щоб істота не виходила за межі мапи
    private bool Action_Move(Creature creature, MemoryNeuron? memoryNeuron)
    {
        int id = creature.Id;
        Coordinates position = creature.Position;
        byte radius = (byte)creature.CreatureBody.Flagella.Length;

        if (memoryNeuron != null)
        {
            switch (memoryNeuron.Subtype)
            {
                case MemoryNeuronType.Coordinates:
                    if (TryMove(id, position, (Coordinates)memoryNeuron.StoredData, radius))
                    {
                        return true;
                    }
                    if (TryMove(id, position, position.GetDirection((Coordinates)memoryNeuron.StoredData), radius))
                    {
                        return true;
                    }
                    break;

                case MemoryNeuronType.Direction:
                    if (TryMove(id, position, position.GetDirection((Coordinates)memoryNeuron.StoredData), radius))
                    {
                        return true;
                    }
                    break;

                case MemoryNeuronType.Number:
                    if (TryMove(id, position, null, (byte)memoryNeuron.StoredData))
                    {
                        return true;
                    }
                    break;

                default:
                    if (TryMove(id, position, null, radius))
                    {
                        return true;
                    }
                    break;
            }
        }

        if (TryMove(id, position, null, radius))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryMove(int creatureId, Coordinates position, Coordinates target, byte radius)
    {
        var creature = _creaturesAlive.FirstOrDefault(x => x.Id == creatureId);
        if (creature != null && (byte)position.DistanceTo(target) <= radius)
        {
            if (_creaturesAlive.Any(x => x.Position == target))
            {
                return false;
            }


            creature.Position = target;
            return true;
        }

        if (creature == null)
        {
            throw new Exception($"Wrong creature Id {creatureId} on TryMove_");
        }

        return false;

    }

    private bool TryMove(int creatureId, Coordinates position, Directions? direction, byte radius)
    {
        var creature = _creaturesAlive.FirstOrDefault(x => x.Id == creatureId);

        if (creature == null)
        {
            throw new Exception($"Wrong creature Id {creatureId} on TryMove");
        }

        var candidates = GetAvailableCoordinates(position, direction, radius)
            .OrderBy(_ => Guid.NewGuid())
            .ToList();

        foreach (var target in candidates)
        {
            if (!_creaturesAlive.Any(x => x.Position == target))
            {
                creature.Position = target;
                return true;
            }
        }

        return false;
    }

    private bool Action_DropFeromon(Creature creature, MemoryNeuron? memoryNeuron)
    {
        var glands = creature.CreatureBody.PheromoneGlands;

        if (glands == null)
        {
            return false;
        }

        float intensity = glands.MaxIntensity;
        byte pheromoneType = (byte)_random.Next(0, 256);

        if (memoryNeuron != null)
        {
            switch (memoryNeuron.Subtype)
            {
                case MemoryNeuronType.Float:
                    intensity = Math.Clamp((float)memoryNeuron.StoredData, 0.1f, glands.MaxIntensity);
                    break;

                case MemoryNeuronType.Pheromone:
                    pheromoneType = (byte)Math.Clamp((byte)memoryNeuron.StoredData * 255f, 0, 255);
                    break;

                default:
                    intensity = glands.MaxIntensity;
                    pheromoneType = (byte)_random.Next(0, 256);
                    break;
            }
        }

        Coordinates position = creature.Position;

        Pheromone existing = _pheromones.FirstOrDefault(p => p.Position == position);

        if (existing.Position == position)
        {
            float newStrength = (existing.Strengh + intensity) / 2f;
            byte newType = (byte)((existing.Type + pheromoneType) / 2);

            _pheromones.Remove(existing);
            _pheromones.Add(new Pheromone(position, newStrength, newType));
        }
        else
        {
            _pheromones.Add(new Pheromone(position, intensity, pheromoneType));
        }

        return true;
    }

    private bool Action_AttackByMouth(Creature creature, MemoryNeuron? memoryNeuron)
    {
        Creature? target = GetTargetByMemoryOrFindNearby(creature, memoryNeuron, 1);
        var mouth = creature.CreatureBody.Mouth;

        if (target != null && mouth != null)
        {
            float damage = creature.MouthDamage;
            target.TakeDamage(damage);
            target.Attacked = true;

            if (!target.IsAlive)
            {
                _creaturesToDie.Add(target);
            }

            return true;
        }

        return false;
    }

    private bool Action_AttacByFlagella(Creature creature, MemoryNeuron? memoryNeuron)
    {
        var flagella = creature.CreatureBody.Flagella;

        int maxDistance = Math.Max(1, (int)MathF.Ceiling(flagella.Length));
        Creature? target = GetTargetByMemoryOrFindNearby(creature, memoryNeuron, maxDistance);

        if (target != null)
        {
            float damage = creature.FlagellaDamage;
            target.TakeDamage(damage);
            target.Attacked = true;

            if (!target.IsAlive)
            {
                _creaturesToDie.Add(target);
            }

            return true;
        }

        return false;
    }

    private bool Action_AttacByAll(Creature creature, MemoryNeuron? memoryNeuron)
    {
        var mouth = creature.CreatureBody.Mouth;
        var flagella = creature.CreatureBody.Flagella;

        Creature? target = GetTargetByMemoryOrFindNearby(creature, memoryNeuron, 1);

        if (target != null)
        {
            float totalDamage = (creature.MouthDamage + creature.FlagellaDamage) * 0.8f;

            target.TakeDamage(totalDamage);
            target.Attacked = true;

            if (!target.IsAlive)
            {
                _creaturesToDie.Add(target);
            }

            return true;
        }

        return false;
    }

    private bool Action_Eat(Creature creature, MemoryNeuron? memoryNeuron)
    {
        var mouth = creature.CreatureBody.Mouth;

        Coordinates origin = creature.Position;
        Coordinates? targetPos = null;

        if (memoryNeuron is not null)
        {
            switch (memoryNeuron.Subtype)
            {
                case MemoryNeuronType.Direction:
                    if ((Directions)memoryNeuron.StoredData is Directions dir && dir != Directions.None)
                    {
                        targetPos = origin.OffsetByDirection(dir);
                    }
                    break;

                case MemoryNeuronType.Coordinates:
                    if ((Coordinates)memoryNeuron.StoredData is Coordinates coords)
                    {
                        targetPos = coords;
                    }
                    break;
            }
        }

        if (targetPos is null || origin.DistanceTo(targetPos.Value) > 1)
        {
            targetPos = _biomaterials
                .Where(b => origin.DistanceTo(b.Position) <= 1)
                .OrderBy(b => origin.DistanceTo(b.Position))
                .Select(b => (Coordinates?)b.Position)
                .FirstOrDefault();
        }

        if (targetPos == null && _biomaterials.Exists(x => x.Position == creature.Position))
        {
            return false;
        }
        else
        {
            targetPos = creature.Position;
        }

        int index = _biomaterials.FindIndex(b => b.Position == targetPos.Value);
        
        if (index == -1)
        {
            return false;
        }

        var material = _biomaterials[index];

        float absorbable = MathF.Min(material.Biomass, mouth.Size);
        
        if (absorbable <= 0f)
        {
            return false;
        }

        float biomassLeft = creature.AbsorbBiomass(absorbable);
        if (biomassLeft <= 0)
        {
            _biomaterials.RemoveAt(index);
        }
        else
        {
            _biomaterials[index].ChangeBiomass(biomassLeft);
        }

        return true;
    }


    private void Action_Mate(Creature creature, MemoryNeuron? memoryNeuron)
    {
        // todo спочатку потрібно створити класс gen який зможе мутувати гени або створювати рандомних істот
    }

    private bool Action_GiveBirth(Creature creature, MemoryNeuron? memoryNeuron)
    {
        var reproductive = creature.CreatureBody.ReproductiveSystem;
        if (!reproductive.IsReadyToGiveBirth)
        {
            return false;
        }

        Coordinates originPos = creature.Position;
        List<Coordinates> freePositions = GetSortedFreePositions(originPos, 3);

        List<Creature> embryos = reproductive.Children;
        int maxToPlace = Math.Min(embryos.Count, freePositions.Count);

        for (int i = 0; i < maxToPlace; i++)
        {
            Creature child = embryos[i];
            child.Position = freePositions[i];
            _creaturesAlive.Add(child);
        }

        for (int i = maxToPlace; i < embryos.Count; i++)
        {
            Creature deadChild = embryos[i];
            deadChild.Position = originPos;
            _creaturesToDie.Add(deadChild);
        }

        reproductive.GiveBirth();
        return true;
    }


    private bool Action_DepositEnergy(Creature creature, MemoryNeuron? memoryNeuron)
    {
        float biomassToDrop;

        if (memoryNeuron == null ||
            memoryNeuron.Subtype != MemoryNeuronType.Float ||
            memoryNeuron.Subtype != MemoryNeuronType.Float ||
            (float)memoryNeuron.StoredData <= 0f)
        {
            biomassToDrop = creature.CreatureBody.DigestiveCavity.BiomassAmount * 0.1f;
        }
        else
        {
            biomassToDrop = creature.CreatureBody.DigestiveCavity.BiomassAmount
                * (float)memoryNeuron.StoredData;
        }

        biomassToDrop = MathF.Min(biomassToDrop, creature.CreatureBody.DigestiveCavity.BiomassAmount);
        if (biomassToDrop <= 0.01f) return false;

        creature.DropBiomass(biomassToDrop);

        var existing = _biomaterials.FirstOrDefault(x => x.Position == creature.Position);

        if (existing.Biomass > 0 && _biomaterials.Exists(x => x.Position == creature.Position))
        {
            _biomaterials.Remove(existing);
            float newBiomass = existing.Biomass + biomassToDrop;
            _biomaterials.Add(new Biomaterial(creature.Position, newBiomass));
        }
        else
        {
            _biomaterials.Add(new Biomaterial(creature.Position, biomassToDrop));
        }

        return true;
    }

    private IEnumerable<Coordinates> GetAvailableCoordinates(Coordinates from, Directions? direction, byte radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                if (dist > radius)
                    continue;

                Coordinates candidate = new Coordinates((short)(from.X + dx), (short)(from.Y + dy));

                if (!IsInsideMap(candidate))
                    continue;

                if (direction == null || from.GetDirection(candidate) == direction.Value)
                {
                    yield return candidate;
                }
            }
        }
    }

    private bool IsInsideMap(Coordinates coord)
    {
        return coord.X >= 0 && coord.X < Width && coord.Y >= 0 && coord.Y < Height;
    }

    private Creature? GetTargetByMemoryOrFindNearby(Creature creature, MemoryNeuron? memoryNeuron, int range)
    {
        Coordinates origin = creature.Position;

        if (memoryNeuron is not null)
        {
            switch (memoryNeuron.Subtype)
            {
                case MemoryNeuronType.Direction:
                    if ((Directions)memoryNeuron.StoredData is Directions dir && dir != Directions.None)
                    {
                        var targetPos = origin.OffsetByDirection(dir);
                        if (origin.DistanceTo(targetPos) <= range)
                            return _creaturesAlive.FirstOrDefault(c => c.Position == targetPos && c.Id != creature.Id);
                    }
                    break;

                case MemoryNeuronType.Coordinates:
                    if ((Coordinates)memoryNeuron.StoredData is Coordinates coords &&
                        origin.DistanceTo(coords) <= range)
                    {
                        return _creaturesAlive.FirstOrDefault(c => c.Position == coords && c.Id != creature.Id);
                    }
                    break;
            }
        }

        return _creaturesAlive
            .Where(c => c.Id != creature.Id && origin.DistanceTo(c.Position) <= range)
            .OrderBy(c => origin.DistanceTo(c.Position))
            .OrderBy(c => c.Health)
            .FirstOrDefault();
    }

    private List<Coordinates> GetSortedFreePositions(Coordinates origin, int radius)
    {
        var positions = new List<(Coordinates pos, float dist)>();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var pos = new Coordinates((short)(origin.X + dx), (short)(origin.Y + dy));
                float distance = origin.DistanceTo(pos);

                if (distance <= radius && IsCellFree(pos))
                    positions.Add((pos, distance));
            }
        }

        return positions
            .OrderBy(p => p.dist)
            .Select(p => p.pos)
            .ToList();
    }

    private bool IsCellFree(Coordinates pos)
    {
        return !_creaturesAlive.Any(c => c.Position == pos);
    }

    #endregion
}

