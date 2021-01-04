using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    [SerializeField]
    Map m_map;

    public int m_mapWidth = 1;
    public int m_mapHeight = 1;

    public InputField m_widthField;
    public InputField m_heightField;

    public Transform m_cameraTransform;

    private void Update()
    {
        CheckSizeLimit(m_widthField);
        CheckSizeLimit(m_heightField);
    }

    void CheckSizeLimit(InputField t_field)
    {
        if (int.Parse(t_field.text) < 1)
        {
            t_field.text = "1";
        }
        else if (int.Parse(t_field.text) > 30)
        {
            t_field.text = "30";
        }
    }

    public void SetMapSize()
    {
        if(m_map != null)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        m_mapWidth = int.Parse(m_widthField.text);
        m_mapHeight = int.Parse(m_heightField.text);

        m_map.SetSize(m_mapWidth, m_mapHeight);
        m_map.CreateMap();

        Vector3 cameraPos = m_cameraTransform.position;

        cameraPos.x = m_mapWidth / 2.0f * m_map.tileSize;
        cameraPos.y = m_mapWidth / 2.0f * m_map.tileSize;

        m_cameraTransform.position = cameraPos;
    }
}
