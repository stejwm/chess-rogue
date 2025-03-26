using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI.Extensions;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "Sentinel", menuName = "Abilities/Sentinel")]
public class Sentinel : Ability
{
    private Chessman piece;
    private int bonus = 1;
    private Dictionary<Chessman, int> appliedBonus = new Dictionary<Chessman, int>();
    
    public Sentinel() : base("Sentinel", "+1 to all rooks, bonus increases for each rook added") {}

    public override void Apply(Chessman piece)
    {
        if(piece.type!=PieceType.Rook)
            return;
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceAdded.AddListener(PieceAdded);
        Game._instance.OnChessMatchStart.AddListener(ApplyBonus);
        piece.releaseCost+=20;
        CreateGeneral();
        base.Apply(piece);
        piece.OnChessmanStateChanged += HandleChessmanStateChanged;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceAdded.RemoveListener(PieceAdded);
        Game._instance.OnChessMatchStart.RemoveListener(ApplyBonus);
        piece.OnChessmanStateChanged -= HandleChessmanStateChanged;
        ResetBonus();
    }

    public void CreateGeneral(){
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            if(cm != null && cm.type==PieceType.Rook && !appliedBonus.ContainsKey(cm)){
                appliedBonus.Add(cm,0);
            }
        }
    }

    public void PieceAdded(Chessman addedPiece){
        if(addedPiece.owner==piece.owner && addedPiece.type==PieceType.Rook){ 
            if(!appliedBonus.ContainsKey(addedPiece)){
                appliedBonus.Add(addedPiece,0);
                Debug.Log($"{addedPiece.name} added to dictionary, count {appliedBonus.Count}");
                bonus++;
            }
        }
    }

    public void ApplyBonus(){
        Debug.Log($"Adding General bonus dictionary size {appliedBonus.Count}");
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            //Debug.Log($"Piece name {cm.name} piece type {cm.type}");
            if(cm != null && cm.type==PieceType.Rook){
                if (appliedBonus.ContainsKey(cm))
                {
                    var currentlyAppliedBonus = appliedBonus[cm];
                    cm.attackBonus += bonus;
                    cm.defenseBonus += bonus;
                    cm.supportBonus += bonus;
                    appliedBonus[cm] = bonus;
                    Debug.Log($"{cm.name} bonus applying, currently applied bonus {currentlyAppliedBonus} total bonus amount {bonus} amount to apply {bonus-currentlyAppliedBonus}");
                }else{
                    Debug.Log($"Untracked Bishop {cm.name} not in dictionary or destroyed while adding");
                }
            }
        }
    }

    public void ResetBonus()
    {
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            if(cm != null && cm.type==PieceType.Bishop){
                if (appliedBonus.ContainsKey(cm))
                {
                    var currentlyAppliedBonus = appliedBonus[cm];
                    cm.attackBonus -= currentlyAppliedBonus;
                    cm.defenseBonus -= currentlyAppliedBonus;
                    cm.supportBonus -= currentlyAppliedBonus;
                    appliedBonus[cm] = 0;
                    Debug.Log($"{cm.name} bonus removing, currently applied bonus {currentlyAppliedBonus} total bonus amount {bonus} amount to remove {currentlyAppliedBonus}");
                }else{
                    Debug.LogWarning($"Untracked Bishop {cm.name} not in dictionary or destroyed while removing");
                }
            }
        }
    }

    private void HandleChessmanStateChanged(bool isEnabled)
    {
        if(!isEnabled)
            ResetBonus();
    }

}