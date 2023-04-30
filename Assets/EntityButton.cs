using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityButton : MonoBehaviour {
    [SerializeField] private EntityType entityType;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private ResearchType researchRequired = ResearchType.None;

    private Game game;
    private Hud hud;

    public EntityType EntityType => entityType;
    public ResearchType ResearchRequired => researchRequired;

    void Awake() {
        game = FindObjectOfType<Game>();
        hud = FindObjectOfType<Hud>();
    }

    private void Start() {
        SetCost(game.GetEntityCost(entityType));
    }

    public void OnButtonClick() {
        game.StartPlacingBuilding(entityType);
    }
    
    public void SetCost(float cost) {
        costText.text = cost.ToString("C", CultureInfo.InvariantCulture);
    }

    public void SetCanAfford(bool canAfford) {
        GetComponent<Button>().interactable = canAfford;
    }

    public void OnMouseEnter() {
        hud.ShowBottomButtonTooltip(this);
    }

    public void OnMouseExit() {
        hud.HideBottomButtonTooltip();
    }
}
