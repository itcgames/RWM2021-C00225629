using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LevelEditor : MonoBehaviour
{
    [SerializeField]
    Map m_map;

    int m_mapWidth = 1;
    int m_mapHeight = 1;
    int m_spriteCount = 0;

    public InputField m_widthField;
    public InputField m_heightField;
    public InputField m_spriteNameField;

    public Transform m_cameraTransform;

    public Text m_spriteAddResponse;

    public GameObject m_tileSpriteButtonPrefab;

    public Canvas m_menuCanvas;

    Sprite m_selectedSprite;

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

    public void ClearAddResponseText()
    {
        m_spriteAddResponse.text = "";
    }

    public void LoadTileBackground()
    {
       string path = "Assets/Resources/MapGridComponent/Sprites/" + m_spriteNameField.text;

       Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;

        if(sprite != null)
        {
            m_spriteCount++;

            GameObject tileButton = Instantiate(m_tileSpriteButtonPrefab, m_menuCanvas.transform);
            tileButton.transform.SetParent(m_menuCanvas.transform);
            tileButton.transform.position += new Vector3(60 * ((m_spriteCount - 1) % 2), -60 * ((m_spriteCount - 1) / 2), 0);
            tileButton.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

            m_spriteAddResponse.text = "Sprite Has Been Added!";
            m_spriteAddResponse.color = Color.green;
        }
        else
        {
            m_spriteAddResponse.text = "Error Sprite Not Found!";
            m_spriteAddResponse.color = Color.red;
        }
    }
}
