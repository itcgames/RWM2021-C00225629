using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct MapIndex
{
    public int m_x;     //X component of the map index position.
    public int m_y;     //Y component of the map index position.

    public MapIndex(int t_xIndex, int t_yIndex)
    {
        m_x = t_xIndex;
        m_y = t_yIndex;
    }

    public MapIndex(MapIndex t_mapIndex)
    {
        m_x = t_mapIndex.m_x;
        m_y = t_mapIndex.m_y;
    }
}

public class Map : MonoBehaviour
{
    [Range(1, 1000)]
    public int m_width = 0;                 //Number of tiles in a singal row of the map.

    [Range(1, 1000)]
    public int m_hight = 0;                 //Number of tiles in a single column of the map.

    //2D list of tiles.
    List<List<Tile>> m_grid = new List<List<Tile>>();

    //List of gameobjects that are part of the 2D tile map.
    List<GameObject> m_mapEnteties = new List<GameObject>();

    public float m_tileSize = 0.32f;                        //The size of a singular tile in pixels.
    
    public List<string> m_charsTag = new List<string>();    //The tags used for entities that move around the map.
    public List<string> m_objectsTag = new List<string>();  //The tags used for enteties that when placed in a tile prevent other entities from ever being added to the tile.

    public bool m_isCreated = false;

    /// <summary>
    /// Sets the size of the width and hight of the 2D tile grid.
    /// </summary>
    /// <param name="t_width">Sets the width of the 2D tile grid</param>
    /// <param name="t_hight">Sets the hight of the 2D tile grid</param>
    public void SetSize(int t_width, int t_hight)
    {
        m_width = t_width;
        m_hight = t_hight;
    }

    /// <summary>
    /// Sets the width of the 2D tile grid.
    /// </summary>
    /// <param name="t_width">Sets the width of the 2D tile grid</param>
    public void SetWidth(int t_width)
    {
        m_width = t_width;
    }

    /// <summary>
    /// Sets the hight of the 2D tile grid.
    /// </summary>
    /// <param name="t_hight">Sets the hight of the 2D tile grid</param>
    public void SetHight(int t_hight)
    {
        m_hight = t_hight;
    }

    /// <summary>
    /// Getter method for the width of the 2D tile grid.
    /// </summary>
    /// <returns>The width of the 2D tile grid</returns>
    public int GetWidth()
    {
        return m_width;
    }

    /// <summary>
    /// Getter method for the hight of the 2D tile grid.
    /// </summary>
    /// <returns>The hight of th 2D tile grid</returns>
    public int GetHight()
    {
        return m_hight;
    }
    
    /// <summary>
    /// Creates a 2D tile grid adding each time the tile component
    /// to the 2D tile grid.
    /// For now if this function can ONLY be ran if the size of the
    /// grid equals 0 otherwise it will do nothing this can be changed.
    /// </summary>
    public void CreateMap()
    {
        m_grid = new List<List<Tile>>();

        for (int x = 0; x < m_width; x++)
        {
            List<Tile> col = new List<Tile>();

            for (int y = 0; y < m_hight; y++)
            {
                col.Add(CreateTileObject(new MapIndex(x, y)));
            }
            m_grid.Add(col);
        }

        m_isCreated = true;
    }

    /// <summary>
    /// Creates a tile gameobject and sets it's name to the passed in
    /// map index value and its position is set accordingly. The object is then attached as a child to this gameobject.
    /// </summary>
    /// <param name="t_mapIndex">The index postion of the tile within the 2D grid</param>
    /// <returns>The tile component of the tile object</returns>
    Tile CreateTileObject(MapIndex t_mapIndex)
    {
        GameObject tileObject = new GameObject();

        //Set the name of the tile object to the id of the tile.
        tileObject.name = "Tile[" + t_mapIndex.m_x + ", " + t_mapIndex.m_y + "]";

        //Set the position to corresponding index position. 1 for the z axis is to avoid sprite layers which are destroyed when
        //pushed to github.
        tileObject.transform.position = new Vector3(m_tileSize / 2 + t_mapIndex.m_x * m_tileSize, m_tileSize / 2 + t_mapIndex.m_y * m_tileSize, 1);
        tileObject.AddComponent<SpriteRenderer>();
        tileObject.AddComponent<Tile>();

        tileObject.transform.parent = transform;

        Tile newTile = tileObject.GetComponent<Tile>();
        newTile.SetIndexPos(t_mapIndex);
        newTile.LoadSprite(LoadTileSprite(t_mapIndex));

       return newTile;
    }

    /// <summary>
    /// Returns the string to the spirte path for the passed in
    /// map index position. For now it only loads the default path for
    /// each tile as reading from file and map creator have not been implmented.
    /// </summary>
    /// <param name="t_mapIndex">The index position with the 2D tile grid</param>
    /// <returns>The string to the spirte asset location within the resources folder</returns>
    string LoadTileSprite(MapIndex t_mapIndex)
    {
        return "MapGridComponent/Sprites/Tile";
    }

    /// <summary>
    /// Returns the total count of all the tiles with the map.
    /// </summary>
    /// <returns>The total count of all the tile sin the map</returns>
    public int GetTileCount()
    {
        int count = 0;
        foreach(List<Tile> col in m_grid)
        {
            count += col.Count;
        }
        return count;
    }

    /// <summary>
    /// Adds the gameobject to the entity list if it
    /// is not aleady within the list.
    /// </summary>
    /// <param name="t_newEntity">The gameobject to be added to the entity list</param>
    public void AddEntityToList(GameObject t_newEntity)
    {
        if(!m_mapEnteties.Contains(t_newEntity))
        {
            m_mapEnteties.Add(t_newEntity);
        }
    }

    public void RemoveEntityFromList(GameObject t_newEntity)
    {
        if (GetTilesWithEntity(t_newEntity).Count == 0)
        {
            m_mapEnteties.Remove(t_newEntity);
        }
    }

    public void FixedUpdate()
    {
        //For loop used to do the characters within the map.
        foreach(GameObject entity in m_mapEnteties)
        {
            if (m_charsTag.Contains(entity.tag))
            {
                UpdateCharacterInMap(entity);
            }
        }
    }

    /// <summary>
    /// Updates which tiles the character is occuping.
    /// If the character enters a tile which is not traversable it is marked to be dealt with.
    /// If the character moves outside the boundries of the map it is marked to be dealth with.
    /// </summary>
    /// <param name="t_charEntity">The character gameobject that needs to be updated</param>
    void UpdateCharacterInMap(GameObject t_charEntity)
    {
        //List of the tiles the character was previously.
        List<MapIndex> prevOccupiedTiles = GetTilesWithEntity(t_charEntity);

        //List of the tiles the character is going to be in now.
        List<MapIndex> newOccupiedTiles = new List<MapIndex>();

        //Collider of character used to check for change in tile.
        TestCollider collider = t_charEntity.GetComponent<TestCollider>();

        //Bool for if a collision has occured with a tile that can not be traversed.
        bool collisionDetected = false;

        //Bool for if a character has move out side the boundry that is unguarded.
        bool outOfBounds = false;

        //Check if the collider scripts exists on the object.
        if (collider != null)
        {
            newOccupiedTiles.Add(WorldPositionToMapIndex(collider.m_pos + new Vector2(-collider.m_width / 2, -collider.m_hight / 2)));
            newOccupiedTiles.Add(WorldPositionToMapIndex(collider.m_pos + new Vector2(collider.m_width / 2, -collider.m_hight / 2)));
            newOccupiedTiles.Add(WorldPositionToMapIndex(collider.m_pos + new Vector2(collider.m_width / 2, collider.m_hight / 2)));
            newOccupiedTiles.Add(WorldPositionToMapIndex(collider.m_pos + new Vector2(-collider.m_width / 2, collider.m_hight / 2)));

            //Check each corner for the position with the world and the map.
            foreach (MapIndex corner in newOccupiedTiles)
            {
                //Check if we are not checking a tile that we are already in as that would make us be stuck in that tile for
                //ever is something was spawned in that tile.
                if (corner.m_x >= 0 && corner.m_x < m_width &&
                    corner.m_y >= 0 && corner.m_y < m_hight)
                {
                    if (!m_grid[corner.m_x][corner.m_y].GetEntityList().Contains(t_charEntity))
                    {
                        //Check if the tile is traversable.
                        if (!m_grid[corner.m_x][corner.m_y].GetIsTraversable())
                        {
                            //The tile is not traversable and thus collsion has been detected.
                            collisionDetected = true;
                        }
                    }
                }
                else
                {
                    outOfBounds = true;
                }

            }

            //Check if the collision has been detected.
            if (!collisionDetected && !outOfBounds)
            {
                foreach (MapIndex indexPos in prevOccupiedTiles)
                {
                    if (!newOccupiedTiles.Contains(indexPos))
                    {
                        m_grid[indexPos.m_x][indexPos.m_y].DeleteEntity(t_charEntity);
                    }
                }

                foreach (MapIndex indexPos in newOccupiedTiles)
                {
                    m_grid[indexPos.m_x][indexPos.m_y].AddEntity(t_charEntity);
                }
            }
            else if (outOfBounds)
            {
                //Do something here as a response to the character eg move back.
                Debug.Log("Character out of bounds detected!");
            }
            else
            {
                //Do something here as a response to the character eg move back.
                Debug.Log("Character Collision Detected!");
            }
        }
    }

    /// <summary>
    /// Converts the positon within the game world into a map index position.
    /// It is assumed that the bottom left corner of the map is at the position (0,0).
    /// It also assumes that the negative position result in negative map index.
    /// </summary>
    /// <param name="t_position">The positon on the x and y axis in the game world</param>
    /// <returns>The map index for the passsed in position</returns>
    public MapIndex WorldPositionToMapIndex(Vector2 t_position)
    {
        MapIndex indexPos = new MapIndex((int)(t_position.x / m_tileSize), (int)(t_position.y / m_tileSize));

        //When position goes into negative it is assumed it goes to negative index number 
        //thus it needs to be adjusted.
        if (t_position.x < 0)
        {
            indexPos.m_x -= 1;
        }

        if (t_position.y < 0)
        {
            indexPos.m_y -= 1;
        }

        return indexPos;
    }

    public Vector2 MapIndexToWorldPos(MapIndex t_mapIndex)
    {
        if (!GetIsOutOfBounds(t_mapIndex))
        {
            return new Vector2(t_mapIndex.m_x * m_tileSize + m_tileSize / 2, t_mapIndex.m_y * m_tileSize + m_tileSize / 2);
        }

        return new Vector2(-500,-500);
    }

    /// <summary>
    /// Gets the list of all the index positions that for the tiles 
    /// that contain the passed in gameobject.
    /// </summary>
    /// <param name="t_entity">The gameobject to be check for which tiles it occupies</param>
    /// <returns>List of map index position for the tiles the gameobject occupies</returns>
    List<MapIndex> GetTilesWithEntity(GameObject t_entity)
    {
        List<MapIndex> listOccupiedTiles = new List<MapIndex>();

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_hight; y++)
            {
                if(m_grid[x][y].GetEntityList().Contains(t_entity))
                {
                    listOccupiedTiles.Add(new MapIndex(x, y));
                }
            }
        }
        return listOccupiedTiles;
    }

    /// <summary>
    /// Adds the passed in gameobject to the tile in the passed in map
    /// index posittion. This can not be done if the tile contains a gameobject that
    /// maeks the tilenot traversable. If the gameobject is added succesful to the tile
    /// and it has a tag of object it makes the tile not traversable.
    /// </summary>
    /// <param name="t_tileIndex">The map index position of the tile to which the gameobject will be added</param>
    /// <param name="t_entity">The gameobject that will be added to the tile</param>
    /// <returns>A bool for if the gameobject has been added to the tile</returns>
    public bool AddEntity(MapIndex t_tileIndex, GameObject t_entity)
    {
        bool success = false;

        List<GameObject> entityList = m_grid[t_tileIndex.m_x][t_tileIndex.m_y].GetEntityList();

        foreach (GameObject entity in entityList)
        {
            if (m_objectsTag.Contains(entity.tag))
            {
                return success;
            }
        }

        success = m_grid[t_tileIndex.m_x][t_tileIndex.m_y].AddEntity(t_entity);

        if(success)
        {
            AddEntityToList(t_entity);

            if (m_objectsTag.Contains(t_entity.tag))
            {
                //The tile now contains an entity of type object meaning a character can no longer enter this tile.
                m_grid[t_tileIndex.m_x][t_tileIndex.m_y].SetIsTraversable(false);
            }
        }

        return success;
    }

    /// <summary>
    /// Removes the passed in gameobject from the tile at the passed in map indes postion
    /// if the gameobject is found in the list of gamobjects within that tile. If the gameobject
    /// was the last remaining gameobject in the list of type object the tile becomes traversable.
    /// </summary>
    /// <param name="t_tileIndex">The map index position of the tile from which the gameobject will be removed from</param>
    /// <param name="t_entity">The gameobject that will be removed from the tile</param>
    /// <returns></returns>
    public bool RemoveEntity(MapIndex t_tileIndex, GameObject t_entity)
    {
        bool success = false;

        success = m_grid[t_tileIndex.m_x][t_tileIndex.m_y].DeleteEntity(t_entity);

        if (success)
        {
            RemoveEntityFromList(t_entity);

            bool containsObject = false;

            List<GameObject> entityList = m_grid[t_tileIndex.m_x][t_tileIndex.m_y].GetEntityList();

            foreach (GameObject entity in entityList)
            {
                if (m_objectsTag.Contains(t_entity.tag))
                {
                    containsObject = true;
                }
            }
            
            //The tile no longer contains an entity of type object meaning a character can enter this tile.
            if(!containsObject)
            {
                m_grid[t_tileIndex.m_x][t_tileIndex.m_y].SetIsTraversable(true);
            }
        }

        return success;
    }

    public List<GameObject> GetEntity(MapIndex t_tileIndex)
    {
        return m_grid[t_tileIndex.m_x][t_tileIndex.m_y].GetEntityList();
    }

    public void RemoveEntityFromAllTiles(GameObject t_entity)
    {
        List<MapIndex> mapIndexList = GetTilesWithEntity(t_entity);

        foreach (MapIndex mapIndex in mapIndexList)
        {
            RemoveEntity(mapIndex, t_entity);
        }
    }

    public Tile GetTile(MapIndex t_mapIndex)
    {
        if (!GetIsOutOfBounds(t_mapIndex))
        {
            return m_grid[t_mapIndex.m_x][t_mapIndex.m_y];
        }

        return null;
    }

    public bool GetIsTileEmpty(MapIndex t_mapIndex)
    {
        Tile tile = m_grid[t_mapIndex.m_x][t_mapIndex.m_y];

        if (tile.GetEntityList().Count == 0)
        {
            return true;
        }

        return false;
    }

    public bool GetIsOutOfBounds(MapIndex t_mapIndex)
    {
        if (t_mapIndex.m_x >= 0 && t_mapIndex.m_x < m_width)
        {
            if(t_mapIndex.m_y >= 0 && t_mapIndex.m_y < m_hight)
            {
                return false;
            }
        }

        return true;
    }

    public List<GameObject> GetAllEnteties()
    {
        return m_mapEnteties;
    }
}