using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Switchstance", menuName = "Abilities/Switchstance")]
public class Switchstance : Ability
{
    private Chessman piece;
    
    public Switchstance() : base("Switchstance", "Swap attack and defense values on every combat (Attacking or Defending)") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttackEnd.AddListener(Swap);
        piece.releaseCost+=20;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttackEnd.RemoveListener(Swap); 

    }
    public void Swap(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (defender==piece || attacker==piece){
            piece.effectsFeedback.PlayFeedbacks();
            int attack = piece.attack;
            int defense = piece.defense;
            int bonusAttack= piece.attackBonus;
            int bonusDefense = piece.defenseBonus;
            piece.attack=defense;
            piece.defense=attack;
            piece.attackBonus=bonusDefense;
            piece.defenseBonus=bonusAttack;
        }
    }

}
