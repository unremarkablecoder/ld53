using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour {
    [SerializeField] private GameObject pivot;
    [SerializeField] private SpriteRenderer sprite;

    public Entity InEntity;
    public Entity OutEntity;

    public void SetInEntity(Entity inEntity) {
        InEntity = inEntity;
        transform.position = inEntity.transform.position;
    }

    public void SetEndPos(Vector3 pos) {
        var diff = pos - transform.position;
        float rad = Mathf.Atan2(diff.normalized.y, diff.normalized.x);
        float deg = rad * Mathf.Rad2Deg;
        pivot.transform.localScale = new Vector3(diff.magnitude, 1, 1);
        pivot.transform.rotation = Quaternion.Euler(0, 0, deg);
    }

    public void SetColor(Color color) {
        sprite.color = color;
    }

    public void Set(Entity inEntity, Entity outEntity, Color color) {
        SetInEntity(inEntity);
        OutEntity = outEntity;
        SetEndPos(outEntity.transform.position);
        SetColor(color);
    }
}