using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI infoText;

    private RectTransform rectTransform;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void SetData(string header, string info) {
        headerText.text = header;
        infoText.text = info;
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