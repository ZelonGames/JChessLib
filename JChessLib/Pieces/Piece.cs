using JChessLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib.Pieces;

public abstract record Piece
{
    public required Coordinate coordinate;
    public required PlayerColor color;
    public abstract Dictionary<Coordinate, Move> GetLegalMoves(ChessBoardState chessBoardState);
    protected void ExcludeMovesLeavingKingInCheck(Dictionary<Coordinate, Move> moves, ChessBoardState chessBoardState)
    {
        foreach (var move in moves)
        {
            var legalMove = new LegalMove(chessBoardState, coordinate, move.Value.coordinate, move.Value.promotion, false);
            var testState = MoveHelper.GetNextStateFromMove(legalMove);
            King king = (King)testState.PiecesState.Pieces
                .Where(x => x.Value.color == color && x.Value is King).First().Value;
            
            if (king.IsInCheck(chessBoardState))
            {

            }
        }
    }

    public PlayerColor GetEnemyColor()
    {
        if (color == PlayerColor.White)
            return PlayerColor.Black;

        return PlayerColor.White;
    }
}
