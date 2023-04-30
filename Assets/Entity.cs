using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour {
    [SerializeField] private EntityType entityType;
    [SerializeField] private EntityType upgradesInto = EntityType.None;
    [SerializeField] private float collisionRadius = 0.5f;
    [SerializeField] private float connectionRadius = 2;
    [SerializeField] private ProgressBar inputBar;
    [SerializeField] private ProgressBar outputBar;
    [SerializeField] private bool selectable = true;
    [SerializeField] private bool connectable = true;

    public ResType InputType;
    public ResType OutputType;
    [SerializeField] private float inPerSec = 1;
    [SerializeField] private float outPerSec = 1;

    public EntityType EntityType => entityType;
    public EntityType UpgradesInto => upgradesInto;
    public float CollisionRadius => collisionRadius;
    public float ConnectionRadius => connectionRadius;
    public bool Selectable => selectable;
    public bool Connectable => connectable;

    public float inBuffer = 0;
    public float outBuffer = 0;

    private List<Entity> inConnections = new List<Entity>();
    private List<Entity> outConnections = new List<Entity>();

    public List<Entity> InConnections => inConnections;
    public List<Entity> OutConnections => outConnections;

    private Game game;
    private ConnectionLines connectionLines;
    private Research research;

    private float inputMeasure = 0;
    private float outputMeasure = 0;
    private float measureTimer = 0.0001f;

    public bool Happy = false;

    void Awake() {
        game = FindObjectOfType<Game>();
        connectionLines = FindObjectOfType<ConnectionLines>();
        research = FindObjectOfType<Research>();
    }

    public void Process(float dt) {
        float inAmount = inPerSec * dt;
        if (InputType == ResType.Generator) {
            AddInput(inAmount);
        }
        else if (InputType == ResType.Wind) {
            AddInput(Mathf.Min(inAmount, game.Wind * dt));
        }
        else if (InputType == ResType.Sun) {
            AddInput(Mathf.Min(inAmount, game.Sun * dt));
        }

        inBuffer = Mathf.Min(inBuffer, inPerSec);

        float outputRatio = outPerSec / inPerSec;
        float wantedOutAmount = outPerSec * dt;
        float usedAmount = Mathf.Min(wantedOutAmount / outputRatio, inBuffer);
        inBuffer -= usedAmount;
        outBuffer += usedAmount * outputRatio;
        outputMeasure += usedAmount * outputRatio;

        outBuffer = Mathf.Min(outBuffer, outPerSec);
        if (OutputType == ResType.Money) {
            float multiplier = research.IsUnlocked(ResearchType.Marketing) ? 2.0f : 1.0f;
            game.AddMoney(outBuffer * multiplier);
            outBuffer = 0;
        }
        else if (OutputType == ResType.Rp) {
            game.AddRp(outBuffer);
            outBuffer = 0;
        }
        else if (outConnections.Count > 0) {
            float toGiveAmountMax = outBuffer / outConnections.Count;
            foreach (var outConnection in outConnections) {
                //float wantedAmount = outConnection.inPerSec / outConnection.InConnections.Count * dt;
                float toGive = toGiveAmountMax; // Mathf.Min(wantedAmount, toGiveAmountMax);
                outConnection.AddInput(toGive);
                outBuffer -= toGive;
            }
        }

        measureTimer += dt;
        if (measureTimer >= 1) {
            if (inputBar) {
                float progress = inputMeasure / measureTimer / inPerSec;
                inputBar.SetProgress(progress);
                if (entityType == EntityType.City) {
                    Happy = progress >= 1;
                }
            }

            if (outputBar) {
                outputBar.SetProgress(outputMeasure / measureTimer / outPerSec);
            }

            inputMeasure = 0;
            outputMeasure = 0;
            measureTimer = 0.0001f;
        }
    }

    public void AddInput(float amount) {
        inputMeasure += amount;
        inBuffer += amount;
        inBuffer = Mathf.Min(inBuffer, inPerSec);
    }

    private void Update() {
    }

    public void DrawPendingConnection(Vector3 pos, Color color) {
        connectionLines.SetPendingLine(this, pos, color);
    }

    public void AddOutConnection(Entity entity) {
        if (!outConnections.Contains(entity)) {
            outConnections.Add(entity);
            connectionLines.DrawConnectionLine(this, entity);
        }
    }

    public void AddInConnection(Entity entity) {
        if (!inConnections.Contains(entity)) {
            inConnections.Add(entity);
        }
    }

    public void RemoveConnections() {
        foreach (var inConnection in inConnections) {
            inConnection.RemoveOutConnection(this);
        }

        inConnections.Clear();

        foreach (var outConnection in outConnections) {
            outConnection.RemoveInConnection(this);
        }

        outConnections.Clear();
    }

    public void RemoveOutConnection(Entity connection) {
        outConnections.Remove(connection);
        connectionLines.RemoveConnectionLine(this, connection);
    }

    public void RemoveInConnection(Entity connection) {
        inConnections.Remove(connection);
        connectionLines.RemoveConnectionLine(connection, this);
    }

    public static string GetNameText(EntityType entityType) {
        switch (entityType) {
            case EntityType.Obstacle:
            case EntityType.HiddenCoalDeposit:
            case EntityType.HiddenUraniumDeposit:
                return "Obstacle";
            case EntityType.CharcoalKiln:
                return "Charcoal Kiln";
            case EntityType.CoalDeposit:
                return "Coal Deposit";
            case EntityType.CoalMine:
                return "Coal Mine";
            case EntityType.CoalPlant:
                return "Coal Plant";
            case EntityType.CoalWarehouse:
                return "Coal Warehouse";
            case EntityType.DigSite:
                return "Dig Site";
            case EntityType.LogWarehouse:
                return "Log Warehouse";
            case EntityType.NuclearPlant:
                return "Nuclear Plant";
            case EntityType.PowerPole:
                return "Power Pole";
            case EntityType.ResearchLab:
                return "Research Lab";
            case EntityType.SawMill:
                return "Saw Mill";
            case EntityType.SolarPanel:
                return "Solar Panel";
            case EntityType.UraniumDeposit:
                return "Uranium Deposit";
            case EntityType.UraniumMill:
                return "Uranium Mill";
            case EntityType.UraniumMine:
                return "Uranium Mine";
            case EntityType.WaterFall:
                return "Waterfall";
            case EntityType.WindTurbine:
                return "Wind Turbine";
            case EntityType.WoodBurner:
                return "Wood Burner";
            case EntityType.WoodWarehouse:
                return "Wood Warehouse";
            case EntityType.BigPowerPole:
                return "Big Power Pole";
            case EntityType.BigWindTurbine:
                return "Big Wind Turbine";
        }

        return entityType.ToString();
    }

    public static string GetInfoText(EntityType entityType) {
        switch (entityType) {
            case EntityType.SawMill:
                return "Saw logs into wood. Place near forest.";
            case EntityType.WoodBurner:
                return "Connect a Saw Mill to burn wood into power.";
            case EntityType.WindTurbine:
                return "Make power from wind. Requires a lot of space. Can be unreliable.";
            case EntityType.BigWindTurbine:
                return "Generates more power than Wind Turbine and can take advantage of stronger winds.";
            case EntityType.PowerPole:
                return "Transfer power.";
            case EntityType.BigPowerPole:
                return "Transfer power. Has higher bandwidth and longer reach than Power Pole.";
            case EntityType.ResearchLab:
                return "Earn RP by researching old bones. Connect with a dig site.";
            case EntityType.SolarPanel:
                return "Turn solar rays into power. Output depends on day/night.";
            case EntityType.Dam:
                return "Use water to make power. Place near a waterfall.";
            case EntityType.CharcoalKiln:
                return "Turn wood into coal.";
            case EntityType.CoalPlant:
                return "Burn coal into power.";
            case EntityType.CoalMine:
                return "Mine coal. Place near coal deposit.";
            case EntityType.UraniumMine:
                return "Mine raw uranium. Place near uranium deposit.";
            case EntityType.UraniumMill:
                return "Process raw uranium into uranium.";
            case EntityType.NuclearPlant:
                return "Make power from uranium.";
            case EntityType.City:
                return "Deliver power to the cities. Will give you money for power.";
            case EntityType.Forest:
                return "Connect to a Saw Mill to produce logs.";
            case EntityType.DigSite:
                return "Connect a Research Lab to earn RP.";
            case EntityType.WaterFall:
                return "Connect a Dam to generate power. Requires Hydro Power research.";
            case EntityType.CoalDeposit:
                return "Connect a Coal Mine to mine raw coal.";
            case EntityType.UraniumDeposit:
                return "Connect a Uranium Mine to mine raw uranium.";
            case EntityType.Obstacle:
            case EntityType.HiddenCoalDeposit:
            case EntityType.HiddenUraniumDeposit:
                return "An obstacle. You can't build here.";
            default:
                return "Undefined!";
        }
    }
}