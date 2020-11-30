using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TileSuite
{
    Tile tile;
    GameObject tileObject;

    [SetUp]
    public void Setup()
    {
        tileObject = new GameObject();
        tileObject.AddComponent<Tile>();

        tile = tileObject.GetComponent<Tile>();
    }

    //Test For Task 1
    [UnityTest]
    public IEnumerator TileIndex()
    {
        MapIndex indexPos = new MapIndex(1, 5);

        tile.SetIndexPos(indexPos);

        Assert.AreEqual(indexPos, tile.GetIndexPos());

        yield return new WaitForSeconds(0.1f);
    }

    //Test For Task 1
    [UnityTest]
    public IEnumerator TileEntityList()
    {
        GameObject mapObject = new GameObject();
        mapObject.tag = "Object";

        Assert.True(tile.AddEntity(mapObject));

        List<GameObject> list = tile.GetEntityList();

        Assert.True(list.Count == 1);
        Assert.True(list.Contains(mapObject));

        Assert.True(tile.DeleteEntity(mapObject));
        Assert.False(tile.DeleteEntity(mapObject));

        yield return new WaitForSeconds(0.1f);
    }
}