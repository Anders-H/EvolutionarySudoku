namespace EvolutionarySudoku;

public class Sudoku
{
    private readonly int[,] _field = new int[9, 9];
    private const int MutationSpeed = 3;
    private const int DeadEndLimit = 60000;

    public void Build()
    {
        var r = new Random();

        //Create a list of possible characters on the game field.
        var l = new List<int>();

        for (var i = 1; i < 10; i++)
        {
            for (var j = 1; j < 10; j++)
                l.Add(i);
        }

        //Place them on the field randomly.
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var index = r.Next(0, l.Count);
                var value = l[index];
                l.RemoveAt(index);
                _field[j, i] = value;
            }
        }

        //Prepare for score counting. Low is better.
        int score = GetScore(_field), iterations = 0, generations = 0, iterationsSinceLastClimb = 0;
        var child1 = new int[9, 9];
        var child2 = new int[9, 9];

        //Show progress.

        void Status() =>
            Console.Title = $"Score: {score:00} - Generations: {generations:000} - " +
                            $"Iterations: {iterations:000000}" +
                            $" - Since last climb: {iterationsSinceLastClimb:00000}";

        //Let evolution work.
        do
        {
            iterations++;

            //If adaptation has stopped, the parent must mutate.
            if (iterationsSinceLastClimb >= DeadEndLimit)
            {
                Mutate(_field);
                score = GetScore(_field);
                iterationsSinceLastClimb = 0;
            }
            
            //Breed two new sudokos and modify them slightly.
            
            Array.Copy(_field, child1, _field.Length);
            Array.Copy(_field, child2, _field.Length);
            Mutate(child1);
            Mutate(child2);
            
            //Check new scores.
            var child1Score = GetScore(child1);
            var child2Score = GetScore(child2);

            if (child1Score < score && child1Score <= child2Score)
                Keep(child1, child1Score);
            else if (child2Score < score)
                Keep(child2, child2Score);
            else
                iterationsSinceLastClimb++;
            
            if ((iterations % 1000) == 0)
            {
                Status();
#if DEBUG
                Display();
                Console.CursorTop = 0;
#endif
            }

            continue;

            //Keep the best one, if any.
            void Keep(int[,] a, int s)
            {
                Array.Copy(a, _field, _field.Length);
                score = s;
                generations++;
                iterationsSinceLastClimb = 0;
            }
        } while (score > 0);

        Status();
        return;

        //Define how a mutation works - a random element swap.
        void Mutate(int[,] a)
        {
            var speed = (score > 2 ? MutationSpeed : 1);

            for (var i = 0; i < speed; i++)
            {
                int x1 = r.Next(0, 9),
                    y1 = r.Next(0, 9),
                    x2 = r.Next(0, 9),
                    y2 = r.Next(0, 9);

                (a[x1, y1], a[x2, y2]) = (a[x2, y2], a[x1, y1]);
            }
        }
    }

    private int GetScore(int[,] p)
    {
        // Count each digit once per row, column and 3x3 box.
        Span<int> counts = stackalloc int[27 * 9];
        counts.Clear();

        for (var y = 0; y < 9; y++)
        {
            for (var x = 0; x < 9; x++)
            {
                var digit = p[x, y] - 1;
                counts[y * 9 + digit]++;
                counts[(9 + x) * 9 + digit]++;
                var box = (y / 3) * 3 + x / 3;
                counts[(18 + box) * 9 + digit]++;
            }
        }

        // Each conflicting cell contributes one point per violated constraint.
        var score = 0;

        foreach (var count in counts)
        {
            if (count > 1)
                score += count;
        }

        return score;
    }

    public void Display()
    {
        for (var y = 0; y < 9; y++)
        {
            for (var x = 0; x < 9; x++)
                Console.Write(_field[x, y]);

            Console.WriteLine();
        }
    }
}
