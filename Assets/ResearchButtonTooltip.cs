using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ResearchButtonTooltip : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI costText;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        //gameObject.SetActive(false);
    }

    public void SetData(ResearchButton button, float cost) {
        nameText.text = Research.GetNameText(button.ResearchType);
        costText.text = cost.ToString("F", CultureInfo.InvariantCulture) + " RP";
        infoText.text = Research.GetInfoText(button.ResearchType);
    }

    void Update() {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float xOffset = 0;
        float yOffset = 0;
        if (corners[0].x < 0) {
            xOffset = -corners[0].x;
        }

        if (corners[1].y > Screen.height) {
            yOffset = Screen.height - corners[1].y;
        }

        if (corners[0].y < 0) {
            yOffset = -corners[0].y;
        }

        if (corners[2].x > Screen.width) {
            xOffset = Screen.width - corners[2].x;
        }

        transform.position += new Vector3(xOffset, yOffset, 0);
    }
}