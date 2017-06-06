using System;
using UnityEngine;

[Serializable]
public struct Date
{
    private static int Seasons = 4;
    private static int Days = 40;
    private static int Hours = 24;
    private static int Minutes = 15;

    public static int secondsInYear = 14400; //Seconds in a year 14400
    public static int secondsInSeason = 3600;
    public static int secondsInDay = 360;
    public static int secondsInHour = 15;

    public float time;
    public float deltaTime;
    private int year;
    private int season;
    private int day;
    private int hour;
    private float minute;

    public int Day
    {
        get
        {
            return day;
        }

        set
        {
            day = value;
        }
    }

    public int Hour
    {
        get
        {
            return hour;
        }

        set
        {
            hour = value;
        }
    }

    public int Season
    {
        get
        {
            return season;
        }

        set
        {
            season = value;
        }
    }

    public int Year
    {
        get
        {
            return year;
        }

        set
        {
            year = value;
        }
    }

    public Date(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / secondsInYear);
        _time = _time % secondsInYear;
        season = Mathf.FloorToInt(_time / secondsInSeason);
        day = Mathf.FloorToInt(_time / secondsInDay);
        _time = _time % secondsInDay;
        hour = Mathf.FloorToInt(_time / secondsInHour);
        _time = _time % secondsInHour;
        minute = _time;
    }

    public void AddTime(float _time)
    {
        deltaTime = _time;
        time += _time;

        minute += _time;
        if (minute > Minutes) //Setting the Add Time parts
        {
            Hour += Mathf.FloorToInt(minute / Minutes);
            minute = minute % Minutes;

            if (Hour > Hours)
            {
                Day += Mathf.FloorToInt(Hour / Hours);
                Hour = Hour % Hours;

                season = Mathf.FloorToInt(Day / 10f);

                if (Day > Days)
                {
                    year += Mathf.FloorToInt(Day / Days);
                    Day = Day % Days;
                    
                }
            }
        }



    }

    internal void UpdateDate(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / secondsInYear);
        _time = _time % secondsInYear;
        season = Mathf.FloorToInt(_time / secondsInSeason);
        Day = Mathf.FloorToInt(_time / secondsInDay);
        _time = _time % secondsInDay;
        Hour = Mathf.FloorToInt(_time / secondsInHour);
        _time = _time % secondsInHour;
    }

    public string GetDate()
    {
        return year + " Year(s), " + (Day + 1) + " Day(s)";
    }

    public string GetDateTime()
    {
        return GetSeason() + " - " + Hour.ToString().PadLeft(2, '0') + " h " + Mathf.FloorToInt(minute).ToString().PadLeft(2, '0') + " m\n" + "Day " + (Day + 1) + ", Year " + year;
    }
    //public string GetDate(float _time)
    //{
    //    float year = Mathf.FloorToInt(_time / Year);
    //    _time = _time % Year;
    //    float season = Mathf.FloorToInt(_time / Season);
    //    float day = Mathf.FloorToInt(_time / Day);
    //    _time = _time % Day;
    //    float hour = Mathf.FloorToInt(_time / Hour);
    //    _time = _time % Hour;

    //    return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    //}

    private string GetSeason()
    {
        string[] seasonNames = new string[4] { "Spring", "Summer", "Fall", "Winter" };
        return seasonNames[season];
    }

    public static Date operator -( Date date1, Date date2)
    {
        float diff = date1.time - date2.time;

        Date finalDate = new Date(diff);

        return finalDate;

    }

    public static bool operator >(Date date1, Date date2)
    {
        return (date1.time > date2.time) ? true:false;


    }

    public static bool operator <(Date date1, Date date2)
    {
        return (date1.time < date2.time) ? true : false;
    }

    public static bool operator >=(Date date1, Date date2)
    {
        return (date1.time >= date2.time) ? true : false;
    }
    public static bool operator <=(Date date1, Date date2)
    {
        return (date1.time <= date2.time) ? true : false;
    }
}