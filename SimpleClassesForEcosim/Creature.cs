using Body;
using Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures;

public class Creature
{
    public readonly int Id;
    public readonly float MaxHealth;
    public readonly float Armor;
    public readonly float Speed;
    public readonly float Size;
    public readonly float EnergyBaseConsumption;
    public readonly short LifeExpectation;
    public readonly float FlagellaDamage;
    public readonly float MouthDamage;

    public Body.Body CreatureBody;
    public NeuralNet CreatureBrain;
    // додати макс енергію
    // додати поступову втрату здоров'я якщо енергія = 0
    // додати метод виробити енергію та відхілитись
    public Coordinates Position { get; set; }
    public float Energy { get; private set; }
    public float Biomass { get; private set; }
    public float Health { get; private set; }
    public short Age { get; private set; }
    public bool Attacked { get; set; }
    public Directions lastDirection { get; set; }
    public bool IsAlive => Health > 0;

    public bool IsPregnant => CreatureBody.ReproductiveSystem.IsPregnant;

    public Creature(int id, Coordinates position, Body.Body body, NeuralNet brain)
    {
        Id = id;
        CreatureBody = body;
        CreatureBrain = brain;
        LifeExpectation = CalculateLifeExpectation();
        FlagellaDamage = CalculateFlagellaDamage();
        MouthDamage = CalculateMouthDamage();

        Size = CalculateSize();
        MaxHealth = CalculateMaxHealth();
        Armor = CalculateArmor();
        Speed = CalculateSpeed();
        EnergyBaseConsumption = CalculateEnergy();

        Position = position;
        Health = MaxHealth;
        Energy = 5f;
        Biomass = 10f;
        Attacked = false;
        lastDirection = Directions.None;
        Age = 0;
    }

    public ActionNeuronType Step(Map map)
    {
        // todo

        Attacked = false;

        return 0;
    }

    public float AbsorbBiomass(float biomass)
    {
        float mouthLimit = CreatureBody.Mouth.Size * 5.0f;
        float maxAbsorbable = MathF.Min(biomass, mouthLimit);

        float cavityFree = CreatureBody.DigestiveCavity.Capacity - CreatureBody.DigestiveCavity.BiomassAmount;
        float absorbAmount = MathF.Min(maxAbsorbable, cavityFree);

        if (absorbAmount > 0)
        {
            CreatureBody.DigestiveCavity.GetNewBiomass(absorbAmount);
        }

        return biomass - absorbAmount;
    }

    public void DropBiomass(float biomass)
    {
        if(CreatureBody.DigestiveCavity.BiomassAmount - biomass >= 0)
        {
            CreatureBody.DigestiveCavity.GetNewBiomass(-biomass);
        }
        else
        {
            CreatureBody.DigestiveCavity.GetNewBiomass(-CreatureBody.DigestiveCavity.BiomassAmount);
        }
    }

    public bool ConsumeEnergy(float ActionEnergyCost)
    {
        Energy -= EnergyBaseConsumption + ActionEnergyCost;

        if (Energy > 0)
        {
            return true;
        }
        else
        {
            Energy = 0;
            Health = 0;

            return false;
        }
    }

    public void TakeDamage(float damage)
    {
        if(Health - damage + 0.5f*Armor <= 0)
        {
            Health = 0;
        }
        else
        {
            damage -= 0.5f * Armor;

            if(damage < 0)
            {
                damage = 0;
            }

            Health -= damage;
        }
    }

    public float Die()
    {
        return Energy + CreatureBody.GetBodyVolume() + CreatureBrain.GetBrainVolume() * 0.5f;
    }

    private float CalculateSize()
    {
        return CreatureBody.GetBodyVolume() + CreatureBrain.GetBrainVolume();
    }

    private float CalculateMaxHealth()
    {
        return CreatureBody.Covering.Thickness * 5f + Size * 2f;
    }

    private float CalculateArmor()
    {
        return CreatureBody.Covering.Toughness * 3f;
    }

    private float CalculateSpeed()
    {
        return CreatureBody.Covering.Flexibility * CreatureBody.Flagella.Power * 2f;
    }

    private float CalculateEnergy()
    {
        return CreatureBody.BodyConsumeEnergy() + CreatureBrain.BrainConsumeEnergy(); // Сума пасивних витрат енергії всіх органів
    }

    private short CalculateLifeExpectation()
    {
        float complexity = CreatureBody.GetBodyVolume() + CreatureBrain.GetBrainVolume();
        float life = 1000f / (1f + complexity);

        return Math.Max((short)100, (short)life);
    }

    public float CalculateFlagellaDamage()
    {
        var flagella = CreatureBody.Flagella;

        float baseDamagePerFlagellum = 0.2f;
        float spikeBonus = flagella.HasSpikes ? 0.5f : 0f;
        float lengthBonus = flagella.Length * 0.2f;
        float powerBonus = flagella.Power * 0.4f;

        float damagePerFlagellum = baseDamagePerFlagellum + spikeBonus + lengthBonus + powerBonus;

        return damagePerFlagellum * flagella.Count;
    }

    public float CalculateMouthDamage()
    {
        var mouth = CreatureBody.Mouth;
        if (!mouth.HasTeeth)
        {
            return 0f;
        }

        float baseDamage = 1.0f;
        float sizeBonus = mouth.Size * 0.3f;
        float powerBonus = mouth.Power * 0.6f;

        return baseDamage + sizeBonus + powerBonus;
    }


}

