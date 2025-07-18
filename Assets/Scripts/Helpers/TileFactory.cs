using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileFactory : MonoBehaviour
{
    public GameObject tilePrefab; // Assign this in the inspector
    public GameObject boardParent;
    public static TileFactory _instance;
    //private BoardManager boardManager;
    private void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public Tile CreateTile(int x, int y)
    {
        // Instantiate the tile prefab
        GameObject tileObject = Instantiate(tilePrefab, boardParent.transform);
        
        // Get the Tile component
        Tile tile = tileObject.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.LogError("Tile prefab must have a Tile component!");
            return null;
        }

        // Initialize the tile
        tile.Initialize(x, y);

        return tile;
    }
}