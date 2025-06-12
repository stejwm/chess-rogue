using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    int x;
    int y;
    public Sprite lightTileSprite;
    public Sprite darkTileSprite;

    private Chessman currentPiece;
    private Chessman startingPiece;
    private Chessman reference;
    private bool isValidMove = false;
    private bool isLightTile;
    private int bloodCount=0;
    [SerializeField] private Material bloodMat;

    public Chessman StartingPiece { get => startingPiece; set => startingPiece = value; }
    public Chessman CurrentPiece { get => currentPiece; set => currentPiece = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

    private void OnMouseEnter(){
        Chessman piece = currentPiece;
        if(piece){
            if(piece.team==Team.Hero)
                StatBoxManager._instance.SetAndShowStats(piece);
            else if(piece.team == Team.Enemy)
                StatBoxManager._instance.SetAndShowEnemyStats(piece);

            piece.GetComponent<MMSpringPosition>().Bump(new Vector3(0, 5f, 0f));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Tile clicked at position ({X}, {Y})");
        FindObjectOfType<GameInputRouter>().OnClick(gameObject);
    }

    public PieceColor GetColor()
    {
        if (isLightTile)
            return PieceColor.White;
        else
            return PieceColor.Black;
    }

    public void Initialize(int x, int y){
        this.X = x;
        this.Y = y;
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


        float UIx = X;
        float UIy = Y;

        //Adjust by variable offset
        UIx *= .96f;
        UIy *= .96f;

        //Add constants (pos 0,0)
        UIx += -3.33f;
        UIy += -3.33f;
        
        name = $"Tile ({X}, {Y})";
        //Debug.Log("positions: "+x+","+y);
        //Set actual unity values
        this.transform.position = new Vector3(UIx, UIy, -1.0f);

        isLightTile = !((X + Y) % 2 == 0); // Even sum for light, odd for dark

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

        if (currentPiece != null)
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
        /* if (GameManager._instance.tileSelect)
        {
            Board._instance.selectedPosition= this.position;
        }
        else if (GameManager._instance.isInMenu)
        {
            return;
        }
        else if(GameManager._instance.state == ScreenState.ManagementScreen){
            ArmyManager._instance.PositionSelect(this.position);
        }
        else if(!isValidMove && !board.CurrentMatch.isSetUpPhase){
            
            var piece = currentPiece;
            if(piece== null || piece.owner == GameManager._instance.hero)
                Board._instance.ClearTiles();
            if(piece!=null && piece.owner==GameManager._instance.hero && piece==StatBoxManager._instance.lockedPiece){
                StatBoxManager._instance.UnlockView();
                //piece.flames.Stop();
                piece.validMoves.Clear();
            }
            else if(piece!=null && piece.isValidForAttack && piece.owner==GameManager._instance.hero){
                //Game._instance.StopHeroFlames();
                StatBoxManager._instance.UnlockView();
                piece.validMoves.Clear();
                piece.validMoves=piece.GetValidMoves();
                piece.DisplayValidMoves();
                //piece.flames.Play();
                StatBoxManager._instance.SetAndShowStats(piece);
                StatBoxManager._instance.LockView(piece);
            }
            else if(piece!=null && piece.owner!=GameManager._instance.hero && piece == StatBoxManager._instance.enemyLockedPiece){
                StatBoxManager._instance.UnlockEnemyView();
                //piece.validMoves.Clear();
            }
            else if(piece!=null && piece.owner!=GameManager._instance.hero){
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.SetAndShowEnemyStats(piece);
                StatBoxManager._instance.LockEnemyView(piece);
            }
            else if(piece==null){
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.UnlockView();
            }
        } */
        
        
    }

    
    private void OnMouseUp(){
        /* if (GameManager._instance.isInMenu || board.CurrentMatch==null)
        {
            return;
        }
        else if (board.CurrentMatch.isSetUpPhase && reference !=null){
            Board._instance.PlacePiece(reference, this);
        }
        else if(reference!=null && reference.isValidForAttack){
            
            Debug.Log("On Move true");
            StatBoxManager._instance.UnlockView(true);
            //reference.flames.Stop();
            board.CurrentMatch.ExecuteTurn(reference, position.x, position.y);
        } */
        
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        Tile other = (Tile)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
}
