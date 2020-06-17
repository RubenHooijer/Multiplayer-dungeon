using UnityEngine;

[CreateAssetMenu(fileName ="New Room", menuName ="Custom/New Room", order =1)]
public class RoomObject : ScriptableObject
{
    [Header("Room")]
    public GameObject roomPrefab;
    public Direction availableDirections;

    [Header("Camera")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
}