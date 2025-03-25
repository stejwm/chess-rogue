using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public Dictionary<BoardPosition, Tile> tiles = new Dictionary<BoardPosition, Tile>();
    public List<Tile> validTiles= new List<Tile>();

    //current turn
    public static BoardManager _instance;
    public BoardPosition selectedPosition;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public void Start()
    {
        
    }

    public void SetActiveTile(Chessman piece, BoardPosition position){
        var tile = tiles[position];
        validTiles.Add(tile);
        tile.SetReference(piece);
        tile.SetValidMove();
    }

    public Tile GetTileAt(int x, int y){
        //Debug.Log("X: "+x+" Y: "+y);
        return tiles[new BoardPosition(x,y)];
    }

    public void CreateBoard(){

        for(int i =0; i<8; i++){
            for (int j=0; j<8; j++){
                BoardPosition pos = new BoardPosition(i,j);
                tiles.Add(pos, TileFactory._instance.CreateTile(pos));
            }
        }
    }

    public void CreateManagementBoard(){

        for(int i =0; i<8; i++){
            for (int j=0; j<3; j++){
                BoardPosition pos = new BoardPosition(i,j);
                tiles.Add(pos, TileFactory._instance.CreateTile(pos));
            }
        }
        foreach (var tile in tiles.Values){
            SpriteRenderer rend = tile.GetComponent<SpriteRenderer>();
            rend.sortingOrder = 4;
        }
    }

    public void DestroyBoard(){
        foreach (var item in tiles.Values)
        {
            Destroy(item.gameObject);
        }
        tiles.Clear();
        validTiles.Clear();
    }
    
    public void ClearTiles(){
        foreach (var tile in validTiles)
        {
            tile.Clear();
        }
        validTiles.Clear();
    }

    public void toggleTileColliders(bool active){
        
        foreach (var tile in tiles.Values)
        {
            tile.GetComponent<BoxCollider2D>().enabled=active;
        }
    }

    

    public void SelectPieceToPlace(Chessman piece){
        foreach (var item in Game._instance.hero.openPositions)
        {
            SetActiveTile(piece,item);
        }
    }

    public void PlacePiece(Chessman piece, Tile tile){
        
        
        piece.startingPosition = tile.position;
        piece.xBoard=tile.position.x;
        piece.yBoard=tile.position.y;
        Game._instance.hero.inventoryPieces.Remove(piece.gameObject);
        Game._instance.hero.openPositions.Remove(tile.position);
        Game._instance.hero.pieces.Add(piece.gameObject);
        Game._instance.OnPieceAdded.Invoke(piece);
        ClearTiles();
        piece.UpdateUIPosition();
        Game._instance.currentMatch.CheckInventory();
    }


}
