using static Taluva.Model.Rotation;

namespace Taluva.Model
{
   public enum Rotation
   {
      N = 0,
      NE = 1,
      SE = 2,
      S = 3,
      SW = 4,
      NW = 5
   }

   public static class RotationExt
   {
      // public static float Degree(this Rotation value) =>
      //    360f / Enum.GetValues(typeof(Rotation)).Length * (int) value;

      public static Rotation Of(float degree) => degree switch
      {
         270 => SW,
         330 => NW,
         30 => N,
         90 => NE,
         150 => SE,
         210 => S,
         
         _ => throw new()
      };

      public static float YDegree(this Rotation rot) => rot switch
      {
         SW => 270,
         NW => 330,
         N => 30,
         NE => 90,
         SE => 150,
         S => 210,

         _ => throw new()
      };
   }
}