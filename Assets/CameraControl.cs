using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    private Camera cam;
    private bool dragging;
    private Vector3 dragStart;
    private Vector3 dragStartWorld;
    private Vector3 dragOffset;

    void Awake() {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            var viewPos = cam.ScreenToViewportPoint(Input.mousePosition);
            viewPos.z = 0;
            var worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            dragging = true;
            dragStart = viewPos;
            dragStartWorld = worldPos;
            dragOffset = worldPos - cam.transform.position;
        }

        if (dragging) {
            var viewPos = cam.ScreenToViewportPoint(Input.mousePosition);
            var diff = viewPos - dragStart;

            var viewToWorldRatio = cam.ViewportToWorldPoint(Vector3.one) - cam.ViewportToWorldPoint(Vector3.zero);
            viewToWorldRatio.z = 0;

            var diffWorld = diff;
            diffWorld.Scale(viewToWorldRatio);
            cam.transform.position = dragStartWorld - diffWorld - dragOffset;
        }

        if (Input.GetMouseButtonUp(1)) {
            dragging = false;
        }

        const float zoomSpeed = 0.1f;
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0) {
            cam.orthographicSize *= (1 - zoomSpeed);
        }
        else if (scroll < 0) {
            cam.orthographicSize *= (1 / (1 - zoomSpeed));
        }
    }
}