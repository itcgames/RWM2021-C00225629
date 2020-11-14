using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class MapSuite
{
    Map m_map = null;
    List<GameObject> m_enityList = new List<GameObject>();

    [SetUp]
    public void Setup()
    {
        m_map = new Map();
    }

    [UnityTest]
    //Test For Feature 1 Task 2
    public IEnumerator MapDimensions()
    {
        int width = 25;
        int hight = 37;

        m_map.SetSize(width, hight);

        Assert.AreEqual(width, m_map.GetWidth());
        Assert.AreEqual(hight, m_map.GetHight());

        m_map.CreateMap();

        Assert.AreEqual(width * hight, m_map.GetTileCount());

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

        m_map.SetSize(1, 1);
        m_map.CreateMap();

        Assert.True(m_map.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        idList = m_map.GetEntityIDs(new Pair<int, int>(0, 0));

        Assert.True(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        Assert.True(m_map.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

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

        m_map.SetSize(1, 1);
        m_map.CreateMap();

        Assert.True(m_map.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        idList = m_map.GetEntityIDs(new Pair<int, int>(0, 0));

        Assert.True(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        Assert.True(m_map.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

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

        m_map.SetSize(1, 1);
        m_map.CreateMap();

        Assert.True(m_map.AddEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        List<int> idList = new List<int>();

        Assert.True(m_map.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));
        
        Assert.False(m_map.RemoveEntityID(new Pair<int, int>(0, 0), mapObject.GetComponent<MapEntity>().GetID()));

        Assert.False(idList.Contains(mapObject.GetComponent<MapEntity>().GetID()));

        yield return new WaitForSeconds(0.1f);
    }
}
