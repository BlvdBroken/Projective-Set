using System.Collections.Generic;
using UnityEngine;

class mazeGen {
   private string[][] maze = new string[11][]
  {
        new string[] { "█", "█", "█", "█", "█", "█", "█", "█", "█", "█", "█" },
        new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
        new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
        new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
        new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
        new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
        new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
        new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
        new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
        new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
        new string[] { "█", "█", "█", "█", "█", "█", "█", "█", "█", "█", "█" },
  };

   private bool[][] visited = new bool[5][]
   {
        new bool[] { false, false, false, false, false },
        new bool[] { false, false, false, false, false },
        new bool[] { false, false, false, false, false },
        new bool[] { false, false, false, false, false },
        new bool[] { false, false, false, false, false }
   };

   void GenerateMaze () {
      int[] vals = new int[] { 1, 3, 5, 7, 9 };
      int x = UnityEngine.Random.Range(0, 5);
      int y = UnityEngine.Random.Range(0, 5);
      Debug.Log("Starting position: (" + x + ", " + y + ")");
      while (ContainsFalse()) {
         visited[x][y] = true;
         // U, D, L, R
         string[] moves = new string[4];
         List<int> posDirs = new List<int>();
         moves[0] = maze[vals[y] - 1][vals[x]];
         moves[1] = maze[vals[y] + 1][vals[x]];
         moves[2] = maze[vals[y]][vals[x] - 1];
         moves[3] = maze[vals[y]][vals[x] + 1];
         if (moves[0] == "▓") {
            if (!visited[x][y - 1])
               posDirs.Add(0);
         }
         if (moves[1] == "▓") {
            if (!visited[x][y + 1])
               posDirs.Add(1);
         }
         if (moves[2] == "▓") {
            if (!visited[x - 1][y])
               posDirs.Add(2);
         }
         if (moves[3] == "▓") {
            if (!visited[x + 1][y])
               posDirs.Add(3);
         }
         if (posDirs.Count == 0) {
            Debug.Log("No possible moves, getting new start");
            bool found = false;
            for (int i = 0; i < 5; i++) {
               for (int j = 0; j < 5; j++) {
                  if (!visited[i][j] && HasVisitors(i, j)) {
                     x = i;
                     y = j;
                     Debug.Log("New start: (" + x + ", " + y + ")");
                     List<int> newAvs = GetAvenues(x, y);
                     int num = UnityEngine.Random.Range(1, newAvs.Count);
                     for (int k = 0; k < num; k++) {
                        int choice = UnityEngine.Random.Range(0, newAvs.Count);
                        if (newAvs[choice] == 0) {
                           maze[vals[y] - 1][vals[x]] = "░";
                        }
                        else if (newAvs[choice] == 1) {
                           maze[vals[y] + 1][vals[x]] = "░";
                        }
                        else if (newAvs[choice] == 2) {
                           maze[vals[y]][vals[x] - 1] = "░";
                        }
                        else if (newAvs[choice] == 3) {
                           maze[vals[y]][vals[x] + 1] = "░";
                        }
                        string[] logNames = new string[] { "UP", "DOWN", "LEFT", "RIGHT" };
                        Debug.Log("Removing a wall " + logNames[newAvs[choice]] + " from start");
                        newAvs.RemoveAt(choice);
                     }
                     visited[x][y] = true;
                     found = true;
                     break;
                  }
               }
               if (found)
                  break;
            }
         }
         else {
            int choice = UnityEngine.Random.Range(0, posDirs.Count);
            if (posDirs[choice] == 0) {
               maze[vals[y] - 1][vals[x]] = "░";
               y--;
            }
            else if (posDirs[choice] == 1) {
               maze[vals[y] + 1][vals[x]] = "░";
               y++;
            }
            else if (posDirs[choice] == 2) {
               maze[vals[y]][vals[x] - 1] = "░";
               x--;
            }
            else if (posDirs[choice] == 3) {
               maze[vals[y]][vals[x] + 1] = "░";
               x++;
            }
            string[] logNames = new string[] { "UP", "DOWN", "LEFT", "RIGHT" };
            Debug.Log("Found moves, going " + logNames[posDirs[choice]] + " to (" + x + ", " + y + ")");
         }
      }
      string mazelog = "";
      for (int i = 0; i < 11; i++) {
         for (int j = 0; j < 11; j++) {
            mazelog += maze[i][j];
         }
         if (i != 10)
            mazelog += "\n";
      }
      Debug.Log(mazelog);
   }

   List<int> GetAvenues (int x, int y) {
      List<int> temp = new List<int>();
      if (y - 1 >= 0)
         if (visited[x][y - 1])
            temp.Add(0);
      if (y + 1 <= 4)
         if (visited[x][y + 1])
            temp.Add(1);
      if (x - 1 >= 0)
         if (visited[x - 1][y])
            temp.Add(2);
      if (x + 1 <= 4)
         if (visited[x + 1][y])
            temp.Add(3);
      return temp;
   }

   bool HasVisitors (int x, int y) {
      if (y - 1 >= 0)
         if (visited[x][y - 1])
            return true;
      if (y + 1 <= 4)
         if (visited[x][y + 1])
            return true;
      if (x - 1 >= 0)
         if (visited[x - 1][y])
            return true;
      if (x + 1 <= 4)
         if (visited[x + 1][y])
            return true;
      return false;
   }

   bool ContainsFalse () {
      for (int i = 0; i < 5; i++) {
         for (int j = 0; j < 5; j++) {
            if (visited[i][j] == false)
               return true;
         }
      }
      return false;
   }
}