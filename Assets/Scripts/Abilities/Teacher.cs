using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI.Extensions;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "Teacher", menuName = "Abilities/Teacher")]
public class Teacher : Ability
{
    private Chessman piece;
    private int bonus = 5;
    private Dictionary<Chessman, int> appliedBonus = new Dictionary<Chessman, int>();
    
    public Teacher() : base("Teacher", "+5 to all pieces with no abilities") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceAdded.AddListener(PieceAdded);
        board.EventHub.OnChessMatchStart.AddListener(ApplyBonus);
        board.EventHub.OnAbilityAdded.AddListener(RemoveBonusFromPiece);
        piece.OnChessmanStateChanged += HandleChessmanStateChanged;
        CreateGeneral();
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceAdded.RemoveListener(PieceAdded);
        eventHub.OnChessMatchStart.RemoveListener(ApplyBonus);
        eventHub.OnAbilityAdded.RemoveListener(RemoveBonusFromPiece);
        piece.OnChessmanStateChanged -= HandleChessmanStateChanged;
        ResetBonus();
    }

    public void CreateGeneral(){
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            if(cm != null && cm.abilities.Count==0 && !appliedBonus.ContainsKey(cm)){
                appliedBonus.Add(cm,0);
            }
        }
    }

    public void PieceAdded(Chessman addedPiece){
        if(addedPiece.owner==piece.owner && addedPiece.abilities.Count==0){ 
            if(!appliedBonus.ContainsKey(addedPiece)){
                appliedBonus.Add(addedPiece,0);
            }
        }
    }

    public void ApplyBonus(){
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            //Debug.Log($"Piece name {cm.name} piece type {cm.type}");
            if(cm != null && cm.abilities.Count==0){
                if (appliedBonus.ContainsKey(cm))
                {
                    var currentlyAppliedBonus = appliedBonus[cm];
                    cm.attackBonus += bonus;
                    cm.defenseBonus += bonus;
                    cm.supportBonus += bonus;
                    appliedBonus[cm] = bonus;
                }else{
                    Debug.Log($"Untracked knight {cm.name} not in dictionary or destroyed while adding");
                }
            }
        }
    }

    public void RemoveBonusFromPiece(Chessman addedPiece, Ability ability){
        if(addedPiece.owner==piece.owner && addedPiece.abilities.Count>0){ 
            if(appliedBonus.ContainsKey(addedPiece)){
                var currentlyAppliedBonus = appliedBonus[addedPiece];
                if(board.CurrentMatch!=null){
                    addedPiece.attackBonus = Mathf.Max(-addedPiece.attack, addedPiece.attackBonus - currentlyAppliedBonus);
                    addedPiece.defenseBonus = Mathf.Max(-addedPiece.defense, addedPiece.defenseBonus - currentlyAppliedBonus);
                    addedPiece.supportBonus = Mathf.Max(-addedPiece.support, addedPiece.supportBonus - currentlyAppliedBonus);
                }else{
                    addedPiece.attackBonus = Mathf.Max(0, addedPiece.attackBonus - currentlyAppliedBonus);
                    addedPiece.defenseBonus = Mathf.Max(0, addedPiece.defenseBonus - currentlyAppliedBonus);
                    addedPiece.supportBonus = Mathf.Max(0, addedPiece.supportBonus - currentlyAppliedBonus);
                }
                appliedBonus.Remove(addedPiece);
            }
        }
    }

    public void ResetBonus()
    {
        foreach (var piece in piece.owner.pieces){
            Chessman cm = piece.GetComponent<Chessman>();
            if(cm != null && cm.abilities.Count==0){
                if (appliedBonus.ContainsKey(cm))
                {
                    var currentlyAppliedBonus = appliedBonus[cm];
                    cm.attackBonus = Mathf.Max(-cm.attack, cm.attackBonus - currentlyAppliedBonus);
                    cm.defenseBonus = Mathf.Max(-cm.defense, cm.defenseBonus - currentlyAppliedBonus);
                    cm.supportBonus = Mathf.Max(-cm.support, cm.supportBonus - currentlyAppliedBonus);
                    appliedBonus[cm] = 0;
                }else{
                    Debug.Log($"Untracked knight {cm.name} not in dictionary or destroyed while removing");
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