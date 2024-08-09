using UnityEngine;


public static class MathExtension
{
  public static bool Approximately(float a, float b, float epsilon = 1E-06f)
  {
    return Mathf.Abs(b - a) < epsilon;
  }

  public static bool Approximately(Vector3 a, Vector3 b, float epsilon = 1E-06f)
  {
    return Mathf.Abs(b.x - a.x) < epsilon &&
           Mathf.Abs(b.y - a.y) < epsilon &&
           Mathf.Abs(b.z - a.z) < epsilon;
  }

  public static Vector3 Round(this Vector3 vector, uint decimalPlaces)
  {
    float multiplier = Mathf.Pow(10, decimalPlaces);
    return new Vector3(
        Mathf.Round(vector.x * multiplier) / multiplier,
        Mathf.Round(vector.y * multiplier) / multiplier,
        Mathf.Round(vector.z * multiplier) / multiplier
    );
  }
}
