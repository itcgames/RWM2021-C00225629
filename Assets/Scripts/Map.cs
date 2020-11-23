using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int m_width = 0;
    public int m_hight = 0;

    List<List<Tile>> m_mapGrid = new List<List<Tile>>();
    List<GameObject> m_mapEnteties = new List<GameObject>();

    public float tileSize = 0.32f;
    public string m_charTag = "Character";
    public string m_objectTag = "Object";

    public void SetSize(int t_width, int t_hight)
    {
        m_width = t_width;
        m_hight = t_hight;
    }

    public void SetWidth(int t_width)
    {
        m_width = t_width;
    }

    public void SetHight(int t_hight)
    {
        m_hight = t_hight;
    }

    public int GetWidth()
    {
        return m_width;
    }

    public int GetHight()
    {
        return m_hight;
    }

    public void CreateMap()
    {
        if(m_mapGrid.Count == 0)
        {
            for (int x = 0; x < m_width; x++)
            {
                List<Tile> col = new List<Tile>();

                for (int y = 0; y < m_hight; y++)
                {
                    Tile newTile = new Tile();
                    newTile.SetIndexPos(new Pair<int, int>(x, y));

                    col.Add(newTile);
                }
                m_mapGrid.Add(col);
            }
        }
    }

    public int GetTileCount()
    {
        int count = 0;
        foreach(List<Tile> col in m_mapGrid)
        {
            count += col.Count;
        }
        return count;
    }

    public void AddEntityToList(GameObject t_newEntity)
    {
        if(!m_mapEnteties.Contains(t_newEntity))
        {
            m_mapEnteties.Add(t_newEntity);
        }
    }

    public void FixedUpdate()
    {
        //List for a quick way to get each corner of the collider.
        List<Pair<int, int>> cornerList = new List<Pair<int, int>>();
        cornerList.Add(new Pair<int, int>(-1, -1));
        cornerList.Add(new Pair<int, int>(1, -1));
        cornerList.Add(new Pair<int, int>(1, 1));
        cornerList.Add(new Pair<int, int>(-1, 1));

        foreach(GameObject entity in m_mapEnteties)
        {
            if (entity.tag == "Character")
            {
                //List of the tiles the character was previously.
                List<Pair<int, int>> previousTiles = GetTilesWithEntity(entity);

                //List of the tiles the character is going to be in now
                List<Pair<int, int>> newTiles = new List<Pair<int, int>>();

                //Collider of character used to check for change in tile.
                RectangularCollider collider = entity.GetComponent<RectangularCollider>();

                //Bool for if a collision has occured.
                bool collisionDetected = false;

                //Check if the collider scripts exists on the object.
                if (collider != null)
                {
                    //Position of the corner within the world.
                    Vector2 cornerPos;

                    //Index position of the corner within the map.
                    Pair<int, int> cornerMapIndex;

                    //Check each corner for the position with the world and the map.
                    foreach (Pair<int, int> corner in cornerList)
                    {
                        cornerPos = collider.m_pos + new Vector2(corner.m_first * collider.m_width / 2, corner.m_second * collider.m_hight / 2);
                        cornerMapIndex = new Pair<int, int>((int)(cornerPos.x / tileSize), (int)(cornerPos.y / tileSize));

                        //Check if we are not checking a tile that we are already in as that would make us be stuck in that tile for
                        //ever is something was spawned in that tile.
                        if (!m_mapGrid[cornerMapIndex.m_first][cornerMapIndex.m_second].GetEntityList().Contains(entity))
                        {
                            //Check if the tile is traversable.
                            if (!m_mapGrid[cornerMapIndex.m_first][cornerMapIndex.m_second].GetIsTraversable())
                            {
                                //The tile is not traversable and thus collsion has been detected.
                                collisionDetected = true;
                            }
                            else
                            {
                                newTiles.Add(new Pair<int, int>(cornerMapIndex.m_first, cornerMapIndex.m_second));
                            }
                        }
                        else
                        {
                            newTiles.Add(new Pair<int, int>(cornerMapIndex.m_first, cornerMapIndex.m_second));
                        }
                    }

                    //Check if the collision has been detected.
                    if (!collisionDetected)
                    {
                        foreach (Pair<int, int> indexPos in previousTiles)
                        {
                            if (!newTiles.Contains(indexPos))
                            {
                                m_mapGrid[indexPos.m_first][indexPos.m_second].DeleteEntity(entity);
                            }
                        }

                        foreach (Pair<int, int> indexPos in newTiles)
                        {
                            m_mapGrid[indexPos.m_first][indexPos.m_second].AddEntity(entity);
                        }
                    }
                    else
                    {
                        Debug.Log("Character Collision Detected");
                    }
                }
            }
        }
    }

    List<Pair<int, int>> GetTilesWithEntity(GameObject t_entity)
    {
        List<Pair<int, int>> listOccupiedTiles = new List<Pair<int, int>>();

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_hight; y++)
            {
                if(m_mapGrid[x][y].GetEntityList().Contains(t_entity))
                {
                    listOccupiedTiles.Add(new Pair<int, int>(x, y));
                }
            }
        }
        return listOccupiedTiles;
    }

    public bool AddEntity(Pair<int,int> t_tileIndex, GameObject t_entity)
    {
        bool success = false;

        List<GameObject> entityList = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].GetEntityList();

        foreach (GameObject entity in entityList)
        {
            if(entity.tag == m_objectTag)
            {
                return success;
            }
        }

        success = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].AddEntity(t_entity);

        if(success)
        {
            if(t_entity.tag == m_objectTag)
            {
                //The tile now contains an entity of type object meaning a character can no longer enter this tile.
                m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].SetIsTraversable(false);
            }
        }

        return success;
    }

    public bool RemoveEntity(Pair<int, int> t_tileIndex, GameObject t_entity)
    {
        bool success = false;

        success = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].DeleteEntity(t_entity);

        if (success)
        {
            bool containsObject = false;

            List<GameObject> entityList = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].GetEntityList();

            foreach (GameObject entity in entityList)
            {
                if (entity.tag == m_objectTag)
                {
                    containsObject = true;
                }
            }
            
            //The tile no longer contains an entity of type object meaning a character can enter this tile.
            if(!containsObject)
            {
                m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].SetIsTraversable(true);
            }
        }

        return success;
    }

    public List<GameObject> GetEntity(Pair<int, int> t_tileIndex)
    {
        return m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].GetEntityList();
    }
}
