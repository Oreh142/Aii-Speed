using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormatNumsHelper
{
    private static string[] names = new[]
        {
            "",
            "K",
            "M",
            "B",
            "T",
            "Q",
            "QQ",
            "Hex",
            "Hep"
        };
    private static string[] timeNames = new[]
    {
        " Ñ",
        " Ì",
        " ×",
        " Ä"
    };

    public static string FormatNum(double num)
    {
        if (num == 0) return "0";

        num = Mathf.Round((float)num);

        int i = 0;
        while (i + 1 < names.Length && num >= 1000d)
        {
            num /= 1000;
            i++;
        }

        return num.ToString(format: "#.#") + names[i];
    }

    public static string FormatNum(float num)
    {
        if (num == 0) return "0";

        num = Mathf.Round(num);

        int i = 0;
        while (i + 1 < names.Length && num >= 1000f)
        {
            num /= 1000f;
            i++;
        }

        return num.ToString(format: "#.#") + names[i];
    }

    public static string FormatNum(decimal num)
    {
        if (num == 0) return "0";

        num = decimal.Round(num);

        int i = 0;
        while (i + 1 < names.Length && num >= 1000m)
        {
            num /= 1000m;
            i++;
        }

        return num.ToString(format: "#.#") + names[i];
    }
    public static float FormatSpeed(float num)
    {
        if (num == 0) return 0;

        num *= 33;
        return num;
    }
    public static float FormatToUnitySpeed(float num)
    {
        if (num == 0) return 0;

        num /= 33;
        return num;
    }
    public static string FormatTime(float num)
    {

        int i;

        num = Mathf.Round(num);

        for (i = 0; i < 2 && num > 60; i++)
        {
            num /= 60f;
        }
        if (num > 24 && i == 2)
        {
            if (num > 24)
            {
                num /= 24f;
            }
            i++;
        }

        return num.ToString(format: "#") + timeNames[i];
    }

    public static DateTime ChangeTime(this DateTime dateTime, int day, int hours, int minutes, int seconds, int milliseconds)
    {
        return new DateTime(
        dateTime.Year,
        dateTime.Month,
        dateTime.Day - day,
        hours,
        minutes,
        seconds,
        milliseconds,
        dateTime.Kind);
    }

    public static int DayOfTheWeek()
    {
        int num = 0;
        switch (DateTime.Now.DayOfWeek.ToString())
        {
            case "Monday":
                num = 0;
                break;
            case "Tuesday":
                num = 1;
                break;
            case "Wednesday":
                num = 2;
                break;
            case "Thursday":
                num = 3;
                break;
            case "Friday":
                num = 4;
                break;
            case "Saturday":
                num = 5;
                break;
            case "Sunday":
                num = 6;
                break;
        }
        return num;
    }

    public static int DayOfTheYear()
    {
        TimeSpan ts = DateTime.Now - Convert.ToDateTime("01.01.2023 00:00:00");
        return (int)ts.TotalDays;
    }
}
