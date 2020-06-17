using System.Collections.Generic;
using UnityEngine;

public static class DungeonCreator
{
    private const float MIN_GAPS = 0.15f;
    private const float MAX_GAPS = 0.20f;

    private const float MIN_TREASURE = 0.05f;
    private const float MAX_TREASURE = 0.11f;

    private const float MIN_ENEMY = 0.06f;
    private const float MAX_ENEMY = 0.10f;

    private const ushort MIN_TREASURE_WORTH = 125;
    private const ushort MAX_TREASURE_WORTH = 550;


    public static Room[,] GenerateDungeon(Vector2Int dungeonSize)
    {
        Room[,] dungeon = new Room[dungeonSize.x, dungeonSize.y];

        int dungeonArea = dungeonSize.x * dungeonSize.y;

        int gaps = (int) (Random.Range(MIN_GAPS, MAX_GAPS) * dungeonArea);
        int treasure = (int) (Random.Range(MIN_TREASURE, MAX_TREASURE) * dungeonArea);
        int enemies = (int) (Random.Range(MIN_ENEMY, MAX_ENEMY) * dungeonArea);

        GenerateGaps(gaps, dungeonSize, ref dungeon);
        SetDirections(dungeonSize, ref dungeon);
        GenerateTreasure(treasure, dungeonSize, ref dungeon);
        GenerateEnemies(enemies, dungeonSize, ref dungeon);
        GenerateExit(dungeonSize, ref dungeon);

        return dungeon;
    }

    public static List<Vector2Int> GetSpawnPoints(int amountOfPlayers, Room[,] dungeon)
    {
        List<Vector2Int> spawnPoints = new List<Vector2Int>();

        //Only spawn players in the bottom one third of the dungeon
        int oneThirdYSize = Mathf.RoundToInt(dungeon.GetLength(1) * 0.33f);
        Vector2Int bottomSize = new Vector2Int(dungeon.GetLength(0), dungeon.GetLength(1));
        bottomSize.y = oneThirdYSize;

        int spawnPointsSet = 0;

        while (spawnPointsSet < amountOfPlayers)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, bottomSize.x), Random.Range(0, bottomSize.y));
            Room room = dungeon[randomPos.x, randomPos.y];
            if (
                room.isGap ||
                room.containsMonster
                ) continue;

            spawnPoints.Add(randomPos);
            spawnPointsSet++;
        }

        return spawnPoints;
    }

    private static void GenerateGaps(int amountOfGaps, Vector2Int dungeonSize, ref Room[,] dungeon)
    {
        int gapsSet = 0;

        while (gapsSet < amountOfGaps)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, dungeonSize.x), Random.Range(0, dungeonSize.y));
            if (dungeon[randomPos.x, randomPos.y].isGap) continue;
            dungeon[randomPos.x, randomPos.y].isGap = true;
            gapsSet++;
        }
    }

    private static void SetDirections(Vector2Int dungeonSize, ref Room[,] dungeon)
    {
        for (int y = 0; y < dungeonSize.y; y++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                dungeon[x, y].possibleDirections = CalculateDirections(new Vector2Int(x, y), dungeonSize, dungeon);
            }
        }
    }

    private static void GenerateTreasure(int amountOfTreasure, Vector2Int dungeonSize, ref Room[,] dungeon)
    {
        int treasureSet = 0;

        while (treasureSet < amountOfTreasure)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, dungeonSize.x), Random.Range(0, dungeonSize.y));
            if (dungeon[randomPos.x, randomPos.y].isGap) continue;
            if (!CanPlaceTreasure(randomPos, dungeonSize, dungeon)) continue;
            dungeon[randomPos.x, randomPos.y].treasureAmount = CalculateTreasureWorth();
            treasureSet++;
        }
    }

    private static void GenerateEnemies(int amountOfEnemies, Vector2Int dungeonSize, ref Room[,] dungeon)
    {
        int enemiesSet = 0;

        while (enemiesSet < amountOfEnemies)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, dungeonSize.x), Random.Range(0, dungeonSize.y));
            if (dungeon[randomPos.x, randomPos.y].isGap) continue;
            dungeon[randomPos.x, randomPos.y].containsMonster = true;
            enemiesSet++;
        }
    }

    private static void GenerateExit(Vector2Int dungeonSize, ref Room[,] dungeon)
    {
        //Get the top third part of the dungeon
        int oneThirdYSize = Mathf.RoundToInt(dungeonSize.y * 0.33f);

        Vector2Int topSize = dungeonSize;
        topSize.y = oneThirdYSize * 2;

        while (true)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, dungeonSize.x), Random.Range(topSize.y, dungeonSize.y));
            Room room = dungeon[randomPos.x, randomPos.y];
            if (room.isGap || room.containsMonster || !CanPlaceExit(randomPos, dungeonSize, dungeon)) continue;
            dungeon[randomPos.x, randomPos.y].containsExit = true;
            break;
        }
    }

    private static Direction CalculateDirections(Vector2Int pos, Vector2Int dungeonSize, Room[,] dungeon)
    {
        if (dungeon[pos.x, pos.y].isGap) return Direction.None;

        Direction direction = Direction.None;

        var neighbours = GetNeighbours(pos, dungeonSize);

        for (int i = 0; i < neighbours.Count; i++)
        {
            bool isRoomEmpty = dungeon[neighbours[i].x, neighbours[i].y].isGap;

            if (neighbours[i].y > pos.y && !isRoomEmpty)
            {
                direction |= Direction.up;
                continue;
            }

            if(neighbours[i].y < pos.y && !isRoomEmpty)
            {
                direction |= Direction.down;
                continue;
            }

            if (neighbours[i].x > pos.x && !isRoomEmpty)
            {
                direction |= Direction.right;
                continue;
            }

            if (neighbours[i].x < pos.x && !isRoomEmpty)
            {
                direction |= Direction.left;
                continue;
            }
        }

        return direction;
    }

    private static ushort CalculateTreasureWorth()
    {
        ushort worth = (ushort)Random.Range(MIN_TREASURE_WORTH, MAX_TREASURE_WORTH);

        ushort temp = (ushort)(worth % 25);
        worth += temp; //Round to nearest 25

        return worth;
    }

    private static bool CanPlaceTreasure(Vector2Int pos, Vector2Int dungeonSize, Room[,] dungeon)
    {
        var neightbours = GetNeighbours(pos, dungeonSize);
        for (int i = 0; i < neightbours.Count; i++)
        {
            if(dungeon[neightbours[i].x, neightbours[i].y].treasureAmount >= MIN_TREASURE_WORTH)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CanPlaceExit(Vector2Int pos, Vector2Int dungeonSize, Room[,] dungeon)
    {
        var neightbours = GetNeighbours(pos, dungeonSize);
        int amountOfGaps = 0;

        for (int i = 0; i < neightbours.Count; i++)
        {
            if (dungeon[neightbours[i].x, neightbours[i].y].isGap)
            {
                amountOfGaps++;
            }
        }

        if(amountOfGaps > 2)
        {
            return false;
        }
        return true;
    }

    private static List<Vector2Int> GetNeighbours(Vector2Int pos, Vector2Int dungeonSize, bool directNeighbours = true)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int xPos = pos.x + x;
                int yPos = pos.y + y;

                if (
                    xPos < 0 ||
                    xPos >= dungeonSize.x ||
                    yPos < 0 ||
                    yPos >= dungeonSize.y ||
                    (directNeighbours && (Mathf.Abs(x) == Mathf.Abs(y))))
                {
                    continue;
                }

                neighbours.Add(new Vector2Int(xPos, yPos));
            }
        }

        return neighbours;
    }
}
