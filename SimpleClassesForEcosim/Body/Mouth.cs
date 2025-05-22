using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;
//якщо істота атакує ротом, вона має здійснити рух до істоти (половина вартості енергії для дії руху)
public class Mouth
{
    public byte Size { get => _size; } // вказує скільки біомаси може поглинути істота за раз

    public bool HasTeeth { get => _hasTeeth; }// дає можливість істоті атакувати ротом

    public float Power { get => _power; } // впливає на дальність руху та силу атаки

    private readonly byte _size;
    private readonly bool _hasTeeth;
    private readonly float _power;

    public Mouth(byte size, bool hasTeeth, float power)
    {
        _size = size;
        _hasTeeth = hasTeeth;
        _power = power;
    }

    public float MaintenanceEnergyCost =>
        0.05f +                                    // базова підтримка
        (_hasTeeth ? 0.05f : 0f) +                 // зуби складніші в обслуговуванні
        _power * 0.03f +                           // підтримка потужності
        _size * 0.01f;                             // більший розмір вимагає більше енергії

    public float AttackEnergyCost =>
        0.3f +                                     // базова енергія на атаку
        (_hasTeeth ? 0.2f : 0f) +                  // зуби дають змогу атакувати
        _power * 0.1f;                             // більше потужності — дорожче

    public float AbsorbEnergyCost =>
            0.1f +                                 // базова енергія на поглинання
            _size * 0.05f +                        // більший рот — більше енергії
            _power * 0.05f;                        // потужність допомагає поглинати, але дорожче}

    public float GetVolume()
    {
        float baseVolume = _size * 0.1f + _power * 0.05f;
        if (_hasTeeth)
            baseVolume += 0.05f;

        return baseVolume;
    }

}