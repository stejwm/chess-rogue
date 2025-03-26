using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
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
    private int bloodCount=0;
    [SerializeField] private Material bloodMat;

    private void OnMouseEnter(){
        Chessman piece = getPiece();
        if(piece){
            if(piece.team==Team.Hero)
                StatBoxManager._instance.SetAndShowStats(piece);
            else if(piece.team == Team.Enemy)
                StatBoxManager._instance.SetAndShowEnemyStats(piece);

            piece.GetComponent<MMSpringPosition>().Bump(new Vector3(0, 5f, 0f));
        }
    }

    private void OnMouseExit()
    {
        /* Chessman piece = getPiece();
        if(piece){
            if(piece.owner != Game._instance.hero && StatBoxManager._instance.enemyLockedPiece==piece)
                return;
            else
                piece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        } */
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
        var rand = UnityEngine.Random.Range(0,1000);
        Debug.Log(rand);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material=bloodMat;
        spriteRenderer.material.SetFloat("_RandomSeed", rand);
        spriteRenderer.material.SetFloat("_BloodCount", ++bloodCount);
        
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
        }
        else if (Game._instance.isInMenu)
        {
            return;
        }
        else if(Game._instance.state == ScreenState.ManagementScreen){
            ArmyManager._instance.PositionSelect(this.position);
        }
        else if(!isValidMove && !Game._instance.currentMatch.isSetUpPhase){
            
            var piece =getPiece();
            if(piece== null || piece.owner == Game._instance.hero)
                BoardManager._instance.ClearTiles();
            if(piece!=null && piece.owner==Game._instance.hero && piece==StatBoxManager._instance.lockedPiece){
                StatBoxManager._instance.UnlockView();
                //piece.flames.Stop();
                piece.validMoves.Clear();
            }
            else if(piece!=null && piece.isValidForAttack && piece.owner==Game._instance.hero){
                //Game._instance.StopHeroFlames();
                StatBoxManager._instance.UnlockView();
                piece.validMoves.Clear();
                piece.validMoves=piece.GetValidMoves();
                piece.DisplayValidMoves();
                //piece.flames.Play();
                StatBoxManager._instance.SetAndShowStats(piece);
                StatBoxManager._instance.LockView(piece);
            }
            else if(piece!=null && piece.owner!=Game._instance.hero && piece == StatBoxManager._instance.enemyLockedPiece){
                StatBoxManager._instance.UnlockEnemyView();
                //piece.validMoves.Clear();
            }
            else if(piece!=null && piece.owner!=Game._instance.hero){
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.SetAndShowEnemyStats(piece);
                StatBoxManager._instance.LockEnemyView(piece);
            }
            else if(piece==null){
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.UnlockView();
            }
        }
        
        
    }

    
    private void OnMouseUp(){
        if (Game._instance.isInMenu || Game._instance.currentMatch==null)
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
            Debug.Log("On Move true");
            StatBoxManager._instance.UnlockView(true);
            //reference.flames.Stop();
            Game._instance.currentMatch.ExecuteTurn(reference, position.x, position.y);
        }
        
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        Tile other = (Tile)obj;
        return position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
