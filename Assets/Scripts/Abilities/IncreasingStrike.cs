using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreasingStrike", menuName = "Abilities/IncreasingStrike")]
public class IncreasingStrike : Ability
{
    private Chessman piece;
    
    public IncreasingStrike() : base("Increasing Strike", "Permanently gain +1 to attack for every capture") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceCaptured.AddListener(AddBonus);
        piece.releaseCost+=15;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if (attacker==piece){
            //Debug.Log("adding 1 bonus");
            piece.attack+=1;
        }
    }

}
