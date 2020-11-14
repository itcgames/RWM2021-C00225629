using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class MapSuite
{
    Map m_map = null;

    [SetUp]
    public void Setup()
    {
        m_map = new Map();
    }

    [UnityTest]
    //Test For Task 2
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
}
