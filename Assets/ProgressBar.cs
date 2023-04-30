using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {
    [SerializeField] private GameObject bar;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Gradient gradient;

    private void Start() {
        SetProgress(0);
    }

    public void SetProgress(float val) {
        float scaled = Mathf.Min(1.0f, val * 0.5f);
        bar.transform.localScale = new Vector3(1, scaled, 1);
        sprite.color = gradient.Evaluate(scaled);
    }
}