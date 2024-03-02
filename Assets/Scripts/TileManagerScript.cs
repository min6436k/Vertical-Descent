using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public GameObject prefab;
    public float belowSpace; // 아래쪽 간격
    public bool randomFlipable = false;
}

public class TileManagerScript : MonoBehaviour
{
    [Header("타일 데이터 관련")]
    public List<TileData> datas;

    public List<TileData> spawnables;
    public Queue<TileData> standbies = new();

    [Header("게임 오브젝트 관련")]
    public GameObject ballObject;
    public BallScript ballScript;

    public GameObject tilesObject;

    public GameObject WallObject;

    public float startTileOffsetY = 7.5F;

    public float moveTileSpeed = 0;
    public float moveTileSpeedOffSet = 0;

    public float levelTileSpeed = 0;

    public bool isSlowing = false;
    public bool moveTile = false;
    public bool spawnable = true;

    public List<GameObject> tileList = new();

    public GameObject testPrefab;

    void Start()
    {
        spawnables = new(Shuffle(datas));
        ballScript = ballObject.GetComponent<BallScript>();
        SpawnTile();
    }

    private void Update()
    {
        if (!isSlowing && !ballScript.isFever)
            moveTileSpeed = moveTileSpeedOffSet;
    }

    public void WallBreak()
    {
        foreach (Transform i in WallObject.transform)
        {
            i.GetComponent<Collider2D>().enabled = false;
            i.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void RemoveWall()
    {
        Destroy(WallObject);
    }

    public void RemoveTiles()
    {
        tileList.ForEach(tile => Destroy(tile));
        tileList.Clear();
    }

    public GameObject SpawnTile()
    {
        return SpawnTile(RandomData(), null, new(0, startTileOffsetY));
    }

    public GameObject SpawnTile(GameObject gObject, TileScript script) // 타일에서 호출함
    {
        return SpawnTile(RandomData(), gObject, new(0, script.data.belowSpace));
    }

    public GameObject SpawnTile(TileData data, GameObject lastTileObject, Vector2 offset)
    {
        GameObject tile = Instantiate(data.prefab, tilesObject.transform);
        BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();

        float referencePositionY = 0;
        if (lastTileObject != null)
        {
            BoxCollider2D otherCollider = lastTileObject.GetComponent<BoxCollider2D>();
            referencePositionY = lastTileObject.transform.position.y + otherCollider.offset.y - otherCollider.size.y / 2.0f;
        }

        tile.transform.SetPositionAndRotation(new(-collider.offset.x - offset.x, -collider.offset.y - collider.size.y / 2.0f + referencePositionY - offset.y), new());
        if (Random.Range(0, 2) == 1 && data.randomFlipable)
            tile.transform.localScale = new Vector3(-1, 1);

        TileScript tileScript = tile.AddComponent<TileScript>();
        tileScript.tileManagerScript = this;
        tileScript.data = data;
        tileList.Add(tile);

        return tile;
    }

    public TileData RandomData()
    {
        TileData pick = spawnables[0];
        spawnables.RemoveAt(0);
        standbies.Enqueue(pick);
        CheckStandby();
        return pick;
    }

    public void CheckStandby()
    {
        if (standbies.Count < 3)
            return;
        spawnables.Insert(Random.Range(0, spawnables.Count + 1), standbies.Dequeue());
    }

    public bool IsSpawnable()
    {
        return spawnable;
    }

    public bool IsDestoryable() // MoveDown 호출 중 삭제되는 것 방지
    {
        return ballScript.isHandleable;
    }

    public IList<TileData> Shuffle(IList<TileData> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            TileData value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        if (testPrefab != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TileData data = list[i];
                data.prefab = testPrefab;
                list[i] = data;
            }
        }
        return list;
    }
}
