using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public struct FakeEntity
{
    public string m_prefabPath;

    public Sprite m_image;

    public int m_width;
    public int m_height;
}

public class LevelEditor : MonoBehaviour
{
    [SerializeField]
    Map m_map;

    int m_mapWidth = 1;
    int m_mapHeight = 1;
    int m_spriteCount = 0;
    int m_prefabCount = 0;

    public InputField m_widthField;
    public InputField m_heightField;
    public InputField m_prefabWidthField;
    public InputField m_prefabHeightField;
    public InputField m_spriteNameField;
    public InputField m_prefabNameField;

    public Transform m_cameraTransform;

    public Text m_spriteAddResponse;
    public Text m_prefabAddResponse;
    public Text m_selectedText;

    public GameObject m_tileSpriteButtonPrefab;
    public GameObject m_prefabButtonPrefab;

    public GameObject m_prefabContainer;
    public GameObject m_spriteContainer;

    Sprite m_selectedSprite;

    List<FakeEntity> m_fakeEntities = new List<FakeEntity>();

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
        if(t_field.text != "")
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

        if(m_widthField.text == "")
        {
            m_mapWidth = 1;
        }
        else
        {
            m_mapWidth = int.Parse(m_widthField.text);
        }

        if (m_heightField.text == "")
        {
            m_mapHeight = 1;
        }
        else
        {
            m_mapHeight = int.Parse(m_heightField.text);
        }

        m_map.SetSize(m_mapWidth, m_mapHeight);
        m_map.CreateMap();

        Vector3 cameraPos = m_cameraTransform.position;

        cameraPos.x = m_mapWidth / 2.0f * m_map.tileSize;
        cameraPos.y = m_mapHeight / 2.0f * m_map.tileSize;

        m_cameraTransform.position = cameraPos;
    }

    public void ClearAddResponseText()
    {
        m_spriteAddResponse.text = "";
        m_prefabAddResponse.text = ""; 
    }

    public void LoadTileBackground()
    {
       string path = "Assets/Resources/MapGridComponent/Sprites/" + m_spriteNameField.text;

       Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;

        if(sprite != null)
        {
            m_spriteCount++;

            GameObject tileButton = Instantiate(m_tileSpriteButtonPrefab, m_spriteContainer.transform);
            tileButton.transform.SetParent(m_spriteContainer.transform);
            tileButton.transform.position += new Vector3(60 * ((m_spriteCount - 1) % 2), -60 * ((m_spriteCount - 1) / 2), 0);
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

    public void LoadPrefab()
    {
        string path = "Assets/Resources/MapGridComponent/Prefabs/" + m_prefabNameField.text;

        GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

        if (loadedPrefab != null)
        {
            FakeEntity fakeEntity;
            fakeEntity.m_prefabPath = path;
            fakeEntity.m_image = loadedPrefab.GetComponent<SpriteRenderer>().sprite;

            if (m_prefabWidthField.text != "")
            {
                fakeEntity.m_width = int.Parse(m_prefabWidthField.text);
            }
            else
            {
                fakeEntity.m_width = 1;
            }

            if (m_prefabHeightField.text != "")
            {
                fakeEntity.m_height = int.Parse(m_prefabHeightField.text);
            }
            else
            {
                fakeEntity.m_height = 1;
            }

            CreatePrefabButton(fakeEntity);
            m_fakeEntities.Add(fakeEntity);
            m_prefabAddResponse.text = "Prefab Has Been Added!";
            m_prefabAddResponse.color = Color.green;
        }

        else
        {
            m_prefabAddResponse.text = "Error Prefab Not Found!";
            m_prefabAddResponse.color = Color.red;
        }
    }

    public void CreatePrefabButton(FakeEntity t_fakeEntity)
    {
        m_prefabCount++;

        GameObject prefabButton = Instantiate(m_prefabButtonPrefab, m_prefabContainer.transform);
        prefabButton.transform.SetParent(m_prefabContainer.transform);
        prefabButton.transform.position += new Vector3(60 * ((m_prefabCount - 1) % 2), -70 * ((m_prefabCount - 1) / 2), 0);
        prefabButton.transform.GetChild(0).GetComponent<Image>().sprite = t_fakeEntity.m_image;
        prefabButton.transform.GetChild(1).GetComponent<Text>().text = t_fakeEntity.m_width + "x" + t_fakeEntity.m_height;
    }

    public void SelectSprite()
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        Sprite sprite = buttonObject.transform.GetChild(0).GetComponent<Image>().sprite;

        if(sprite != m_selectedSprite)
        {
            m_selectedSprite = sprite;
            m_selectedText.text = "SELECTED";
            m_selectedText.transform.position = buttonObject.transform.position;
            m_selectedText.transform.SetAsLastSibling();
        }
        else
        {
            Deselect();
        }
    }

    public void Deselect()
    {
        m_selectedSprite = null;
        m_selectedText.text = "";
        m_selectedText.transform.position = new Vector2(500, 500);
    }

    public void ShowSprites()
    {
        m_prefabContainer.SetActive(false);

        if(m_spriteContainer.activeSelf == false)
        {
            m_spriteContainer.SetActive(true);
        }
        else
        {
            m_spriteContainer.SetActive(false);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowPrefabs()
    {
        m_spriteContainer.SetActive(false);

        if (m_prefabContainer.activeSelf == false)
        {
            m_prefabContainer.SetActive(true);
        }
        else
        {
            m_prefabContainer.SetActive(false);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}
