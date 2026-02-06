using UnityEngine;
/// <summary>
/// Cette classe gère le formattage avant display d'un timer ou d'un chronomètre. 
/// </summary>
public static class TimeFormatter
{
    public static string Format(float totalSeconds)
    {
        if (totalSeconds < 0) totalSeconds = 0;
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);
        int tenths = (int)((totalSeconds * 10) % 10);
        return $"{minutes:D2}:{seconds:D2}.{tenths}";
    }
}