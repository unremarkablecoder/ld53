using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchEntry {
    public ResearchType researchType;
    public ResearchType dependency;
    public float rpCost;
    public bool unlocked = false;

    public ResearchEntry(ResearchType type, ResearchType dependency, float rpCost) {
        this.researchType = type;
        this.dependency = dependency;
        this.rpCost = rpCost;
    }
}

public class Research : MonoBehaviour {
    private List<ResearchEntry> entries = new List<ResearchEntry>();
    private Game game;

    void Awake() {
        game = FindObjectOfType<Game>();
        var r = new ResearchEntry(ResearchType.HydroPower, ResearchType.None, 4);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.Nuclear, ResearchType.None, 20);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.BigPowerPole, ResearchType.None, 5);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.BigWindTurbine, ResearchType.None, 10);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.MoreWind, ResearchType.BigWindTurbine, 10);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.CoalPower, ResearchType.None, 8);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.LumberLogistics, ResearchType.None, 2);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.CoalLogistics, ResearchType.CoalPower, 3);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.SolarPower, ResearchType.None, 5);
        entries.Add(r);
        
        r = new ResearchEntry(ResearchType.Marketing, ResearchType.None, 20);
        entries.Add(r);
    }

    public float GetCost(ResearchType researchType) {
        var entry = Find(researchType);
        return entry.rpCost;
    }

    public ResearchEntry Find(ResearchType type) {
        foreach (var researchEntry in entries) {
            if (researchEntry.researchType == type) {
                return researchEntry;
            }
        }

        return null;
    }

    public bool IsUnlocked(ResearchType type) {
        var entry = Find(type);
        return entry.unlocked;
    }

    public bool CanBuy(ResearchType type) {
        var entry = Find(type);
        if (entry.dependency != ResearchType.None && !Find(entry.dependency).unlocked) {
            return false;
        }
        return (game.Rp >= entry.rpCost);
    }

    public void TryToBuy(ResearchType type) {
        if (!CanBuy(type)) {
            return;
        }
        var entry = Find(type);
        entry.unlocked = true;
        game.RemoveRp(entry.rpCost);
        game.ResearchUnlocked(type);
    }

    public static string GetNameText(ResearchType type) {
        switch (type) {
            case ResearchType.Nuclear:
                return "Nuclear Power";
            case ResearchType.CoalPower:
                return "Coal Power";
            case ResearchType.HydroPower:
                return "Hydro Power";
            case ResearchType.SolarPower:
                return "Solar Power";
            case ResearchType.CoalLogistics:
                return "Coal Logistics";
            case ResearchType.LumberLogistics:
                return "Lumber Logistics";
            case ResearchType.MoreWind:
                return "More Wind";
            case ResearchType.BigWindTurbine:
                return "Big Wind Turbine";
            case ResearchType.BigPowerPole:
                return "Big Power Pole";
            case ResearchType.Marketing:
                return "Marketing";
            default:
                return "Undefined!";
        }
    }

    public static string GetInfoText(ResearchType type) {
        switch (type) {
            case ResearchType.Nuclear:
                return "Unlocks nuclear power buildings. Reveals Uranium Deposits on map.";
            case ResearchType.CoalPower:
                return "Unlocks coal power buildings. Reveals Coal Deposits on map.";
            case ResearchType.HydroPower:
                return "Unlocks Dam to generate power from Waterfalls.";
            case ResearchType.SolarPower:
                return "Unlocks Solar Panels. Generates Power from the sun.";
            case ResearchType.CoalLogistics:
                return "Unlocks Coal Warehouse that can transfer Coal.";
            case ResearchType.LumberLogistics:
                return "Unlocks Log and Wood Warehouses that can transfer Logs/Wood.";
            case ResearchType.MoreWind:
                return "Make Wind stronger. Magic!";
            case ResearchType.BigWindTurbine:
                return "Unlocks Big Wind Turbine.";
            case ResearchType.BigPowerPole:
                return "Unlocks Big Power Pole that has longer reach.";
            case ResearchType.Marketing:
                return "Double money earned from power";
            default:
                return "Undefined!";
        }
    }
}
