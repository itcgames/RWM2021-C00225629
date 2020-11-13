using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TileSuite
{
    Tile tile;

    [SetUp]
    public void Setup()
    {
        tile = new Tile();
    }

    // 1
    [UnityTest]
    public IEnumerator TileIndex()
    {
        Pair<int, int> indexPos = new Pair<int, int>(1, 5);

        tile.SetIndexPos(indexPos);

        Assert.AreEqual(indexPos, tile.GetIndexPos());

        yield return new WaitForSeconds(0.1f);
    }

    // 2
    [UnityTest]
    public IEnumerator TileEntityIDList()
    {
        Assert.True(tile.AddEntityID(5));

        List<int> list = tile.GetIDList();

        Assert.True(list.Count == 1);
        Assert.True(list.Contains(5));

        Assert.True(tile.DeleteEntityID(5));
        Assert.False(tile.DeleteEntityID(5));

        yield return new WaitForSeconds(0.1f);
    }
}