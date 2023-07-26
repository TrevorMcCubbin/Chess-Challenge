using System;
using System.Linq;
using System.Threading.Tasks.Sources;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    private readonly int[] PIECE_VALUES = { 0, 100, 300, 300, 500, 900, 10000 };
    private const int MAXIMUM_DEPTH = 3;

    int highestValueCapture = 0;

    public Move Think(Board board, Timer timer)
    {
        var playerIsWhite = board.IsWhiteToMove;
        highestValueCapture = 0;
        var moves = board.GetLegalMoves();
        var moveToMake = moves[0];
        foreach (var move in moves)
        {
            // Always play checkmate in one move
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }

            var moveScore = MoveDepthMaximumValue(board, move);

            if (moveScore > highestValueCapture)
            {
                highestValueCapture = moveScore;
                moveToMake = move;
            }
        }
        return moveToMake;
    }

    private bool CapturedPieceIsOfHigherValue(int highestValueCapture, Board board, Move move)
    {
        var capturedPiece = board.GetPiece(move.TargetSquare);

        var capturedPieceValue = PIECE_VALUES[(int)capturedPiece.PieceType];
        return capturedPieceValue > highestValueCapture;
    }

    private int MoveDepthMaximumValue(Board board, Move move, int index = 0, int kills = 0, int losses = 0)
    {
        if (index == MAXIMUM_DEPTH)
        {
            return kills - losses;
        }

        var totalScore = 0;

        var capturedPiece = board.GetPiece(move.TargetSquare);
        kills += PIECE_VALUES[(int)capturedPiece.PieceType];
        losses += (MoveIsSafe(board, move) ? 0 : (int)move.MovePieceType);

        board.MakeMove(move);
        foreach (var newMove in board.GetLegalMoves())
        {
            totalScore += MoveDepthMaximumValue(board, newMove, index + 1, kills, losses);
        }
        board.UndoMove(move);
        return totalScore;

    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    private bool MoveIsDraw(Board board, Move move)
    {
        board.MakeMove(move);
        bool isDraw = board.IsDraw();
        board.UndoMove(move);
        return isDraw;
    }

    private bool MoveIsCheck(Board board, Move move)
    {
        board.MakeMove(move);
        bool isDraw = board.IsInCheck();
        board.UndoMove(move);
        return isDraw;
    }

    private bool MoveIsFork(Board board, Move move)
    {
        board.MakeMove(move);
        var captures = board.GetLegalMoves(true);
        board.UndoMove(move);
        Console.WriteLine(captures.Length);
        return captures.Length > 1;
    }

    private bool MoveIsSafe(Board board, Move move)
    {
        return !board.SquareIsAttackedByOpponent(board.GetPiece(move.TargetSquare).Square);
    }
}
