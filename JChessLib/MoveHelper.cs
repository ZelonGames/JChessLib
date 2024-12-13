﻿using JChessLib.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib;

public class MoveHelper
{
    public static ChessBoardState GetNextStateFromMove(LegalMove legalMove)
    {
        return new ChessBoardState
        (
            new PiecesState() { Pieces = GetUpdatedPiecesFromMove(legalMove) },
            GetUpdatedPlayerColorFromMove(legalMove),
            GetUpdatedCastlingStateFromMove(legalMove),
            GetUpdatedEnPassantSquareFromMove(legalMove),
            GetUpdatedFiftyMoveRuleFromMove(legalMove),
            GetUpdatedFullMoveFromMove(legalMove)
        );
    }

    private static Dictionary<Coordinate, Piece> GetUpdatedPiecesFromMove(LegalMove legalMove)
    {
        Piece piece = legalMove.GetPieceToMove();
        Dictionary<Coordinate, Piece> pieces = legalMove.ChessBoardStateBeforeMoveMade.PiecesState.Pieces.ToDictionary();
        Piece movedPiece = piece with { coordinate = legalMove.ToCoordinate };

        #region Move Piece
        pieces.Remove(piece.coordinate);

        switch (legalMove.Type)
        {
            case Move.Type.Movement:
            case Move.Type.DoublePawnMove:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                break;
            case Move.Type.Capture:
                pieces.Remove(legalMove.ToCoordinate);
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                break;
            case Move.Type.EnPassant:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                int direction = movedPiece.color == PlayerColor.White ? -1 : 1;
                var enPassantedSquare = new Coordinate(legalMove.ToCoordinate.X, legalMove.ToCoordinate.Y + direction);
                pieces.Remove(enPassantedSquare);
                break;
            case Move.Type.WhiteKingSideCastle:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                pieces.Remove(new Coordinate(7, 0), out Piece? whiteKingSideRook);
                whiteKingSideRook = whiteKingSideRook! with { coordinate = new Coordinate(5, 0) };
                pieces.Add(whiteKingSideRook.coordinate, whiteKingSideRook!);
                break;
            case Move.Type.WhiteQueenSideCastle:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                pieces.Remove(new Coordinate(0, 0), out Piece? whiteQueenSideRook);
                whiteQueenSideRook = whiteQueenSideRook! with { coordinate = new Coordinate(3, 0) };
                pieces.Add(whiteQueenSideRook.coordinate, whiteQueenSideRook!);
                break;
            case Move.Type.BlackKingSideCastle:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                pieces.Remove(new Coordinate(7, 7), out Piece? blackKingSideRook);
                blackKingSideRook = blackKingSideRook! with { coordinate = new Coordinate(5, 7) };
                pieces.Add(blackKingSideRook.coordinate, blackKingSideRook!);
                break;
            case Move.Type.BlackQueenSideCastle:
                pieces.Add(legalMove.ToCoordinate, movedPiece);
                pieces.Remove(new Coordinate(0, 7), out Piece? blackQueenSideRook);
                blackQueenSideRook = blackQueenSideRook! with { coordinate = new Coordinate(3, 7) };
                pieces.Add(blackQueenSideRook.coordinate, blackQueenSideRook!);
                break;
            default:
                break;
        }
        #endregion

        #region Promote Piece
        if (legalMove.PromotionType is not Move.Promotion.None)
            pieces.Remove(movedPiece.coordinate);

        switch (legalMove.PromotionType)
        {
            case Move.Promotion.Rook:
                pieces.Add(legalMove.ToCoordinate, new Rook()
                {
                    color = movedPiece.color,
                    coordinate = movedPiece.coordinate
                });
                break;
            case Move.Promotion.Knight:
                pieces.Add(legalMove.ToCoordinate, new Knight()
                {
                    color = movedPiece.color,
                    coordinate = movedPiece.coordinate
                });
                break;
            case Move.Promotion.Bishop:
                pieces.Add(legalMove.ToCoordinate, new Bishop()
                {
                    color = movedPiece.color,
                    coordinate = movedPiece.coordinate
                });
                break;
            case Move.Promotion.Queen:
                pieces.Add(legalMove.ToCoordinate, new Queen()
                {
                    color = movedPiece.color,
                    coordinate = movedPiece.coordinate
                });
                break;
            case Move.Promotion.None:
                break;
            default:
                break;
        }
        #endregion

        return pieces;
    }

    private static PlayerColor GetUpdatedPlayerColorFromMove(LegalMove legalMove)
    {
        Piece movingPiece = legalMove.GetPieceToMove();
        return movingPiece.GetEnemyColor();
    }

    private static CastlingState GetUpdatedCastlingStateFromMove(LegalMove legalMove)
    {
        Piece movingPiece = legalMove.GetPieceToMove();
        var allowedKingCastlingMoves = new HashSet<CastlingMove>(legalMove.ChessBoardStateBeforeMoveMade.CastlingState.AllowedKingCastlingMoves);
        if (movingPiece is Rook)
        {
            if (movingPiece.color == PlayerColor.White &&
                movingPiece.coordinate.Y == 0)
            {
                if (movingPiece.coordinate.X == 7)
                    allowedKingCastlingMoves.Remove(CastlingMove.WhiteKingSide);
                else if (movingPiece.coordinate.X == 0)
                    allowedKingCastlingMoves.Remove(CastlingMove.WhiteQueenSide);
            }
            else if (movingPiece.color == PlayerColor.Black &&
                movingPiece.coordinate.Y == 7)
            {
                if (movingPiece.coordinate.X == 7)
                    allowedKingCastlingMoves.Remove(CastlingMove.BlackKingSide);
                else if (movingPiece.coordinate.X == 0)
                    allowedKingCastlingMoves.Remove(CastlingMove.BlackQueenSide);
            }
        }
        else if (movingPiece is King)
        {
            if (movingPiece.color == PlayerColor.White)
            {
                allowedKingCastlingMoves.Remove(CastlingMove.WhiteQueenSide);
                allowedKingCastlingMoves.Remove(CastlingMove.WhiteKingSide);
            }
            else
            {
                allowedKingCastlingMoves.Remove(CastlingMove.BlackQueenSide);
                allowedKingCastlingMoves.Remove(CastlingMove.BlackKingSide);
            }
        }

        return new CastlingState() { AllowedKingCastlingMoves = allowedKingCastlingMoves };
    }

    private static int GetUpdatedFullMoveFromMove(LegalMove legalMove)
    {
        Piece movingPiece = legalMove.GetPieceToMove();
        if (movingPiece.color == PlayerColor.Black)
            return legalMove.ChessBoardStateBeforeMoveMade.FullMoves + 1;
        return legalMove.ChessBoardStateBeforeMoveMade.FullMoves;
    }

    private static Coordinate? GetUpdatedEnPassantSquareFromMove(LegalMove legalMove)
    {
        if (legalMove.ChessBoardStateBeforeMoveMade.EnpassantTarget != null)
            return null;

        Piece movingPiece = legalMove.GetPieceToMove();

        var enemyPawnNeighboor = legalMove.ChessBoardStateBeforeMoveMade.PiecesState.Pieces
            .Where(x =>
            x.Value is Pawn &&
            x.Value.color == movingPiece.GetEnemyColor() &&
            x.Value.coordinate.Y == legalMove.ToCoordinate.Y &&
            (x.Value.coordinate.X == legalMove.ToCoordinate.X + 1 ||
            x.Value.coordinate.X == legalMove.ToCoordinate.X - 1)).FirstOrDefault();

        if (movingPiece is Pawn && legalMove.Type == Move.Type.DoublePawnMove)
        {
            if (enemyPawnNeighboor.Value != null)
            {
                int direction = movingPiece.color == PlayerColor.White ? 1 : -1;
                return new Coordinate(movingPiece.coordinate.X, movingPiece.coordinate.Y + direction);
            }
        }

        return null;
    }

    private static int GetUpdatedFiftyMoveRuleFromMove(LegalMove legalMove)
    {
        Piece movingPiece = legalMove.GetPieceToMove();

        if (legalMove.Type == Move.Type.Capture || movingPiece is Pawn)
            return 0;

        return legalMove.ChessBoardStateBeforeMoveMade.FiftyMoveRuleCounter + 1;
    }
}
