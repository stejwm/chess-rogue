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
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        this.piece = piece;
        piece.info += " " + abilityName;
        game.OnPieceCaptured.AddListener(AddBonus);
    }

    public override void Remove(Chessman piece)
    {
        game.OnMove.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker){
        if (attacker==piece){
            Debug.Log("adding 1 bonus");
            piece.attack+=1;
        }
    }

}
