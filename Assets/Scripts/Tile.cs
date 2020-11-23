using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Pair<int, int> m_indexPos = null;
    private List<GameObject> m_entities = new List<GameObject>();
    bool m_isTraversable = true;

    public void SetIndexPos(Pair<int,int> t_newIndexPos)
    {
        m_indexPos = t_newIndexPos;
    }

    public Pair<int,int> GetIndexPos()
    {
        return m_indexPos;
    }

    public bool AddEntity(GameObject t_gameObject)
    {
        if(!m_entities.Contains(t_gameObject))
        {
            m_entities.Add(t_gameObject);
            return true;
        }

        return false;
    }

    public bool DeleteEntity(GameObject t_gameObject)
    {
        if (m_entities.Contains(t_gameObject))
        {
            m_entities.Remove(t_gameObject);
            return true;
        }

        return false;
    }

    public void SetIsTraversable(bool t_isTraversable)
    {
        m_isTraversable = t_isTraversable;
    }

    public bool GetIsTraversable()
    {
        return m_isTraversable;
    }

    public List<GameObject> GetEntityList()
    {
        return m_entities;
    }
}
