using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MapObject
{
    public string m_name;           //Name of the object.
    public string m_prefabPath;     //Path to the prefab used to create this map object.

    public Sprite m_image;          //The image used by the original prefab.

    public int m_width;             //The width in tiles of the prefab.
    public int m_height;            //The height in tiles of the prefab.
}

public class MapEditor : MonoBehaviour
{
    //The level editor script that displays the data in map editor. Only used in Level Editor scene.
    public LevelEditor m_levelEditor;

    //The map in which will be affected and all objects will be placed into.
    public Map m_map;

    [HideInInspector]
    //Dictionary of map object that can be easily accessed using the name of the map object.
    public Dictionary<string, MapObject> m_mapObjects = new Dictionary<string, MapObject>();

    [HideInInspector]
    //Dictionary of sprites that can be easily accessed using the name of the sprite.
    public Dictionary<string, Sprite> m_tileSprites = new Dictionary<string, Sprite>();

    /// <summary>
    /// Changes the sprite of the Tile at the passed in map index posiiton.
    /// </summary>
    /// <param name="t_mapIndex">The map index position of the Tile to change</param>
    /// <param name="t_sprite">The new sprite to which the Tile will be set to</param>
    public void ChangeTileSprite(MapIndex t_mapIndex, Sprite t_sprite)
    {
        Tile tile = m_map.GetTile(t_mapIndex);

        if (tile != null)
        {
            tile.SetSprite(t_sprite);
        }
    }

    /// <summary>
    /// Checks if a map object can be placed in the area starting with the passed
    /// map index position as the bottom left corent of the map object if it bigger
    /// that a 1x1 size.
    /// </summary>
    /// <param name="t_mapIndex">The map index position for the Tile to be check first</param>
    /// <param name="t_mapObject">The map object for which we want to check if it can be placed</param>
    /// <returns>bool for if the map object can be placed at the passed in location</returns>
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

    /// <summary>
    /// Removes an map object from a Tile at the passed in map index position.
    /// If an object is found it that Tile it is removed from all tiles and destoryed.
    /// </summary>
    /// <param name="t_mapIndex">The map index position of the Tile we want to remove object from</param>
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

    /// <summary>
    /// Creates a Gameobject instance of the map object using the sprite it contains.
    /// This Gameobject does not other than serve as a visual representation for the user.
    /// </summary>
    /// <param name="t_startMapIndex">The start index position of the Tile where the Gameobject will be created</param>
    /// <param name="t_mapObject">The map object who's data will be used to create Gameobject</param>
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

    /// <summary>
    /// Instanciates the a prefab of a Gameobject using the path from the map object.
    /// The instants of the prefab will behave as normal and will be centred starthing with
    /// the passed in map index position as the bottom left corner based on the deimensions
    /// with the map object.
    /// </summary>
    /// <param name="t_startMapIndex">The start index position of the Tile where the Gameobject will be created</param>
    /// <param name="t_mapObject">The map object who's data will be used to create Gameobject</param>
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

    /// <summary>
    /// Creates a new instance of map using the passed width and height
    /// as its dimensions and centering the camera. If there was a map already
    /// created it deletes all the mapObject that could exist in the scene and
    /// all the Tile objects from the previous map.
    /// </summary>
    /// <param name="t_mapWidth">The width of the new map</param>
    /// <param name="t_mapHeight">The height of the new map</param>
    public void CreateMap(int t_mapWidth, int t_mapHeight)
    {
        if (m_map != null)
        {
            GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("MapObject");

            for (int i = 0; i < mapObjects.Length; i++)
            {
                m_map.RemoveEntityFromAllTiles(mapObjects[i]);
                Destroy(mapObjects[i]);
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

    /// <summary>
    /// Loads the sprite from the project. 
    /// If there is already a sprite within the dictioanry that has the same path
    /// or the sprite fails to load it returns false otherwise it returns true.
    /// </summary>
    /// <param name="t_spritePath">The path to destination of the sprite to load</param>
    /// <returns>Bool for the sprite was loaded succesfuly</returns>
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

    /// <summary>
    /// Loads in a prefab using the passed in name fromthe prefab folder.
    /// If that does not already exist with the dictionary and the prefab is loaded succesfuly
    /// a map object is created using the data from the prefab and true is returned.
    /// Other false is returned and no new map object is created.
    /// </summary>
    /// <param name="t_fileName">The name of the prefab to be loaded from the prefab folder</param>
    /// <param name="t_width">The width of the map object in tiles</param>
    /// <param name="t_height">The height of th map object in tiles</param>
    /// <returns>Bool for the prefab was loaded and map object was created</returns>
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

    /// <summary>
    /// Save all the data from the map editor to ajson file.
    /// </summary>
    /// <param name="t_fileName">The name of the file in which the data will be saved</param>
    /// <returns>Bool for if the file was save succesfully</returns>
    public bool SaveLevel(string t_fileName)
    {
        LevelSave.SaveLevel(m_map, t_fileName, m_tileSprites, m_mapObjects);

        return true;
    }

    /// <summary>
    /// Loads in the data from the a file who's name matches the passed in
    /// name. If the file fails to load false is returned otherwise the
    /// map editor data is populated. If the level editor is not null its
    /// UI data is populated. If the passed in bool is true then the real 
    /// prefabs are instanciated within the scene instead of the map objects.
    /// </summary>
    /// <param name="t_fileName">The name of the file that is to be loaded</param>
    /// <param name="t_loadRealObjects">Bool for it the original prefabs should be placed or the fake ones</param>
    /// <returns>Bool for if the file was loaded succesfuly or not</returns>
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
                    ColumnWrapper wrapperClass = saveData.m_spriteNames[x];

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

    /// <summary>
    /// Clears all data that is attached to the map editor.
    /// </summary>
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