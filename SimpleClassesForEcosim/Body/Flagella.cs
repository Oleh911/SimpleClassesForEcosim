using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Body;
public class Flagella
{
    public byte Count { get => _count;} // кількість джгутиків
    
    public float Length { get => _length;} // довжина кожного
    
    public bool HasSpikes { get => _hasSpikes; }

    public float Power { get => _power; } // сила джгутиків, впливає на дальність руху та силу атаки

    private readonly byte _count;
    private readonly float _length;
    private bool _hasSpikes;
    private float _power;

    public Flagella(byte count, float lenght, bool hasSpikes, float power)
    {
        _count = count;
        _length = lenght;
        _hasSpikes = hasSpikes;
        _power = power;
    }

    public float MaintenanceEnergyCost =>
        0.05f +
        _count * (0.01f + _length * 0.01f + (_hasSpikes ? 0.02f : 0f) + _power * 0.01f);

    public float MoveEnergyCost(float distance) =>
        _count * (
            0.1f +
            MathF.Pow(distance, 1.8f) * (_power + 0.5f) * 0.05f +
            _length * 0.05f
        );

    public float AttackEnergyCost =>
        _count * (
            0.2f +
            (_hasSpikes ? 0.3f : 0.1f) +
            _power * 0.2f
        );

    public float GetVolume()
    {
        float baseVolumePerFlagellum = _length * 0.05f + _power * 0.03f;
        if (_hasSpikes)
            baseVolumePerFlagellum += 0.02f;

        return _count * baseVolumePerFlagellum;
    }

}
