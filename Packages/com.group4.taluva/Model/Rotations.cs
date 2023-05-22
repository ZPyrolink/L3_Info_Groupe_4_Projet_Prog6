using static Taluva.Model.Rotation;

namespace Taluva.Model
{
    /// <summary>
    /// Represents the different rotational directions.
    /// </summary>
    public enum Rotation
    {
        N = 0,
        NE = 1,
        SE = 2,
        S = 3,
        SW = 4,
        NW = 5
    }
    /// <summary>
    /// Provides extension methods for the <see cref="Rotation"/> enum.
    /// </summary>
   public static class RotationExt
   {
      
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
      /// <summary>
      /// Converts the rotation value to its corresponding degree.
      /// </summary>
      /// <param name="value">The rotation value.</param>
      /// <returns>The degree value.</returns>
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