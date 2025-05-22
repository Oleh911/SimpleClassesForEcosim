using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class ReproductiveSystem
{
    public float FormationTime { get => _formationTime; }
    public byte Capacity { get => _capacity; }
    public float FertilityRate { get => _fertilityRate; }
    public float RecoveryTime { get => _recoveryTime; }

    public List<Creatures.Creature> Children { get => _children; }

    public bool IsPregnant { get; private set; }
    public short ChildrenNum => (short)_children.Count;
    public short PregnancyInterval { get; private set; }
    public bool IsReadyToGiveBirth =>
    IsPregnant && PregnancyInterval >= FormationTime;

    private readonly float _formationTime;
    private readonly byte _capacity;
    private readonly float _fertilityRate;
    private readonly float _recoveryTime;
    private List<Creatures.Creature> _children;

    public ReproductiveSystem
        (float formationTime, byte capacity, float fertilityRate, float recoveryTime)
    {
        _formationTime = formationTime;
        _capacity = capacity;
        _fertilityRate = fertilityRate;
        _recoveryTime = recoveryTime;
        _children = new List<Creatures.Creature>();
    }

    public void IncreasePregnancyInterval()
    {
        if (IsPregnant)
            PregnancyInterval += 1;
    }

    public void GiveBirth()
    {
        if (!IsReadyToGiveBirth) return;

         _children.Clear();
        IsPregnant = false;
        PregnancyInterval = 0;
    }

    public void StartPregnancy(List<Creatures.Creature> embryos)
    {
        if (IsPregnant || embryos.Count == 0 || embryos.Count > Capacity) return;

        _children = embryos;
        IsPregnant = true;
        PregnancyInterval = 0;
    }

    public float MaintenanceEnergyCost =>
        0.1f + 
        Capacity * 0.05f + 
        FertilityRate * 0.1f;

    public float PregnancyBiomassCostPerStep(float geneticCostPerChild)
    {
        if (!IsPregnant || FormationTime == 0f || _children.Count == 0) return 0f;

        float totalCost = _children.Count * geneticCostPerChild;
        return totalCost / FormationTime;
    }

    /// <summary>
    /// Оновлює стан вагітності, споживає біомасу, вбиває ембріонів за потреби.
    /// </summary>
    /// <param name="availableBiomass">Поточна біомаса істоти</param>
    /// <param name="geneticCostPerChild">Генетична вартість 1 ембріона</param>
    /// <returns>Оновлена біомаса після споживання або повернення вартості померлих ембріонів</returns>
    public float UpdatePregnancy(float availableBiomass, float geneticCostPerChild)
    {
        if (!IsPregnant || _children.Count == 0) return availableBiomass;

        float costPerStep = PregnancyBiomassCostPerStep(geneticCostPerChild);

        if (availableBiomass >= costPerStep)
        {
            IncreasePregnancyInterval();
            return availableBiomass - costPerStep;
        }
        else
        {
            // Недостатньо біомаси — спроба "пожертвувати" дітьми
            while (availableBiomass < costPerStep && _children.Count > 0)
            {
                _children.RemoveAt(_children.Count - 1);
                availableBiomass += geneticCostPerChild;
                costPerStep = PregnancyBiomassCostPerStep(geneticCostPerChild);
            }

            if (_children.Count == 0)
            {
                // Вагітність провалена
                IsPregnant = false;
                PregnancyInterval = 0;
                return availableBiomass;
            }

            IncreasePregnancyInterval();

            return availableBiomass - costPerStep;
        }
    }

    public float GetVolume()
    {
        return FormationTime * 0.5f + Capacity * 0.3f;
    }

}

