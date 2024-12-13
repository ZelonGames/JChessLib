using JChessLib.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib;

public record TerritoryState
{
    private readonly ChessBoardState chessBoardState;
    public readonly Dictionary<Coordinate, int> controlledSquares;

    public TerritoryState(ChessBoardState chessBoardState, PlayerColor color)
    {
        this.chessBoardState = chessBoardState;
        controlledSquares = GetControlledSquaresFromColor(color);
    }

    private Dictionary<Coordinate, int> GetControlledSquaresFromColor(PlayerColor color)
    {
        var controlledSquares = new Dictionary<Coordinate, int>();
        var piecesOfColor = chessBoardState.PiecesState.Pieces.Values.Where(x => x.color == color && x is not King);

        foreach (var piece in piecesOfColor)
        {
            foreach (var controlledSquare in piece.GetLegalMoves(chessBoardState).Values)
            {
                if (controlledSquares.ContainsKey(controlledSquare.coordinate))
                    controlledSquares[controlledSquare.coordinate]++;
                else
                {
                    if (piece is not Pawn || 
                        (piece is Pawn && controlledSquare.type is Move.Type.Capture))
                    controlledSquares.Add(controlledSquare.coordinate, 1);
                }
            }
        }

        return controlledSquares;
    }
}
