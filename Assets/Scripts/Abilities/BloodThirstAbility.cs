using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBloodThirstyAbility", menuName = "Abilities/BloodThirstyAbility")]
public class BloodThirstAbility : Ability
{
    private Chessman piece;
    private GameObject controller;
    
    public BloodThirstAbility() : base("Assassin", "Gains +5 attack when unsupported.") {}


    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();

        this.piece = piece;
        piece.info += " " + this.name;
        //game.OnPieceCaptured += Thirst;
        Debug.Log(game==null);

        game.OnPieceCaptured.AddListener(Thirst);
        Debug.Log("Blood Thirst ability applied, subscribed to OnAttackTriggered");
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        game.OnPieceCaptured.RemoveListener(Thirst);  // Unsubscribe from the event

    }

    public void Thirst(Chessman attacker)
    {
        Debug.Log("Thirsting");
        if (attacker == piece)
        {
            EnableSecondAttack();
        }
    }

    private void EnableSecondAttack()
    {
        Debug.Log("you can attack again");
    }
}
