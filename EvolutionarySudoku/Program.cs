var s = new Sudoku();
s.Build();
s.Display();
Console.Write("Done. Press enter to quit.");
Console.ReadLine();

class Sudoku
{
    private int[,] _field = new int[9, 9];
    private const int MutationSpeed = 3;
    private const int DeadEndLimit = 60000;

    public void Build()
    {
        var r = new Random();

        //Create a list of possible characters on the game field.
        var l = new List<int>();
        
        for (var i = 1; i < 10; i++)
            for (var j = 1; j < 10; j++)
                l.Add(i);
        
        //Place them on the field randomly.
        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
            {
                var index = r.Next(0, l.Count);
                var value = l[index]; l.RemoveAt(index);
                _field[j, i] = value;
            }
        
        //Prepare for score counting. Low is better.
        int score = int.MaxValue, iterations = 0, generations = 0, iterationsSinceLastClimb = 0;

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
            }
            
            //Breed two new sudokos and modify them slightly.
            
            var child1 = (int[,])_field.Clone();
            var child2 = (int[,])_field.Clone();
            Mutate(child1); Mutate(child2);
            
            //Check new scores.
            var child1Score = GetScore(child1);
            var child2Score = GetScore(child2);

            //Keep the best one, if any.
            void Keep(int[,] a, int s)
            {
                _field = (int[,])a.Clone();
                score = s;
                generations++;
                iterationsSinceLastClimb = 0;
            }

            if (child1Score < score)
                Keep(child1, child1Score);
            else if (child2Score < score)
                Keep(child2, child2Score);
            else
                iterationsSinceLastClimb++;
            
            Thread.Yield();

            if ((iterations % 1000) == 0)
                Status();
#if DEBUG
            Display();
            Console.CursorTop = 0;
#endif
        } while (score > 0);

        Status();
    }

    private int GetScore(int[,] p)
    {
        bool CheckRow(int[,] arr, int x, int y)
        {
            var val = arr[x, y];
            var count = 0;
            for (var i = 0; i < 9; i++)
                if (arr[i, y] == val)
                    count++;
            return (count == 1);
        }

        bool CheckCol(int[,] arr, int x, int y)
        {
            var val = arr[x, y];
            var count = 0;

            for (var i = 0; i < 9; i++)
                if (arr[x, i] == val)
                    count++;

            return (count == 1);
        }

        bool CheckSection(int[,] arr, int x, int y)
        {
            var sectionX = x;

            while (sectionX % 3 != 0) sectionX--;

            var sectionY = y;
            while (sectionY % 3 != 0) sectionY--;

            var val = arr[x, y];
            var count = 0;

            for (var row = sectionX; row < sectionX + 3; row++)
            for (var col = sectionY; col < sectionY + 3; col++)
                if (arr[col, row] == val)
                    count++;

            return (count == 1);
        }

        var score = 0;

        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
            {
                if (!CheckRow(p, j, i))
                    score++;

                if (!CheckCol(p, j, i))
                    score++;

                if (!CheckSection(p, j, i))
                    score++;
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