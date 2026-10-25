using System.Reflection;
using EvolutionarySudoku;

var sudoku = new Sudoku();
var method = typeof(Sudoku).GetMethod("GetScore", BindingFlags.Instance | BindingFlags.NonPublic)!;

var board = new int[9, 9];

for (var y = 0; y < 9; y++)
{
    for (var x = 0; x < 9; x++)
        board[x, y] = (x + y * 3 + y / 3) % 9 + 1;
}

if (Score(board) != 0)
    throw new Exception("Valid Sudoku rejected.");

(board[3, 0], board[4, 1]) = (board[4, 1], board[3, 0]);

if (Score(board) == 0 || Score(board) != Reference(board))
    throw new Exception("Off-diagonal conflict scored incorrectly.");

var random = new Random(42);

for (var sample = 0; sample < 10000; sample++)
{
    for (var y = 0; y < 9; y++)
    {
        for (var x = 0; x < 9; x++)
            board[x, y] = random.Next(1, 10);
    }

    if (Score(board) != Reference(board))
        throw new Exception($"Score mismatch in sample {sample}.");
}

Console.WriteLine("Passed: valid grid, off-diagonal conflict, and 10,000 random grids.");
return;

static int Reference(int[,] board)
{
    var score = 0;
    for (var y = 0; y < 9; y++)
    {
        for (var x = 0; x < 9; x++)
        {
            int row = 0, col = 0, box = 0;

            for (var i = 0; i < 9; i++)
            {
                if (board[i, y] == board[x, y])
                    row++;

                if (board[x, i] == board[x, y])
                    col++;

                if (board[x / 3 * 3 + i % 3, y / 3 * 3 + i / 3] == board[x, y])
                    box++;
            }

            score += (row > 1 ? 1 : 0) + (col > 1 ? 1 : 0) + (box > 1 ? 1 : 0);
        }
    }

    return score;
}

int Score(int[,] boardArray) => (int)method.Invoke(sudoku, [boardArray])!;
