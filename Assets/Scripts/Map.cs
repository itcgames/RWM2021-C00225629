using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int m_width = 0;
    int m_hight = 0;

    List<List<Tile>> m_mapGrid = new List<List<Tile>>();

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

    public bool AddEntityID(Pair<int,int> t_tileIndex, int t_entityID)
    {
        bool success = false;

        success = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].AddEntityID(t_entityID);

        return success;
    }

    public bool RemoveEntityID(Pair<int, int> t_tileIndex, int t_entityID)
    {
        bool success = false;

        success = m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].DeleteEntityID(t_entityID);

        return success;
    }

    public List<int> GetEntityIDs(Pair<int, int> t_tileIndex)
    {
        return m_mapGrid[t_tileIndex.m_first][t_tileIndex.m_second].GetIDList();
    }
}
