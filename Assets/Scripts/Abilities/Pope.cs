using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI.Extensions;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "Pope", menuName = "Abilities/Pope")]
public class Pope : Ability
{
    private Chessman piece;
    private int bonus = 1;
    private Dictionary<Chessman, int> appliedBonus = new Dictionary<Chessman, int>();
    
    public Pope() : base("Pope", "+1 to all bishops, bonus increases for each bishop added") {}

    public override void Apply(Board board, Chessman piece)
    {
        if(piece.type!=PieceType.Bishop)
            return;
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceAdded.AddListener(PieceAdded);
        board.EventHub.OnChessMatchStart.AddListener(ApplyBonus);
        CreateGeneral();
        base.Apply(board, piece);
        piece.OnChessmanStateChanged += HandleChessmanStateChanged;
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceAdded.RemoveListener(PieceAdded);
        eventHub.OnChessMatchStart.RemoveListener(ApplyBonus);
        piece.OnChessmanStateChanged -= HandleChessmanStateChanged;
        ResetBonus();
    }

    public void CreateGeneral(){
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            if(cm != null && cm.type==PieceType.Bishop && !appliedBonus.ContainsKey(cm)){
                appliedBonus.Add(cm,0);
            }
        }
    }

    public void PieceAdded(Chessman addedPiece){
        if(addedPiece.owner==piece.owner && addedPiece.type==PieceType.Bishop){ 
            if(!appliedBonus.ContainsKey(addedPiece)){
                appliedBonus.Add(addedPiece,0);
                bonus++;
            }
        }
    }

    public void ApplyBonus(){
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            //Debug.Log($"Piece name {cm.name} piece type {cm.type}");
            if(cm != null && cm.type==PieceType.Bishop){
                if (appliedBonus.ContainsKey(cm))
                {
                    var currentlyAppliedBonus = appliedBonus[cm];
                    cm.AddBonus(StatType.Attack,bonus, abilityName);
                    cm.AddBonus(StatType.Defense,bonus, abilityName);
                    cm.AddBonus(StatType.Support,bonus, abilityName);
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
                    cm.SetBonus(StatType.Attack, Mathf.Max(-cm.attack, cm.attackBonus - currentlyAppliedBonus), abilityName);
                    cm.SetBonus(StatType.Defense, Mathf.Max(-cm.defense, cm.defenseBonus - currentlyAppliedBonus), abilityName);
                    cm.SetBonus(StatType.Support, Mathf.Max(-cm.support, cm.supportBonus - currentlyAppliedBonus), abilityName);
                    appliedBonus[cm] = 0;
                    Debug.Log($"{cm.name} bonus removing, currently applied bonus {currentlyAppliedBonus} total bonus amount {bonus} amount to remove {currentlyAppliedBonus}");
                }else{
                    Debug.LogWarning($"Untracked bishop {cm.name} not in dictionary or destroyed while removing");
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