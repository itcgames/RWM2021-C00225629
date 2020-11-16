using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    NONE = 0,
    CHARACTER = 1,
    OBJECT = 2
}

public class MapEntity : MonoBehaviour
{
    static int s_nextID = 0;

    EntityType m_type = EntityType.NONE;

    int m_id;

    void Awake()
    {
        m_id = s_nextID;

        s_nextID++;
    }

    public void SetEntityType(EntityType t_type)
    {
        m_type = t_type;
    }

    public EntityType GetEntityType()
    {
        return m_type;
    }

    public int GetID()
    {
        return m_id;
    }
}
