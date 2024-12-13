﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib.Pieces;

public record Rook : Piece
{
    public static Dictionary<Coordinate, Move> GetLegalMoves(ChessBoardState chessBoardState, Piece rook)
    {
        var moves = new Dictionary<Coordinate, Move>();

        for (int y = rook.coordinate.Y + 1; y < 8; y++)
        {
            var nextCoordinate = new Coordinate(rook.coordinate.X, y);
            if (chessBoardState.PiecesState.Pieces.TryGetValue(nextCoordinate, out Piece? piece))
            {
                if (piece.color != rook.color)
                    moves.Add(nextCoordinate, new Move(Move.Type.Capture, nextCoordinate));
                break;
            }
            else
                moves.Add(nextCoordinate, new Move(Move.Type.Movement, nextCoordinate));
        }

        for (int y = rook.coordinate.Y - 1; y >= 0; y--)
        {
            var nextCoordinate = new Coordinate(rook.coordinate.X, y);
            if (chessBoardState.PiecesState.Pieces.TryGetValue(nextCoordinate, out Piece? piece))
            {
                if (piece.color != rook.color)
                    moves.Add(nextCoordinate, new Move(Move.Type.Capture, nextCoordinate));
                break;
            }
            else
                moves.Add(nextCoordinate, new Move(Move.Type.Movement, nextCoordinate));
        }

        for (int x = rook.coordinate.X + 1; x < 8; x++)
        {
            var nextCoordinate = new Coordinate(x, rook.coordinate.Y);
            if (chessBoardState.PiecesState.Pieces.ContainsKey(nextCoordinate))
            {
                if (chessBoardState.PiecesState.Pieces[nextCoordinate].color != rook.color)
                    moves.Add(nextCoordinate, new Move(Move.Type.Capture, nextCoordinate));
                break;
            }
            else
                moves.Add(nextCoordinate, new Move(Move.Type.Movement, nextCoordinate));
        }

        for (int x = rook.coordinate.X - 1; x >= 0; x--)
        {
            var nextCoordinate = new Coordinate(x, rook.coordinate.Y);
            if (chessBoardState.PiecesState.Pieces.ContainsKey(nextCoordinate))
            {
                if (chessBoardState.PiecesState.Pieces[nextCoordinate].color != rook.color)
                    moves.Add(nextCoordinate, new Move(Move.Type.Capture, nextCoordinate));
                break;
            }
            else
                moves.Add(nextCoordinate, new Move(Move.Type.Movement, nextCoordinate));
        }

        return moves;
    }
    public override Dictionary<Coordinate, Move> GetLegalMoves(ChessBoardState chessBoardState)
    {
        return GetLegalMoves(chessBoardState, this);
    }
}
