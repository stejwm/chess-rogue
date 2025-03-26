using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Promotion", menuName = "KingsOrders/Promotion")]
public class Promotion : KingsOrder
{
    [SerializeField]GameObject Knight;
    [SerializeField]GameObject Bishop;
    [SerializeField]GameObject Queen;
    [SerializeField]GameObject Rook;

    public Promotion() : base("Promotion", "Promotes a pawn to a new rank") {}

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition=null;
        var Chessobj = Game._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman piece = Chessobj.GetComponent<Chessman>();
        if(piece.type!=PieceType.Pawn){
            Debug.Log("Not a pawn");
            yield break;
        }
        PopUpManager._instance.ShowPieceTypes();
        Debug.Log("waiting for piece type select");
        yield return new WaitUntil(() => PopUpManager._instance.selectedPieceType !=PieceType.None);
        PieceType promotedRank = PopUpManager._instance.selectedPieceType;
        PopUpManager._instance.HidePieceTypes();
        GameObject newPieceObj;
        switch(promotedRank) 
        {
            case PieceType.Knight:
                newPieceObj = Instantiate(Knight);
                break;
            case PieceType.Bishop:
                newPieceObj = Instantiate(Bishop);
                break;
            case PieceType.Queen:
                newPieceObj = Instantiate(Queen);
                break;
            case PieceType.Rook:
                newPieceObj = Instantiate(Rook);
                break;
            default:
                newPieceObj=null;
                break;
        }
        newPieceObj.name=piece.gameObject.name;
        Chessman newPiece = newPieceObj.GetComponent<Chessman>();
        newPiece.xBoard=piece.xBoard;
        newPiece.yBoard=piece.yBoard;
        newPiece.owner =piece.owner;
        newPiece.startingPosition=piece.startingPosition;
        newPiece.moveProfile=piece.moveProfile;
        newPiece.attack=piece.attack;
        newPiece.defense=piece.defense;
        newPiece.support=piece.support;
        newPiece.attackBonus=piece.attackBonus;
        newPiece.defenseBonus=piece.defenseBonus;
        newPiece.supportBonus=piece.supportBonus;
        newPiece.releaseCost=piece.releaseCost;
        newPiece.blood=piece.blood;
        newPiece.info=piece.info;
        newPiece.color=piece.color;
        newPiece.team=piece.team;
        newPiece.type=piece.type;
        newPiece.uniqueId=piece.uniqueId;
        //newPiece.abilities= new List<Ability>(piece.abilities);
        newPiece.isValidForAttack=piece.isValidForAttack;
        newPiece.name=piece.name;

        foreach (var ability in piece.abilities){
            newPiece.AddAbility(ability.Clone());
        }
        Game._instance.hero.pieces.Remove(piece.gameObject);
        Game._instance.hero.pieces.Add(newPiece.gameObject);
        Destroy(piece.gameObject);
        Game._instance.togglePieceColliders(new List<GameObject> { newPiece.gameObject },false);
        Game._instance.currentMatch.MovePiece(newPiece, targetPosition.x, targetPosition.y);        

    }


}
