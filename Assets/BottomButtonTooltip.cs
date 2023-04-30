using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BottomButtonTooltip : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI costText;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        //gameObject.SetActive(false);
    }

    public void SetData(EntityButton entityButton, float cost) {
        nameText.text = Entity.GetNameText(entityButton.EntityType);
        costText.text = cost.ToString("C", CultureInfo.InvariantCulture);
        infoText.text = Entity.GetInfoText(entityButton.EntityType);

    }

    void Update() {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float xOffset = 0;
        if (corners[0].x < 0) {
            xOffset = -corners[0].x;
        }

        if (corners[2].x > Screen.width) {
            xOffset = Screen.width - corners[2].x;
        }

        transform.position += new Vector3(xOffset, 0, 0);
    }
}
