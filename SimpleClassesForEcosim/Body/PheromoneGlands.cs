using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class PheromoneGlands
{
    public byte Complexity { get => _complexity; }// скільки біт інформації можна зчитати

    //"кількість" феромону, тобто наскільки довго випаровуватиметься феромон
    public float MaxIntensity { get => _maxIntensity;}

    private readonly byte _complexity;
    private readonly float _maxIntensity;

    public PheromoneGlands(byte complexity, float maxIntensity)
    {
        _maxIntensity = maxIntensity;
        _complexity = complexity;
    }

    public float MaintenanceEnergyCost =>
        0.05f +                             // базова вартість
        _complexity * 0.03f +              // вартість підтримки складності
        _maxIntensity * _complexity * 0.02f;

    public float EmissionEnergyCost =>
    _maxIntensity * 0.4f +                      // витрата за інтенсивність
    _complexity * _complexity * 0.3f +          // складність інформації
    _maxIntensity * _complexity * 0.05f;

    public float GetVolume()
    {
        return _maxIntensity * 0.5f + _complexity * 0.1f;
    }
}
