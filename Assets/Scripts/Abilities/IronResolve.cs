using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IronResolve", menuName = "Abilities/IronResolve")]
public class IronResolve : Ability
{
    private Chessman piece;
    
    public IronResolve() : base("Iron Resolve", "Permanently gain +1 to defense every time you bounce an enemy") {}

    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        this.piece = piece;
        piece.info += " " + abilityName;
        game.OnPieceBounced.AddListener(AddBonus);
        piece.releaseCost+=10;
    }

    public override void Remove(Chessman piece)
    {
        game.OnPieceBounced.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender, bool isBounceReduced){
        if (defender==piece){
            piece.defense+=1;
        }
    }

}