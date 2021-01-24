using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tile : MonoBehaviour
{
    private MapIndex m_indexPos;    //The index position of the tile within the map.
    private List<GameObject> m_entities = new List<GameObject>();   //List of enteties that can be found in this tile.
    private SpriteRenderer m_spriteRenderer = null;                 //Sprite renderer component of the tile object.
    
    bool m_isTraversable = true;                                    //Bool for if a character entity can enter this tile.

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Loads the sprite of the sprite renderer componet of the tile object
    /// if the tile object has a sprite render attached to it.
    /// </summary>
    /// <param name="t_spritePath">String for the path to the location of the sprite</param>
    public void LoadSprite(string t_spritePath)
    {
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.sprite = Resources.Load(t_spritePath, typeof(Sprite)) as Sprite;
        }
    }

    public void SetSprite(Sprite t_sprite)
    {
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.sprite = t_sprite;
        }
    }

    /// <summary>
    /// Sets the index position of the tile to the passed in
    /// map index position
    /// </summary>
    /// <param name="t_newIndexPos">The new index position of the tile</param>
    public void SetIndexPos(MapIndex t_newIndexPos)
    {
        m_indexPos = t_newIndexPos;
    }

    /// <summary>
    /// Returns the index position of the tile within the map.
    /// </summary>
    /// <returns>The index position within the map for this tile</returns>
    public MapIndex GetIndexPos()
    {
        return m_indexPos;
    }

    /// <summary>
    /// Adds the game object to the entity list within the tile.
    /// If the game object is already within the entity list it is not added.
    /// </summary>
    /// <param name="t_gameObject">The game object to be added to the enitty list</param>
    /// <returns>A bool for if the game object was added to the entity list succesful</returns>
    public bool AddEntity(GameObject t_gameObject)
    {
        if(!m_entities.Contains(t_gameObject))
        {
            m_entities.Add(t_gameObject);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the passed in game object from the entity list.
    /// If the passed in gameobject is not found within the entity list
    /// it is not removed.
    /// </summary>
    /// <param name="t_gameObject">The gameobject to be removed from the entity list</param>
    /// <returns>A bool for if the gameobject was removed from the entity list succesfuly</returns>
    public bool DeleteEntity(GameObject t_gameObject)
    {
        if (m_entities.Contains(t_gameObject))
        {
            m_entities.Remove(t_gameObject);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Set method for if the tile is traverasble or not.
    /// </summary>
    /// <param name="t_isTraversable">Bool for if the tile is traverasble or not</param>
    public void SetIsTraversable(bool t_isTraversable)
    {
        m_isTraversable = t_isTraversable;
    }

    /// <summary>
    /// Getter method for if the tile is traversable or not.
    /// </summary>
    /// <returns>A bool for if the tile is traversable or not</returns>
    public bool GetIsTraversable()
    {
        return m_isTraversable;
    }


    /// <summary>
    /// Getter method for the entity list
    /// </summary>
    /// <returns>List of gameobjects that are found within this tile</returns>
    public List<GameObject> GetEntityList()
    {
        return m_entities;
    }

    public Sprite GetSprite()
    {
        return m_spriteRenderer.sprite;
    }
}
