using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private MapIndex m_indexPos;
    private List<GameObject> m_entities = new List<GameObject>();
    private SpriteRenderer m_spriteRenderer = null;
    private Sprite m_sprite = null;
    bool m_isTraversable = true;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(string t_spritePath)
    {
        m_sprite = Resources.Load<Sprite>(t_spritePath);

        m_spriteRenderer.sprite = m_sprite;
    }

    public void SetIndexPos(MapIndex t_newIndexPos)
    {
        m_indexPos = t_newIndexPos;
    }

    public MapIndex GetIndexPos()
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
