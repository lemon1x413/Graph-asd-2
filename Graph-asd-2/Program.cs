using System;
using System.Collections.Generic;

namespace Graph_asd_2;

public static class FileHandler
{
    public static int[,] ReadMatrixFromFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        int n = int.Parse(lines[0]);
        int m = int.Parse(lines[1]);
        int[,] matrix = new int[n, m];
        for (int i = 0; i < n; i++)
        {
            var row = lines[i + 2].Split(' ');
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = int.Parse(row[j]);
            }
        }

        return matrix;
    }

    public static void WritePathToFile(List<int> path, string fileName)
    {
        for (int i = 0; i < path.Count; i++)
        {
            File.AppendAllText(fileName, $"{path[i] + 1}" + (i == path.Count - 1 ? "" : "->"));
        }
    }
    
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter input file name.");
        var file = Console.ReadLine();
        var folderPath = "../../../graphs/" + file + ".txt";

        int[,] incidence = FileHandler.ReadMatrixFromFile(folderPath);
        PrintMatrix(incidence);
        Console.WriteLine("Enter start and end vertices.");
        int start = int.Parse(Console.ReadLine());
        int end = int.Parse(Console.ReadLine());
        var path = TarryAlgorithm(incidence, start - 1, end - 1);
        Console.WriteLine($"Маршрут з {start} до {end}:");
        for (int i = 0; i < path.Count; i++)
        {
            Console.Write($"{path[i] + 1}" + (i == path.Count - 1 ? "" : "->"));
        }

        FileHandler.WritePathToFile(path, "../../../graphs/" + file + "_output.txt");
    }

    static List<int> TarryAlgorithm(int[,] incidence, int start, int end)
    {
        int n = incidence.GetLength(0);
        int m = incidence.GetLength(1);

        var edges = new (int u, int v)[m];
        for (int e = 0; e < m; e++)
        {
            int u = -1, v = -1;
            for (int i = 0; i < n; i++)
            {
                if (incidence[i, e] == 1)
                {
                    if (u < 0) u = i;
                    else v = i;
                }
            }

            edges[e] = (u, v);
        }

        var visitedVertex = new bool[n];
        var visitedEdgeDir = new bool[m, 2];

        var path = new List<int>();
        var stack = new Stack<(int current, int nextEdge)>();

        stack.Push((start, 0));
        visitedVertex[start] = true;
        path.Add(start);

        while (stack.Count > 0)
        {
            var (current, nextEdge) = stack.Pop();

            if (current == end)
                break;

            bool moved = false;
            for (int e = nextEdge; e < m; e++)
            {
                var (u, v) = edges[e];
                int dir = -1, neighbor = -1;

                if (current == u)
                {
                    dir = 0;
                    neighbor = v;
                }
                else if (current == v)
                {
                    dir = 1;
                    neighbor = u;
                }
                else continue;

                if (visitedEdgeDir[e, dir] || visitedVertex[neighbor])
                    continue;

                stack.Push((current, e + 1));

                visitedEdgeDir[e, dir] = true;
                visitedVertex[neighbor] = true;
                path.Add(neighbor);
                stack.Push((neighbor, 0));

                moved = true;
                break;
            }

            if (!moved)
            {
                if (path.Count > 0 && path[path.Count - 1] == current)
                    path.RemoveAt(path.Count - 1);
            }
        }

        return path;
    }
    
    static void PrintMatrix(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write(matrix[i, j] + " ");
            }

            Console.WriteLine();
        }
    }
}

public static class Generator
{
    public static void GenerateMatrixFiles()
    {
        (int n, int m)[] size = [(5, 10), (10, 20), (20, 40)];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                WriteMatrixToFile(MatrixGenerator(size[i].n, size[i].m),
                    "../../../graphs/graph_" + $"{size[i].n}" + "_" + $"{j + 1}" + ".txt");
            }
        }
    }
    public static void WriteMatrixToFile(int[,] matrix, string fileName)
    {
        File.WriteAllText(fileName, $"{matrix.GetLength(0)}\n{matrix.GetLength(1)}\n");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                File.AppendAllText(fileName, $"{matrix[i, j]} ");
            }

            File.AppendAllText(fileName, "\n");
        }
    }
    public static int[,] MatrixGenerator(int n, int m)
    {
        Random rand = new Random();

        // Генеруємо випадково зв'язний граф
        var edges = new List<(int u, int v)>();
        var visited = new HashSet<int> { 0 };
        var remaining = new HashSet<int>(Enumerable.Range(1, n - 1));

        // Спочатку створюємо остовне дерево
        while (remaining.Count > 0)
        {
            int u = visited.ElementAt(rand.Next(visited.Count));
            int v = remaining.ElementAt(rand.Next(remaining.Count));
            edges.Add((u, v));
            visited.Add(v);
            remaining.Remove(v);
        }

        // Додаємо випадкові ребра до досягнення m, уникаючи дублікатів та петель
        while (edges.Count < m)
        {
            int u = rand.Next(n);
            int v = rand.Next(n);
            if (u == v) continue;
            var e = u < v ? (u, v) : (v, u);
            if (edges.Contains(e)) continue;
            edges.Add(e);
        }

        // Ініціалізація матриці інцидентності
        int[,] incidence = new int[n, m];

        // Заповнюємо матрицю
        for (int j = 0; j < m; j++)
        {
            var (u, v) = edges[j];
            incidence[u, j] = 1;
            incidence[v, j] = 1;
        }

        return incidence;
    }
}