using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusOverlay : MonoBehaviour {
    [SerializeField] private SpriteRenderer circlePrefab;

    private Entities entities;
    private Dictionary<Entity, SpriteRenderer> visibleCollRadiuses = new Dictionary<Entity, SpriteRenderer>();
    private Dictionary<Entity, SpriteRenderer> visibleConnRadiuses = new Dictionary<Entity, SpriteRenderer>();

    void Awake() {
        entities = FindObjectOfType<Entities>();
    }

    public void ShowRadius(Entity entity, bool collision, bool connection) {
        if (collision) {
            ShowRadius(entity, visibleCollRadiuses, entity.CollisionRadius, new Color(1, 0, 0, 0.5f));
        }

        if (connection) {
            ShowRadius(entity, visibleConnRadiuses, entity.ConnectionRadius, new Color(1,1,1, 0.1f));
        }
    }

    private void ShowRadius(Entity entity, Dictionary<Entity, SpriteRenderer> dict, float radius, Color color) {
        SpriteRenderer circle = null;
        if (!dict.TryGetValue(entity, out circle)) {
            circle = Instantiate(circlePrefab);
            dict[entity] = circle;
        }

        circle.gameObject.SetActive(true);
        circle.color = color;
        circle.transform.position = entity.transform.position;
        circle.transform.localScale = Vector3.one * radius * 2;
    }

    public void Hide(bool collision = true, bool connection = true) {
        if (collision) {
            Hide(visibleCollRadiuses);
        }

        if (connection) {
            Hide(visibleConnRadiuses);
        }
    }

    private void Hide(Dictionary<Entity, SpriteRenderer> dict) {
        foreach (var kv in dict) {
            kv.Value.gameObject.SetActive(false);
        }
    }
}