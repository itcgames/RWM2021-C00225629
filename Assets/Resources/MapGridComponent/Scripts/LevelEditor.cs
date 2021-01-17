using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LevelEditor : MonoBehaviour
{
    public MapEditor m_mapEditor;

    //Input fields for the dimensions of the map.
    public InputField m_widthField;
    public InputField m_heightField;

    //Name of the file to load from the project.
    public InputField m_spriteNameField;
    public InputField m_objectNameField;

    //Input fields for the width and height of a map object prefab.
    public InputField m_objectWidthField;
    public InputField m_objectHeightField;

    //Input field for the name of file to save/load.
    public InputField m_saveFileNameField;
    public InputField m_loadFileNameField;

    //Prefabs for the buttons.
    public GameObject m_tileSpriteButton;
    public GameObject m_mapObjectButton;
    public GameObject m_removeObjectButton;

    //Response text for each load/save screen.
    public Text m_spriteLoadResponse;
    public Text m_objectLoadResponse;
    public Text m_levelSaveResponse;
    public Text m_levelLoadResponse;

    //Text that appear over the buttons that can alter/add/remove to/form map.
    public Text m_selectedText;

    //Containers for all the add/remove buttons.
    public GameObject m_objectContainer;
    public GameObject m_spriteContainer;

    //The current sprite that is selected.
    Sprite m_selectedSprite;

    //The current map object that is selected.
    MapObject m_selectedObject;

    bool m_removeObjectFromMap;

    void Update()
    {
        CheckSizeLimit(m_widthField);
        CheckSizeLimit(m_heightField);

        if (Input.GetMouseButtonDown(0))
        {
            if (m_mapEditor.m_map.m_isCreated)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 1;
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

                MapIndex mapIndex = m_mapEditor.m_map.WorldPositionToMapIndex(worldPosition);

                if (m_selectedSprite != null)
                {
                    m_mapEditor.ChangeTileSprite(mapIndex, m_selectedSprite);
                }

                else if (m_selectedObject != null)
                {
                    PlaceMapObject(mapIndex);
                }

                else if (m_removeObjectFromMap)
                {
                    m_mapEditor.RemoveMapObject(mapIndex);
                }
            }
        }
    }

    void CheckSizeLimit(InputField t_field)
    {
        if (t_field.text != "")
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

    void PlaceMapObject(MapIndex t_mapIndex)
    {
        if(m_mapEditor.CheckCanPlaceMapObject(t_mapIndex, m_selectedObject))
        {
            m_mapEditor.InstantiateMapObject(t_mapIndex, m_selectedObject);
        }
    }

    public void CreateMap()
    {
        int mapWidth = 1;
        int mapHeight = 1;

        if (m_widthField.text != "")
        {
            mapWidth = int.Parse(m_widthField.text);
        }

        if (m_heightField.text != "")
        {
            mapHeight = int.Parse(m_heightField.text);
        }

        m_mapEditor.CreateMap(mapWidth, mapHeight);
    }

    public void ClearResponseTexts()
    {
        m_spriteLoadResponse.text = "";
        m_objectLoadResponse.text = "";
        m_levelSaveResponse.text = "";
        m_levelLoadResponse.text = "";
    }

    public void LoadMapObject()
    {
        string fileName = m_objectNameField.text;
        
        int width = 1;
        int height = 1;

        if (m_objectWidthField.text != "")
        {
            width = int.Parse(m_objectWidthField.text);
        }

        if (m_objectHeightField.text != "")
        {
            height = int.Parse(m_objectHeightField.text);
        }

        if(m_mapEditor.LoadMapObject(fileName, width, height))
        {
            m_objectLoadResponse.text = "Prefab Has Been Loaded!";
            m_objectLoadResponse.color = Color.green;

            CreateObjectButton(m_mapEditor.m_mapObjects[fileName]);
        }
        else
        {
            m_objectLoadResponse.text = "Error Prefab Not Found!";
            m_objectLoadResponse.color = Color.red;
        }
    }

    public void LoadTileSprite()
    {
        string path = "Assets/Resources/MapGridComponent/Sprites/" + m_spriteNameField.text;

        if (m_mapEditor.LoadtTileSprite(path))
        {
            m_spriteLoadResponse.text = "Sprite Has Been Added!";
            m_spriteLoadResponse.color = Color.green;

            CreateSpriteButton(path);
        }

        else
        {
            m_spriteLoadResponse.text = "Error Failed To Load Sprite";
            m_spriteLoadResponse.color = Color.red;
        }
    }

    public void CreateSpriteButton(string t_path)
    {
        Sprite sprite = m_mapEditor.m_tileSprites[t_path];

        int spriteCount = m_mapEditor.m_tileSprites.Count;

        GameObject tileButton = Instantiate(m_tileSpriteButton, m_spriteContainer.transform);
        tileButton.transform.SetParent(m_spriteContainer.transform);
        tileButton.transform.position += new Vector3(60 * ((spriteCount - 1) % 2), -60 * ((spriteCount - 1) / 2), 0);
        tileButton.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        tileButton.GetComponent<Button>().onClick.AddListener(SelectSprite);
    }

    public void CreateObjectButton(MapObject t_mapObject)
    {
        int objectCount = m_mapEditor.m_mapObjects.Count;

        if (objectCount == 1)
        {
            CreateRemoveObjectButton();
        }

        GameObject prefabButton = Instantiate(m_mapObjectButton, m_objectContainer.transform);
        prefabButton.transform.SetParent(m_objectContainer.transform);
        prefabButton.transform.position += new Vector3(60 * (objectCount % 2), -70 * (objectCount / 2), 0);
        prefabButton.transform.GetChild(0).GetComponent<Image>().sprite = t_mapObject.m_image;
        prefabButton.transform.GetChild(1).GetComponent<Text>().text = t_mapObject.m_width + "x" + t_mapObject.m_height;
        prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectMapObject(t_mapObject));
    }

    void CreateRemoveObjectButton()
    {
        GameObject removeObjectButtom = Instantiate(m_removeObjectButton, m_objectContainer.transform);
        removeObjectButtom.transform.SetParent(m_objectContainer.transform);
        removeObjectButtom.GetComponent<Button>().onClick.AddListener(SelectRemoveMapObject);
    }

    void SelectMapObject(MapObject t_mapObject)
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (t_mapObject != m_selectedObject)
        {
            m_selectedObject = t_mapObject;
            m_removeObjectFromMap = false;
            m_selectedText.text = "SELECTED";
            m_selectedText.transform.position = buttonObject.transform.position;
            m_selectedText.transform.SetAsLastSibling();
            return;
        }

        DeselectAll();
    }

    void SelectRemoveMapObject()
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (!m_removeObjectFromMap)
        {
            m_removeObjectFromMap = true;
            m_selectedObject = null;
            m_selectedText.text = "SELECTED";
            m_selectedText.transform.position = buttonObject.transform.position;
            m_selectedText.transform.SetAsLastSibling();
            return;
        }

        DeselectAll();
    }

    void SelectSprite()
    {
        GameObject buttonObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        Sprite sprite = buttonObject.transform.GetChild(0).GetComponent<Image>().sprite;

        if (sprite != m_selectedSprite)
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

    public void SaveLevel()
    {
        string fileName = m_saveFileNameField.text;

        if (fileName == "")
        {
            fileName = "levelSave";
        }

        if(m_mapEditor.SaveLevel(fileName))
        {
            m_levelSaveResponse.text = "File Saved Successfully!";
            m_levelSaveResponse.color = Color.green;
        }

        else
        {
            m_levelSaveResponse.text = "Error File Failed To Save!";
            m_levelSaveResponse.color = Color.red;
        }
    }

    public void LoadLevel()
    {
        ClearAllData();

        string fileName = m_loadFileNameField.text;

        if(m_mapEditor.LoadLevel(fileName, false))
        {
            m_levelLoadResponse.text = "File Loaded Successfully!";
            m_levelLoadResponse.color = Color.green;
        }

        else
        {
            m_levelLoadResponse.text = "Error File Failed To Load!";
            m_levelLoadResponse.color = Color.red;
        }
    }

    public void ShowSprites()
    {
        m_objectContainer.SetActive(false);

        if (m_spriteContainer.activeSelf == false)
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

    public void ShowMapObjects()
    {
        m_spriteContainer.SetActive(false);

        if (m_objectContainer.activeSelf == false)
        {
            m_objectContainer.SetActive(true);
            DeselectAll();
        }
        else
        {
            m_objectContainer.SetActive(false);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void DeselectAll()
    {
        m_selectedSprite = null;
        m_selectedObject = null;
        m_selectedText.text = "";
        m_removeObjectFromMap = false;
        m_selectedText.transform.position = new Vector2(500, 500);
    }

    public void ClearAllData()
    {
        while (m_spriteContainer.transform.childCount != 0)
        {
            DestroyImmediate(m_spriteContainer.transform.GetChild(0).gameObject);
        }

        while (m_objectContainer.transform.childCount != 0)
        {
            DestroyImmediate(m_objectContainer.transform.GetChild(0).gameObject);
        }

        m_mapEditor.ClearAllData();
    }
}