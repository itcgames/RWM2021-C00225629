using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class MapSuite
{
    GameObject m_map = null;
    Map m_mapScript = null;
    List<GameObject> m_enityList = new List<GameObject>();

    [SetUp]
    public void Setup()
    {
        m_map = new GameObject();
        m_map.AddComponent<Map>();

        m_mapScript = m_map.GetComponent<Map>();
    }

    [UnityTest]
    //Test For Feature 1 Task 2
    public IEnumerator MapDimensions()
    {
        int width = 25;
        int hight = 37;

        m_mapScript.SetSize(width, hight);

        Assert.AreEqual(width, m_mapScript.GetWidth());
        Assert.AreEqual(hight, m_mapScript.GetHight());

        m_mapScript.CreateMap();

        Assert.AreEqual(width * hight, m_mapScript.GetTileCount());

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 1
    public IEnumerator MapRemoveAddObject()
    {
        GameObject mapObject = new GameObject();
        mapObject.AddComponent<MapEntity>();
        mapObject.GetComponent<MapEntity>().SetEntityType(EntityType.OBJECT);
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        idList = m_mapScript.GetEntityIDs(new Pair<int, int>(0, 0));

        Assert.True(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        Assert.True(m_mapScript.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.False(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 1
    public IEnumerator MapRemoveAddCharacter()
    {
        GameObject mapObject = new GameObject();
        mapObject.AddComponent<MapEntity>();
        mapObject.GetComponent<MapEntity>().SetEntityType(EntityType.CHARACTER);
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        idList = m_mapScript.GetEntityIDs(new Pair<int, int>(0, 0));

        Assert.True(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        Assert.True(m_mapScript.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.False(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 1
    public IEnumerator MapRemoveObjectTwice()
    {
        GameObject mapObject = new GameObject();
        mapObject.AddComponent<MapEntity>();
        mapObject.GetComponent<MapEntity>().SetEntityType(EntityType.CHARACTER);
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        Assert.True(m_mapScript.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.False(m_mapScript.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.False(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 2
    public IEnumerator MapAddCharToObjectTile()
    {
        GameObject mapCharacter = new GameObject();
        mapCharacter.AddComponent<MapEntity>();
        mapCharacter.GetComponent<MapEntity>().SetEntityType(EntityType.CHARACTER);
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapCharacter.GetComponent<MapEntity>().GetID()));

        GameObject mapObject = new GameObject();
        mapObject.AddComponent<MapEntity>();
        mapObject.GetComponent<MapEntity>().SetEntityType(EntityType.OBJECT);

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.True(m_mapScript.RemoveEntityID(new Pair<int, int>(0, 0), mapCharacter.GetComponent<MapEntity>().GetID()));

        Assert.False(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapCharacter.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 2
    public IEnumerator MapAddObjectToObjectTile()
    {
        GameObject mapObject = new GameObject();
        mapObject.AddComponent<MapEntity>();
        mapObject.GetComponent<MapEntity>().SetEntityType(EntityType.OBJECT);
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();

        Assert.True(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        GameObject mapObject2 = new GameObject();
        mapObject2.AddComponent<MapEntity>();
        mapObject2.GetComponent<MapEntity>().SetEntityType(EntityType.OBJECT);
        yield return new WaitForSeconds(0.1f);

        Assert.False(m_mapScript.AddEntityID(new Pair<int, int>(0, 0), mapObject2.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }
}
