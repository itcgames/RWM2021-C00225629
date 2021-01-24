using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class MapEditorSuite
{
    GameObject m_mapObject = null;
    Map m_map = null;
    MapEditor m_mapEditor = null;

    [OneTimeSetUp]
    public void Setup()
    {
        m_mapObject = new GameObject();
        m_mapObject.AddComponent<Map>();
        m_mapObject.AddComponent<MapEditor>();

        m_map = m_mapObject.GetComponent<Map>();
        m_mapEditor = m_mapObject.GetComponent<MapEditor>();
        m_mapEditor.m_map = m_map;
    }

    [UnityTest, Order(1)]
    public IEnumerator CustomSize()
    {
        int width = 13;
        int hight = 12;

        m_mapEditor.CreateMap(width, hight);

        Assert.AreEqual(width, m_map.GetWidth());
        Assert.AreEqual(hight, m_map.GetHight());
        Assert.AreEqual(width * hight, m_map.GetTileCount());

        yield return new WaitForSeconds(0.1f);

        width = 17;
        hight = 11;

        m_mapEditor.CreateMap(width, hight);

        Assert.AreEqual(width, m_map.GetWidth());
        Assert.AreEqual(hight, m_map.GetHight());
        Assert.AreEqual(width * hight, m_map.GetTileCount());

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest, Order(2)]
    public IEnumerator LoadTileSprite()
    {
        string path = "MapGridComponent/Sprites/Tile";

        Assert.True(m_mapEditor.LoadtTileSprite(path) == true);
        Assert.True(m_mapEditor.m_tileSprites.ContainsKey(path) == true);
        Assert.True(m_mapEditor.m_tileSprites[path].name == "Tile");

        yield return new WaitForSeconds(0.1f);

        path = "MapGridComponent/Sprites/NotReal";

        Assert.True(m_mapEditor.LoadtTileSprite(path) == false);
        Assert.True(m_mapEditor.m_tileSprites.ContainsKey(path) == false);

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest, Order(3)]
    public IEnumerator LoadPrefab()
    {
        string name = "TestObject";

        Assert.True(m_mapEditor.LoadMapObject(name, 2, 1) == true);
        Assert.True(m_mapEditor.m_mapObjects.ContainsKey(name) == true);
        Assert.True(m_mapEditor.m_mapObjects[name].m_name == "TestObject");
        Assert.True(m_mapEditor.m_mapObjects[name].m_width == 2);
        Assert.True(m_mapEditor.m_mapObjects[name].m_height == 1);

        yield return new WaitForSeconds(0.1f);

        name = "NotRealObject";

        Assert.True(m_mapEditor.LoadMapObject(name, 1, 1) == false);
        Assert.True(m_mapEditor.m_mapObjects.ContainsKey(name) == false);

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest, Order(4)]
    public IEnumerator PlaceSprite()
    {
        string path = "MapGridComponent/Sprites/Wall";
        MapIndex indexPos = new MapIndex(2, 1);

        Assert.True(m_mapEditor.LoadtTileSprite(path) == true);
        Assert.True(m_mapEditor.m_tileSprites.ContainsKey(path) == true);

        yield return new WaitForSeconds(0.1f);

        m_mapEditor.ChangeTileSprite(indexPos, m_mapEditor.m_tileSprites[path]);

        Assert.True(m_map.GetTile(indexPos).GetSprite() == m_mapEditor.m_tileSprites[path]);

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest, Order(5)]
    public IEnumerator PlacePrefab()
    {
        string name = "TestObject";
        MapIndex indexPos = new MapIndex(2, 1);

        Assert.True(m_mapEditor.CheckCanPlaceMapObject(indexPos, m_mapEditor.m_mapObjects[name]) == true);

        m_mapEditor.InstantiateMapObject(indexPos, m_mapEditor.m_mapObjects[name]);

        Assert.True(m_map.GetTile(indexPos).GetEntityList().Count == 1);
        Assert.True(m_map.GetTile(new MapIndex(3,1)).GetEntityList().Count == 1);

        yield return new WaitForSeconds(0.1f);

        Assert.True(m_mapEditor.CheckCanPlaceMapObject(new MapIndex(-1, 0), m_mapEditor.m_mapObjects[name]) == false);

        yield return new WaitForSeconds(0.1f);

        Assert.True(m_mapEditor.CheckCanPlaceMapObject(indexPos, m_mapEditor.m_mapObjects[name]) == false);
    }

    [UnityTest, Order(6)]
    public IEnumerator SaveAndLoad()
    {
        Assert.True(m_mapEditor.SaveLevel("TestLevel") == true);

        yield return new WaitForSeconds(10.0f);

        int width = 20;
        int hight = 21;

        m_mapEditor.CreateMap(width, hight);

        Assert.AreEqual(width, m_map.GetWidth());
        Assert.AreEqual(hight, m_map.GetHight());
        Assert.AreEqual(width * hight, m_map.GetTileCount());

        yield return new WaitForSeconds(3.0f);

        Assert.True(m_mapEditor.LoadLevel("TestLevel", false) == true);

        Assert.AreEqual(17, m_map.GetWidth());
        Assert.AreEqual(11, m_map.GetHight());
        Assert.AreEqual(11 * 17, m_map.GetTileCount());

        MapIndex indexPos = new MapIndex(2, 1);

        Assert.True(m_map.GetTile(indexPos).GetEntityList().Count == 1);
        Assert.True(m_map.GetTile(new MapIndex(3, 1)).GetEntityList().Count == 1);

        string path = "MapGridComponent/Sprites/Wall";

        Assert.True(m_map.GetTile(indexPos).GetSprite() == m_mapEditor.m_tileSprites[path]);

        yield return new WaitForSeconds(0.1f);
    }
}