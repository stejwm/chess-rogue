using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assassin", menuName = "Abilities/Assassin")]
public class Assassin : Ability
{
    private Chessman piece;
    
    public Assassin() : base("Assassin", "+5 attack if attacks with no support") {}


    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        this.piece = piece;
        piece.info += " " + abilityName;
        game.OnAttack.AddListener(AddBonus);
        game.OnAttackEnd.AddListener(RemoveBonus);
        piece.releaseCost+=20;

    }

    public override void Remove(Chessman piece)
    {
        game.OnAttack.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, int support){
        if (attacker==piece && support==0)
            piece.attackBonus+=5;
    }
    public void RemoveBonus(Chessman attacker, int support){
        if (attacker==piece && support==0)
            piece.attackBonus-=5;
    }

}
