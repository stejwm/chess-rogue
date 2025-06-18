using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpectralStrideMovement : MovementProfile
{
    private MovementProfile oldMovementProfile;

    public SpectralStrideMovement(Board board, MovementProfile old) : base(board) {oldMovementProfile = old;}
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture)
    {
        if (allowFriendlyCapture)
            return GetSpectralStrideMoves(board, piece, piece.xBoard, piece.yBoard);
        else
            return Movement.RemoveFriendlyPieces(board, GetSpectralStrideMoves(board, piece, piece.xBoard, piece.yBoard), piece);
    }
    public List<Tile> GetSpectralStrideMoves(Board board, Chessman piece, int xBoard, int yBoard)
    {
        var spectralMoves = new List<Tile>();
        var directions = oldMovementProfile.GetDirections(piece);
        
        if (directions==null)
            return oldMovementProfile.GetValidMoves(piece, true);
            
        foreach (var direction in directions)
        {
            int currentX = xBoard + direction.x;
            int currentY = yBoard + direction.y;

            while (BoardPosition.IsPositionOnBoard(currentX, currentY))
            {
                // Check if the position is blocked by an ally
                var occupyingPiece = board.GetPieceAtPosition(currentX, currentY);
                if (occupyingPiece != null)
                {
                    var occupyingChessman = occupyingPiece.GetComponent<Chessman>();
                    if (occupyingChessman.color == piece.color)
                    {
                        // If it's an allied piece, skip to the next position
                        spectralMoves.Add(board.GetTileAt(currentX, currentY));
                        currentX += direction.x;
                        currentY += direction.y;
                        continue;
                    }
                    else
                    {
                        // If it's an enemy piece, add it as a valid move and stop the line
                        spectralMoves.Add(board.GetTileAt(currentX, currentY));
                        break;
                    }
                }

                // Add the current position as a valid move
                spectralMoves.Add(board.GetTileAt(currentX, currentY));

                // Move to the next position in the direction
                currentX += direction.x;
                currentY += direction.y;
            }
        }

        return spectralMoves;
    }


    public override List<Tile> GetValidSupportMoves(Chessman piece){
        if (piece.type!=PieceType.Pawn){
            return GetSpectralStrideMoves(board, piece, piece.xBoard, piece.yBoard);
        }else{
            return oldMovementProfile.GetValidSupportMoves(piece);
        }
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldMovementProfile.GetDirections(piece);
    } 
}