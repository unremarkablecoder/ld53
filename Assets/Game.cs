using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    private GameState gameState;

    public GameState GameState => gameState;
    public float Money => money;
    public float Rp => rp;
    public float Wind => wind;
    public float Sun => sun;

    private RadiusOverlay radiusOverlay;
    private Entities entities;
    private Hud hud;
    private Research research;
    private float stateTimer = 0;
    private float money = 200;
    private float rp = 0;
    private float moneyMeasure = 0;
    private float moneyMeasureTimer = 0;
    private float rpMeasure;
    private float wind = 0;
    private float windTimer = 0;
    private float sun = 0;
    private float sunTimer = 0;
    private bool won = false;

    private Entity selectedEntity;


    void Awake() {
        radiusOverlay = FindObjectOfType<RadiusOverlay>();
        entities = FindObjectOfType<Entities>();
        hud = FindObjectOfType<Hud>();
        research = FindObjectOfType<Research>();
    }


    // Update is called once per frame
    void Update() {
        hud.HideTooltip();
        switch (gameState) {
            case GameState.Default:
                UpdateDefaultState();
                break;
            case GameState.PlacingBuilding:
                UpdatePlacingBuildingState();
                break;
            case GameState.Connecting:
                UpdateConnectingState();
                break;
            case GameState.Selected:
                UpdateSelectedState();
                break;
        }

        stateTimer += Time.deltaTime;
        moneyMeasureTimer += Time.deltaTime;
        if (moneyMeasureTimer >= 1) {
            hud.SetMoneyGain(moneyMeasure);
            hud.SetRpGain(rpMeasure);
            moneyMeasureTimer = 0;
            moneyMeasure = 0;
            rpMeasure = 0;
        }
    }

    private void FixedUpdate() {
        float dt = Time.fixedDeltaTime;

        windTimer -= dt;
        if (windTimer < 0) {
            windTimer = Random.Range(5, 15);
            if (research.IsUnlocked(ResearchType.MoreWind)) {
                wind = Random.Range(0.2f, 0.9f) + Random.Range(0.2f, 0.9f);
            }
            else {
                wind = Random.Range(0.0f, 0.7f) + Random.Range(0.0f, 0.7f);
            }
        }

        sunTimer += dt;
        sun = (Mathf.Sin(sunTimer * 0.1f) + 1) * 0.5f;

        if (!won) {
            List<Entity> cities = entities.GetCities();
            int numHappy = 0;
            foreach (var entity in cities) {
                if (entity.Happy) {
                    ++numHappy;
                }
            }

            if (numHappy >= 2) {
                won = true;
                hud.ShowWin();
            }
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Keypad4)) {
            AddMoney(10, false);
        }

        if (Input.GetKey(KeyCode.Keypad5)) {
            AddRp(1);
        }
#endif
    }


    public static Vector3 GetInputWorldPos() {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    private void UpdateDefaultState() {
        var worldPos = GetInputWorldPos();
        radiusOverlay.Hide(true, true);
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        var entity = entities.FindClosestEntity(worldPos, true);
        if (entity) {
            radiusOverlay.ShowRadius(entity, true, true);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && entity.Connectable) {
                entities.StartConnectingEntity(entity);
                SetState(GameState.Connecting);
            }

            hud.ShowTooltip(Entity.GetNameText(entity.EntityType), Entity.GetInfoText(entity.EntityType), Camera.main.WorldToScreenPoint(entity.transform.position) + new Vector3(0, 100, 0), 0.7f);
        }
    }

    private void UpdatePlacingBuildingState() {
        var worldPos = GetInputWorldPos();
        entities.UpdatePendingPosition(worldPos);

        if (stateTimer < 0.1f) {
            return;
        }

        radiusOverlay.Hide();
        var pendingEntity = entities.GetPending();
        var connectedEntities = entities.FindEntitiesWithinConnection(worldPos, pendingEntity.ConnectionRadius, pendingEntity.OutputType, pendingEntity.InputType);
        foreach (var connectedEntity in connectedEntities) {
            radiusOverlay.ShowRadius(connectedEntity, false, true);
        }

        var collidingEntities = entities.FindEntitiesWithinCollision(worldPos, pendingEntity.CollisionRadius);
        foreach (var connectedEntity in collidingEntities) {
            radiusOverlay.ShowRadius(connectedEntity, true, false);
        }

        radiusOverlay.ShowRadius(pendingEntity, collidingEntities.Count > 0, true);
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if (collidingEntities.Count > 0) {
                return;
            }

            float cost = GetEntityCost(pendingEntity.EntityType);
            if (money < cost) {
                return;
            }

            entities.PlacePending();
            RemoveMoney(cost);
            StartPlacingBuilding(pendingEntity.EntityType);
            return;
        }
        else if (Input.GetMouseButtonUp(1)) {
            entities.CancelPending();
            SetState(GameState.Default);
            return;
        }
    }

    private void UpdateConnectingState() {
        var worldPos = GetInputWorldPos();
        entities.UpdateConnectionPos(worldPos);

        if (Input.GetMouseButtonUp(0)) {
            selectedEntity = entities.ApplyOrCancelConnection();
            hud.SetSelectedEntity(selectedEntity);
            if (selectedEntity) {
                SetState(GameState.Selected);
            }
            else {
                SetState(GameState.Default);
            }
        }
    }

    private void UpdateSelectedState() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            hud.SetSelectedEntity(null);
            selectedEntity = null;
            SetState(GameState.Default);
        }
    }

    public void StartPlacingBuilding(EntityType entityType) {
        entities.CreatePendingEntity(entityType);
        SetState(GameState.PlacingBuilding);
    }

    public void SetState(GameState state) {
        gameState = state;
        stateTimer = 0;
    }

    public void AddMoney(float amount, bool countAsIncome = true) {
        money += amount;
        moneyMeasure += amount;
        hud.UpdateButtons();
    }

    public void RemoveMoney(float amount) {
        money -= amount;
        hud.UpdateButtons();
    }

    public void SellSelected() {
        selectedEntity.RemoveConnections();
        AddMoney(GetEntityCost(selectedEntity.EntityType) * 0.8f, false);
        entities.SellEntity(selectedEntity);
        selectedEntity = null;
        hud.SetSelectedEntity(null);
        SetState(GameState.Default);
    }

    public void RemoveConnectionsFromSelected() {
        selectedEntity.RemoveConnections();
    }

    public float GetEntityCost(EntityType entityType) {
        float baseCost = 0;
        switch (entityType) {
            case EntityType.SawMill:
                baseCost = 45;
                break;
            case EntityType.WoodBurner:
                baseCost = 80;
                break;
            case EntityType.WindTurbine:
                baseCost = 50;
                break;
            case EntityType.BigWindTurbine:
                baseCost = 125;
                break;
            case EntityType.ResearchLab:
                baseCost = 350;
                break;
            case EntityType.SolarPanel:
                baseCost = 110;
                break;
            case EntityType.PowerPole:
                baseCost = 20;
                break;
            case EntityType.BigPowerPole:
                baseCost = 35;
                break;
            case EntityType.Dam:
                baseCost = 150;
                break;
            case EntityType.CharcoalKiln:
                baseCost = 80;
                break;
            case EntityType.CoalPlant:
                baseCost = 200;
                break;
            case EntityType.CoalMine:
                baseCost = 60;
                break;
            case EntityType.UraniumMine:
                baseCost = 140;
                break;
            case EntityType.UraniumMill:
                baseCost = 175;
                break;
            case EntityType.NuclearPlant:
                baseCost = 260;
                break;
            default:
                baseCost = 50;
                break;
        }

        return baseCost;
    }

    public float GetUpgradeCost(EntityType entityType) {
        float baseCost = 0;
        switch (entityType) {
            case EntityType.BigPowerPole:
                baseCost = GetEntityCost(EntityType.BigPowerPole) - GetEntityCost(EntityType.PowerPole);
                break;
        }

        return baseCost;
    }

    public bool IsUpgradeTypeUnlocked(EntityType entityType) {
        switch (entityType) {
            case EntityType.BigPowerPole:
                return research.IsUnlocked(ResearchType.BigPowerPole);
            case EntityType.BigWindTurbine:
                return research.IsUnlocked(ResearchType.BigWindTurbine);
            default:
                return false;
        }
    }

    public void AddRp(float amount) {
        rp += amount;
        rpMeasure += amount;
    }

    public void RemoveRp(float amount) {
        rp -= amount;
        hud.UpdateButtons();
    }

    public void ResearchUnlocked(ResearchType researchType) {
        if (researchType == ResearchType.Nuclear) {
            entities.ReplaceEntities(EntityType.HiddenUraniumDeposit, EntityType.UraniumDeposit);
        }
        else if (researchType == ResearchType.CoalPower) {
            entities.ReplaceEntities(EntityType.HiddenCoalDeposit, EntityType.CoalDeposit);
        }
    }

    public void TryToUpgrade() {
        if (!selectedEntity) {
            return;
        }

        float cost = GetUpgradeCost(selectedEntity.UpgradesInto);
        if (money < cost) {
            return;
        }

        entities.ReplaceEntity(selectedEntity, selectedEntity.UpgradesInto, true);
        hud.SetSelectedEntity(null);
        selectedEntity = null;
        SetState(GameState.Default);
    }
}