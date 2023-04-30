using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {
    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    public void SetCanAfford(bool canAfford) {
        button.interactable = canAfford;
    }
}
