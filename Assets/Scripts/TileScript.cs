using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    public TileData data;
    public TileManagerScript tileManagerScript;

    public bool isSpawned = false;
    public Rigidbody2D rbody;

    private void FixedUpdate()
    {
        if (rbody == null)
            rbody = GetComponent<Rigidbody2D>();
        if (tileManagerScript.moveTile)
            rbody.MovePosition(transform.position + new Vector3(0, tileManagerScript.moveTileSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TileSpawn") && !isSpawned && tileManagerScript.IsSpawnable())
        {
            isSpawned = true;
            tileManagerScript.SpawnTile(gameObject, this);
        }
        else if (collision.gameObject.CompareTag("TileDespawn") && tileManagerScript.IsDestoryable())
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        tileManagerScript.tileList.Remove(gameObject);
    }

}
