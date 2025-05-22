using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class Covering
{
    public float Thickness { get => _thickness; }// збільшує здоров'я
    public float Toughness { get => _toughness; }// працює як броня, зменшує силу атаки ворога, при цьому здоров'я втрачається менше
    public float Flexibility => 1f / (1f + Thickness + Toughness);//гнучкість, впливає на свободу руху (менша — вищі витрати енергії на рух), виводитья з двох параметрів вище

    private readonly float _thickness;
    private readonly float _toughness;

    public Covering(float thickness, float toughness)
    {
        _thickness = thickness;
        _toughness = toughness;
    }

    public float MaintenanceEnergyCost =>
        0.1f +             // базові витрати на утримання покриву
        Thickness * 0.3f +  // збільшення витрат за товщиною
        Toughness * 0.4f;   // збільшення витрат за міцністю

    public float GetVolume()
    {
        return 0.1f + _thickness * 0.5f + _toughness * 0.2f;
    }
}

