using Minimax;


var parametersList = new List<Parameters>
{
    new(10, 10, 5),
    new(10, 10, 10),
    
    new(900, 900, 100),
    new(900, 900, 200),
    new(900, 900, 900),
    
    new(9000, 9000, 100),
    new(9000, 9000, 300),
    new(9000, 9000, 1000),
    new(9000, 9000, 3000),
    
    new(900000, 1000, 500),
};


var random = new Random();
foreach (var parameters in parametersList)
{
    var matrix = new double[parameters.RowsAmount,parameters.ColumnsAmount];
    for (int i = 0; i < parameters.RowsAmount; i++)
    {
        for (int j = 0; j < parameters.ColumnsAmount; j++)
        {
            matrix[i, j] = random.Next(-999, 999);
        }
    }

    var result1 = MinimaxAlgorithm.FindMinimax(matrix);
    var result2 = MinimaxAlgorithm.FindMinimaxParallel(matrix, parameters.ThreadsAmount);
    
    Console.WriteLine();
    Console.WriteLine("Для матрицi " + parameters.RowsAmount + "X" + parameters.ColumnsAmount);
    Console.WriteLine("Мiнiмакс однопотокового: " + result1.Item1);
    Console.WriteLine("Мiнiмакс багатопотокового: " + result2.Item1);
    Console.WriteLine("Час виконання однопотокового алгоритму: " + result1.Item2 + " тiкiв");
    Console.WriteLine("Час виконання багатопотокового алгоритму: " + result2.Item2 + " тiкiв");
    Console.WriteLine("Потокiв задiяно: " + result2.Item3);
    Console.WriteLine("Величина прискорення: " + result1.Item2 / (double)result2.Item2);
}


class Parameters
{
    public Parameters(int rowsAmount, int columnsAmount, int threadsAmount)
    {
        RowsAmount = rowsAmount;
        ColumnsAmount = columnsAmount;
        ThreadsAmount = threadsAmount;
    }

    public int RowsAmount { get; }
    public int ColumnsAmount { get; }
    public int ThreadsAmount { get; }
}