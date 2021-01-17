using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public int m_mapWidth;
    public int m_mapHeight;

    public List<ListWrapper> m_spriteNames = new List<ListWrapper>();
    public List<string> m_tileSpritePaths = new List<string>();

    public List<SaveObjectData> m_objectsData = new List<SaveObjectData>();
    public List<PlacedObjectData> m_placedObjects = new List<PlacedObjectData>();
}

[System.Serializable]
public class ListWrapper
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
    static string m_jsonPath = "C:/Users/Krystian/Desktop/4th Year Project/RWM2021-C00225629/Assets/Resources/MapGridComponent/";

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
            ListWrapper col = new ListWrapper();

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
        string fullPath = m_jsonPath + t_fileName + ".json";

        System.IO.File.WriteAllText(fullPath, jsonData);
    }

    static public LevelSaveData LoadLevel(string t_fileName)
    {
        string fullPath = m_jsonPath + t_fileName + ".json";

        if (System.IO.File.Exists(fullPath))
        {
            LevelSaveData loadedLevel = JsonUtility.FromJson<LevelSaveData>(System.IO.File.ReadAllText(fullPath));

            return loadedLevel;
        }

        return null;
    }
}