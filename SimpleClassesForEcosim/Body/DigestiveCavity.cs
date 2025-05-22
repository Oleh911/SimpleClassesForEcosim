using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class DigestiveCavity
{
    public float Capacity { get => _capacity; } // макс к-ть їжі що може зберігати шлунок
    
    public float Efficiency { get => _efficiency; }// ефективність травлення, втрати енергії під час травлення

    public float DigestionRate { get => _digestionRate; }// швидкість травлення, скільки біомаси можна перетворити на енергію за хід

    public float BiomassAmount { get => _biomassAmount; }

    private readonly float _capacity;
    private readonly float _efficiency;
    private readonly float _digestionRate;
    private float _biomassAmount;

    public DigestiveCavity(float capacity, float efficiency, float digestionRate)
    {
        _capacity = capacity;
        _efficiency = Math.Clamp(efficiency, 0.01f, 1f);
        _digestionRate = digestionRate;
        _biomassAmount = capacity*0.1f;
    }

    public void GetNewBiomass(float biomass)
    {
        _biomassAmount += biomass;

        if(_biomassAmount > _capacity)
        {
            _biomassAmount = _capacity;
        }
    }

    public float MaintenanceEnergyCost =>
        0.1f + 
        Capacity * 0.05f 
        + Efficiency * 0.1f;

    public float GetDigestionCost(float biomassAmount)
    {
        float clampedAmount = Math.Clamp(biomassAmount, 0f, DigestionRate);
        float efficiencyModifier = 1f / Efficiency;
        return clampedAmount * 0.2f * efficiencyModifier;
    }

    public float GetVolume()
    {
        return 0.2f + Capacity * 0.6f + DigestionRate * 0.1f + (1f - Efficiency) * 0.2f;
    }
}
