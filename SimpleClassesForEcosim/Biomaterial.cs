using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Biomaterial
{
    public readonly Coordinates Position;

    public float Biomass { get; private set; }

    public Biomaterial(Coordinates position, float biomass)
    {
        Position = position;
        Biomass = biomass;
    }

    public void ChangeBiomass(float newValue)
    {
        Biomass = newValue;
    }

}
