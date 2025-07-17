using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class EventHub
{

    public UnityEvent<Chessman, Chessman> OnPieceCaptured = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, int, Tile> OnAttack = new UnityEvent<Chessman, int, Tile>();
    public UnityEvent<Chessman, Chessman> OnAttackStart = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, Chessman, int, int> OnAttackEnd = new UnityEvent<Chessman, Chessman, int, int>();
    public UnityEvent<Chessman, Tile> OnMove = new UnityEvent<Chessman, Tile>();
    public UnityEvent<Chessman, Tile> OnRawMoveEnd = new UnityEvent<Chessman, Tile>();
    public UnityEvent<Chessman, Chessman> OnPieceBounced = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, Chessman, Chessman> OnSupportAdded = new UnityEvent<Chessman, Chessman, Chessman>();
    public UnityEvent OnChessMatchStart = new UnityEvent();
    public UnityEvent<Chessman, Ability> OnAbilityAdded = new UnityEvent<Chessman, Ability>();
    public UnityEvent<Chessman> OnPieceAdded = new UnityEvent<Chessman>();
    public UnityEvent<PieceColor> OnGameEnd = new UnityEvent<PieceColor>();
    public UnityEvent OnSoulBonded = new UnityEvent();


    public void RaisePieceMoved(Chessman piece, Tile tile)
    {
        OnMove?.Invoke(piece, tile);
    }

    public void RaisePieceCaptured(Chessman attacker, Chessman defender)
    {
        OnPieceCaptured?.Invoke(attacker, defender);
    }
    public void RaiseAttacked(Chessman piece, int support, Tile tile)
    {
        OnAttack?.Invoke(piece, support, tile);
    }

    public void RaiseAttackStart(Chessman attacker, Chessman defender)
    {
        OnAttackStart?.Invoke(attacker, defender);
    }

    public void RaiseAttackEnd(Chessman attacker, Chessman defender, int damageDealt, int damageTaken)
    {
        OnAttackEnd?.Invoke(attacker, defender, damageDealt, damageTaken);
    }

    public void RaiseRawMoveEnd(Chessman piece, Tile tile)
    {
        OnRawMoveEnd?.Invoke(piece, tile);
    }

    public void RaisePieceBounced(Chessman piece, Chessman target)
    {
        OnPieceBounced?.Invoke(piece, target);
    }

    public void RaiseSupportAdded(Chessman attacker, Chessman defender, Chessman supporter)
    {
        OnSupportAdded?.Invoke(attacker, defender, supporter);
    }

    public void RaiseChessMatchStart()
    {
        OnChessMatchStart?.Invoke();
    }

    public void RaiseAbilityAdded(Chessman piece, Ability ability)
    {
        OnAbilityAdded?.Invoke(piece, ability);
    }
    public void RaisePieceAdded(Chessman piece)
    {
        OnPieceAdded?.Invoke(piece);
    }
    public void RaiseGameEnd(PieceColor winner)
    {
        OnGameEnd?.Invoke(winner);
    }
    public void RaiseSoulBonded()
    {
        OnSoulBonded?.Invoke();
    }







}