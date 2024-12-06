using System;
using System.Linq;

public static class ExMath {

   public static int Mod (int Input, int Operator) {
      return ((Input % Operator) + Operator) % Operator;
   }

   public static int DRoot (int Input) {
      return ((Input - 1) % 9) + 1;
   }

   public static bool IsPrime (int Input) {
      if (Input <= 1) return false;
      if (Input == 2) return true;

      var Limit = (int) Math.Floor(Math.Sqrt(Input));

      for (int i = 2; i <= Limit; i++) {
         if (Input % i == 0) {
            return false;
         }
      }
      return true;
   }

   public static bool IsSquare (int Input) {
      return Math.Sqrt(Input) % 1 <= .001f;
   }

   public static int BaseTo10 (int Input, int Base) { //From base N to base 10. With N being less than 10
      int Total = 0;
      int NumberLength = Input.ToString().Length;
      for (int i = 0; i < NumberLength; i++) {
         Total += (int) Math.Pow(Base, NumberLength - (i + 1)) * int.Parse(Input.ToString()[i].ToString());
      }
      return Total;
   }

   public static string ConvertToBase (int Input, int Base) { //Is a string for bases above 10.
      string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      string Current = "";
      while (Input != 0) {
         Current = Digits[Input % Base] + Current;
         Input /= Base;
      }
      return Current;
   }

   public static int HexToDecimal (string Input) {
      Input = Input.ToUpper();
      string Hex = "0123456789ABCDEF";
      int L = Input.Length;
      int Answer = 0;

      for (int i = Input.Length - 1; i >= 0; i--) {
         Answer += Hex.IndexOf(Input[i].ToString()) * (int) Math.Pow(16, L - i);
      }
      return Answer;
   }

   public static int DigitSum (int Input) {
      if (Input == 0) {
         return 0;
      }
      return DigitSum(Input / 10) + Input % 10;
   }

   public static int[] ToIntArray (int Input) {

      int[] result = Input.ToString().Select(o => Convert.ToInt32(o) - 48).ToArray();

      return result;
   }

   public static int GCD (int x, int y) {
      var temp = 0;
      if (x < y) {
         temp = x;
         x = y;
         y = temp;
      }
      while (y != 0) {
         temp = x % y;
         x = y;
         y = temp;
      }
      return x;
   }

   public static bool IsCoprime (int x, int y) {
      return GCD(x, y) == 1;
   }
}
