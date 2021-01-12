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

    public List<SaveMapPrefabData> m_mapPrefabsData = new List<SaveMapPrefabData>();
    public List<PlacedPrefabData> m_placedMapPrefabs = new List<PlacedPrefabData>();


}

[System.Serializable]
public class ListWrapper
{
    public List<string> m_spriteNames = new List<string>();
}

[System.Serializable]
public class SaveMapPrefabData
{
    public string m_name;
    public string m_prefabPath;

    public int m_width;
    public int m_height;
}

[System.Serializable]
public class PlacedPrefabData
{
    public string m_name;
    public Vector2 m_position;
}

public class LevelSave
{
    static string m_jsonPath = "C:/Users/Krystian/Desktop/4th Year Project/RWM2021-C00225629/Assets/Resources/MapGridComponent/";

    static public void SaveLevel(Map t_map, string t_fileName, List<string> t_spritePaths, List<MapPrefabData> t_fakeMapPrefabs)
    {
        LevelSaveData saveData = new LevelSaveData();

        saveData.m_mapWidth = t_map.m_width;
        saveData.m_mapHeight = t_map.m_hight;
        saveData.m_tileSpritePaths = t_spritePaths;

        for (int x = 0; x < saveData.m_mapWidth; x++)
        {
            ListWrapper col = new ListWrapper();

            for (int y = 0; y < saveData.m_mapHeight; y++)
            {
                col.m_spriteNames.Add(t_map.GetTile(new MapIndex(x, y)).GetSprite().name);
            }

            saveData.m_spriteNames.Add(col);
        }

        foreach (MapPrefabData mapPrefabData in t_fakeMapPrefabs)
        {
            SaveMapPrefabData saveMapPrefabData = new SaveMapPrefabData();
            saveMapPrefabData.m_name = mapPrefabData.m_name;
            saveMapPrefabData.m_prefabPath = mapPrefabData.m_prefabPath;
            saveMapPrefabData.m_width = mapPrefabData.m_width;
            saveMapPrefabData.m_height = mapPrefabData.m_height;

            saveData.m_mapPrefabsData.Add(saveMapPrefabData);
        }

        List<GameObject> placedPrefabs = t_map.GetAllEnteties();

        foreach(GameObject placedPrefab in placedPrefabs)
        {
            PlacedPrefabData mapPrefab = new PlacedPrefabData();

            mapPrefab.m_name = placedPrefab.name;
            mapPrefab.m_position = placedPrefab.transform.position;

            saveData.m_placedMapPrefabs.Add(mapPrefab);
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        string fullPath = m_jsonPath + t_fileName + ".json";

        System.IO.File.WriteAllText(fullPath, jsonData);
    }
}