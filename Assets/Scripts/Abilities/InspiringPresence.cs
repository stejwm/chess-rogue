using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspiringPresence", menuName = "Abilities/InspiringPresence")]
public class InspiringPresence : Ability
{
    private Chessman piece;
    
    public InspiringPresence() : base("Inspiring Presence", "Permanently gain +1 to support every time you successfully support a piece") {}

    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        this.piece = piece;
        piece.info += " " + abilityName;
        game.OnSupportAdded.AddListener(CheckForResult);
        piece.releaseCost+=10;
    }

    public override void Remove(Chessman piece)
    {
        game.OnSupportAdded.RemoveListener(CheckForResult); 

    }
    public void CheckForResult(Chessman supporter){
        if(supporter==piece){
            game.OnPieceCaptured.AddListener(IsCapture);
            game.OnPieceBounced.AddListener(IsBounce);
        }
    }

    public void IsCapture(Chessman attacker){
        if(attacker.team==piece.team)
            AddBonus();

        game.OnPieceCaptured.RemoveListener(IsCapture);
    }
    public void IsBounce(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(defender.team==piece.team)
            AddBonus();

        game.OnPieceBounced.RemoveListener(IsBounce);
    }
    public void AddBonus(){
            piece.support+=1;
    }

}