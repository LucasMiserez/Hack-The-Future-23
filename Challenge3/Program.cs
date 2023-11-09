using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Challenge3
{
    class Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    internal class Program
    {
        static readonly HttpClient client = new();

        static async Task Main()
        {
            // Initial setup
            client.BaseAddress = new Uri("https://exs-htf-2023.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "team bd486984-077b-4c27-b203-b5d1e317da74");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/challenges/pathfinder?isTest=false");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong");
                return;
            }

            dynamic respJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            var mazeRows = respJson.pathfinderData;
            List<List<string>> maze = new List<List<string>>();
            foreach (var row in mazeRows)
            {
                maze.Add(new List<string>(row.ToObject<List<string>>()));
            }

            var startpoint = JsonConvert.DeserializeObject<Point>(respJson.startpoint.ToString());
            var endpoint = JsonConvert.DeserializeObject<Point>(respJson.endpoint.ToString());

            var solution = SolveMaze(maze, startpoint, endpoint);        

            Console.WriteLine(JsonConvert.SerializeObject(new { answer = solution}));

            HttpResponseMessage postResponse = await client.PostAsJsonAsync("/api/challenges/pathfinder", new { answer = solution} );
            postResponse.EnsureSuccessStatusCode();

            string response2 = await postResponse.Content.ReadAsStringAsync();


        }
        static string[] SolveMaze(List<List<string>> maze, Point startpoint, Point endpoint)
        {
            var path = new List<string>();
            if (DFS(maze, startpoint, endpoint, path))
            {
                return path.ToArray();
            }

            return new[] { "No solution found" };

            static bool DFS(List<List<string>> maze, Point current, Point target, List<string> path)
            {
                if (current.x == target.x && current.y == target.y)
                {
                    return true;
                }

                var directions = new[] { (0, -1), (0, 1), (-1, 0), (1, 0) };

                foreach (var (dx, dy) in directions)
                {
                    var newX = current.x + dx;
                    var newY = current.y + dy;

                    if (IsValidMove(maze, newX, newY))
                    {
                        maze[newY][newX] = "V";

                        path.Add(GetMoveDirection(dx, dy));

                        if (DFS(maze, new Point { x = newX, y = newY }, target, path))
                        {
                            return true;
                        }

                        path.RemoveAt(path.Count - 1);
                    }
                }

                return false;
            }

            static bool IsValidMove(List<List<string>> maze, int x, int y)
            {
                // Checks if the maze has rows (non-empty maze)
                var hasRows = maze.Count > 0;

                // Checks if the x-coordinate is within the bounds of the maze's columns
                var withinXBounds = hasRows && x >= 0 && x < maze[0].Count;

                // Checks if the y-coordinate is within the bounds of the maze's rows
                var withinYBounds = hasRows && y >= 0 && y < maze.Count;

                // Checks if the current cell is within the maze boundaries and is either an open path ("0") or the endpoint ("E")
                var validCell = withinXBounds && withinYBounds && (maze[y][x] == "0" || maze[y][x] == "E");

                // Returns true if the cell is valid, otherwise returns false
                return validCell;

            }

            static string GetMoveDirection(int dx, int dy)
            {
                return (dx, dy) switch
                {
                    (0, -1) => "U",
                    (0, 1) => "D",
                    (-1, 0) => "L",
                    (1, 0) => "R",
                    _ => throw new InvalidOperationException("Invalid move direction"),
                };
            }
        }
    }
}