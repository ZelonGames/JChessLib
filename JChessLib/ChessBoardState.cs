using JChessLib.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib;

public enum PlayerColor
{
    White,
    Black
}
public record struct ChessBoardState(
    PiecesState PiecesState,
    PlayerColor CurrentTurn,
    CastlingState CastlingState,
    Coordinate? EnpassantTarget,
    int FiftyMoveRuleCounter,
    int FullMoves)
{
    public King GetEnemyKing()
    {
        PlayerColor currentTurn = CurrentTurn;
        return (King)PiecesState.Pieces.Where(x => x.Value.color != currentTurn && x.Value is King).First().Value;
    }
}
