using ChessChallenge.API;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    private readonly int[] PIECE_VALUES = { 0, 100, 300, 300, 500, 900, 10000 };
    int highestValueCapture = 0;

    public Move Think(Board board, Timer timer)
    {
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

            if (CapturedPieceIsOfHigherValue(highestValueCapture, board, move))
            {
                    highestValueCapture = PIECE_VALUES[(int)board.GetPiece(move.TargetSquare).PieceType];
                    moveToMake = move;
            }
        }
        return moveToMake;
    }

    private bool CapturedPieceIsOfHigherValue(int highestValueCapture, Board board, Move move)
    {
        var capturedPiece = board.GetPiece(move.TargetSquare);

        // for this iteration only take unguarded pieces
        if (board.SquareIsAttackedByOpponent(capturedPiece.Square))
        {
            return false;
        }

        var capturedPieceValue = PIECE_VALUES[(int)capturedPiece.PieceType];
        return capturedPieceValue > highestValueCapture;
    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}