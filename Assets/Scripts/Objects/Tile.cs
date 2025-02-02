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

    private void OnMouseEnter(){
        Chessman piece = getPiece();
        if(piece){
            var sprite = piece.GetComponent<SpriteRenderer>().sprite;
            if(piece.team==Team.Hero)
                StatBoxManager._instance.SetAndShowStats(piece.CalculateAttack(),piece.CalculateDefense(),piece.CalculateSupport(),piece.info,piece.name, sprite);
            else if(piece.team == Team.Enemy)
                EnemyStatBoxManager._instance.SetAndShowStats(piece.CalculateAttack(),piece.CalculateDefense(),piece.CalculateSupport(),piece.info,piece.name, sprite);
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

    public void Initialize(BoardPosition boardPosition){
        position = boardPosition;
        SetUIPosition();
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

        bool isLightTile = (position.x + position.y) % 2 == 0; // Even sum for light, odd for dark

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
        if (Game._instance.isInMenu)
        {
            return;
        }
        if(!isValidMove && !Game._instance.currentMatch.isSetUpPhase){
            BoardManager._instance.ClearTiles();
            var piece =getPiece();
            if(piece!=null && piece.isValidForAttack && piece.owner==Game._instance.hero)
                piece.validMoves.Clear();
                piece.validMoves=piece.GetValidMoves();
                piece.DisplayValidMoves();
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
            Game._instance.currentMatch.ExecuteTurn(reference, position.x, position.y);
        }
        
    }

}
