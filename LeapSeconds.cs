using System;
using System.Xml.Serialization;
using System.IO;

namespace TimeConversion
{
    /// <summary>
    /// Class, which contains the data for the leapseconds.
    /// </summary>
    [XmlRoot("LeapSecond")]
    public class LeapSecond
    {
        /// <summary>
        /// Constructor, which contains the data for the leapseconds.
        /// </summary>
        public LeapSecond()
        { }

        /// <summary>
        /// Time tag for knowing the last update of the data.
        /// </summary>
        [XmlElement("lastUpDate")]
        public DateTime lastUpDate;

        /// <summary>
        /// Temporary object, which contains the data for the leapseconds entries.
        /// </summary>
        [XmlIgnore()]
        public LeapSecondEntry[] leapSecondTMP = null;

        /// <summary>
        /// Object, which contains the data for the leapseconds entries.
        /// </summary>
        [XmlElement("LeapSecond")]
        public LeapSecondEntry[] leapSecond = null;

        /// <summary>
        /// Class, which contains the data for the leapseconds entries.
        /// </summary>
        public class LeapSecondEntry
        {
            /// <summary>
            /// Time of the according data in modifed Julian day format.
            /// </summary>
            [XmlAttribute("MJD")]
            public double MJD;

            /// <summary>
            ///  Time of the according data.
            /// </summary>
            [XmlAttribute("Time")]
            public DateTime Time;

            /// <summary>
            /// Value of the leap second.
            /// </summary>
            [XmlAttribute("TAI_UTC")]
            public int TAI_UTC;

            /// <summary>
            /// Constructor for the class, which contains the data for the leapseconds.
            /// </summary>
            public LeapSecondEntry()
            {
            }

            /// <summary>
            /// Constructor for the class, which contains the data for the leapseconds.
            /// </summary>
            /// <param name="MJD">Time of the according data in modifed Julian day format.</param>
            /// <param name="Time">Time of the according data.</param>
            /// <param name="TAI_UTC">Value of the leap second.</param>
            public LeapSecondEntry(double MJD, DateTime Time, int TAI_UTC)
            {
                this.MJD = MJD;
                this.Time = Time;
                this.TAI_UTC = TAI_UTC;
            }
        }

        /// <summary>
        /// Analyses the 'Leap_Second_History.dat'
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="adress">Address of the host, where the file to be analyses are found.</param>
        /// <param name="message">Error message, if something was going wrong on reading(std. if everthing ok =null).</param>
        /// <returns>Returns the the object for 'LeapSecondEntry' (favorable is used the temporary object)</returns>
        public LeapSecondEntry[] analyseFileLeapSecond(string path, string adress, ref string message)
        {
            LeapSecondEntry[] leapsecond = null;
            System.Collections.ArrayList tmp = new System.Collections.ArrayList();
            string strTmp = null;
            try
            {
                StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open));
                while ((strTmp = reader.ReadLine()) != null)
                {
                    if (strTmp != null && strTmp[0] != '#')
                        tmp.Add(strTmp);
                }
                reader.Close();

                leapsecond = new LeapSecondEntry[tmp.Count];

                char[] delimeter = new char[] { ' ', '\t' };
                string[] parts = null;
                for (int i = 0; i < tmp.Count; i++)
                {
                    parts = ((string)tmp[i]).Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                    leapsecond[i] = new LeapSecondEntry(double.Parse(parts[0], Constants.NumberFormatEN),
                                                   new DateTime(int.Parse(parts[3]), int.Parse(parts[2]), int.Parse(parts[1]), 0, 0, 0),
                                                   int.Parse(parts[4]));
                }
            }
            catch (Exception ex)
            {
                leapsecond = null;
                message += "Error on analysis of the leapsecond file from:" + Environment.NewLine +
                           " " + adress + Environment.NewLine +
                           "System Error:" + ex.Message + Environment.NewLine;
            }
            return leapsecond;
        }

        /// <summary>
        /// Serialization of the LeapSecond class by using the properties path.
        /// </summary>
        /// <param name="leapSecond"> object LeapSecond</param>
        /// <param name="Path">Path of the object to be serialized.</param>
        public static void serialisieren(LeapSecond leapSecond, string Path)
        {
            XmlSerializer s = new XmlSerializer(typeof(LeapSecond));
            TextWriter w = new StreamWriter(Path);
            s.Serialize(w, leapSecond);
            w.Close();
        }

        /// <summary>
        /// Deserialization of the LeapSecond class by using the properties path.
        /// </summary>
        /// <returns>object LeapSecond</returns>
        public static LeapSecond deserialisieren(string path)
        {
            LeapSecond leapSecond;
            XmlSerializer s = new XmlSerializer(typeof(LeapSecond));
            TextReader r = new StreamReader(path);
            leapSecond = (LeapSecond)s.Deserialize(r);
            r.Close();
            return leapSecond;
        }
    }
}
