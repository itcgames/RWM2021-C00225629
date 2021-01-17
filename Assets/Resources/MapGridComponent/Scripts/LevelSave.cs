using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public int m_mapWidth;  //The width of the map.
    public int m_mapHeight; //The height of the map.

    //List of columns of the names of the sprites used for each tile with the map.
    public List<ColumnWrapper> m_spriteNames = new List<ColumnWrapper>();

    //List of all the sprite paths used for the sprites with the level.
    public List<string> m_tileSpritePaths = new List<string>();

    //List of the diffrent map objects used with the level editor.
    public List<SaveObjectData> m_objectsData = new List<SaveObjectData>();

    //List of all the Gameobject that have been placed.
    public List<PlacedObjectData> m_placedObjects = new List<PlacedObjectData>();
}

[System.Serializable]
public class ColumnWrapper
{
    public List<string> m_spriteNames = new List<string>();
}

[System.Serializable]
public class SaveObjectData
{
    public string m_name;
    public string m_prefabPath;

    public int m_width;
    public int m_height;
}

[System.Serializable]
public class PlacedObjectData
{
    public string m_name;
    public Vector2 m_position;
}

public class LevelSave
{
    static string s_jsonPath = "C:/Users/Krystian/Desktop/4th Year Project/RWM2021-C00225629/Assets/Resources/MapGridComponent/";

    static public void SaveLevel(Map t_map, string t_fileName, Dictionary<string, Sprite> t_tileSprites, Dictionary<string, MapObject> t_mapObjects)
    {
        LevelSaveData saveData = new LevelSaveData();

        saveData.m_mapWidth = t_map.m_width;
        saveData.m_mapHeight = t_map.m_hight;

        foreach (KeyValuePair<string, Sprite> tileSprite in t_tileSprites)
        {
            saveData.m_tileSpritePaths.Add(tileSprite.Key);
        }

        for (int x = 0; x < saveData.m_mapWidth; x++)
        {
            ColumnWrapper col = new ColumnWrapper();

            for (int y = 0; y < saveData.m_mapHeight; y++)
            {
                col.m_spriteNames.Add(t_map.GetTile(new MapIndex(x, y)).GetSprite().name);
            }

            saveData.m_spriteNames.Add(col);
        }

        foreach (KeyValuePair<string, MapObject> mapObject in t_mapObjects)
        {
            SaveObjectData saveMapPrefabData = new SaveObjectData();
            saveMapPrefabData.m_name = mapObject.Value.m_name;
            saveMapPrefabData.m_prefabPath = mapObject.Value.m_prefabPath;
            saveMapPrefabData.m_width = mapObject.Value.m_width;
            saveMapPrefabData.m_height = mapObject.Value.m_height;

            saveData.m_objectsData.Add(saveMapPrefabData);
        }

        List<GameObject> placedPrefabs = t_map.GetAllEnteties();

        foreach(GameObject placedPrefab in placedPrefabs)
        {
            PlacedObjectData mapPrefab = new PlacedObjectData();

            mapPrefab.m_name = placedPrefab.name;
            mapPrefab.m_position = placedPrefab.transform.position;

            saveData.m_placedObjects.Add(mapPrefab);
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        string fullPath = s_jsonPath + t_fileName + ".json";

        System.IO.File.WriteAllText(fullPath, jsonData);
    }

    static public LevelSaveData LoadLevel(string t_fileName)
    {
        string fullPath = s_jsonPath + t_fileName + ".json";

        if (System.IO.File.Exists(fullPath))
        {
            LevelSaveData loadedLevel = JsonUtility.FromJson<LevelSaveData>(System.IO.File.ReadAllText(fullPath));

            return loadedLevel;
        }

        return null;
    }
}