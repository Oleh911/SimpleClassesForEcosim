using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body;

public class Body
{
    public ChemicalReceptors ChemicalReceptors { get => _chemicalReceptors; }
    public PheromoneGlands PheromoneGlands { get => _pheromoneGlands; }
    public Flagella Flagella { get => _flagella; }
    public Mouth Mouth { get => _mouth; }
    public DigestiveCavity DigestiveCavity { get => _digestiveCavity; }
    public Covering Covering { get => _covering; }
    public ReproductiveSystem ReproductiveSystem { get => _reproductiveSystem; }

    private readonly ChemicalReceptors _chemicalReceptors;
    private readonly PheromoneGlands _pheromoneGlands;
    private readonly Flagella _flagella;
    private readonly Mouth _mouth;
    private readonly DigestiveCavity _digestiveCavity;
    private readonly Covering _covering;
    private readonly ReproductiveSystem _reproductiveSystem;

    public Body(
    ChemicalReceptors chemicalReceptors,
    PheromoneGlands pheromoneGlands,
    Flagella flagella,
    Mouth mouth,
    DigestiveCavity digestiveCavity,
    Covering covering,
    ReproductiveSystem reproductiveSystem)
    {
        _chemicalReceptors = chemicalReceptors;
        _pheromoneGlands = pheromoneGlands;
        _flagella = flagella;
        _mouth = mouth;
        _digestiveCavity = digestiveCavity;
        _covering = covering;
        _reproductiveSystem = reproductiveSystem;
    }

    public float GetBodyVolume()
    {
        float volume = 0f;

        volume += ChemicalReceptors.GetVolume();
        volume += PheromoneGlands.GetVolume();
        volume += Flagella.GetVolume();
        volume += Mouth.GetVolume();
        volume += DigestiveCavity.GetVolume();
        volume += Covering.GetVolume();
        volume += ReproductiveSystem.GetVolume();

        return volume;
    }

    public float BodyConsumeEnergy()
    {
        float consumption = 0f;

        consumption += _chemicalReceptors.MaintenanceEnergyCost;
        consumption += _pheromoneGlands.MaintenanceEnergyCost;
        consumption += _flagella.MaintenanceEnergyCost;
        consumption += _mouth.MaintenanceEnergyCost;
        consumption += _digestiveCavity.MaintenanceEnergyCost;
        consumption += _covering.MaintenanceEnergyCost;
        consumption += _reproductiveSystem.MaintenanceEnergyCost;

        return consumption;
    }
}
