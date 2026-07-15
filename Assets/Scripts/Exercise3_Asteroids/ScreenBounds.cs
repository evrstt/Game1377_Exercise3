using UnityEngine;
/// <summary>
/// This Script Stores the boundaries of the game screen based on the size
/// and aspect ratio of the main camera
/// </summary>

public class ScreenBounds
{
    private static float screenHalfHeight = Camera.main.orthographicSize;
    private static float screenHalfWidth = Camera.main.aspect * screenHalfHeight;

    /// <summary>
    /// Stores the highest and lowest visible Y positions
    /// </summary>
    public static float screenTop = screenHalfHeight;
    public static float screenBottom = -screenHalfHeight;

    /// <summary>
    /// Stores the furthest left/right X positions, with positive being right and negative being left
    /// </summary>
    public static float screenRight = screenHalfWidth;
    public static float screenLeft = -screenHalfWidth;
}
