using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class ChemicalReceptors
{
    public byte Complexity { get => _complexity; }// скільки біт інформації можна зчитати

    public byte Distance { get => _distance;}//в якому радіусі можна відчути феромон

    private readonly byte _complexity;

    private readonly byte _distance;

    public ChemicalReceptors(byte complexity, byte distance)
    {
        _complexity = complexity;
        _distance = distance;
    }

    public float MaintenanceEnergyCost =>
           0.2f +                           // базові витрати
           Complexity * Complexity *0.3f + // складність обробки хімічного сигналу
           Distance * Distance * 0.1f;

    public float GetVolume()
    {
        return 0.05f + _complexity * 0.02f + _distance * 0.03f;
    }
}