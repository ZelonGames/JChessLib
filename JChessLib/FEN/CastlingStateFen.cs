using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib.FEN;

public record CastlingStateFen
{
    public readonly CastlingState castlingState;

    public CastlingStateFen(string fen)
    {
        string fenCastlingState = fen.Split(' ')[2];

        var allowedKingCastlingMoves = new HashSet<CastlingMove>();
        foreach (var c in fenCastlingState)
        {
            if (char.IsLetter(c))
                allowedKingCastlingMoves.Add(FenHelper.GetStateFromFenChar(c));
        }

        castlingState = new CastlingState() { AllowedKingCastlingMoves = allowedKingCastlingMoves };
    }
}
