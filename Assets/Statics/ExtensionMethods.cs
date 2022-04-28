using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using System.Text;

/// <summary>
/// The file of each extension method, which are methods that can be added to existing types to extend their functionality.
/// Unfortunately, the XML comments in Visual Studio don't seem to work fo rthese
/// </summary>
public static class ExtensionMethods
{
    // Returns the Vector2 rotated by some angle
    public static Vector2 Rotated(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    // Returns the transform position of a MonoBehaviour object projected onto the xz plane
    public static Vector2 FlatPosition(this MonoBehaviour obj)
    {
        return new Vector2(obj.transform.position.x, obj.transform.position.z);
    }

    // Sets the Euler angle y rotation of a MonoBehaviour object to face the angle 
    public static void SetRotation(this MonoBehaviour obj, float newAngle)
    {
        obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, newAngle, obj.transform.localEulerAngles.z);
    }

    // Rotates the MonoBehaviour object to face in the TileDirection direction using SetRotation above
    public static void RotateToFace(this MonoBehaviour obj, Tile.TileDirection facingDirection)
    {
        switch (facingDirection)
        {
            case Tile.TileDirection.Down: obj.SetRotation(0f); break;
            case Tile.TileDirection.Left: obj.SetRotation(90f); break;
            case Tile.TileDirection.Up: obj.SetRotation(180f); break;
            case Tile.TileDirection.Right: obj.SetRotation(270f); break;
        }
    }

    // Inverses the tile direction, I.E. right becomes left
    static public Tile.TileDirection Inversed(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Left;
            case Tile.TileDirection.Down: return Tile.TileDirection.Up;
            case Tile.TileDirection.Left: return Tile.TileDirection.Right;
            case Tile.TileDirection.Up: return Tile.TileDirection.Down;
        }
        return Tile.TileDirection.None;
    }

    // Rotates the tile direction clockwise by one rotation
    static public Tile.TileDirection RotatedCW(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Down;
            case Tile.TileDirection.Down: return Tile.TileDirection.Left;
            case Tile.TileDirection.Left: return Tile.TileDirection.Up;
            case Tile.TileDirection.Up: return Tile.TileDirection.Right;
        }
        return Tile.TileDirection.None;
    }

    // Rotates the tile direction counterclockwise by one rotation
    static public Tile.TileDirection RotatedCCW(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Up;
            case Tile.TileDirection.Down: return Tile.TileDirection.Right;
            case Tile.TileDirection.Left: return Tile.TileDirection.Down;
            case Tile.TileDirection.Up: return Tile.TileDirection.Left;
        }
        return Tile.TileDirection.None;
    }

    /// <summary>
    /// Returns the name of a mana type in string form
    /// </summary>
    static public string ToString(this Mana.ManaType type)
    {
        switch (type)
        {
            case Mana.ManaType.Clubs: return "Clubs";
            case Mana.ManaType.Spades: return "Spades";
            case Mana.ManaType.Hearts: return "Hearts";
            case Mana.ManaType.Diamonds: return "Diamonds";
        }
        return "Suitless";
    }

    private static System.Random rng = new System.Random();

    /// <summary>
    /// Shuffles the list in place
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Get a list contaiing just this object
    /// </summary>
    public static List<T> IndividualList<T>(this T individual)
    {
        List<T> newList = new List<T>();
        newList.Add(individual);
        return newList;
    }

    /// <summary>
    /// Returns the integer as an English word
    /// </summary>
    public static string ToWord(this int number)
    {
        if (number == 0)
            return "zero";

        if (number < 0)
            return "minus " + Mathf.Abs(number).ToWord();

        string words = "";

        if ((number / 1000000) > 0)
        {
            words += (number / 1000000).ToWord() + " million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += (number / 1000).ToWord() + " thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += (number / 100).ToWord() + " hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words;
    }

    public static Transform Clear(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }

    public static Color AdjustedBrightness(this Color color, float correctionFactor)
    {
        float red = color.r;
        float green = color.g;
        float blue = color.b;

        if (correctionFactor < 0)
        {
            correctionFactor = 1f + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else
        {
            red = (1f - red) * correctionFactor + red;
            green = (1f - green) * correctionFactor + green;
            blue = (1f - blue) * correctionFactor + blue;
        }

        return new Color(red, green, blue, color.a);
    }

    public static string FirstCharToUpper(this string input)
    {
        if (input.Length <= 0)
            return "";
        return input[0].ToString().ToUpper() + input.Substring(1);
    }

    public static bool Contains<IEquatable>(this IEquatable[] arr, IEquatable input)
    {
        return Array.Exists(arr, val => val.Equals(input));
    }

    /// <summary>
    /// probably some in built way to do this but idk lol
    /// </summary>
    public static void ForEach<T>(this T[,] arr, Action<T> fnc, Action rowFnc = null)
    {
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            if (rowFnc != null)
                rowFnc();

            for (int x = 0; x < arr.GetLength(1); x++)
                fnc(arr[y, x]);
        }
    }

    public static T[,] Transposed<T>(this T[,] arr)
    {
        T[,] transposedArray = new T[arr.GetLength(1), arr.GetLength(0)];

        for (int y = 0; y < arr.GetLength(0); y++)
            for (int x = 0; x < arr.GetLength(1); x++)
                transposedArray[x, y] = arr[y, x];

        return transposedArray;
    }

    public static T[,] RowsReversed<T>(this T[,] arr)
    {
        T[,] reversedArray = new T[arr.GetLength(0), arr.GetLength(1)];

        for (int y = 0; y < arr.GetLength(0); y++)
            for (int x = 0; x < arr.GetLength(1); x++)
                reversedArray[y, x] = arr[y, arr.GetLength(1) - x - 1];

        return reversedArray;
    }

    public static T[,] ColumnsReversed<T>(this T[,] arr)
    {
        T[,] reversedArray = new T[arr.GetLength(0), arr.GetLength(1)];

        for (int y = 0; y < arr.GetLength(0); y++)
            for (int x = 0; x < arr.GetLength(1); x++)
                reversedArray[y, x] = arr[arr.GetLength(0) - y - 1, x];

        return reversedArray;
    }

    public static T[,] RotatedRight<T>(this T[,] arr)
    {
        return arr.Transposed().ColumnsReversed();
    }

    public static T[,] RotatedLeft<T>(this T[,] arr)
    {
        return arr.Transposed().RowsReversed();
    }

    public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Color SmoothDamp(this Color baseColor, Color targetColor, ref float speed, float acceleration = .15f)
    {
        Color color = baseColor;

        float newSpeedR = speed;
        color.r = Mathf.SmoothDamp(color.r, targetColor.r, ref newSpeedR, acceleration);
        float newSpeedG = speed;
        color.g = Mathf.SmoothDamp(color.g, targetColor.g, ref newSpeedG, acceleration);
        float newSpeedB = speed;
        color.b = Mathf.SmoothDamp(color.b, targetColor.b, ref newSpeedB, acceleration);
        float newSpeedA = speed;
        color.a = Mathf.SmoothDamp(color.a, targetColor.a, ref newSpeedA, acceleration);

        speed = (newSpeedR + newSpeedG + newSpeedB + newSpeedA) / 4f;
        return color;
    }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }
}