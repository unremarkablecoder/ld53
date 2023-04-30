using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entities : MonoBehaviour {
    [SerializeField] private Entity[] prefabs;
    private List<Entity> entities = new List<Entity>();

    private Entity pendingEntity = null;
    private Entity startEntity = null;
    private Entity endEntity = null;

    private ConnectionLines connectionLines;
    private Vector3 lastConnectionPos;

    private List<Entity> cities = new List<Entity>();
    
    void Awake() {
        connectionLines = FindObjectOfType<ConnectionLines>();
    }

    // Start is called before the first frame update
    void Start() {
        entities = FindObjectsOfType<Entity>().ToList();
        foreach (var entity in entities) {
            if (entity.EntityType == EntityType.City) {
                cities.Add(entity);
            }
        }
    }

    public void FixedUpdate() {
        float dt = Time.fixedDeltaTime;
        foreach (var entity in entities) {
            entity.Process(dt);
        }
    }

    public Entity FindClosestEntity(Vector3 worldPos, bool withinCollRadius = false) {
        float clostestDistSq = float.MaxValue;
        Entity closestEntity = null;
        foreach (var entity in entities) {
            var distSq = (worldPos - entity.transform.position).sqrMagnitude;
            if (distSq < clostestDistSq) {
                if (!withinCollRadius || distSq < entity.CollisionRadius * entity.CollisionRadius) {
                    closestEntity = entity;
                    clostestDistSq = distSq;
                }
            }
        }

        return closestEntity;
    }

    public Entity FindClosestEntityToConnectTo(Vector3 worldPos, ResType inputType = ResType.None) {
        float clostestDistSq = float.MaxValue;
        Entity closestEntity = null;
        foreach (var entity in entities) {
            var distSq = (worldPos - entity.transform.position).sqrMagnitude;
            if (distSq < clostestDistSq) {
                if (distSq < entity.ConnectionRadius * entity.CollisionRadius && (inputType == ResType.None || inputType == entity.InputType)) {
                    closestEntity = entity;
                    clostestDistSq = distSq;
                }
            }
        }

        return closestEntity;
    }

    public List<Entity> FindEntitiesWithinConnection(Vector3 worldPos, float radius, ResType inputType, ResType outputType) {
        List<Entity> results = new List<Entity>();
        foreach (var entity in entities) {
            var distSq = (worldPos - entity.transform.position).sqrMagnitude;
            float totalRadius = entity.ConnectionRadius + radius;
            if (distSq < totalRadius * totalRadius &&
                ((inputType == ResType.None || inputType == entity.InputType) || (outputType == ResType.None || outputType == entity.OutputType))) {
                results.Add(entity);
            }
        }

        return results;
    }

    public List<Entity> FindEntitiesWithinCollision(Vector3 worldPos, float radius, ResType inputType = ResType.None) {
        List<Entity> results = new List<Entity>();
        foreach (var entity in entities) {
            var distSq = (worldPos - entity.transform.position).sqrMagnitude;
            float totalRadius = entity.CollisionRadius + radius;
            if (distSq < totalRadius * totalRadius) {
                if (inputType == ResType.None || inputType == entity.InputType) {
                    results.Add(entity);
                }
            }
        }

        return results;
    }

    public bool CreatePendingEntity(EntityType entityType) {
        if (pendingEntity) {
            CancelPending();
        }

        pendingEntity = Instantiate(prefabs[(int)entityType], transform);
        return true;
    }

    public void UpdatePendingPosition(Vector3 pos) {
        if (pendingEntity) {
            pendingEntity.transform.position = pos;
        }
    }

    public void CancelPending() {
        if (!pendingEntity) {
            return;
        }

        Destroy(pendingEntity.gameObject);
        pendingEntity = null;
    }

    public void PlacePending() {
        if (!pendingEntity) {
            return;
        }

        entities.Add(pendingEntity);
        pendingEntity = null;
        //UpdateConnections();
    }

    public Entity GetPending() {
        return pendingEntity;
    }

    public void StartConnectingEntity(Entity entity) {
        startEntity = entity;
    }

    public void UpdateConnectionPos(Vector3 pos) {
        bool canConnect = false;
        endEntity = null;
        lastConnectionPos = pos;

        var entity = FindClosestEntityToConnectTo(pos, startEntity.OutputType);
        if (entity) {
            var distSq = (entity.transform.position - startEntity.transform.position).sqrMagnitude;
            float totalRadius = entity.ConnectionRadius + startEntity.ConnectionRadius;
            pos = entity.transform.position;
            if (distSq <= totalRadius * totalRadius) {
                canConnect = true;
                endEntity = entity;
            }
            else {
                canConnect = false;
            }

            startEntity.DrawPendingConnection(pos, canConnect ? Color.green : Color.red);
        }
        else {
            startEntity.DrawPendingConnection(pos, Color.red);
        }
    }

    public Entity ApplyOrCancelConnection() {
        Entity selectedEntity = null;
        //if over start, return it to select it
        float distSq = (lastConnectionPos - startEntity.transform.position).sqrMagnitude;
        if (distSq < startEntity.CollisionRadius * startEntity.CollisionRadius && startEntity.Selectable) {
            selectedEntity = startEntity;
        }

        if (startEntity && endEntity) {
            startEntity.AddOutConnection(endEntity);
            endEntity.AddInConnection(startEntity);
        }

        startEntity = null;
        endEntity = null;
        connectionLines.RemovePendingLine();
        return selectedEntity;
    }

    public void SellEntity(Entity entity) {
        Destroy(entity.gameObject);
        entities.Remove(entity);
    }

    public void ReplaceEntity(Entity entity, EntityType newType, bool copyConnections) {
        int index = entities.IndexOf(entity);
        var newEntity = Instantiate(prefabs[(int)newType], transform);
        newEntity.transform.position = entity.transform.position;

        foreach (var conn in entity.InConnections) {
            newEntity.AddInConnection(conn);
            conn.AddOutConnection(newEntity);
        }

        foreach (var conn in entity.OutConnections) {
            newEntity.AddOutConnection(conn);
            conn.AddInConnection(newEntity);
        }

        entity.RemoveConnections();

        entities[index] = newEntity;
        Destroy(entity.gameObject);
    }

    public void ReplaceEntities(EntityType sourceType, EntityType destType) {
        for (int i = 0; i < entities.Count; ++i) {
            var src = entities[i];
            if (src.EntityType == sourceType) {
                ReplaceEntity(src, destType, false);
            }
        }
    }

    public List<Entity> GetCities() {
        return cities;
    }
}