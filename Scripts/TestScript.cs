using EasyAttributes;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Preview]
    public Texture2D dungeonPreview;
    public Vector2Int dungeonSize;
    private Room[,] dungeon;

    [Button]
    public void GenerateDungeon()
    {
        dungeon = DungeonCreator.GenerateDungeon(dungeonSize);
        GeneratePreviewTexture();
    }

    [Button]
    public void ColorSpawnPoints()
    {
        var spawnPoints = DungeonCreator.GetSpawnPoints(4, dungeon);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Debug.Log("Set spawnpoint!");
            dungeonPreview.SetPixel(spawnPoints[i].x, spawnPoints[i].y, Color.grey);
        }

        dungeonPreview.Apply();
    }

    private void GeneratePreviewTexture()
    {
        Color[] pix = new Color[dungeonSize.x * dungeonSize.y];
        dungeonPreview = new Texture2D(dungeonSize.x, dungeonSize.y);

        for (int y = 0; y < dungeonSize.y; y++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                pix[y * dungeonSize.x + x] = GetRoomColor(dungeon[x, y]);
            }
        }

        dungeonPreview.filterMode = FilterMode.Point;
        dungeonPreview.SetPixels(pix);
        dungeonPreview.Apply();
    }

    private Color GetRoomColor(Room room)
    {
        if ((room.possibleDirections | Direction.None) == Direction.None)
        {
            return Color.black;
        }

        if(room.treasureAmount > 0)
        {
            if (room.containsMonster)
            {
                return Color.magenta;
            } else if (room.containsExit)
            {
                return Color.blue;
            }
            return Color.yellow;
        }

        if (room.containsMonster)
        {
            return Color.red;
        }

        if (room.containsExit)
        {
            return Color.green;
        }

        return Color.white;
    }
}
