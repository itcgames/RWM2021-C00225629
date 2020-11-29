using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelScript : MonoBehaviour
{
    public Map map;
    public GameObject objectPrefab;
    public GameObject charPrefab;

    [HideInInspector]
    public GameObject charEntity;

    void Awake()
    {
        map.CreateMap();

        charEntity = Instantiate(charPrefab, new Vector2(0.48f, 0.48f), Quaternion.identity);
        map.AddEntity(new MapIndex(1, 1), charEntity);
        map.AddEntityToList(charEntity);

        GameObject objectEntity = Instantiate(objectPrefab, new Vector2(0.16f, 0.48f), Quaternion.identity);
        map.AddEntity(new MapIndex(0, 1), objectEntity);
        map.AddEntityToList(objectEntity);
    }
}
