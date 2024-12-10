using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdamantAssault", menuName = "Abilities/AdamantAssault")]
public class AdamantAssault : Ability
{
    private Chessman piece;
    MovementProfile startingProfile;
    bool alreadyBounced = false;
    
    public AdamantAssault() : base("Adamant Assault", "Automatically trigger second attack after being bounced") {}


    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        //startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;
        //game.OnPieceCaptured += Thirst;
        //Debug.Log(game==null);

        game.OnPieceBounced.AddListener(Assault);
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        game.OnPieceBounced.RemoveListener(Assault);  // Unsubscribe from the event

    }

    public void Assault(Chessman attacker, Chessman defender, bool isBounceReduced)
    {   
        game.PlayerTurn();
        if (attacker == piece && !alreadyBounced)
        {
            game.ExecuteTurn(attacker, defender.xBoard, defender.yBoard);
            alreadyBounced=true;
        }else{
            alreadyBounced=false;
        }
    }
}
