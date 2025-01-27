using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpectralStrideMovement : MovementProfile
{
    private MovementProfile oldMovementProfile;

    public SpectralStrideMovement(MovementProfile oldMovementProfile){
        this.oldMovementProfile=oldMovementProfile;
    }
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return GetSpectralStrideMoves(piece, piece.xBoard, piece.yBoard);
    }
    public List<BoardPosition> GetSpectralStrideMoves(Chessman piece, int xBoard, int yBoard)
    {
        var controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var spectralMoves = new List<BoardPosition>();
        var directions = oldMovementProfile.GetDirections(piece);
        
        if (directions==null)
            return oldMovementProfile.GetValidMoves(piece);
            
        foreach (var direction in directions)
        {
            int currentX = xBoard + direction.x;
            int currentY = yBoard + direction.y;

            while (sc.PositionOnBoard(currentX, currentY))
            {
                // Check if the position is blocked by an ally
                var occupyingPiece = Game._instance.currentMatch.GetPieceAtPosition(currentX, currentY);
                if (occupyingPiece != null)
                {
                    var occupyingChessman = occupyingPiece.GetComponent<Chessman>();
                    if (occupyingChessman.color == piece.color)
                    {
                        // If it's an allied piece, skip to the next position
                        currentX += direction.x;
                        currentY += direction.y;
                        continue;
                    }
                    else
                    {
                        // If it's an enemy piece, add it as a valid move and stop the line
                        spectralMoves.Add(new BoardPosition(currentX, currentY));
                        break;
                    }
                }

                // Add the current position as a valid move
                spectralMoves.Add(new BoardPosition(currentX, currentY));

                // Move to the next position in the direction
                currentX += direction.x;
                currentY += direction.y;
            }
        }

        return spectralMoves;
    }


    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        if (piece.type!=PieceType.Pawn){
            return GetSpectralStrideMoves(piece, piece.xBoard, piece.yBoard);
        }else{
            return oldMovementProfile.GetValidSupportMoves(piece);
        }
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldMovementProfile.GetDirections(piece);
    } 
}