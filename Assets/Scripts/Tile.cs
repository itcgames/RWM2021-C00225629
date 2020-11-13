using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Pair<int, int> m_indexPos = null;
    private List<int> m_entityIDs = new List<int>();

    public void SetIndexPos(Pair<int,int> t_newIndexPos)
    {
        m_indexPos = t_newIndexPos;
    }

    public Pair<int,int> GetIndexPos()
    {
        return m_indexPos;
    }

    public bool AddEntityID(int t_entityID)
    {
        if(!m_entityIDs.Contains(t_entityID))
        {
            m_entityIDs.Add(t_entityID);
            return true;
        }

        return false;
    }

    public bool DeleteEntityID(int t_entityID)
    {
        if (m_entityIDs.Contains(t_entityID))
        {
            m_entityIDs.Remove(t_entityID);
            return true;
        }

        return false;
    }

    public List<int> GetIDList()
    {
        return m_entityIDs;
    }
}
