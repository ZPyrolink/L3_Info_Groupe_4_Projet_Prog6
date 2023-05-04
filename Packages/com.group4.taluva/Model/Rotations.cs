using System;

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
      public static float Degree(this Rotation value) =>
         360f / Enum.GetValues(typeof(Rotation)).Length * (int) value;
   }
}