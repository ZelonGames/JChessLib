using JChessLib.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JChessLib;

public class MoveNotationHelper
{
    public static string GetCoordinatesFromMoveNotation(string moveNotation)
    {
        moveNotation = moveNotation.Replace("+", "").Replace("#", "");
        if (moveNotation is "O-O" or "O-O-O")
            return moveNotation;

        var regex = new Regex(@"([a-h][1-8])");
        Match match = regex.Matches(moveNotation.ToLower()).Last();
        return match.Groups[0].Value;
    }
    public static string GetFromCoordinatesFromLongMoveNotation(string moveNotation)
    {
        moveNotation = moveNotation.Replace("+", "").Replace("#", "");
        if (moveNotation is "O-O" or "O-O-O")
            return moveNotation;

        var regex = new Regex(@"([a-h][1-8])");
        Match match = regex.Match(moveNotation.ToLower());
        return match.Groups[0].Value;
    }

    public static Move.Promotion GetPromotionFromMoveNotation(string moveNotation)
    {
        var regex = new Regex(@"=([nbrq])");
        Match match = regex.Match(moveNotation.ToLower());
        char? promotionChar = match.Success ? match.Groups[1].Value.First() : null;

        if (!promotionChar.HasValue)
            return Move.Promotion.None;

        return promotionChar.Value switch
        {
            'n' => Move.Promotion.Knight,
            'b' => Move.Promotion.Bishop,
            'r' => Move.Promotion.Rook,
            'q' => Move.Promotion.Queen,
            _ => Move.Promotion.None
        };
    }

    public static char? GetSpecificationFromMoveNotation(string moveNotation)
    {
        if (moveNotation.Contains('='))
            moveNotation = moveNotation[..moveNotation.IndexOf('=')];
        moveNotation = Regex.Replace(moveNotation, "[#+]", "");
        bool isCaptureMove = moveNotation.Contains('x');
        if (isCaptureMove)
        {
            string leftSide = moveNotation.Split('x')[0];
            if (leftSide.Length == 2)
                return leftSide[1];
            else if (leftSide.Length == 1 && char.IsUpper(leftSide[0]))
                return null;
        }
        else
        {
            if (char.IsUpper(moveNotation[0]) && moveNotation.Length == 3 ||
                moveNotation.Length == 2)
                return null;
            if (char.IsLower(moveNotation[0]) && moveNotation.Length == 3)
                return moveNotation[0];
        }

        var regex = new Regex(@"[nbrq]([a-h1-8])|([a-h1-8])x");
        Match match = regex.Match(moveNotation.ToLower()[..2]);

        if (match.Success)
        {
            return match.Groups[1].Value.Length == 0 ?
                 match.Groups[2].Value.First() : match.Groups[1].Value.First();
        }

        return null;
    }

    public static char? GetPieceTypeCharFromMoveNotation(string moveNotation)
    {
        // If the format is for example e4e5 we don't know which type of piece we are moving
        if (IsLongAlgebraicNotation(moveNotation))
            return null;

        moveNotation = moveNotation.Replace("+", "").Replace("#", "");
        if (moveNotation is "O-O" or "O-O-O")
            return 'K';
        char firstChar = moveNotation[0];
        return char.IsUpper(firstChar) ? firstChar : 'P';
    }

    public static LegalMove TryGetLegalMoveFromNotation(ChessBoardState chessBoardState, string moveNotation)
    {
        PlayerColor color = chessBoardState.CurrentTurn;
        string toCoordinateString = GetCoordinatesFromMoveNotation(moveNotation);
        Coordinate toCoordinate = Coordinate.ConvertAlphabeticToCoordinate(toCoordinateString, color);
        Type? pieceType = GetPieceTypeFromNoveNotation(moveNotation);
        char? pieceSpecification = GetSpecificationFromMoveNotation(moveNotation);
        int? fromY = GetRowFromSpecification(pieceSpecification);
        int? fromX = GetColumnFromSpecification(pieceSpecification);
        Move.Promotion promotionType = GetPromotionFromMoveNotation(moveNotation);
        var legalMove = new LegalMove();

        if (pieceType == null && IsLongAlgebraicNotation(moveNotation))
        {
            string fromCoordinateString = GetFromCoordinatesFromLongMoveNotation(moveNotation);
            Coordinate fromCoordinate = Coordinate.ConvertAlphabeticToCoordinate(fromCoordinateString, color);
            legalMove = new LegalMove(chessBoardState, fromCoordinate, toCoordinate, promotionType);
            ChessBoardState testState = MoveHelper.GetNextStateFromMove(legalMove);
            if (testState.GetEnemyKing().IsInCheck(testState))
                throw new IllegalMoveException();
            else
                return legalMove;

        }

        var piecesOfColor = chessBoardState.PiecesState.Pieces
             .Where(x => x.Value.color == color && (pieceType == null || x.Value.GetType() == pieceType));

        Piece? movingPiece = null;
        foreach (var piece in piecesOfColor!)
        {
            Dictionary<Coordinate, Move> legalMoves = piece.Value.GetLegalMoves(chessBoardState);
            bool canPieceMoveToSquare = legalMoves.ContainsKey(toCoordinate);
            if (!canPieceMoveToSquare)
                continue;

            if (!pieceSpecification.HasValue ||
                char.IsNumber(pieceSpecification.Value) && piece.Value.coordinate.Y == fromY!.Value ||
                char.IsLetter(pieceSpecification.Value) && piece.Value.coordinate.X == fromX!.Value)
            {
                movingPiece = piece.Value;
                legalMove = new LegalMove(chessBoardState, movingPiece.coordinate, toCoordinate, promotionType);
                ChessBoardState testState = MoveHelper.GetNextStateFromMove(legalMove);
                if (testState.GetEnemyKing().IsInCheck(testState))
                {
                    movingPiece = null;
                    continue;
                }
                break;
            }
        }

        if (movingPiece == null)
            throw new PieceNotFoundException();

        return legalMove;
    }

    private static int? GetColumnFromSpecification(char? specification)
    {
        return specification.HasValue ? Coordinate.rows.IndexOf(specification.Value) : null;
    }

    private static int? GetRowFromSpecification(char? specification)
    {
        return specification.HasValue ? (int)char.GetNumericValue(specification.Value) - 1 : null;
    }

    private static Type? GetPieceTypeFromNoveNotation(string moveNotation)
    {
        char? pieceType = GetPieceTypeCharFromMoveNotation(moveNotation);

        return pieceType switch
        {
            'P' => typeof(Pawn),
            'N' => typeof(Knight),
            'B' => typeof(Bishop),
            'R' => typeof(Rook),
            'Q' => typeof(Queen),
            'K' => typeof(King),
            _ => null,
        };
    }

    public static bool IsLongAlgebraicNotation(string moveNotation)
    {
        return moveNotation.Length == 4 &&
            moveNotation.Where(char.IsDigit).Count() == 2 &&
            moveNotation.Where(x => char.IsLetter(x) && char.IsLower(x)).Count() == 2;
    }
}
