using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monk", menuName = "Abilities/Monk")]
public class Monk : Ability
{
    private Chessman piece;
    
    public Monk() : base("Monk", "Gain +1 to random stat for each turn piece does not move") {}
    private int attackIncrease=0;
    private int defenseIncrease=0;
    private int supportIncrease=0;

    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        this.piece = piece;
        piece.info += " " + abilityName;
        game.OnMove.AddListener(AddBonus);
        piece.releaseCost+=10;
        //game.OnGameEnd.AddListener(RemoveBonus);

    }

    public override void Remove(Chessman piece)
    {
        game.OnMove.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman movedPiece){
        if (movedPiece.color == piece.color && movedPiece!=piece){
            int s = Random.Range (0, 3);
            switch(s){
                case 0: piece.attackBonus++; break;
                case 1: piece.defenseBonus++; break;
                case 2: piece.supportBonus++; break;

            }
        }
    }

}
