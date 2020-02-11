using System;

namespace TimeConversion
{
    public static class TimeConversion
    {
        /// <summary>
        /// Conversion factor between Julian Day and Modified Julian Day
        /// </summary>
        public const double dJD = 2400000.5d;

        /// <summary>
        /// Converts Time(DateTime object) to Julian Day.
        /// </summary>
        /// <param name="dt">Time to Convert.</param>
        /// <returns>Julian Day.</returns>
        public static double Time2JulDay(DateTime dt)
        {
            double Y = (double)dt.Year;
            double M = (double)dt.Month;
            double H = (dt - dt.Date).TotalDays;

            if (M <= 2)
            {
                Y -= 1;
                M += 12;
            }
            double A = Math.Truncate(Y / 100d);
            double B = 2 - A + Math.Truncate(A / 4d);

            return Math.Truncate(365.25f * (Y + 4716d)) + Math.Truncate(30.6001d * (M + 1)) + dt.Day + H + B - 1524.5d;
        }
        
        /// <summary>
        /// Converts Time(DateTime object) to modified Julian Day.
        /// </summary>
        /// <param name="dt">Time to Convert.</param>
        /// <returns>modified Julian Day.</returns>
        public static double Time2ModJulDay(DateTime dt)
        {
            return Time2JulDay(dt) - dJD;
        }

        /// <summary>
        /// Converts Julian Day to Time(DateTime object).
        /// </summary>
        /// <param name="mjd"> Julian Day to convert.</param>
        /// <returns>Time (DateTime object).</returns>
        public static DateTime JulDay2Time(double mjd)
        {
            int Z = (int)Math.Truncate(mjd + 0.5);
            double F = (mjd + 0.5) - Z;

            int g = (int)Math.Truncate((Z - 1867216.25f) / 36524.25f);
            int A = (int)Math.Truncate(Z + 1 + g - Math.Truncate((double)g / 4f));
            int B = A + 1524;
            int C = (int)Math.Truncate(((double)B - 122.1f) / 365.25f);
            int D = (int)Math.Truncate(365.25 * C);
            int E = (int)Math.Truncate((B - D) / 30.6001);

            double Tag = B - D - (int)Math.Truncate(30.6001f * E) + F;            // Tag, inklusive Tagesbruchteil

            int Monat = E - 13;
            if (E < 14)
                Monat = E - 1;
            int Jahr = C - 4715;
            if (Monat > 2)
                Jahr = C - 4716;

            return new DateTime(Jahr, Monat, 1).AddDays(Tag - 1);
        }
       
        /// <summary>
        /// Converts modified Julian Day to Time(DateTime object).
        /// </summary>
        /// <param name="mjd">modified Julian Day to convert.</param>
        /// <returns>Time (DateTime object).</returns>
        public static DateTime ModJulDay2Time(double mjd)
        {
            return JulDay2Time(mjd + dJD);
        }

        /// <summary>
        /// Converts Julian Day to Modified Julian Day.
        /// </summary>
        /// <param name="jd">Julian Day.</param>
        /// <returns>Modified Julian Day</returns>
        public static double JulDay2ModJulDay(double jd)
        {
            return (jd - dJD);
        }

        /// <summary>
        /// Converts Modified Julian Day to Julian Day.
        /// </summary>
        /// <param name="mjd">Modified Julian Day</param>
        /// <returns>Julian Day</returns>
        public static double ModJulDay2JulDay(double mjd)
        {
            return (mjd + dJD);
        }

        /// <summary>
        /// Converts Julian Day to DT2000
        /// </summary>
        /// <param name="djuld">Julian Day</param>
        /// <returns>DT2000</returns>
        public static double djuld2DT2000(double djuld)
        {
            return (djuld - 2451544.0) / 36525.0;
        }

        /// <summary>
        /// Calculates the needed leap second steps for the GPS second and saved in ArrayList
        /// </summary>
        /// <param name="leapSecond"> Leap second object from the xml file.</param>
        /// <returns>List of the leap seconds for calculation the GPS second.</returns>
        public static System.Collections.ArrayList getLeapSecondsForGPS(LeapSecond leapSecond)
        {
            System.Collections.ArrayList lsec = new System.Collections.ArrayList();

            DateTime dtGPSstart = new DateTime(1980, 1, 6, 0, 0, 0);

            for (int i = 0; i < leapSecond.leapSecond.Length; i++)
                if (leapSecond.leapSecond[i].Time.Year >= 1981)
                    lsec.Add((int)(leapSecond.leapSecond[i].Time - dtGPSstart).TotalSeconds + lsec.Count);

            return lsec;
        }

        /// <summary>
        /// According to the GPS second and the leapseconds is calculated the time offset in seconds to UTC 
        /// </summary>
        /// <param name="GPS_Seconds">GPS second wherefore the offset has to be calculated.</param>
        /// <param name="LeapSeconds">List of the leap seconds in GPS seconds.</param>
        /// <returns>Offset between the UTC time scale and the GPS second.</returns>
        private static int countLeadSecondsForGPS(long GPS_Seconds, System.Collections.ArrayList LeapSeconds)
        {
            int count = 0;
            for (int i = 1; i < LeapSeconds.Count; i++)
            {
                if (GPS_Seconds >= (int)LeapSeconds[i - 1] && GPS_Seconds < (int)LeapSeconds[i])
                {
                    count = i;
                    break;
                }
                else if (i == LeapSeconds.Count - 1 && GPS_Seconds >= (int)LeapSeconds[i])
                    count = i + 1;
            }
            return count;
        }

        /// <summary>
        /// Calculates from the GPS second and the according GPS leap seconds a DateTime.
        /// </summary>
        /// <param name="GPS_Second">GPS second, which has to be converted into a DateTime.</param>
        /// <param name="Leapseconds">List of the leap seconds in GPS seconds.</param>
        /// <returns>Converted DateTime.</returns>
        public static DateTime getDateTimefromGPSSeconds(long GPS_Second, System.Collections.ArrayList Leapseconds)
        {
            return new DateTime(1980, 1, 6, 0, 0, 0).AddSeconds(GPS_Second - countLeadSecondsForGPS(GPS_Second, Leapseconds));
        }

        /// <summary>
        /// Calculates from from a DataTime object and according GPS leap second array the GPS second.
        /// </summary>
        /// <param name="dt">DateTime which has to be converted into the GPS second.</param>
        /// <param name="Leapseconds">List of the leap seconds for calculation the GPS second. </param>
        /// <returns>The calculated GPS second.</returns>
        public static long getGPSSecondsFromDateTime(DateTime dt, System.Collections.ArrayList leapSeconds)
        {
            long gpssec = (long)(dt - new DateTime(1980, 1, 6, 0, 0, 0)).TotalSeconds;
            return (gpssec + countLeadSecondsForGPS(gpssec, leapSeconds));
        }

        /// <summary>
        /// GPS time structure for calculations
        /// </summary>
        public struct GPSTime
        {
            public int Week;
            public int Day;
            public int SecondofWeek;
            public int Rollover;
        }

        /// <summary>
        /// Calculates from the GPS second the GPS time given at the GPS time structure.
        /// </summary>
        /// <param name="GPS_Second">GPS second, which has to be converted in the GPS time structure.</param>
        /// <returns>GPS time structure.</returns>
        public static GPSTime getGPSWeekSecondFromGPSSeconds(long GPS_Second)
        {
            GPSTime GPSWS = new GPSTime();

            GPSWS.SecondofWeek = (int)(GPS_Second % (86400 * 7));
            GPSWS.Day = (int)Math.Floor(GPSWS.SecondofWeek / 86400.0);
            GPSWS.Week = (int)((GPS_Second - (long)GPSWS.SecondofWeek) / (long)(86400 * 7));
            GPSWS.Rollover = (int)Math.Floor((double)(GPSWS.Week + 1) / 1024.0);

            if (GPSWS.Rollover > 0)
                GPSWS.Week -= GPSWS.Rollover * 1024;

            return GPSWS;
        }

        /// <summary>
        /// Determines the GPS second by using the GPS time structure of the week, rollover and sekond of the week.
        /// </summary>
        /// <param name="GPSWS">GPS time structure</param>
        /// <returns>The calculated GPS second.</returns>
        public static long getGPSSecondFromGPSWeekSecond(GPSTime GPSWS)
        {
            return (((long)GPSWS.Week + ((long)GPSWS.Rollover * 1024)) * 86400 * 7) + (long)GPSWS.SecondofWeek;
        }
    }
}
