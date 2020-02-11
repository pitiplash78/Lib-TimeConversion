using System;
using System.Xml.Serialization;
using System.IO;

namespace TimeConversion
{
    /// <summary>
    ///  Class for ETDDT.
    /// </summary>
    [XmlRoot("ETDDT")]
    public class TAI_UTC
    {
        /// <summary>
        ///  Constructor for ETDDT.
        /// </summary>
        public TAI_UTC()
        {
        }

        /// <summary>
        /// Returns the difference  DDT = ET-UTC or TDT - UTC for given year. Equal to Eterna method.
        /// </summary>
        /// <param name="year">Year for difference  DDT = ET-UTC or TDT - UTC</param>
        /// <returns>DDT = ET-UTC or TDT - UTC</returns>
        public double GetDifferenceFrom_TDT_UTC(double year)
        {
            int itab = 1;

            if (year >= etddt[etddt.Length - 1].Year)                               // IF(DTUJD.LT.DDTTAB(2,NDDTAB)) GOTO 100
            {
                return etddt[etddt.Length - 1].DDT;                                 // DDT=DDTTAB(3,NDDTAB) & // RETURN
            }
            else
            {
                // Look at table at position ITAB:
                bool found = false;
                while (!found)
                {                                                                   //   100 CONTINUE
                    if (year >= etddt[itab - 1].Year && year < etddt[itab].Year)    // IF(DTUJD.GE.DDTTAB(2,ITAB).AND.DTUJD.LT.DDTTAB(2,ITAB+1)) GOTO 230
                        found = true;

                    if (year < etddt[itab - 1].Year)                                // IF(DTUJD.LT.DDTTAB(2,ITAB)) THEN
                    {
                        itab--;                                                     // ITAB=ITAB-1
                        if (itab <= 1)                                              // IF(ITAB.GT.0) GOTO 100
                            found = true;
                        else
                            continue;

                        // Set DDT to first tabulated value and return:
                        itab = 1;                                                   // ITAB=1
                        return etddt[0].DDT;                                        // DDT=DDTTAB(3,1) & // RETURN
                    }

                    if (year > etddt[itab].Year)                                    // IF(DTUJD.GT.DDTTAB(2,ITAB+1)) THEN
                    {
                        itab++;                                                     // ITAB=ITAB+1
                        if (itab > etddt.Length - 1)                                // IF(ITAB.LT.NDDTAB) GOTO 100
                            found = true;
                        else
                            continue;

                        // Set DDT to last tabulated value and return:
                        itab = etddt.Length - 1;                                    // ITAB=NDDTAB
                        return etddt[etddt.Length - 1].DDT;                         // DDT=DDTTAB(3,NDDTAB) & // RETURN
                    }

                    // Interpolate table between position ITAB and ITAB+1:
                    
                    return (etddt[itab].DDT * (year - etddt[itab - 1].Year) -
                           etddt[itab - 1].DDT * (year - etddt[itab].Year)) /
                          (etddt[itab].Year - etddt[itab - 1].Year);                // DDT=(DDTTAB(3,ITAB+1)*(DTUJD-DDTTAB(2,ITAB))-DDTTAB(3,ITAB)*(DTUJD-DDTTAB(2,ITAB+1)))/(DDTTAB(2,ITAB+1)-DDTTAB(2,ITAB))
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Object for ETDDT entries.
        /// </summary>
        [XmlElement("ETDDT")]
        public ETDDTEntry[] etddt = null;

        /// <summary>
        /// Time tag for knowing the last update of the data.
        /// </summary>
        [XmlElement("lastUpDate")]
        public DateTime lastUpDate;

        /// <summary>
        /// Class for ETDDT entries.
        /// </summary>
        public class ETDDTEntry
        {
            /// <summary>
            /// Year, where a leap second was introduced.
            /// </summary>
            [XmlAttribute("Year")]
            public double Year = 0;

            /// <summary>
            /// Time in Julian day, where a leap second was introduced.
            /// </summary>
            [XmlAttribute("JD")]
            public double JD = 0;

            /// <summary>
            /// Value for DDT = ET - UTC.
            /// </summary>
            [XmlAttribute("DDT")]
            public double DDT = 0;

            /// <summary>
            /// Constructor, for ETDDT entries.
            /// </summary>
            public ETDDTEntry()
            { }

            /// <summary>
            /// Constructor, for ETDDT entries.
            /// </summary>
            /// <param name="Year">Year, where a leap second was introduced.</param>
            /// <param name="JD">Time in Julian day, where a leap second was introduced.</param>
            /// <param name="DDT">Value for DDT = ET - UTC.</param>
            public ETDDTEntry(double Year, double JD, double DDT)
            {
                this.Year = Year;
                this.JD = JD;
                this.DDT = DDT;
            }
        }

        /// <summary>
        /// The function sets the data for 'ETDDT' by using the leap seconds found in 'LeapSecond' object.
        /// </summary>
        /// <param name="leapSecond">Object for the leap seconds.</param>
        /// <returns>Object for the 'ETDDTEntry'</returns>
        public ETDDTEntry[] setData(LeapSecond leapSecond)
        {
            int startIndexLeapSecond = 0;

            for (int i = leapSecond.leapSecond.Length - 1; i > 0; i--)
                if (leapSecond.leapSecond[i].Time.Year > 1983)
                    startIndexLeapSecond = i;

            int length = 203 + (leapSecond.leapSecond.Length - startIndexLeapSecond) * 2 + 1;
            ETDDTEntry[] etddtTmP = new ETDDTEntry[length];

            etddtTmP[0] = new ETDDTEntry(1820.00000, 2385800.500000, 12.000);
            etddtTmP[1] = new ETDDTEntry(1821.00000, 2386166.500000, 11.700);
            etddtTmP[2] = new ETDDTEntry(1822.00000, 2386531.500000, 11.400);
            etddtTmP[3] = new ETDDTEntry(1823.00000, 2386896.500000, 11.100);
            etddtTmP[4] = new ETDDTEntry(1824.00000, 2387261.500000, 10.600);
            etddtTmP[5] = new ETDDTEntry(1825.00000, 2387627.500000, 10.200);
            etddtTmP[6] = new ETDDTEntry(1826.00000, 2387992.500000, 9.600);
            etddtTmP[7] = new ETDDTEntry(1827.00000, 2388357.500000, 9.100);
            etddtTmP[8] = new ETDDTEntry(1828.00000, 2388722.500000, 8.600);
            etddtTmP[9] = new ETDDTEntry(1829.00000, 2389088.500000, 8.000);
            etddtTmP[10] = new ETDDTEntry(1830.00000, 2389453.500000, 7.500);
            etddtTmP[11] = new ETDDTEntry(1831.00000, 2389818.500000, 7.000);
            etddtTmP[12] = new ETDDTEntry(1832.00000, 2390183.500000, 6.600);
            etddtTmP[13] = new ETDDTEntry(1833.00000, 2390549.500000, 6.300);
            etddtTmP[14] = new ETDDTEntry(1834.00000, 2390914.500000, 6.000);
            etddtTmP[15] = new ETDDTEntry(1835.00000, 2391279.500000, 5.800);
            etddtTmP[16] = new ETDDTEntry(1836.00000, 2391644.500000, 5.700);
            etddtTmP[17] = new ETDDTEntry(1837.00000, 2392010.500000, 5.600);
            etddtTmP[18] = new ETDDTEntry(1838.00000, 2392375.500000, 5.600);
            etddtTmP[19] = new ETDDTEntry(1839.00000, 2392740.500000, 5.600);
            etddtTmP[20] = new ETDDTEntry(1840.00000, 2393105.500000, 5.700);
            etddtTmP[21] = new ETDDTEntry(1841.00000, 2393471.500000, 5.800);
            etddtTmP[22] = new ETDDTEntry(1842.00000, 2393836.500000, 5.900);
            etddtTmP[23] = new ETDDTEntry(1843.00000, 2394201.500000, 6.100);
            etddtTmP[24] = new ETDDTEntry(1844.00000, 2394566.500000, 6.200);
            etddtTmP[25] = new ETDDTEntry(1845.00000, 2394932.500000, 6.300);
            etddtTmP[26] = new ETDDTEntry(1846.00000, 2395297.500000, 6.500);
            etddtTmP[27] = new ETDDTEntry(1847.00000, 2395662.500000, 6.600);
            etddtTmP[28] = new ETDDTEntry(1848.00000, 2396027.500000, 6.800);
            etddtTmP[29] = new ETDDTEntry(1849.00000, 2396393.500000, 6.900);
            etddtTmP[30] = new ETDDTEntry(1850.00000, 2396758.500000, 7.100);
            etddtTmP[31] = new ETDDTEntry(1851.00000, 2397123.500000, 7.200);
            etddtTmP[32] = new ETDDTEntry(1852.00000, 2397488.500000, 7.300);
            etddtTmP[33] = new ETDDTEntry(1853.00000, 2397854.500000, 7.400);
            etddtTmP[34] = new ETDDTEntry(1854.00000, 2398219.500000, 7.500);
            etddtTmP[35] = new ETDDTEntry(1855.00000, 2398584.500000, 7.600);
            etddtTmP[36] = new ETDDTEntry(1856.00000, 2398949.500000, 7.700);
            etddtTmP[37] = new ETDDTEntry(1857.00000, 2399315.500000, 7.700);
            etddtTmP[38] = new ETDDTEntry(1858.00000, 2399680.500000, 7.800);
            etddtTmP[39] = new ETDDTEntry(1859.00000, 2400045.500000, 7.800);
            etddtTmP[40] = new ETDDTEntry(1860.00000, 2400410.500000, 7.880);
            etddtTmP[41] = new ETDDTEntry(1861.00000, 2400776.500000, 7.820);
            etddtTmP[42] = new ETDDTEntry(1862.00000, 2401141.500000, 7.540);
            etddtTmP[43] = new ETDDTEntry(1863.00000, 2401506.500000, 6.970);
            etddtTmP[44] = new ETDDTEntry(1864.00000, 2401871.500000, 6.400);
            etddtTmP[45] = new ETDDTEntry(1865.00000, 2402237.500000, 6.020);
            etddtTmP[46] = new ETDDTEntry(1866.00000, 2402602.500000, 5.410);
            etddtTmP[47] = new ETDDTEntry(1867.00000, 2402967.500000, 4.100);
            etddtTmP[48] = new ETDDTEntry(1868.00000, 2403332.500000, 2.920);
            etddtTmP[49] = new ETDDTEntry(1869.00000, 2403698.500000, 1.820);
            etddtTmP[50] = new ETDDTEntry(1870.00000, 2404063.500000, 1.610);
            etddtTmP[51] = new ETDDTEntry(1871.00000, 2404428.500000, 0.100);
            etddtTmP[52] = new ETDDTEntry(1872.00000, 2404793.500000, -1.020);
            etddtTmP[53] = new ETDDTEntry(1873.00000, 2405159.500000, -1.280);
            etddtTmP[54] = new ETDDTEntry(1874.00000, 2405524.500000, -2.690);
            etddtTmP[55] = new ETDDTEntry(1875.00000, 2405889.500000, -3.240);
            etddtTmP[56] = new ETDDTEntry(1876.00000, 2406254.500000, -3.640);
            etddtTmP[57] = new ETDDTEntry(1877.00000, 2406620.500000, -4.540);
            etddtTmP[58] = new ETDDTEntry(1878.00000, 2406985.500000, -4.710);
            etddtTmP[59] = new ETDDTEntry(1879.00000, 2407350.500000, -5.110);
            etddtTmP[60] = new ETDDTEntry(1880.00000, 2407715.500000, -5.400);
            etddtTmP[61] = new ETDDTEntry(1881.00000, 2408081.500000, -5.420);
            etddtTmP[62] = new ETDDTEntry(1882.00000, 2408446.500000, -5.200);
            etddtTmP[63] = new ETDDTEntry(1883.00000, 2408811.500000, -5.460);
            etddtTmP[64] = new ETDDTEntry(1884.00000, 2409176.500000, -5.460);
            etddtTmP[65] = new ETDDTEntry(1885.00000, 2409542.500000, -5.790);
            etddtTmP[66] = new ETDDTEntry(1886.00000, 2409907.500000, -5.630);
            etddtTmP[67] = new ETDDTEntry(1887.00000, 2410272.500000, -5.640);
            etddtTmP[68] = new ETDDTEntry(1888.00000, 2410637.500000, -5.800);
            etddtTmP[69] = new ETDDTEntry(1889.00000, 2411003.500000, -5.660);
            etddtTmP[70] = new ETDDTEntry(1890.00000, 2411368.500000, -5.870);
            etddtTmP[71] = new ETDDTEntry(1891.00000, 2411733.500000, -6.010);
            etddtTmP[72] = new ETDDTEntry(1892.00000, 2412098.500000, -6.190);
            etddtTmP[73] = new ETDDTEntry(1893.00000, 2412464.500000, -6.640);
            etddtTmP[74] = new ETDDTEntry(1894.00000, 2412829.500000, -6.440);
            etddtTmP[75] = new ETDDTEntry(1895.00000, 2413194.500000, -6.470);
            etddtTmP[76] = new ETDDTEntry(1896.00000, 2413559.500000, -6.090);
            etddtTmP[77] = new ETDDTEntry(1897.00000, 2413925.500000, -5.760);
            etddtTmP[78] = new ETDDTEntry(1898.00000, 2414290.500000, -4.660);
            etddtTmP[79] = new ETDDTEntry(1899.00000, 2414655.500000, -3.740);
            etddtTmP[80] = new ETDDTEntry(1900.00000, 2415020.500000, -2.720);
            etddtTmP[81] = new ETDDTEntry(1901.00000, 2415385.500000, -1.540);
            etddtTmP[82] = new ETDDTEntry(1902.00000, 2415750.500000, -0.020);
            etddtTmP[83] = new ETDDTEntry(1903.00000, 2416115.500000, 1.240);
            etddtTmP[84] = new ETDDTEntry(1904.00000, 2416480.500000, 2.640);
            etddtTmP[85] = new ETDDTEntry(1905.00000, 2416846.500000, 3.860);
            etddtTmP[86] = new ETDDTEntry(1906.00000, 2417211.500000, 5.370);
            etddtTmP[87] = new ETDDTEntry(1907.00000, 2417576.500000, 6.140);
            etddtTmP[88] = new ETDDTEntry(1908.00000, 2417941.500000, 7.750);
            etddtTmP[89] = new ETDDTEntry(1909.00000, 2418307.500000, 9.130);
            etddtTmP[90] = new ETDDTEntry(1910.00000, 2418672.500000, 10.460);
            etddtTmP[91] = new ETDDTEntry(1911.00000, 2419037.500000, 11.530);
            etddtTmP[92] = new ETDDTEntry(1912.00000, 2419402.500000, 13.360);
            etddtTmP[93] = new ETDDTEntry(1913.00000, 2419768.500000, 14.650);
            etddtTmP[94] = new ETDDTEntry(1914.00000, 2420133.500000, 16.010);
            etddtTmP[95] = new ETDDTEntry(1915.00000, 2420498.500000, 17.200);
            etddtTmP[96] = new ETDDTEntry(1916.00000, 2420863.500000, 18.240);
            etddtTmP[97] = new ETDDTEntry(1917.00000, 2421229.500000, 19.060);
            etddtTmP[98] = new ETDDTEntry(1918.00000, 2421594.500000, 20.250);
            etddtTmP[99] = new ETDDTEntry(1919.00000, 2421959.500000, 20.950);
            etddtTmP[100] = new ETDDTEntry(1920.00000, 2422324.500000, 21.160);
            etddtTmP[101] = new ETDDTEntry(1921.00000, 2422690.500000, 22.250);
            etddtTmP[102] = new ETDDTEntry(1922.00000, 2423055.500000, 22.410);
            etddtTmP[103] = new ETDDTEntry(1923.00000, 2423420.500000, 23.030);
            etddtTmP[104] = new ETDDTEntry(1924.00000, 2423785.500000, 23.490);
            etddtTmP[105] = new ETDDTEntry(1925.00000, 2424151.500000, 23.620);
            etddtTmP[106] = new ETDDTEntry(1926.00000, 2424516.500000, 23.860);
            etddtTmP[107] = new ETDDTEntry(1927.00000, 2424881.500000, 24.490);
            etddtTmP[108] = new ETDDTEntry(1928.00000, 2425246.500000, 24.340);
            etddtTmP[109] = new ETDDTEntry(1929.00000, 2425612.500000, 24.080);
            etddtTmP[110] = new ETDDTEntry(1930.00000, 2425977.500000, 24.020);
            etddtTmP[111] = new ETDDTEntry(1931.00000, 2426342.500000, 24.000);
            etddtTmP[112] = new ETDDTEntry(1932.00000, 2426707.500000, 23.870);
            etddtTmP[113] = new ETDDTEntry(1933.00000, 2427073.500000, 23.950);
            etddtTmP[114] = new ETDDTEntry(1934.00000, 2427438.500000, 23.860);
            etddtTmP[115] = new ETDDTEntry(1935.00000, 2427803.500000, 23.930);
            etddtTmP[116] = new ETDDTEntry(1936.00000, 2428168.500000, 23.730);
            etddtTmP[117] = new ETDDTEntry(1937.00000, 2428534.500000, 23.920);
            etddtTmP[118] = new ETDDTEntry(1938.00000, 2428899.500000, 23.960);
            etddtTmP[119] = new ETDDTEntry(1939.00000, 2429264.500000, 24.020);
            etddtTmP[120] = new ETDDTEntry(1940.00000, 2429629.500000, 24.330);
            etddtTmP[121] = new ETDDTEntry(1941.00000, 2429995.500000, 24.830);
            etddtTmP[122] = new ETDDTEntry(1942.00000, 2430360.500000, 25.300);
            etddtTmP[123] = new ETDDTEntry(1943.00000, 2430725.500000, 25.700);
            etddtTmP[124] = new ETDDTEntry(1944.00000, 2431090.500000, 26.240);
            etddtTmP[125] = new ETDDTEntry(1945.00000, 2431456.500000, 26.770);
            etddtTmP[126] = new ETDDTEntry(1946.00000, 2431821.500000, 27.280);
            etddtTmP[127] = new ETDDTEntry(1947.00000, 2432186.500000, 27.780);
            etddtTmP[128] = new ETDDTEntry(1948.00000, 2432551.500000, 28.250);
            etddtTmP[129] = new ETDDTEntry(1949.00000, 2432917.500000, 28.710);
            etddtTmP[130] = new ETDDTEntry(1950.00000, 2433282.500000, 29.150);
            etddtTmP[131] = new ETDDTEntry(1951.00000, 2433647.500000, 29.570);
            etddtTmP[132] = new ETDDTEntry(1952.00000, 2434012.500000, 29.970);
            etddtTmP[133] = new ETDDTEntry(1953.00000, 2434378.500000, 30.360);
            etddtTmP[134] = new ETDDTEntry(1954.00000, 2434743.500000, 30.720);
            etddtTmP[135] = new ETDDTEntry(1955.00000, 2435108.500000, 31.070);
            etddtTmP[136] = new ETDDTEntry(1956.00000, 2435473.500000, 31.350);
            etddtTmP[137] = new ETDDTEntry(1957.00000, 2435839.500000, 31.680);
            etddtTmP[138] = new ETDDTEntry(1958.00000, 2436204.500000, 32.180);
            etddtTmP[139] = new ETDDTEntry(1959.00000, 2436569.500000, 32.680);
            etddtTmP[140] = new ETDDTEntry(1960.00000, 2436934.500000, 33.150);
            etddtTmP[141] = new ETDDTEntry(1961.00000, 2437300.500000, 33.590);
            etddtTmP[142] = new ETDDTEntry(1962.00000, 2437665.500000, 34.032);
            etddtTmP[143] = new ETDDTEntry(1962.50000, 2437846.500000, 34.235);
            etddtTmP[144] = new ETDDTEntry(1963.00000, 2438030.500000, 34.441);
            etddtTmP[145] = new ETDDTEntry(1963.50000, 2438211.500000, 34.644);
            etddtTmP[146] = new ETDDTEntry(1964.00000, 2438395.500000, 34.950);
            etddtTmP[147] = new ETDDTEntry(1964.50000, 2438577.500000, 35.286);
            etddtTmP[148] = new ETDDTEntry(1965.00000, 2438761.500000, 35.725);
            etddtTmP[149] = new ETDDTEntry(1965.50000, 2438942.500000, 36.160);
            etddtTmP[150] = new ETDDTEntry(1966.00000, 2439126.500000, 36.498);
            etddtTmP[151] = new ETDDTEntry(1966.50000, 2439307.500000, 36.968);
            etddtTmP[152] = new ETDDTEntry(1967.00000, 2439491.500000, 37.444);
            etddtTmP[153] = new ETDDTEntry(1967.50000, 2439672.500000, 37.913);
            etddtTmP[154] = new ETDDTEntry(1968.00000, 2439856.500000, 38.390);
            etddtTmP[155] = new ETDDTEntry(1968.25000, 2439947.500000, 38.526);
            etddtTmP[156] = new ETDDTEntry(1968.50000, 2440038.500000, 38.760);
            etddtTmP[157] = new ETDDTEntry(1968.75000, 2440130.500000, 39.000);
            etddtTmP[158] = new ETDDTEntry(1969.00000, 2440222.500000, 39.238);
            etddtTmP[159] = new ETDDTEntry(1969.25000, 2440312.500000, 39.472);
            etddtTmP[160] = new ETDDTEntry(1969.50000, 2440403.500000, 39.707);
            etddtTmP[161] = new ETDDTEntry(1969.75000, 2440495.500000, 39.946);
            etddtTmP[162] = new ETDDTEntry(1970.00000, 2440587.500000, 40.185);
            etddtTmP[163] = new ETDDTEntry(1970.25000, 2440677.500000, 40.420);
            etddtTmP[164] = new ETDDTEntry(1970.50000, 2440768.500000, 40.654);
            etddtTmP[165] = new ETDDTEntry(1970.75000, 2440860.500000, 40.892);
            etddtTmP[166] = new ETDDTEntry(1971.00000, 2440952.500000, 41.131);
            etddtTmP[167] = new ETDDTEntry(1971.08500, 2440983.500000, 41.211);
            etddtTmP[168] = new ETDDTEntry(1971.16200, 2441011.500000, 41.284);
            etddtTmP[169] = new ETDDTEntry(1971.24700, 2441042.500000, 41.364);
            etddtTmP[170] = new ETDDTEntry(1971.32900, 2441072.500000, 41.442);
            etddtTmP[171] = new ETDDTEntry(1971.41400, 2441103.500000, 41.522);
            etddtTmP[172] = new ETDDTEntry(1971.49600, 2441133.500000, 41.600);
            etddtTmP[173] = new ETDDTEntry(1971.58100, 2441164.500000, 41.680);
            etddtTmP[174] = new ETDDTEntry(1971.66600, 2441195.500000, 41.761);
            etddtTmP[175] = new ETDDTEntry(1971.74800, 2441225.500000, 41.838);
            etddtTmP[176] = new ETDDTEntry(1971.83300, 2441256.500000, 41.919);
            etddtTmP[177] = new ETDDTEntry(1971.91500, 2441286.500000, 41.996);
            etddtTmP[178] = new ETDDTEntry(1971.99999, 2441317.499999, 42.184);
            etddtTmP[179] = new ETDDTEntry(1972.00000, 2441317.500000, 42.184);
            etddtTmP[180] = new ETDDTEntry(1972.49999, 2441499.499999, 42.184);
            etddtTmP[181] = new ETDDTEntry(1972.50000, 2441499.500000, 43.184);
            etddtTmP[182] = new ETDDTEntry(1972.99999, 2441683.499999, 43.184);
            etddtTmP[183] = new ETDDTEntry(1973.00000, 2441683.500000, 44.184);
            etddtTmP[184] = new ETDDTEntry(1973.99999, 2442048.499999, 44.184);
            etddtTmP[185] = new ETDDTEntry(1974.00000, 2442048.500000, 45.184);
            etddtTmP[186] = new ETDDTEntry(1974.99999, 2442413.499999, 45.184);
            etddtTmP[187] = new ETDDTEntry(1975.00000, 2442413.500000, 46.184);
            etddtTmP[188] = new ETDDTEntry(1975.99999, 2442778.499999, 46.184);
            etddtTmP[189] = new ETDDTEntry(1976.00000, 2442778.500000, 47.184);
            etddtTmP[190] = new ETDDTEntry(1976.99999, 2443144.499999, 47.184);
            etddtTmP[191] = new ETDDTEntry(1977.00000, 2443144.500000, 48.184);
            etddtTmP[192] = new ETDDTEntry(1977.99999, 2443509.499999, 48.184);
            etddtTmP[193] = new ETDDTEntry(1978.00000, 2443509.500000, 49.184);
            etddtTmP[194] = new ETDDTEntry(1978.99999, 2443874.499999, 49.184);
            etddtTmP[195] = new ETDDTEntry(1979.00000, 2443874.500000, 50.184);
            etddtTmP[196] = new ETDDTEntry(1979.99999, 2444239.499999, 50.184);
            etddtTmP[197] = new ETDDTEntry(1980.00000, 2444239.500000, 51.184);
            etddtTmP[198] = new ETDDTEntry(1981.49999, 2444786.499999, 51.184);
            etddtTmP[199] = new ETDDTEntry(1981.50000, 2444786.500000, 52.184);
            etddtTmP[200] = new ETDDTEntry(1982.49999, 2445151.499999, 52.184);
            etddtTmP[201] = new ETDDTEntry(1982.50000, 2445151.500000, 53.184);
            etddtTmP[202] = new ETDDTEntry(1983.49999, 2445516.499999, 53.184);
            etddtTmP[203] = new ETDDTEntry(1983.50000, 2445516.500000, 54.184);

            int j = 204;
            for (int i = startIndexLeapSecond; i < leapSecond.leapSecond.Length; i++)
            {
                double year = leapSecond.leapSecond[i].Time.Year;
                if (leapSecond.leapSecond[i].Time.Month == 7)
                    year += 0.5;

                double jd = leapSecond.leapSecond[i].MJD + 2400000.5f;

                DateTime dt = TimeConversion.JulDay2Time(jd);
                etddtTmP[j] = new ETDDTEntry(year - 0.00001, jd - 0.000001, leapSecond.leapSecond[i - 1].TAI_UTC + 32.184);
                etddtTmP[j + 1] = new ETDDTEntry(year, jd, leapSecond.leapSecond[i].TAI_UTC + 32.184);
                j += 2;
            }
            return etddtTmP;
        }

        #region Eterna orignal output
        /// <summary>
        /// Write the data for DDT in a file.
        /// </summary>
        /// <param name="path">Path for the file.</param>
        public void writeDDT(string path)
        {
            StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create));

            writer.Write(string.Format(FormatStringHeaderDDT, DateTime.Now.ToString("yyyy.MM.dd")));

            for (int i = 0; i < etddt.Length; i++)
                writer.WriteLine(string.Format(Constants.NumberFormatEN,
                                               "{0,15:0.00000}{1,15:0.000000}{2,10:0.000}", etddt[i].Year, etddt[i].JD, etddt[i].DDT));
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Header for writing 'etddt.dat'
        /// </summary>
        internal static string FormatStringHeaderDDT = "File:     ETDDT.DAT" + Environment.NewLine +
                                                       "Status:   {0}" + Environment.NewLine +
                                                       "Contents: Table to interpolate DDT = difference ET - UTC  resp." + Environment.NewLine +
                                                       "          TDT - UTC in seconds from 1820.5 until 2002 June." + Environment.NewLine +
                                                       "          For epochs less 1820.5,  DDT is not defined within this table." + Environment.NewLine + Environment.NewLine +
                                                       "          From 1820.0 until 1962.0, DDT has been taken from" + Environment.NewLine +
                                                       "          The Astronomical Almanach for the Year 1987, Table K9," + Environment.NewLine +
                                                       "          Washington 1986. After 1962.0, DDT has been taken from" + Environment.NewLine +
                                                       "          Bulletins of Bureau International de l'Heure and International" + Environment.NewLine +
                                                       "          Earth Rotation Service, Paris." + Environment.NewLine + Environment.NewLine +
                                                       "          ET  is Ephemeris Time, was in use in dynamical theories from 1960" + Environment.NewLine +
                                                       "              ... 1983. Has been replaced at 1.1.1984 by TDT and TDB." + Environment.NewLine +
                                                       "          TDT is Terrestrial Dynamical Time (the true atomic clock, which" + Environment.NewLine +
                                                       "              is placed at the Earth), which replaces ET after 1984.0." + Environment.NewLine +
                                                       "              TDT = TAI + 32.184 s. TDT is used as time scale of ephemeris" + Environment.NewLine +
                                                       "              for observations from the Earth's surface." + Environment.NewLine +
                                                       "          TDB is Dynamic Barycentric Time (the true atomic clock, which is" + Environment.NewLine +
                                                       "              place in the barycenter of the solar system). TDB is used" + Environment.NewLine +
                                                       "              as time scale of ephemeris referred to the barycentre of" + Environment.NewLine +
                                                       "              the solar system." + Environment.NewLine +
                                                       "          UTC is Universal Time Coordinated, as broadcasted by radio or" + Environment.NewLine +
                                                       "              GPS satellites." + Environment.NewLine + Environment.NewLine +
                                                       "     year        Julian date      DDT" + Environment.NewLine +
                                                       "C******************************************************************************" + Environment.NewLine;
        #endregion

        /// <summary>
        /// Serialization of the ETDDT class by using the properties path.
        /// </summary>
        /// <param name="etddt"> object ETDDT</param>
        public static void serialisieren(TAI_UTC etddt, string path)
        {
            XmlSerializer s = new XmlSerializer(typeof(TAI_UTC));
            TextWriter w = new StreamWriter(path);
            s.Serialize(w, etddt);
            w.Close();
        }

        /// <summary>
        /// Deserialization of the ETDDT class by using the properties path.
        /// </summary>
        /// <returns>object ETDDT</returns>
        public static TAI_UTC deserialisieren(string path)
        {
            TAI_UTC etddt;
            XmlSerializer s = new XmlSerializer(typeof(TAI_UTC));
            TextReader r = new StreamReader(path);
            etddt = (TAI_UTC)s.Deserialize(r);
            r.Close();
            return etddt;
        }
    }
}