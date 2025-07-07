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

    public override IEnumerator Use(Board board)
    {
        Player hero = board.Hero;
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition=null;
        var Chessobj = board.GetPieceAtPosition(targetPosition);
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
        newPiece.Initialize(board);
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
        newPieceObj.GetComponent<SpriteRenderer>().sortingOrder=piece.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        newPiece.flames.GetComponent<Renderer>().sortingOrder=piece.flames.GetComponent<Renderer>().sortingOrder;
        newPiece.isValidForAttack=piece.isValidForAttack;
        newPiece.name=piece.name;

        foreach (var ability in piece.abilities){
            newPiece.AddAbility(board, ability.Clone());
        }
        board.Hero.pieces.Remove(piece.gameObject);
        board.Hero.pieces.Add(newPiece.gameObject);
        Destroy(piece.gameObject);
        board.PlacePiece(newPiece, targetPosition);   
        yield return null;   

    }


}
