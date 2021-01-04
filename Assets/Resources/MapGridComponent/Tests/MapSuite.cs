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
        mapObject.tag = "Object";
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();
        m_mapScript.m_objectsTag.Add("Object");

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), mapObject));

        List<GameObject> entityList = new List<GameObject>();

        entityList = m_mapScript.GetEntity(new MapIndex(0, 0));

        Assert.True(entityList.Contains(mapObject));

        Assert.True(m_mapScript.RemoveEntity(new MapIndex(0, 0), mapObject));

        Assert.False(entityList.Contains(mapObject));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 1
    public IEnumerator MapRemoveAddCharacter()
    {
        GameObject mapChar = new GameObject();
        mapChar.tag = "Character";
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();
        m_mapScript.m_charsTag.Add("Character");
        m_mapScript.m_objectsTag.Add("Object");

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), mapChar));

        List<GameObject> entityList = new List<GameObject>();

        entityList = m_mapScript.GetEntity(new MapIndex(0, 0));

        Assert.True(entityList.Contains(mapChar));

        Assert.True(m_mapScript.RemoveEntity(new MapIndex(0, 0), mapChar));

        Assert.False(entityList.Contains(mapChar));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 1
    public IEnumerator MapRemoveObjectTwice()
    {
        GameObject charEntity = new GameObject();
        charEntity.tag = "Character";
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();
        m_mapScript.m_charsTag.Add("Character");
        m_mapScript.m_objectsTag.Add("Object");

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), charEntity));

        List<GameObject> entityList = new List<GameObject>();

        Assert.True(m_mapScript.RemoveEntity(new MapIndex(0, 0), charEntity));

        Assert.False(m_mapScript.RemoveEntity(new MapIndex(0, 0), charEntity));

        Assert.False(entityList.Contains(charEntity));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 2
    public IEnumerator MapAddCharToObjectTile()
    {
        GameObject mapCharacter = new GameObject();
        mapCharacter.tag = "Character";
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();
        m_mapScript.m_charsTag.Add("Character");
        m_mapScript.m_objectsTag.Add("Object");

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), mapCharacter));

        GameObject mapObject = new GameObject();
        mapObject.tag = "Object";

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), mapObject));

        Assert.True(m_mapScript.RemoveEntity(new MapIndex(0, 0), mapCharacter));

        Assert.False(m_mapScript.AddEntity(new MapIndex(0, 0), mapCharacter));

        yield return new WaitForSeconds(0.1f);
    }

    [UnityTest]
    //Test For Feature 2 Task 2
    public IEnumerator MapAddObjectToObjectTile()
    {
        GameObject mapObject = new GameObject();
        mapObject.tag = "Object";
        yield return new WaitForSeconds(0.1f);

        m_mapScript.SetSize(1, 1);
        m_mapScript.CreateMap();
        m_mapScript.m_objectsTag.Add("Object");

        Assert.True(m_mapScript.AddEntity(new MapIndex(0, 0), mapObject));

        GameObject mapObject2 = new GameObject();
        mapObject2.tag = "Object";
        yield return new WaitForSeconds(0.1f);

        Assert.False(m_mapScript.AddEntity(new MapIndex(0, 0), mapObject2));

        yield return new WaitForSeconds(0.1f);
    }
}
