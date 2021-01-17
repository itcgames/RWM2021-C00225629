using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MapObject
{
    public string m_name;
    public string m_prefabPath;

    public Sprite m_image;

    public int m_width;
    public int m_height;
}

public class MapEditor : MonoBehaviour
{
    public LevelEditor m_levelEditor;

    public Map m_map;

    public Dictionary<string, MapObject> m_mapObjects = new Dictionary<string, MapObject>();
    public Dictionary<string, Sprite> m_tileSprites = new Dictionary<string, Sprite>();

    public void ChangeTileSprite(MapIndex t_mapIndex, Sprite t_sprite)
    {
        Tile tile = m_map.GetTile(t_mapIndex);

        if (tile != null)
        {
            tile.SetSprite(t_sprite);
        }
    }

    public bool CheckCanPlaceMapObject(MapIndex t_mapIndex, MapObject t_mapObject)
    {
        for (int x = 0; x < t_mapObject.m_width; x++)
        {
            for (int y = 0; y < t_mapObject.m_height; y++)
            {
                if (m_map.GetIsOutOfBounds(new MapIndex(t_mapIndex.m_x + x, t_mapIndex.m_y + y)))
                {
                    return false;
                }

                else if (!m_map.GetIsTileEmpty(new MapIndex(t_mapIndex.m_x + x, t_mapIndex.m_y + y)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void RemoveMapObject(MapIndex t_mapIndex)
    {
        if (!m_map.GetIsOutOfBounds(t_mapIndex))
        {
            if (!m_map.GetIsTileEmpty(t_mapIndex))
            {
                List<GameObject> mapPrefabs = m_map.GetEntity(t_mapIndex);

                GameObject mapPrefab = mapPrefabs[0];

                m_map.RemoveEntityFromAllTiles(mapPrefab);

                Destroy(mapPrefab);
            }
        }
    }

    public void InstantiateMapObject(MapIndex t_startMapIndex, MapObject t_mapObject)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = t_mapObject.m_image;
        gameObject.GetComponent<SpriteRenderer>().spriteSortPoint = SpriteSortPoint.Pivot;
        gameObject.tag = "MapObject";
        gameObject.name = t_mapObject.m_name;

        Vector2 positionOffset = new Vector2(m_map.m_tileSize * t_mapObject.m_width / 2,
            m_map.m_tileSize * t_mapObject.m_height / 2);

        gameObject.transform.position = m_map.MapIndexToWorldPos(t_startMapIndex) + positionOffset;

        for (int x = 0; x < t_mapObject.m_width; x++)
        {
            for (int y = 0; y < t_mapObject.m_height; y++)
            {
                m_map.AddEntity(new MapIndex(t_startMapIndex.m_x + x, t_startMapIndex.m_y + y), gameObject);
            }
        }
    }

    public void InstantiateRealObject(MapIndex t_startMapIndex, MapObject t_mapObject)
    {
        GameObject loadedObject = AssetDatabase.LoadAssetAtPath(t_mapObject.m_prefabPath, typeof(GameObject)) as GameObject;

        Vector2 positionOffset = new Vector2(m_map.m_tileSize * t_mapObject.m_width / 2,
           m_map.m_tileSize * t_mapObject.m_height / 2);

        loadedObject.transform.position = m_map.MapIndexToWorldPos(t_startMapIndex) + positionOffset;

        for (int x = 0; x < t_mapObject.m_width; x++)
        {
            for (int y = 0; y < t_mapObject.m_height; y++)
            {
                m_map.AddEntity(new MapIndex(t_startMapIndex.m_x + x, t_startMapIndex.m_y + y), loadedObject);
            }
        }
    }

    public void CreateMap(int t_mapWidth, int t_mapHeight)
    {
        if (m_map != null)
        {
            GameObject[] prefabs = GameObject.FindGameObjectsWithTag("MapObject");

            for (int i = 0; i < prefabs.Length; i++)
            {
                m_map.RemoveEntityFromAllTiles(prefabs[i]);
                Destroy(prefabs[i]);
            }

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        m_map.SetSize(t_mapWidth, t_mapHeight);
        m_map.CreateMap();

        Vector3 cameraPos = Camera.main.transform.position;

        cameraPos.x = t_mapWidth / 2.0f * m_map.m_tileSize;
        cameraPos.y = t_mapHeight / 2.0f * m_map.m_tileSize;

        Camera.main.transform.position = cameraPos;
    }

    public bool LoadtTileSprite(string t_spritePath)
    {
        if(!m_tileSprites.ContainsKey(t_spritePath))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath(t_spritePath, typeof(Sprite)) as Sprite;

            if (sprite != null)
            {
                m_tileSprites.Add(t_spritePath, sprite);

                return true;
            }
        }

        return false;
    }

    public bool LoadMapObject(string t_fileName, int t_width, int t_height)
    {
        string path = "Assets/Resources/MapGridComponent/Prefabs/" + t_fileName + ".prefab";

        GameObject mapObjectLoaded = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

        if(!m_mapObjects.ContainsKey(t_fileName))
        {
            if (mapObjectLoaded != null)
            {
                MapObject mapObject = new MapObject();
                mapObject.m_name = t_fileName;
                mapObject.m_prefabPath = path;
                mapObject.m_image = mapObjectLoaded.GetComponent<SpriteRenderer>().sprite;
                mapObject.m_width = t_width;
                mapObject.m_height = t_height;

                m_mapObjects.Add(mapObject.m_name, mapObject);

                return true;
            }
        }

        return false;
    }

    public bool SaveLevel(string t_fileName)
    {
        LevelSave.SaveLevel(m_map, t_fileName, m_tileSprites, m_mapObjects);

        return true;
    }

    public bool LoadLevel(string t_fileName, bool t_loadRealObjects)
    {
        if (t_fileName != "")
        {
            LevelSaveData saveData = LevelSave.LoadLevel(t_fileName);

            if(saveData != null)
            {
                ClearAllData();
                CreateMap(saveData.m_mapWidth, saveData.m_mapHeight);

                foreach (string path in saveData.m_tileSpritePaths)
                {
                    LoadtTileSprite(path);

                    if (m_levelEditor != null)
                    {
                        m_levelEditor.CreateSpriteButton(path);
                    }
                }

                for (int x = 0; x < saveData.m_spriteNames.Count; x++)
                {
                    ListWrapper wrapperClass = saveData.m_spriteNames[x];

                    for (int y = 0; y < wrapperClass.m_spriteNames.Count; y++)
                    {
                        Sprite sprite = m_map.GetTile(new MapIndex(x, y)).GetSprite();

                        foreach (string path in m_tileSprites.Keys)
                        {
                            if (path.Contains(wrapperClass.m_spriteNames[y]))
                            {
                                sprite = m_tileSprites[path];
                                break;
                            }
                        }

                        m_map.GetTile(new MapIndex(x, y)).SetSprite(sprite);
                    }
                }

                for (int i = 0; i < saveData.m_objectsData.Count; i++)
                {
                    LoadMapObject(saveData.m_objectsData[i].m_name,
                        saveData.m_objectsData[i].m_width,
                        saveData.m_objectsData[i].m_height);

                    if (m_levelEditor != null)
                    {
                        m_levelEditor.CreateObjectButton(m_mapObjects[saveData.m_objectsData[i].m_name]);
                    }
                }

                for (int i = 0; i < saveData.m_placedObjects.Count; i++)
                {
                    if (m_mapObjects.ContainsKey(saveData.m_placedObjects[i].m_name))
                    {
                        MapObject mapObject = m_mapObjects[saveData.m_placedObjects[i].m_name];

                        Vector2 position = saveData.m_placedObjects[i].m_position;

                        position.x = position.x - (mapObject.m_width * m_map.m_tileSize) / 2;
                        position.y = position.y - (mapObject.m_height * m_map.m_tileSize) / 2;

                        MapIndex mapIndex = m_map.WorldPositionToMapIndex(position);

                        if (t_loadRealObjects)
                        {
                            InstantiateRealObject(mapIndex, mapObject);
                        }

                        else
                        {
                            InstantiateMapObject(mapIndex, mapObject);
                        }
                    }
                }

                return true;
            } 
        }

        return false;
    }

    public void ClearAllData()
    {
        m_tileSprites = new Dictionary<string, Sprite>();
        m_mapObjects = new Dictionary<string, MapObject>();

        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("MapObject");

        for(int i = 0; i < mapObjects.Length; i++)
        {
            m_map.RemoveEntityFromAllTiles(mapObjects[i]);
            Destroy(mapObjects[i]);
        }
    }
}