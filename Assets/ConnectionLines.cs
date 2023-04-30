using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLines : MonoBehaviour {
    [SerializeField] private ConnectionLine prefab;
    private List<ConnectionLine> lines = new List<ConnectionLine>();
    private ConnectionLine pendingLine = null;

    public void SetPendingLine(Entity entity, Vector3 endPos, Color color) {
        if (!pendingLine) {
            pendingLine = Instantiate(prefab);
        }
        pendingLine.gameObject.SetActive(true);
        pendingLine.SetInEntity(entity);
        pendingLine.SetEndPos(endPos);
        pendingLine.SetColor(color);
    }

    public void RemovePendingLine() {
        pendingLine.gameObject.SetActive(false);
    }

    public ConnectionLine Find(Entity inEntity, Entity outEntity) {
        foreach (var connectionLine in lines) {
            if (connectionLine.InEntity == inEntity && connectionLine.OutEntity == outEntity) {
                return connectionLine;
            }
        }

        return null;
    }
    
    public void DrawConnectionLine(Entity inEntity, Entity outEntity) {
        var line = Find(inEntity, outEntity);
        if (!line) {
            line = Instantiate(prefab);
            lines.Add(line);
        }
        line.Set(inEntity, outEntity, Color.gray);
    }

    public void RemoveConnectionLine(Entity inEntity, Entity outEntity) {
        for (int i = 0; i < lines.Count; ++i) {
            var line = lines[i];
            if (line.InEntity == inEntity && line.OutEntity == outEntity) {
                Destroy(line.gameObject);
                lines.RemoveAt(i);
                return;
            }
        }
        
        
    }

}