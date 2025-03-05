using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Tile : MonoBehaviour
{
    public BoardPosition position;
    public Sprite lightTileSprite;
    public Sprite darkTileSprite;

    private Chessman reference;
    private bool isValidMove = false;
    private bool isLightTile;
    [SerializeField] private Material bloodMat;
    [SerializeField] List<Texture2D> bloodTextures;

    private void OnMouseEnter(){
        Chessman piece = getPiece();
        if(piece){
            if(piece.team==Team.Hero)
                StatBoxManager._instance.SetAndShowStats(piece);
            else if(piece.team == Team.Enemy)
                EnemyStatBoxManager._instance.SetAndShowStats(piece);
        }
    }

    public Chessman getPiece(){
        GameObject obj =null;
        if (Game._instance.currentMatch!=null)
            obj = Game._instance.currentMatch.GetPieceAtPosition(position.x, position.y);
        if(obj !=null)
            return obj.GetComponent<Chessman>();
        else
            return null;
    }

    public PieceColor GetColor(){
        if (isLightTile)
            return PieceColor.White;
        else
            return PieceColor.Black;
    }

    public void Initialize(BoardPosition boardPosition){
        position = boardPosition;
        SetUIPosition();
    }

    public void SetBloodTile(){
        
        Texture2D randomBloodTexture = bloodTextures[UnityEngine.Random.Range(0, bloodTextures.Count)];
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material=bloodMat;
        spriteRenderer.material.SetFloat("_RandomRotation", UnityEngine.Random.Range(0f, 360f));
        spriteRenderer.material.SetTexture("_BloodTex", randomBloodTexture);
        //spriteRenderer.material.EnableKeyword("_RandomRotation");
        
    }

    public void SetUIPosition(){


        float x = position.x;
        float y = position.y;

        //Adjust by variable offset
        x *= .96f;
        y *= .96f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;
        
        name = $"Tile ({position.x}, {position.y})";
        //Debug.Log("positions: "+x+","+y);
        //Set actual unity values
        this.transform.position = new Vector3(x, y, -1.0f);

        isLightTile = !((position.x + position.y) % 2 == 0); // Even sum for light, odd for dark

    // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the appropriate sprite
        if (isLightTile)
        {
            spriteRenderer.sprite = lightTileSprite; // Assign light sprite
        }
        else
        {
            spriteRenderer.sprite = darkTileSprite;
        }
    }

    public void SetReference(Chessman obj)
    {
        reference = obj;
    }

    public Chessman GetReference()
    {
        return reference;
    }

    public void SetValidMove(){
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (getPiece() != null)
            spriteRenderer.color = Color.red;
        else    
            spriteRenderer.color=Color.green;

        isValidMove=true;
    }

    public void Clear(){
        reference=null;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        isValidMove=false;
    }

    private void OnMouseDown(){
        if (Game._instance.tileSelect)
        {
            BoardManager._instance.selectedPosition= this.position;
            Debug.Log(BoardManager._instance.selectedPosition==null);
            Debug.Log("positiono selected");
        }
        else if (Game._instance.isInMenu)
        {
            return;
        }
        else if(!isValidMove && !Game._instance.currentMatch.isSetUpPhase){
            BoardManager._instance.ClearTiles();
            var piece =getPiece();
            if(piece!=null && piece.isValidForAttack && piece.owner==Game._instance.hero){
                StatBoxManager._instance.UnlockView();
                piece.validMoves.Clear();
                piece.validMoves=piece.GetValidMoves();
                piece.DisplayValidMoves();
                StatBoxManager._instance.SetAndShowStats(piece);
                StatBoxManager._instance.LockView();
            }
            else{
                StatBoxManager._instance.UnlockView();
            }
        }
        
        
    }
    private void OnMouseUp(){
        if (Game._instance.isInMenu)
        {
            return;
        }
        else if (Game._instance.currentMatch.isSetUpPhase && reference !=null){
            BoardManager._instance.PlacePiece(reference, this);
        }
        else if(reference!=null && reference.isValidForAttack){
            /* Game._instance.currentMatch.currentPlayer.SetSelectedPiece(reference);
            Game._instance.currentMatch.currentPlayer.SetSelectedDestination(new BoardPosition(position.x, position.y));
            Game._instance.currentMatch.currentPlayer.RequestDecision(); */
            StatBoxManager._instance.UnlockView();
            Game._instance.currentMatch.ExecuteTurn(reference, position.x, position.y);
        }
        
    }

}
