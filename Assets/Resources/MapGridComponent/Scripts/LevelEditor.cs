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
    public Text m_selectedText;

    public GameObject m_tileSpriteButtonPrefab;
    public GameObject m_cancelButtonPrefab;

    public Canvas m_menuCanvas;

    Sprite m_selectedSprite;

    private void Update()
    {
        CheckSizeLimit(m_widthField);
        CheckSizeLimit(m_heightField);

        if(m_selectedSprite != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(m_map != null)
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 1;
                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

                    MapIndex mapIndex = m_map.WorldPositionToMapIndex(worldPosition);

                    Tile tile = m_map.GetTile(mapIndex);

                    if(tile != null)
                    {
                        tile.SetSprite(m_selectedSprite);
                    }
                }
            }
        }
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
            if (m_spriteCount == 0)
            {
                CreateCancelButton();
            }

            m_spriteCount++;

            GameObject tileButton = Instantiate(m_tileSpriteButtonPrefab, m_menuCanvas.transform);
            tileButton.transform.SetParent(m_menuCanvas.transform);
            tileButton.transform.position += new Vector3(60 * (m_spriteCount % 2), -60 * (m_spriteCount / 2), 0);
            tileButton.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            tileButton.GetComponent<Button>().onClick.AddListener(SelectSprite);

            m_spriteAddResponse.text = "Sprite Has Been Added!";
            m_spriteAddResponse.color = Color.green;
        }
        else
        {
            m_spriteAddResponse.text = "Error Sprite Not Found!";
            m_spriteAddResponse.color = Color.red;
        }
    }

    public void SelectSprite()
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Sprite sprite = buttonObject.transform.GetChild(0).GetComponent<Image>().sprite;

        m_selectedSprite = sprite;
        m_selectedText.text = "SELECTED";
        m_selectedText.transform.position = buttonObject.transform.position;
        m_selectedText.transform.SetAsLastSibling();
    }

    public void DeselectSprite()
    {
        m_selectedSprite = null;
        m_selectedText.text = "";
        m_selectedText.transform.position = new Vector2(500, 500);
    }

    public void CreateCancelButton()
    {
        GameObject cancelButton = Instantiate(m_cancelButtonPrefab, m_menuCanvas.transform);
        cancelButton.transform.SetParent(m_menuCanvas.transform);
        cancelButton.GetComponent<Button>().onClick.AddListener(DeselectSprite);
    }
}
