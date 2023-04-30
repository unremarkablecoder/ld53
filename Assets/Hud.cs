using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour {
    [SerializeField] private GameObject selectedMenu;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI moneyGainText;
    [SerializeField] private TextMeshProUGUI rpText;
    [SerializeField] private TextMeshProUGUI rpGainText;
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private TextMeshProUGUI sunText;
    [SerializeField] private GameObject researchWindow;
    [SerializeField] private GameObject helpWindow;
    [SerializeField] private GameObject winWindow;
    [SerializeField] private BottomButtonTooltip bottomTooltip;
    [SerializeField] private ResearchButtonTooltip researchTooltip;
    [SerializeField] private UpgradeButton upgradeButton;
    [SerializeField] private Tooltip tooltip;

    private Game game;
    private Research research;

    private EntityButton[] entityButtons;

    private float winTime = 0;

    void Awake() {
        game = FindObjectOfType<Game>();
        selectedMenu.SetActive(false);
        entityButtons = FindObjectsOfType<EntityButton>();
        research = FindObjectOfType<Research>();
    }

    public void OnSellButton() {
        game.SellSelected();
    }
    
    public void OnUpgradeButton() {
        game.TryToUpgrade();
    }

    public void OnRemoveConnectionsButton() {
        game.RemoveConnectionsFromSelected();
    }

    private void FixedUpdate() {
        moneyText.text = game.Money.ToString("C", CultureInfo.InvariantCulture);
        rpText.text = game.Rp.ToString("N", CultureInfo.InvariantCulture) + " RP";
        windText.text = game.Wind.ToString("F", CultureInfo.InvariantCulture);
        sunText.text = game.Sun.ToString("F", CultureInfo.InvariantCulture);
    }

    public void SetSelectedEntity(Entity entity) {
        selectedMenu.SetActive(entity);
        if (entity && entity.UpgradesInto != EntityType.None && game.IsUpgradeTypeUnlocked(entity.UpgradesInto)) {
            upgradeButton.gameObject.SetActive(true);
            upgradeButton.SetCanAfford(game.Money >= game.GetUpgradeCost(entity.UpgradesInto));
        }
        else {
            upgradeButton.gameObject.SetActive(false);
        }

    }

    public void SetMoneyGain(float moneyGain) {
        moneyGainText.text = "(" + moneyGain.ToString("C", CultureInfo.InvariantCulture) + "/s)";
    }

    public void SetRpGain(float rpGain) {
        rpGainText.text = "(" + rpGain.ToString("N") + " RP/s)";
    }

    public void UpdateButtons() {
        foreach (var entityButton in entityButtons) {
            entityButton.SetCanAfford(game.Money >= game.GetEntityCost(entityButton.EntityType));
            if (entityButton.ResearchRequired != ResearchType.None) {
                entityButton.gameObject.SetActive(research.IsUnlocked(entityButton.ResearchRequired));
            }
        }
    }

    public void OpenResearchWindow() {
        researchWindow.SetActive(true);
    }

    public void CloseResearchWindow() {
        researchWindow.SetActive(false);
    }

    public void OpenHelpWindow() {
        helpWindow.SetActive(true);
    }

    public void CloseHelpWindow() {
       helpWindow.SetActive(false);
    }

    public void ShowBottomButtonTooltip(EntityButton button) {
        bottomTooltip.gameObject.SetActive(true);
        bottomTooltip.SetData(button, game.GetEntityCost(button.EntityType));
        
        bottomTooltip.gameObject.transform.position = button.transform.position + new Vector3(0, 50, 0);
    }

    public void HideBottomButtonTooltip() {
        bottomTooltip.gameObject.SetActive(false);
    }
    
    public void ShowResearchTooltip(ResearchButton button) {
        researchTooltip.gameObject.SetActive(true);
        researchTooltip.SetData(button, research.GetCost(button.ResearchType));
        
        researchTooltip.gameObject.transform.position = button.transform.position + new Vector3(0, 50, 0);
    }

    public void HideResearchTooltip() {
        researchTooltip.gameObject.SetActive(false);
    }

    public void ShowTooltip(string header, string info, Vector3 pos, float scale = 1) {
        tooltip.gameObject.SetActive(true);
        tooltip.SetData(header, info);
        tooltip.transform.position = pos;
        tooltip.transform.localScale = Vector3.one * scale;
    }

    public void HideTooltip() {
        tooltip.gameObject.SetActive(false);
    }

    public void ToggleMusic() {
        var audio = FindObjectOfType<AudioSource>();
        audio.mute = !audio.mute;
    }

    public void ShowWin() {
        winTime = Time.time;
        winWindow.SetActive(true);
    }

    public void HideWin() {
        if (Time.time - winTime < 2.0f) {
            return;
        }
        winWindow.SetActive(false);
    }
}
