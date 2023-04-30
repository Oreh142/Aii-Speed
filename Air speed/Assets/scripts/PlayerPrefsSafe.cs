using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class PlayerPrefsSafe
{
    private const int salt = 678309397;

    public static void SetBoolArray(string key, bool[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i])
            {
                PlayerPrefs.SetString(key + i, "T");
            }
            else
            {
                PlayerPrefs.SetString(key + i, "F");
            }
        }
    }

    public static void SetBool(string key, bool boolean)
    {
        if (boolean)
        {
            PlayerPrefs.SetString(key, "T");
        }
        else
        {
            PlayerPrefs.SetString(key, "F");
        }
    }

    public static bool[] GetBoolArray(string key, bool[] array)
    {
        bool[] tempArray = array;
        string value;
        for (int i = 0; i < array.Length; i++)
        {
            value = PlayerPrefs.GetString(key + i);
            if (value == "T")
            {
                tempArray[i] = true;
            }
            else if (value == "F")
            {
                tempArray[i] = false;
            }
        }
        return tempArray;
    }

    public static bool GetBool(string key)
    {
        bool boolean = false;
        string value;
        value = PlayerPrefs.GetString(key);
        if (value == "T")
        {
            boolean = true;
        }
        else if (value == "F")
        {
            boolean = false;
        }
        return boolean;
    }

    public static void SetInt(string key, int value)
    {
        int salted = value ^ salt;
        PlayerPrefs.SetInt(StringHash(key), salted);
        PlayerPrefs.SetInt(StringHash("_" + key), IntHash(value));
    }

    public static int GetInt(string key)
    {
        return GetInt(key, 0);
    }

    public static int GetInt(string key, int defaultValue)
    {
        string hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return defaultValue;

        int salted = PlayerPrefs.GetInt(hashedKey);
        int value = salted ^ salt;

        int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
        if (loadedHash != IntHash(value)) return defaultValue;

        return value;
    }

    public static void SetFloat(string key, float value)
    {
        int intValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

        int salted = intValue ^ salt;
        PlayerPrefs.SetInt(StringHash(key), salted);
        PlayerPrefs.SetInt(StringHash("_" + key), IntHash(intValue));
    }

    public static void SetFloatArray(string key, float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            PlayerPrefs.SetFloat(key + i, array[i]);
        }
    }
    public static float[] GetFloatArray(string key, float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = GetFloat(key + i, 0);
        }
        return array;
    }

    public static float GetFloat(string key)
    {
        return GetFloat(key, 0);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        string hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return defaultValue;

        int salted = PlayerPrefs.GetInt(hashedKey);
        int value = salted ^ salt;

        int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
        if (loadedHash != IntHash(value)) return defaultValue;

        return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }

    private static int IntHash(int x)
    {
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = (x >> 16) ^ x;
        return x;
    }

    public static string StringHash(string x)
    {
        HashAlgorithm algorithm = SHA256.Create();
        StringBuilder sb = new StringBuilder();

        var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(x));
        foreach (byte b in bytes) sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(StringHash(key));
        PlayerPrefs.DeleteKey(StringHash("_" + key));
    }

    public static bool HasKey(string key)
    {
        string hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return false;

        int salted = PlayerPrefs.GetInt(hashedKey);
        int value = salted ^ salt;

        int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));

        return loadedHash == IntHash(value);
    }
}