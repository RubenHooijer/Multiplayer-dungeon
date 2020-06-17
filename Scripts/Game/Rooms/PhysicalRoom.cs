using EasyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalRoom : MonoBehaviour
{
    [Header("Room")]
    public Direction availableDirections;
    public Direction whatIsNorth;
    public Transform[] classBasedSpawnPoints; 

    [Header("Camera")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    public Dictionary<PlayerClasses, GameObject> classObject = new Dictionary<PlayerClasses, GameObject>();

    public GameObject SpawnClass(GameObject charObject, PlayerClasses playerClass)
    {
        RemoveClass(playerClass);
        var g = Instantiate(charObject, classBasedSpawnPoints[(int)playerClass].position, classBasedSpawnPoints[(int)playerClass].rotation);
        classObject.Add(playerClass, g);

        return g;
    }

    public void RemoveClass(PlayerClasses playerClass)
    {
        if (!classObject.ContainsKey(playerClass)) return; 
        Destroy(classObject[playerClass]);
        classObject.Remove(playerClass);
    }

    [Button]
    public void ApplyToCamera()
    {
        var t = Camera.main.transform;

        t.position = cameraPosition;
        t.eulerAngles = cameraRotation;
    }
}
