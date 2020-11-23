using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTestScript : MonoBehaviour
{
    public Map map;
    public GameObject objectPrefab;
    public GameObject charPrefab;

    public GameObject charEntity;

    // Start is called before the first frame update
    void Start()
    {
        map.CreateMap();

        charEntity = Instantiate(charPrefab, new Vector2(0.48f, 0.48f), Quaternion.identity);
        map.AddEntity(new Pair<int, int>(1, 1), charEntity);
        map.AddEntityToList(charEntity);

        GameObject objectEntity = Instantiate(objectPrefab, new Vector2(0.16f, 0.48f), Quaternion.identity);
        map.AddEntity(new Pair<int, int>(0, 1), objectEntity);
        map.AddEntityToList(objectEntity);
    }
}
