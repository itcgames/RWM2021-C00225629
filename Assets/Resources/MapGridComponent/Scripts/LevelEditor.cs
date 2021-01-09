using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FakeMapPrefab
{
    public string m_name;
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

    bool m_removeMapPrefab;

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

    public GameObject m_tileSpriteButton;
    public GameObject m_mapPrefabButton;
    public GameObject m_removeMapPrefabButton;

    public GameObject m_prefabContainer;
    public GameObject m_spriteContainer;

    Sprite m_selectedSprite;
    FakeMapPrefab m_selectMapPrefab;

    List<FakeMapPrefab> m_fakeEntities = new List<FakeMapPrefab>();

    private void Update()
    {
        CheckSizeLimit(m_widthField);
        CheckSizeLimit(m_heightField);

        if (Input.GetMouseButtonDown(0))
        {
            if (m_map.m_isCreated)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 1;
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

                MapIndex mapIndex = m_map.WorldPositionToMapIndex(worldPosition);

                if (m_selectedSprite != null)
                {
                    ChangeTileSprite(mapIndex);
                }

                else if (m_selectMapPrefab != null)
                {
                    PlaceMapPrefab(mapIndex);
                }

                else if(m_removeMapPrefab)
                {
                    RemoveMapPrefab(mapIndex);
                }
            }
        }
    }

    void ChangeTileSprite(MapIndex t_mapIndex)
    {
        Tile tile = m_map.GetTile(t_mapIndex);

        if (tile != null)
        {
            tile.SetSprite(m_selectedSprite);
        }
    }

    void PlaceMapPrefab(MapIndex t_mapIndex)
    {
        bool validPlacment = true;

        for (int x = 0; x < m_selectMapPrefab.m_width && validPlacment; x++)
        {
            for (int y = 0; y < m_selectMapPrefab.m_height && validPlacment; y++)
            {
                if (m_map.GetIsOutOfBounds(new MapIndex(t_mapIndex.m_x + x, t_mapIndex.m_y + y)))
                {
                    validPlacment = false;
                }

                else if (!m_map.GetIsTileEmpty(new MapIndex(t_mapIndex.m_x + x, t_mapIndex.m_y + y)))
                {
                    validPlacment = false;
                }
            }
        }

        if (validPlacment)
        {
            GameObject gameObject = CreateMapPrefabObject(t_mapIndex);

            for (int x = 0; x < m_selectMapPrefab.m_width && validPlacment; x++)
            {
                for (int y = 0; y < m_selectMapPrefab.m_height && validPlacment; y++)
                {
                    m_map.AddEntity(new MapIndex(t_mapIndex.m_x + x, t_mapIndex.m_y + y), gameObject);
                }
            }
        }
    }

    void RemoveMapPrefab(MapIndex t_mapIndex)
    {
        if (!m_map.GetIsOutOfBounds(t_mapIndex))
        {
            if (!m_map.GetIsTileEmpty(t_mapIndex))
            {
                List<GameObject> mapPrefabs = m_map.GetEntity(t_mapIndex);

                GameObject mapPrefab = mapPrefabs[0];

                m_map.RemoveEntityFromAllTiles(mapPrefab);

                Destroy(mapPrefab);
            }
        }
    }

    GameObject CreateMapPrefabObject(MapIndex t_mapIndex)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = m_selectMapPrefab.m_image;
        gameObject.GetComponent<SpriteRenderer>().spriteSortPoint = SpriteSortPoint.Pivot;
        gameObject.tag = "FakeEntity";
        gameObject.name = m_selectMapPrefab.m_name;

        Vector2 positionOffset = new Vector2(m_map.m_tileSize * m_selectMapPrefab.m_width / 2,
            m_map.m_tileSize * m_selectMapPrefab.m_height / 2);

        gameObject.transform.position = m_map.MapIndexToWorldPos(t_mapIndex) + positionOffset;

        return gameObject;
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

        cameraPos.x = m_mapWidth / 2.0f * m_map.m_tileSize;
        cameraPos.y = m_mapHeight / 2.0f * m_map.m_tileSize;

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

            GameObject tileButton = Instantiate(m_tileSpriteButton, m_spriteContainer.transform);
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
            string removeType = ".prefab";

            FakeMapPrefab fakeEntity = new FakeMapPrefab();
            fakeEntity.m_name = m_prefabNameField.text.Replace(removeType, "");
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

            CreateMapPrefabButton(fakeEntity);
            m_fakeEntities.Add(fakeEntity);
            m_prefabAddResponse.text = "Prefab Has Been Loaded!";
            m_prefabAddResponse.color = Color.green;
        }

        else
        {
            m_prefabAddResponse.text = "Error Prefab Not Found!";
            m_prefabAddResponse.color = Color.red;
        }
    }

    public void CreateMapPrefabButton(FakeMapPrefab t_fakeEntity)
    {
        m_prefabCount++;

        if(m_prefabCount == 1)
        {
            CreateRemoveMapPrefabButton();
        }

        GameObject prefabButton = Instantiate(m_mapPrefabButton, m_prefabContainer.transform);
        prefabButton.transform.SetParent(m_prefabContainer.transform);
        prefabButton.transform.position += new Vector3(60 * (m_prefabCount  % 2), -70 * (m_prefabCount / 2), 0);
        prefabButton.transform.GetChild(0).GetComponent<Image>().sprite = t_fakeEntity.m_image;
        prefabButton.transform.GetChild(1).GetComponent<Text>().text = t_fakeEntity.m_width + "x" + t_fakeEntity.m_height;
        prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectMapPrefab(t_fakeEntity));
    }

    public void CreateRemoveMapPrefabButton()
    {
        GameObject removeMapPrefabButton = Instantiate(m_removeMapPrefabButton, m_prefabContainer.transform);
        removeMapPrefabButton.transform.SetParent(m_prefabContainer.transform);
        removeMapPrefabButton.GetComponent<Button>().onClick.AddListener(SelectRemoveMapPrefab);
    }

    public void SelectRemoveMapPrefab()
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (!m_removeMapPrefab)
        {
            m_removeMapPrefab = true;
            m_selectMapPrefab = null;
            m_selectedText.text = "SELECTED";
            m_selectedText.transform.position = buttonObject.transform.position;
            m_selectedText.transform.SetAsLastSibling();
        }
        else
        {
            DeselectAll();
        }
    }

    public void SelectMapPrefab(FakeMapPrefab t_fakePrefab)
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (t_fakePrefab != m_selectMapPrefab)
        {
            m_selectMapPrefab = t_fakePrefab;
            m_removeMapPrefab = false;
            m_selectedText.text = "SELECTED";
            m_selectedText.transform.position = buttonObject.transform.position;
            m_selectedText.transform.SetAsLastSibling();
        }
        else
        {
            DeselectAll();
        }
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
            DeselectAll();
        }
    }

    public void DeselectAll()
    {
        m_selectedSprite = null;
        m_selectMapPrefab = null;
        m_selectedText.text = "";
        m_removeMapPrefab = false;
        m_selectedText.transform.position = new Vector2(500, 500);
    }

    public void ShowSprites()
    {
        m_prefabContainer.SetActive(false);

        if(m_spriteContainer.activeSelf == false)
        {
            m_spriteContainer.SetActive(true);
            DeselectAll();
        }
        else
        {
            m_spriteContainer.SetActive(false);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowMapPrefabs()
    {
        m_spriteContainer.SetActive(false);

        if (m_prefabContainer.activeSelf == false)
        {
            m_prefabContainer.SetActive(true);
            DeselectAll();
        }
        else
        {
            m_prefabContainer.SetActive(false);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}
