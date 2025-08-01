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

public class Tile : MonoBehaviour, IInteractable
{
    int x;
    int y;
    public Sprite lightTileSprite;
    public Sprite darkTileSprite;

    private Chessman currentPiece;
    private Chessman startingPiece;
    private Chessman reference;
    public bool isValidMove = false;
    private bool isLightTile;
    private int bloodCount = 0;
    [SerializeField] private Material bloodMat;
    private Material originalMaterial;

    public Chessman StartingPiece { get => startingPiece; set => startingPiece = value; }
    //public Chessman CurrentPiece { get => currentPiece; set => currentPiece = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

    private void OnMouseEnter()
    {
    }


    public PieceColor GetColor()
    {
        if (isLightTile)
            return PieceColor.White;
        else
            return PieceColor.Black;
    }

    public void Initialize(int x, int y)
    {
        this.X = x;
        this.Y = y;
        originalMaterial = GetComponent<SpriteRenderer>().material;
        SetUIPosition();
    }

    public void SetBloodTile()
    {
        var rand = UnityEngine.Random.Range(0, 1000);
        //Debug.Log(rand);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = bloodMat;
        spriteRenderer.material.SetFloat("_RandomSeed", rand);
        spriteRenderer.material.SetFloat("_BloodCount", ++bloodCount);

    }

    public void ClearBloodTile()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = originalMaterial;
        bloodCount = 0;
    }

    public void SetUIPosition()
    {


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

    public void SetValidMove()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        /* if (currentPiece != null)
            spriteRenderer.color = Color.red;
        else  */
        spriteRenderer.color = Color.green;

        isValidMove = true;
    }

    public void Clear()
    {
        reference = null;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        isValidMove = false;
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

    public void HandleActiveMatchClick(Board board)
    {
        if (board.IsInMove)
            return;
        else if (!isValidMove && !board.CurrentMatch.isSetUpPhase)
        {

            var piece = board.GetChessmanAtPosition(this);
            if (piece == null || piece.owner == board.Hero)
                board.ClearTiles();
            if (piece != null && piece.owner == board.Hero && piece == StatBoxManager._instance.lockedPiece)
            {
                StatBoxManager._instance.UnlockView();
                //piece.flames.Stop();
                piece.validMoves.Clear();
            }
            else if (piece != null && piece.isValidForAttack && piece.owner == board.Hero)
            {
                //Game._instance.StopHeroFlames();
                //Debug.Log("Display moves for piece: " + piece.name);
                StatBoxManager._instance.UnlockView();
                DisplayValidMoves(piece, board);
                StatBoxManager._instance.SetAndShowStats(piece);
                StatBoxManager._instance.LockView(piece);
            }
            else if (piece != null && piece.owner != board.Hero && piece == StatBoxManager._instance.enemyLockedPiece)
            {
                //Debug.Log(board.Hero.name);
                StatBoxManager._instance.UnlockEnemyView();
                //piece.validMoves.Clear();
            }
            else if (piece != null && piece.owner != board.Hero)
            {
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.SetAndShowEnemyStats(piece);
                StatBoxManager._instance.LockEnemyView(piece);
            }
            else if (piece == null)
            {
                StatBoxManager._instance.UnlockEnemyView();
                StatBoxManager._instance.UnlockView();
            }
        }
        else if (board.CurrentMatch == null)
        {
            return;
        }
        else if (reference != null && reference.isValidForAttack)
        {

            board.IsInMove = true;
            StatBoxManager._instance.UnlockView(true);
            board.CurrentMatch.ExecuteTurn(reference, this.X, this.Y);
        }

    }
    public void OnClick(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.ActiveMatch:
                HandleActiveMatchClick(board);
                break;
            case BoardState.PrisonersMarket:
                HandleMarketClick(board);
                break;
            case BoardState.RewardScreen:
                HandleRewardClick(board);
                break;
            case BoardState.ShopScreen:
                HandleShopClick(board);
                break;
            case BoardState.ManagementScreen:
                HandleManagementClick(board);
                break;
            case BoardState.KingsOrderActive:
                board.SetSelectedPosition(this);
                break;
        }
    }

    public void HandleMarketClick(Board board)
    {
        Chessman cm = board.GetChessmanAtPosition(this);
        if (cm != null && cm.gameObject.activeSelf)
            board.MarketManager.AddPieceToSelection(cm);
    }
    public void HandleRewardClick(Board board)
    {
        Chessman cm = board.GetChessmanAtPosition(this);
        if (cm != null && cm.gameObject.activeSelf)
            board.RewardManager.SelectedPiece(cm);
    }
    public void HandleShopClick(Board board)
    {
        Chessman cm = board.GetChessmanAtPosition(this);
        if (cm == null)
            return;
        else if (cm.gameObject.activeSelf && cm.owner != null)
            board.ShopManager.SelectedPiece(cm);
        else if (cm.owner == null)
            board.ShopManager.PurchasePiece(cm);
    }
    public void HandleManagementClick(Board board)
    {
        Chessman cm = board.GetChessmanAtPosition(this);
        if (cm != null && cm.gameObject.activeSelf && cm.owner != null)
            board.ArmyManager.PieceSelect(cm);
        else
            board.ArmyManager.PositionSelect(this);
    }
    public void DisplayValidMoves(Chessman piece, Board board)
    {
        foreach (var coordinate in piece.GetValidMoves())
        {
            if (BoardPosition.IsPositionOnBoard(coordinate))
            {
                board.SetActiveTile(piece, coordinate);
            }
        }
    }

    public void OnRightClick(Board board)
    {
        Chessman cm = board.GetChessmanAtPosition(this);
        if (cm != null)
        {
            board.OpenPieceInfo(cm);
        }
    }

    public void OnHover(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.ManagementScreen:
            case BoardState.KingsOrder:
            case BoardState.ActiveMatch:
            case BoardState.RewardScreen:
                SetStatBox(board.GetChessmanAtPosition(this));
                break;
            case BoardState.PrisonersMarket:
            case BoardState.ShopScreen:
                SetStatBox(board.GetChessmanAtPosition(this));
                ShowPieceValues(board, board.GetChessmanAtPosition(this));
                break;
        }
    }
    public void OnHoverExit(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.ShopScreen:
            case BoardState.PrisonersMarket:
                PopUpManager._instance.HideValues();
                break;
        }
    }

    public void SetStatBox(Chessman piece)
    {
        if (piece && piece.gameObject.activeSelf)
        {
            if (piece.color == PieceColor.White)
                StatBoxManager._instance.SetAndShowStats(piece);
            else if (piece.color == PieceColor.Black)
                StatBoxManager._instance.SetAndShowEnemyStats(piece);

            piece.GetComponent<MMSpringPosition>().Bump(new Vector3(0, 5f, 0f));
        }
    }
    public void ShowPieceValues(Board board, Chessman piece)
    {
        if (piece && piece.gameObject.activeSelf)
        {
            if(piece.owner == board.Opponent)
                PopUpManager._instance.SetAndShowValues(piece, false, true);
            else
                PopUpManager._instance.SetAndShowValues(piece, true, false);

                piece.GetComponent<MMSpringPosition>().Bump(new Vector3(0, 5f, 0f));
        }
    }


}
