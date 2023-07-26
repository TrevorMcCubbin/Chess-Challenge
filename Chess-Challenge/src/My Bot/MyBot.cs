using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        var moveToMake = moves[0];
        foreach (var move in moves)
        {
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }
            // Don't move to a square that is under attack
            if (board.SquareIsAttackedByOpponent(move.TargetSquare))
            {
                continue;
            }
            moveToMake = move;
        }
        return moveToMake;
    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}