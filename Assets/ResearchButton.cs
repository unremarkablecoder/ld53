using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchButton : MonoBehaviour {
    public ResearchType ResearchType;
    [SerializeField] private TextMeshProUGUI costText;

    private Research research;
    private Button button;
    private Hud hud;

    void Awake() {
        button = GetComponent<Button>();
        hud = FindObjectOfType<Hud>();
    }

    void Start() {
        research = FindObjectOfType<Research>();

        costText.text = research.GetCost(ResearchType).ToString("F", CultureInfo.InvariantCulture) + " RP";
    }

    public void OnClick() {
        research.TryToBuy(ResearchType);
        if (research.IsUnlocked(ResearchType)) {
            costText.text = "Researched";
        }
    }

    private void Update() {
        button.interactable = research.CanBuy(ResearchType) && !research.IsUnlocked(ResearchType);
    }

    public void OnMouseEnter() {
        hud.ShowResearchTooltip(this);
    }

    public void OnMouseExit() {
        hud.HideResearchTooltip();
    }
}