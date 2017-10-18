using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Script.Serialization;


namespace AcuLink_Bridge_Reader_CSharp
{
    public partial class frmMain : Form
    {
        string appBuild = "2017.05.06.1456";

        bool debugDoNotPostRainData = false;
        int debugAdjustDateTimeTimeMinues = 0;

        DataTable dtRainData = new DataTable();
        DataTable dtWindData = new DataTable();
        decimal cumulRainDay = -1;        
        string waitMessage = "Waiting for bridge data. If this takes more than 5 minutes, please check whether the correct network device is selected." 
            + System.Environment.NewLine;
        int noSensorDataIterations = 0;
        decimal windGust;
        double dewpoint;
        decimal windDegrees;
        decimal windspeed = 0;
        decimal temperature;
        decimal soilTemperature;
        decimal humidity;
        decimal soilHumidity;
        double pressure;
        int signal;
        int signalFails;
        string battery;
        string sensorId;
        string sensorType;
        bool backgroundWorkerRestart;
        char padZeroChar = '0';
        string paddedTemp;
        bool postDelayedCwopDataNow = true;
        bool postDelayedAwDataNow = true;
        int errorCount = 0;
        string NetworkDebugLog = "Debug_Log.txt";
        string rawpacket;
        string aculinkData;
        bool aculinkDataBegin = false;
        bool aculinkDataEnd = false;
        decimal rainLast24HoursValue = -1;
        decimal rainLast60MinutesValue = 0;
        IList<LivePacketDevice> allDevices;
        string timeAcuriteRaw = "";
        string currentAcuriteTimeOnly;
        string currentAcuriteHour;
        string currentAcuriteMinutes;
        string currentAcuriteSeconds;
        DateTime currentAcuriteDateTime;
        string watingForData = "";
        DateTime localDateTime = DateTime.Now;
        
        // For rain data debugging purposes        
        decimal debugRainLast60MinutesValueOld;
        decimal debugCumulRainDayOld;
        decimal debugRainLast24HoursValueOld;

        PacketCommunicator communicator;
                       
        // Callback function invoked by Pcap.Net for every incoming packet
        private void PacketHandler(Packet packet)
        {
            if (!BackgroundWorker1.CancellationPending == true)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() => pbarProgressBar1.Visible = true));

                    using (BerkeleyPacketFilter filter = communicator.CreateFilter("tcp port 80"))
                    {
                        // Set the filter
                        communicator.SetFilter(filter);
                    }
                    
                    if (Encoding.ASCII.GetString(packet.Ethernet.IpV4.Payload.ToMemoryStream().ToArray()).ToString() != null)
                    {
                        // reassemble fragmented packet
                        try
                        {
                            rawpacket = Encoding.ASCII.GetString(packet.Ethernet.IpV4.Tcp.Payload.ToMemoryStream().ToArray());
                        }
                        catch (Exception)
                        {
                            rawpacket = "";
                        }

                        //if (Properties.Settings.Default.debugMode == true)
                        //{
                        //    //this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  " + localDateTime + " Raw network packet: " + rawpacket.ToString() + System.Environment.NewLine + txtOutput.Text));
                        //    writeToDebugFile(localDateTime + "\t" + "DEBUG: Raw network packet data. " + rawpacket.ToString() + System.Environment.NewLine);
                        //}

                        localDateTime = DateTime.Now.AddMinutes(debugAdjustDateTimeTimeMinues);

                        if (rawpacket.ToString().IndexOf("{\"localtime\":") >= 0)
                        {
                            timeAcuriteRaw = rawpacket.ToString();
                            timeAcuriteRaw = timeAcuriteRaw.Substring(rawpacket.ToString().IndexOf("{\"localtime\":")).ToString();

                            if (timeAcuriteRaw.Length >= 22)
                            {                                
                                currentAcuriteTimeOnly = DateTime.Parse(timeAcuriteRaw.Substring(14, 8))
                                    .AddMinutes(debugAdjustDateTimeTimeMinues).ToString("HH:mm:ss");
                                currentAcuriteHour = currentAcuriteTimeOnly.Substring(0, 2);
                                currentAcuriteMinutes = currentAcuriteTimeOnly.Substring(3, 2);
                                currentAcuriteSeconds = currentAcuriteTimeOnly.Substring(6, 2);

                                // Acu-Rite reports time, but not date, so we have to make assumptions about the date and handle instances when
                                // your computer's clock doesn't match the server time exactly.
                                if (int.Parse(currentAcuriteHour) >= 23)
                                {
                                    if (localDateTime.Hour <= 3)
                                    {
                                        currentAcuriteDateTime = DateTime.Parse(
                                            localDateTime.Date.AddDays(-1).ToString("yyyy-MM-dd") + "T" +
                                            currentAcuriteHour + ":" +
                                            currentAcuriteMinutes + ":" +
                                            currentAcuriteSeconds);
                                    }
                                    else
                                    {
                                        currentAcuriteDateTime = DateTime.Parse(
                                            localDateTime.Date.ToString("yyyy-MM-dd") + "T" +
                                            currentAcuriteHour + ":" +
                                            currentAcuriteMinutes + ":" +
                                            currentAcuriteSeconds);
                                    }
                                }
                                else if (int.Parse(currentAcuriteHour) <= 3)
                                {
                                    if (localDateTime.Hour >= 23)
                                    {
                                        currentAcuriteDateTime = DateTime.Parse(
                                            localDateTime.Date.AddDays(1).ToString("yyyy-MM-dd") + "T" +
                                            currentAcuriteHour + ":" +
                                            currentAcuriteMinutes + ":" +
                                            currentAcuriteSeconds);
                                    }
                                    else
                                    {
                                        currentAcuriteDateTime = DateTime.Parse(
                                            localDateTime.Date.ToString("yyyy-MM-dd") + "T" +
                                            currentAcuriteHour + ":" +
                                            currentAcuriteMinutes + ":" +
                                            currentAcuriteSeconds);
                                    }
                                }
                                else
                                {
                                    currentAcuriteDateTime = DateTime.Parse(
                                        localDateTime.Date.ToString("yyyy-MM-dd") + "T" +
                                        currentAcuriteHour + ":" +
                                        currentAcuriteMinutes + ":" +
                                        currentAcuriteSeconds);
                                }

                                this.Invoke(new MethodInvoker(() => txtAcuriteTime.Text = currentAcuriteDateTime.ToString()));
                            }
                        }

                        if (rawpacket.ToLower().IndexOf("&id=24c86") == 0 && rawpacket.ToLower().IndexOf("&rssi=") >= 0)
                        {
                            aculinkData = rawpacket.ToString();
                            aculinkDataBegin = true;
                            aculinkDataEnd = true;
                            if (Properties.Settings.Default.debugMode == true)
                            {
                                //this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  found complete Acu-Rite string." + System.Environment.NewLine + txtOutput.Text));
                                writeToDebugFile(localDateTime + "\t" + "DEBUG - found complete Acu-Rite string." + System.Environment.NewLine);
                            }
                        }
                        else if (rawpacket.ToLower().IndexOf("&id=24c86") == 0)
                        {
                            // Beginning of desired data                        
                            aculinkData = rawpacket.ToString();
                            aculinkDataBegin = true;

                            if (Properties.Settings.Default.debugMode == true)
                            {
                                //this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  found beginning of Acu-Rite string." + System.Environment.NewLine + txtOutput.Text));
                                writeToDebugFile(localDateTime + "\t" + "DEBUG - found beginning of Acu-Rite string." + System.Environment.NewLine);
                            }
                        }
                        else if (rawpacket.ToLower().IndexOf("&rssi=") >= 0 && aculinkDataBegin)
                        {
                            // End of desired data
                            aculinkData = aculinkData + rawpacket.ToString();
                            aculinkDataEnd = true;
                            if (Properties.Settings.Default.debugMode == true)
                            {
                                //this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  found end of Acu-Rite string." + System.Environment.NewLine + txtOutput.Text));
                                writeToDebugFile(localDateTime + "\t" + "DEBUG - found end of Acu-Rite string." + System.Environment.NewLine);
                            }
                        } 
                        else if (aculinkDataBegin && rawpacket.ToLower().IndexOf("=") >= 0)
                        {
                            aculinkData = aculinkData + rawpacket.ToString();
                            if (Properties.Settings.Default.debugMode == true)
                            {
                                //this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  found middle of Acu-Rite string." + System.Environment.NewLine + txtOutput.Text));
                                writeToDebugFile(localDateTime + "\t" + "DEBUG - found middle of Acu-Rite string." + System.Environment.NewLine);
                            }
                        }

                        if (aculinkDataBegin && aculinkDataEnd)
                        {
                            //MessageBox.Show("Debug 4");
                            if (Properties.Settings.Default.debugMode == true)
                            {
                                this.Invoke(new MethodInvoker(() => txtOutput.Text = "DEBUG - " + localDateTime + "  Acu-Rite Output: " + aculinkData.ToString() + System.Environment.NewLine + txtOutput.Text));
                                writeToDebugFile(localDateTime + "\t" + "DEBUG - Acu-Rite Output: " + aculinkData.ToString() + System.Environment.NewLine);
                            }                               

                            Match Match;

                            if (Properties.Settings.Default.filterOnSensorId.ToString().Length > 0)
                            {
                                Match = Regex.Match(aculinkData.ToLower(), "sensor=", RegexOptions.Singleline);

                                if (Match.Success)
                                {
                                    Match = Regex.Match(aculinkData.ToLower(), Properties.Settings.Default.filterOnSensorId.ToString().TrimStart('0'), RegexOptions.Singleline);
                                }  
                            }
                            else if (Properties.Settings.Default.sensorType.ToString().Length > 0)
                            {
                                Match = Regex.Match(aculinkData.ToLower(), "mt=" + Properties.Settings.Default.sensorType.ToString().ToLower(), RegexOptions.Singleline);
                            }
                            else
                            {
                                Match = Regex.Match(aculinkData.ToLower(), "mt=", RegexOptions.Singleline);
                            }

                            string[] bridgeDataArray;
                            double pressureOffset = Properties.Settings.Default.pressureOffset;
                            decimal tempOffset = Properties.Settings.Default.tempOffset;
                            decimal soilTempOffset = Properties.Settings.Default.soilTempOffset;
                            decimal windOffsetPct = Properties.Settings.Default.windOffsetPct;
                            decimal humidityOffset = Properties.Settings.Default.humidityOffset;

                            if (Match.Success)
                            {
                                this.Invoke(new MethodInvoker(() => pbarProgressBar1.Value = progressBarValue(10)));

                                this.Invoke(new MethodInvoker(() => txtOutput.Text = txtOutput.Text.Replace(waitMessage, "")));

                                bridgeDataArray = aculinkData.Split('&');

                                foreach (string element in bridgeDataArray)
                                {
                                    if (element.IndexOf("sensor=") >= 0)
                                    {
                                        sensorId = element.Substring(7);
                                        sensorId = sensorId.TrimStart('0'); // remove leading zeros from Sensor ID
                                    }

                                    if (element.IndexOf("mt=") == 0)
                                    {
                                        sensorType = element.Substring(3, 5).ToLower();
                                    }

                                    if (Properties.Settings.Default.filterOnSensorId.ToString().Length == 0 || sensorId == Properties.Settings.Default.filterOnSensorId.ToString().TrimStart('0'))
                                    {
                                        if (element.IndexOf("winddir=") == 0
                                            && (Properties.Settings.Default.sensorTypeWind == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeWind) != -1)
                                            && (Properties.Settings.Default.sensorIdWind == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdWind.TrimStart('0')) != -1))
                                        {
                                            windDegrees = Convert.ToInt16(element.Substring(8));
                                        }

                                        if (element.IndexOf("windspeedmph=") == 0
                                            && (Properties.Settings.Default.sensorTypeWind == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeWind) != -1)
                                            && (Properties.Settings.Default.sensorIdWind == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdWind.TrimStart('0')) != -1))
                                        {
                                            windspeed = Math.Round(decimal.Parse(element.Substring(13)) * windOffsetPct / 100, 0);

                                            // Delete wind gust data table rows older than 10 minutes
                                            DataRow[] rowsWind = dtWindData.Select("time_local <= #" + localDateTime.AddMinutes(-10) + "#");
                                            foreach (DataRow row in rowsWind)
                                            {
                                                row.Delete();
                                            }

                                            dtWindData.TableName = "kevin";
                                            dtWindData.WriteXml("WindData.xml", true);

                                            GC.Collect();

                                            dtWindData.Rows.Add(new object[] {
                                                localDateTime,
                                                windspeed
                                            });

                                            windGust = calcWindGust();
                                        }

                                        if (element.IndexOf("tempf=") == 0)
                                        {
                                            if ((Properties.Settings.Default.sensorTypeTemp == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeTemp) != -1)
                                                && (Properties.Settings.Default.sensorIdTemp == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdTemp.TrimStart('0')) != -1))
                                            {
                                                temperature = Math.Round(decimal.Parse(element.Substring(6)) + tempOffset, 1);
                                            }

                                            if (Properties.Settings.Default.sensorTypeSoil.Length > 0 || Properties.Settings.Default.sensorIdSoil.Length > 0)
                                            {
                                                if ((Properties.Settings.Default.sensorTypeSoil == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeSoil) != -1)
                                                    && (Properties.Settings.Default.sensorIdSoil == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdSoil.TrimStart('0')) != -1))
                                                {
                                                    soilTemperature = Math.Round(decimal.Parse(element.Substring(6)) + soilTempOffset, 1);
                                                }
                                            }

                                            if (humidity > 0)
                                            {                                    
                                                dewpoint = ((((Convert.ToDouble(temperature) - 32) / 1.8) - (14.55 + 0.114 * ((Convert.ToDouble(temperature) - 32) / 1.8)) *
                                                    (1 - (0.01 * Convert.ToDouble(humidity))) - Math.Pow(((2.5 + 0.007 * ((Convert.ToDouble(temperature) - 32) / 1.8)) *
                                                    (1 - (0.01 * Convert.ToDouble(humidity)))), 3) - (15.9 + 0.117 * ((Convert.ToDouble(temperature) - 32) / 1.8)) *
                                                    Math.Pow((1 - (0.01 * Convert.ToDouble(humidity))), 14)) * 1.8) + 32;

                                                dewpoint = Math.Round(dewpoint, 1);
                                            }
                                        }

                                        if (element.IndexOf("humidity=") == 0)
                                        {
                                            if ((Properties.Settings.Default.sensorTypeHumidity == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeHumidity) != -1)
                                                && (Properties.Settings.Default.sensorIdHumidity == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdHumidity.TrimStart('0')) != -1))
                                            {
                                                humidity = decimal.Parse(element.Substring(9)) + humidityOffset;
                                            }

                                            if (Properties.Settings.Default.sensorTypeSoil.Length > 0 || Properties.Settings.Default.sensorIdSoil.Length > 0)
                                            {
                                                if ((Properties.Settings.Default.sensorTypeSoil == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeSoil) != -1)
                                                    && (Properties.Settings.Default.sensorIdSoil == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdSoil.TrimStart('0')) != -1))
                                                {
                                                    soilHumidity = int.Parse(element.Substring(9));
                                                }
                                            }
                                        }

                                        // The below value is worthless - Acu-Rite resets it on the hour.
                                        //if (element.IndexOf("rainin=") == 0
                                        //    && (Properties.Settings.Default.sensorTypeRain == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeRain) != -1)
                                        //    && (Properties.Settings.Default.sensorIdRain == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdRain.TrimStart('0')) != -1))
                                        //{
                                        //    rainHour = Math.Round(double.Parse(element.Substring(7)), 2);
                                        //}

                                        if (element.IndexOf("dailyrainin=") == 0
                                            && (Properties.Settings.Default.sensorTypeRain == "" || sensorType.IndexOf(Properties.Settings.Default.sensorTypeRain) != -1)
                                            && (Properties.Settings.Default.sensorIdRain == "" || sensorId.IndexOf(Properties.Settings.Default.sensorIdRain.TrimStart('0')) != -1))
                                        {
                                            cumulRainDay = Math.Round(decimal.Parse(element.Substring(12)), 2);

                                            if (currentAcuriteDateTime.Year > 2000)
                                            {
                                                // Append row to rain data table
                                                dtRainData.Rows.Add(new object[] {
                                                    localDateTime,
                                                    currentAcuriteDateTime,
                                                    cumulRainDay});

                                                // Delete rain data rows older than 26 hours Acu-Rite server time
                                                DataRow[] rowsRain = dtRainData.Select(
                                                    "time_acurite <= #" + currentAcuriteDateTime.AddHours(-26) + "#");
                                                foreach (DataRow row in rowsRain)
                                                {
                                                    row.Delete();
                                                }

                                                dtRainData.TableName = "kevin";
                                                dtRainData.WriteXml("RainData.xml", true);

                                                rainLast60MinutesValue = decimal.Round(calcRainLast60Minutes(currentAcuriteDateTime, cumulRainDay), 2, MidpointRounding.AwayFromZero);
                                                rainLast24HoursValue = decimal.Round(calcRainLast24Hours(currentAcuriteDateTime, cumulRainDay), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }

                                        if (element.IndexOf("rssi=") == 0)
                                        {
                                            signal = int.Parse(element.Substring(5, 1));
                                        }

                                        if (element.IndexOf("battery=") == 0)
                                        {
                                            battery = element.Substring(8);
                                        }
                                    }

                                    // Get and process pressure data
                                    if (element.IndexOf("baromin=") == 0)
                                    {
                                        pressure = Math.Round(Convert.ToDouble(element.Substring(8)) + pressureOffset, 2);
                                    }
                                }

                                if (txtOutput.Text.Length > 1000000) // 2016-08-30 changed to 1000000, 2015-07-15 changed from 50000 to 100000
                                {
                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = ""));
                                }

                                string wuResponse = null;
                                string wBugResponse = null;
                                string pwsResponse = null;
                                string aWeatherResponse = null;
                                string openWeatherMapResponse = null;
                                string CWOPResponse = null;
                                string veraTempResponse = null;
                                string veraHumidResponse = null;

                                // For debugging rain data issues
                                if (debugDoNotPostRainData)
                                {
                                    debugRainLast60MinutesValueOld = rainLast60MinutesValue;
                                    debugCumulRainDayOld = cumulRainDay;
                                    debugRainLast24HoursValueOld = rainLast24HoursValue;
                                }

                                watingForData = "";

                                if (pressure <= 0)
                                {
                                    watingForData = "[pressure] " + watingForData;
                                }

                                if (humidity <= 0 || (Properties.Settings.Default.sensorType.ToString() == "water" && temperature <= 0))
                                {
                                    watingForData = "[humidity and/or temperature] " + watingForData;
                                }
                                
                                if ((Properties.Settings.Default.sensorType.ToString() == "5n1" || Properties.Settings.Default.sensorTypeRain.ToString() == "5n1") 
                                    && (cumulRainDay < 0 || rainLast24HoursValue < 0))
                                {
                                    watingForData = "[cumulative rain - even if it's zero] " + watingForData;
                                }
                                
                                if (currentAcuriteDateTime.Year == 1)
                                {
                                    watingForData = "[time from AcuRite device] " + watingForData;
                                }

                                if (watingForData.Length > 0)
                                {
                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = "Waiting for the following data: " + watingForData + System.Environment.NewLine + txtOutput.Text));

                                    if (Properties.Settings.Default.debugMode == true)
                                    {
                                        writeToDebugFile(localDateTime + "\t" + "DEBUG - Waiting for the following data: " + watingForData + System.Environment.NewLine);
                                    }

                                    wuResponse = "Need more data";
                                    wBugResponse = "Need more data";
                                    pwsResponse = "Need more data";
                                    aWeatherResponse = "Need more data";
                                    openWeatherMapResponse = "Need more data";
                                    CWOPResponse = "Need more data";
                                }
                                else
                                {
                                    if (debugDoNotPostRainData)
                                    {
                                        rainLast60MinutesValue = 0;
                                        cumulRainDay = 0;
                                        rainLast24HoursValue = 0;
                                    }

                                    if (Properties.Settings.Default.postToWunderground == true)
                                    {
                                        string wundergroundUpdateString = "http://rtupdate.wunderground.com/weatherstation/updateweatherstation.php?ID=" + Properties.Settings.Default.wuStation +
                                            "&PASSWORD=" + System.Uri.EscapeUriString(Properties.Settings.Default.wuPwd) + "&dateutc=" +
                                            System.Uri.EscapeUriString(Convert.ToString(localDateTime.ToUniversalTime())) + "&winddir=" + windDegrees + "&windspeedmph=" + windspeed + "&tempf=" +
                                            temperature + "&rainin=" + rainLast60MinutesValue + "&dailyrainin=" + cumulRainDay + "&baromin=" + pressure + "&dewptf=" + dewpoint + "&humidity=" + humidity +
                                            "&softwaretype=" + "Kevins%20Acu-Rapid%916&action=updateraw&realtime=1&rtfreq=15" + "&windgustmph=" + windGust + "&indoortempf=" + temperature +
                                            "&soiltempf=" + soilTemperature + "&soilmoisture=" + soilHumidity;

                                        try
                                        {
                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to Weather Underground: " + wundergroundUpdateString + System.Environment.NewLine + txtOutput.Text));
                                            }

                                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(wundergroundUpdateString);
                                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                            Stream receiveStream = response.GetResponseStream();
                                            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                            wuResponse = readStream.ReadToEnd();

                                            readStream.Close();
                                            response.Close();

                                            if (wuResponse.IndexOf("success") != -1)
                                            {
                                                wuResponse = "ok";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            wuResponse = "";
                                            this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to Weather Underground. " + ex.Message +
                                                System.Environment.NewLine + txtOutput.Text));

                                            errorCount++;

                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                writeToDebugFile(localDateTime + "\t" + "ERROR posting data to Weather Underground. " + ex.ToString() + "  String posted: " + wundergroundUpdateString + System.Environment.NewLine);
                                            }                                            
                                        }
                                    }
                                    else
                                    {
                                        wuResponse = "off";
                                    }

                                    if (Properties.Settings.Default.postToWeatherBug == true)
                                    {
                                        string weatherBugUpdateString = "http://data.backyard2.weatherbug.com/data/livedata.aspx?ID=" + Properties.Settings.Default.wbPub + "&Key=" +
                                            Properties.Settings.Default.wbPwd + "&num=" + Properties.Settings.Default.wbStation + "&dateutc=" +
                                            System.Uri.EscapeUriString(Convert.ToString(localDateTime.ToUniversalTime())) + "&winddir=" + windDegrees + "&windspeedmph=" + windspeed +
                                            "&windgustmph=" + windGust + "&tempf=" + temperature + "&rainin=" + rainLast60MinutesValue + "&dailyrainin=" + cumulRainDay + "&baromin=" + pressure +
                                            "&dewptf=" + dewpoint + "&humidity=" + humidity + "&softwaretype=Kevin%27s%20My_Acu-Rite";
                                        try
                                        {
                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to WeatherBug: " + weatherBugUpdateString + System.Environment.NewLine + txtOutput.Text));
                                            }

                                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(weatherBugUpdateString);
                                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                            Stream receiveStream = response.GetResponseStream();
                                            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                            wBugResponse = readStream.ReadToEnd();

                                            readStream.Close();
                                            response.Close();

                                            if (wBugResponse == "Successfully Received QueryString Data")
                                            {
                                                wBugResponse = "ok";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            wBugResponse = "";
                                            this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to Weatherbug. " + ex.Message +
                                                System.Environment.NewLine + txtOutput.Text));
                                            errorCount++;

                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                writeToDebugFile(localDateTime + "\t" + "ERROR posting data to Weatherbug. " + ex.ToString() + "  String posted: " + weatherBugUpdateString  + System.Environment.NewLine);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        wBugResponse = "off";
                                    }

                                    if (Properties.Settings.Default.postToPws == true)
                                    {
                                        string pwsUpdateString = "http://www.pwsweather.com/pwsupdate/pwsupdate.php?ID=" + Properties.Settings.Default.pwsStation + "&PASSWORD=" +
                                            Properties.Settings.Default.pwsPwd + "&dateutc=" +
                                            localDateTime.ToUniversalTime().ToString("yyyy-MM-dd+HH\\%3Amm\\%3Ass") + "&winddir=" + windDegrees + "&windspeedmph=" + windspeed +
                                            "&windgustmph=" + windGust + "&tempf=" + temperature + "&rainin=" + rainLast60MinutesValue + "&dailyrainin=" + cumulRainDay + "&baromin=" + pressure +
                                            "&dewptf=" + dewpoint + "&humidity=" + humidity + "&action=updateraw";

                                        try
                                        {

                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to PWSWeather: " + pwsUpdateString + System.Environment.NewLine + txtOutput.Text));
                                            }

                                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pwsUpdateString);
                                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                            Stream receiveStream = response.GetResponseStream();
                                            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                            pwsResponse = readStream.ReadToEnd();

                                            readStream.Close();
                                            response.Close();

                                            if (pwsResponse.IndexOf("Data Logged and posted in METAR mirror") >= 0)
                                            {
                                                pwsResponse = "ok";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            pwsResponse = "";
                                            this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to PWSweather. " + ex.Message +
                                                System.Environment.NewLine + txtOutput.Text));
                                            errorCount++;

                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                writeToDebugFile(localDateTime + "\t" + "ERROR posting data to PWSweather. " + ex.ToString() + "  String posted: " + pwsUpdateString + System.Environment.NewLine);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        pwsResponse = "off";
                                    }

                                    if (Properties.Settings.Default.postToAWeather == true)
                                    {

                                        if (postDelayedAwDataNow == true)
                                        {
                                            // Support for Anything Weather is on-hold. They recently told me to test by posting HTTP to an FTP site...
                                            string aWeatherUpdateString = "http://ftp.anythingweather.com/dataUpload?username=" + Properties.Settings.Default.awStation +
                                                "&password=" + System.Uri.EscapeUriString(Properties.Settings.Default.awPwd) + "&time=" +
                                                localDateTime.ToString("yyyy\\-MM\\-dd-THH:mm:ss") + "&temp=" + temperature + "&dewPoint=" + dewpoint +
                                                "&pressure=" + pressure + "&windSpeed=" + windspeed + "&windDirection=" + windDegrees + "&rainDay=" + cumulRainDay;

                                            //string aWeatherUpdateString = "http://www.anythingweather.com/feeds/load/WXDATAPOST.ASP?username=" + Properties.Settings.Default.awStation +
                                            //    "&password=" + System.Uri.EscapeUriString(Properties.Settings.Default.awPwd) + "&version=1&WXData=" +
                                            //    localDateTime.ToString("yyyy\\%2DMM\\%2Ddd+HH:mm:ss") + "%2C" + temperature + "%2C" + dewpoint + "%2C" +
                                            //    humidity + "%2C" + pressure + "%2C" + windDegrees + "%2C" + windspeed + "%2C" + cumulRainDay + "%2C%2C%2C" + windGust +
                                            //    "%2C%0D";

                                            try
                                            {
                                                if (Properties.Settings.Default.debugMode == true)
                                                {
                                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to AWeather: " + aWeatherUpdateString + System.Environment.NewLine + txtOutput.Text));
                                                }

                                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(aWeatherUpdateString);
                                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                                Stream receiveStream = response.GetResponseStream();
                                                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                                aWeatherResponse = readStream.ReadToEnd();

                                                readStream.Close();
                                                response.Close();

                                                if (aWeatherResponse == "")
                                                {
                                                    aWeatherResponse = "ok";
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                aWeatherResponse = "";
                                                this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to Anything Weather. " + ex.Message +
                                                    System.Environment.NewLine + txtOutput.Text));
                                                errorCount++;

                                                if (Properties.Settings.Default.debugMode == true)
                                                {
                                                    writeToDebugFile(localDateTime + "\t" + "ERROR posting data to Anything Weather. " + ex.ToString() + "  String posted: " + aWeatherUpdateString + System.Environment.NewLine);
                                                }
                                            }

                                            postDelayedAwDataNow = false;
                                        }
                                        else
                                        {
                                            aWeatherResponse = "wait";
                                        }
                                    }
                                    else
                                    {
                                        aWeatherResponse = "off";
                                    }

                                    if (Properties.Settings.Default.postToOw == true)
                                    {
                                        try
                                        {
                                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.openweathermap.org/data/3.0/measurements?APPID=" +
                                                Properties.Settings.Default.owApiKey);
                
                                            request.ContentType = "application/json";
                                            request.Method = "POST";

                                            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                                            {
                                                string OwJsonData = new JavaScriptSerializer().Serialize(new
                                                {
                                                    station_id = Properties.Settings.Default.owStationId,
                                                    dt = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                                                    temperature = Math.Round((temperature - 32) * 5 / 9, 1),
                                                    wind_speed = Math.Round(windspeed * Convert.ToDecimal(0.44704),1),
                                                    wind_gust = Math.Round(windGust * Convert.ToDecimal(0.44704),1),
                                                    wind_deg = windDegrees,
                                                    pressure = Math.Round(pressure * 33.8637526, 0),
                                                    humidity = humidity,
                                                    rain_1h = decimal.ToDouble(rainLast60MinutesValue) * 25.4,
                                                    rain_24h = decimal.ToDouble(rainLast24HoursValue) * 25.4,
                                                    dew_point = Math.Round((dewpoint - 32) * 5 / 9, 1)
                                                });
                                                                                         

                                                streamWriter.Write(OwJsonData);
                                            }

                                            var response = (HttpWebResponse)request.GetResponse();
                                            using (var streamReader = new StreamReader(response.GetResponseStream()))
                                            {
                                                openWeatherMapResponse = streamReader.ReadToEnd();
                                            }
                                            
                                            if (openWeatherMapResponse.IndexOf("\"cod\":\"204\"", StringComparison.OrdinalIgnoreCase) > 0)
                                            {
                                                openWeatherMapResponse = "ok";
                                            }

                                            //if (Properties.Settings.Default.debugMode == true)
                                            //{
                                            //    this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to OpenWeatherMap: " + OwJsonData +
                                            //        System.Environment.NewLine + txtOutput.Text));
                                            //}

                                        }
                                        catch (Exception ex)
                                        {
                                            openWeatherMapResponse = "";
                                            this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to OpenWeatherMap. " + ex.Message + 
                                                System.Environment.NewLine + txtOutput.Text));
                                            errorCount++;

                                            if (Properties.Settings.Default.debugMode == true)
                                            {
                                                writeToDebugFile(localDateTime + "\t" + "ERROR posting data to OpenWeatherMap. " + ex.ToString() + System.Environment.NewLine);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        openWeatherMapResponse = "off";
                                    }

                                    if (Properties.Settings.Default.postToCw == true)
                                    {                    
                                        if (postDelayedCwopDataNow == true)
                                        {
                                            string CWOPUpdateString = "";

                                            try
                                            {
                                                DnsEndPoint cwHost = new DnsEndPoint(Properties.Settings.Default.cwHostName, 14580); // switched from port 23 to 14580
                                                Socket server = new Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                                                server.Connect(cwHost);

                                                server.Send(Encoding.ASCII.GetBytes("user " + Properties.Settings.Default.cwRegNum + " pass " + Properties.Settings.Default.cwPasscode +
                                                    " vers Kevin's My Acu-Rite\r\n"));

                                                if (temperature <= -1 && temperature >= -9)
                                                {
                                                    paddedTemp = "-0" + Math.Round(Math.Abs(temperature), 0).ToString();
                                                }
                                                else
                                                {
                                                    paddedTemp = Math.Round(temperature, 0).ToString().PadLeft(3, padZeroChar);
                                                }

                                                CWOPUpdateString = Properties.Settings.Default.cwRegNum + ">APRS,TCPIP*:@" +
                                                    localDateTime.ToUniversalTime().Day.ToString().PadLeft(2, padZeroChar) +
                                                    localDateTime.ToUniversalTime().Hour.ToString().PadLeft(2, padZeroChar) +
                                                    localDateTime.ToUniversalTime().Minute.ToString().PadLeft(2, padZeroChar) + "z" +
                                                    Properties.Settings.Default.cwLat +
                                                    "/" + Properties.Settings.Default.cwLon +
                                                    "_" + Math.Round(windDegrees, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "/" + Math.Round(windspeed, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "g" + Math.Round(windGust, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "t" + paddedTemp +
                                                    "r" + Math.Round(rainLast60MinutesValue * 100, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "p" + Math.Round(rainLast24HoursValue * 100, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "P" + Math.Round(cumulRainDay * 100, 0).ToString().PadLeft(3, padZeroChar) +
                                                    "h" + Math.Round(humidity, 0).ToString().PadLeft(2, padZeroChar) +
                                                    "b" + Math.Round(pressure * 33.8637526 * 10, 0) +
                                                    "" + Properties.Settings.Default.cwComment + "\r\n"; // 20170506 - removed 'e' prefix

                                                if (Properties.Settings.Default.debugMode == true)
                                                {
                                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = "   String posted to CWOP: " + CWOPUpdateString +
                                                        System.Environment.NewLine + txtOutput.Text));
                                                }

                                                server.Send(Encoding.ASCII.GetBytes(CWOPUpdateString));

                                                server.Shutdown(SocketShutdown.Both);
                                                server.Close();

                                                CWOPResponse = "ok";

                                            }
                                            catch (Exception ex)
                                            {
                                                CWOPResponse = "";
                                                this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR posting data to CWOP. " + ex.Message +
                                                    System.Environment.NewLine + txtOutput.Text));
                                                errorCount++;

                                                if (Properties.Settings.Default.debugMode == true)
                                                {
                                                    writeToDebugFile(localDateTime + "\t" + "ERROR posting data to CWOP. " + ex.ToString() + "  String posted: " + CWOPUpdateString + System.Environment.NewLine);
                                                }

                                            }

                                            postDelayedCwopDataNow = false;
                                        }
                                        else
                                        {
                                            CWOPResponse = "wait";
                                        }
                                    }
                                    else
                                    {
                                        CWOPResponse = "off";
                                    }

                                    if (Properties.Settings.Default.veraPostTo == true)
                                    {
                                        string veraTemperaturePostString = "";
                                        string veraHumidityPostString = "";

                                        if (Properties.Settings.Default.veraIp.Length > 0)
                                        {
                                            if (Properties.Settings.Default.veraTempDevId.Length > 0)
                                            {
                                                try
                                                {
                                                    veraTemperaturePostString = "http://" + Properties.Settings.Default.veraIp
                                                        + ":3480/data_request?id=variableset&DeviceNum=" + Properties.Settings.Default.veraTempDevId
                                                        + "&serviceId=" + Properties.Settings.Default.veraTempSvc + "&Variable="
                                                        + Properties.Settings.Default.veraTempVar + "&Value=" + soilTemperature;

                                                    if (Properties.Settings.Default.debugMode == true)
                                                    {
                                                        this.Invoke(new MethodInvoker(() => txtOutput.Text = "   Vera temperature string: " + veraTemperaturePostString 
                                                            + System.Environment.NewLine + txtOutput.Text));
                                                    }

                                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(veraTemperaturePostString);
                                                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                                    Stream receiveStream = response.GetResponseStream();
                                                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                                    veraTempResponse = readStream.ReadToEnd();

                                                    readStream.Close();
                                                    response.Close();

                                                    if (veraTempResponse == "OK")
                                                    {
                                                        veraTempResponse = "ok";
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t"
                                                       + "ERROR: Trouble posting temperature to Vera." + ex.Message.ToString()
                                                       + System.Environment.NewLine + txtOutput.Text));

                                                    errorCount++;
                                                }
                                            }
                                            else
                                            {
                                                veraTempResponse = "off";
                                            }

                                            if (Properties.Settings.Default.veraHumidDevId.Length > 0)
                                            {
                                                try
                                                {
                                                    veraHumidityPostString = "http://" + Properties.Settings.Default.veraIp
                                                        + ":3480/data_request?id=variableset&DeviceNum=" + Properties.Settings.Default.veraHumidDevId
                                                        + "&serviceId=" + Properties.Settings.Default.veraHumidSvc + "&Variable="
                                                        + Properties.Settings.Default.veraHumidVar + "&Value=" + soilHumidity;

                                                    if (Properties.Settings.Default.debugMode == true)
                                                    {
                                                        this.Invoke(new MethodInvoker(() => txtOutput.Text = "   Vera humidity string: " 
                                                            + veraHumidityPostString + System.Environment.NewLine + txtOutput.Text));
                                                    }

                                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(veraHumidityPostString);
                                                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                                    Stream receiveStream = response.GetResponseStream();
                                                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                                    veraHumidResponse = readStream.ReadToEnd();

                                                    readStream.Close();
                                                    response.Close();

                                                    if (veraHumidResponse == "OK")
                                                    {
                                                        veraHumidResponse = "ok";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t"
                                                       + "ERROR: Trouble posting humidity to Vera." + ex.Message.ToString() 
                                                       + System.Environment.NewLine + txtOutput.Text));

                                                    errorCount++;
                                                }
                                            }
                                            else
                                            {
                                                veraHumidResponse = "off";
                                            }

                                        } else
                                        {
                                            this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" 
                                               + "ERROR: Vera controller IP address not defined." + System.Environment.NewLine + txtOutput.Text));
                                        }
                                    }
                                    else
                                    {
                                        veraTempResponse = "off";
                                        veraHumidResponse = "off";
                                    }

                                    if (debugDoNotPostRainData)
                                    {
                                        rainLast60MinutesValue = debugRainLast60MinutesValueOld;
                                        cumulRainDay = debugCumulRainDayOld;
                                        rainLast24HoursValue = debugRainLast24HoursValueOld;
                                    }

                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "  " + "T:" + temperature.ToString() + " " + "H:" + humidity + " " + "WD:" +
                                        windDegrees + " " + "WS:" + windspeed.ToString() + " " + "WG:" + windGust.ToString() + " " + "RDAY:" + cumulRainDay.ToString("F") + " " + "R24:" +
                                        rainLast24HoursValue.ToString("F") + " " + "RHR:" + rainLast60MinutesValue.ToString("F") + " " + "BAR:" + pressure.ToString() + " " + "DEW:" + dewpoint.ToString() + " " + "SI: " +
                                        sensorId + "  ST: " + sensorType + " " + "WU: " + wuResponse + " " + "WB: " + wBugResponse + " " + "PWS: " + pwsResponse + " " + "AW: " +
                                        aWeatherResponse + " " + "OW: " + openWeatherMapResponse + " " + "CW: " + CWOPResponse + " " + "VT: " + veraTempResponse + " " + "VH: " +
                                        veraHumidResponse + System.Environment.NewLine + txtOutput.Text));

                                    if (Properties.Settings.Default.writeToCSV == true)
                                    {
                                        string newFileName;

                                        if (Properties.Settings.Default.csvFilePath == "")
                                        {
                                            newFileName = "weather.csv";
                                        }
                                        else
                                        {
                                            newFileName = Properties.Settings.Default.csvFilePath;
                                        }

                                        string weatherData = localDateTime + "," + temperature + "," + humidity + "," + windspeed + "," + windGust + "," + windDegrees + "," +
                                                pressure + "," + rainLast60MinutesValue.ToString("F") + "," + cumulRainDay.ToString("F") + "," + dewpoint + "," + sensorType + "," + sensorId +
                                                "," + rainLast24HoursValue.ToString("F") + Environment.NewLine;

                                        if (!File.Exists(newFileName))
                                        {
                                            string fileHeader = "Local Time" + "," + "Temperature" + "," + "Humidity" + "," + "Wind MPH" + "," + "Wind Gust" + "," + "Wind Dir" + "," +
                                                "Pressure" + "," + "Rain Hour" + "," + "Rain Day" + "," + "Dewpoint" + "," + "Sensor Type" + "," + "Sensor ID" + "," + "Rain 24 Hrs" + Environment.NewLine;

                                            File.WriteAllText(newFileName, fileHeader);
                                        }

                                        File.AppendAllText(newFileName, weatherData);
                                    }
                                }



                                this.Invoke(new MethodInvoker(() => txtSensor.Text = sensorType.ToString()));

                                if (sensorType == "pressure")
                                {
                                    this.Invoke(new MethodInvoker(() => txtSensorId.Text = ""));

                                    this.Invoke(new MethodInvoker(() => txtBattery.Text = ""));
                                    this.Invoke(new MethodInvoker(() => txtBattery.BackColor = Control.DefaultBackColor));

                                    this.Invoke(new MethodInvoker(() => txtSignal.Text = ""));
                                    this.Invoke(new MethodInvoker(() => txtSignal.BackColor = Control.DefaultBackColor));
                                }
                                else
                                {
                                    this.Invoke(new MethodInvoker(() => txtSensorId.Text = sensorId.ToString()));

                                    this.Invoke(new MethodInvoker(() => txtBattery.Text = battery));

                                    if (battery == "normal")
                                    {
                                        this.Invoke(new MethodInvoker(() => txtBattery.BackColor = Control.DefaultBackColor));
                                    }
                                    else
                                    {
                                        this.Invoke(new MethodInvoker(() => txtBattery.BackColor = Color.FromArgb(247, 247, 124)));
                                    }

                                    this.Invoke(new MethodInvoker(() => txtSignal.Text = signal.ToString()));

                                    switch (signal)
                                    {
                                        case 0:
                                            this.Invoke(new MethodInvoker(() => txtSignal.BackColor = Color.Red));
                                            break;
                                        case 1:
                                            this.Invoke(new MethodInvoker(() => txtSignal.BackColor = Color.FromArgb(247, 247, 124)));
                                            break;
                                        case 2:
                                        case 3:
                                        case 4:
                                            this.Invoke(new MethodInvoker(() => txtSignal.BackColor = Control.DefaultBackColor));
                                            break;
                                    }
                                }
                                
                                this.Invoke(new MethodInvoker(() => txtLastUpdated.Text = localDateTime.ToString()));                                

                                this.Invoke(new MethodInvoker(() => txtErrorCount.Text = errorCount.ToString()));

                                if (noSensorDataIterations >= 5)
                                {
                                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "INFO: Not receiving data from sensor. Low batteries? Too far away?" +
                                        System.Environment.NewLine + txtOutput.Text));
                                    battery = "unknown";
                                    signal = 0;
                                    signalFails += 1;
                                }

                                this.Invoke(new MethodInvoker(() => txtSignalFails.Text = signalFails.ToString()));
                            }

                            aculinkDataBegin = false;
                            aculinkDataEnd = false;
                        }
                    }                    

                }
                catch (NullReferenceException ex)
                {
                    //Keep going
                    if (Properties.Settings.Default.debugMode == true)
                    {
                        this.Invoke(new MethodInvoker(() => txtOutput.Text = "ERROR1: " + ex.Message + System.Environment.NewLine + txtOutput.Text));
                    }

                    if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                    {
                        // Get stack trace for the exception with source file information
                        var st = new StackTrace(ex, true);
                        // Get the top stack frame
                        var frame = st.GetFrame(0);
                        // Get the line number from the stack frame
                        var line = frame.GetFileLineNumber();

                        writeToDebugFile(localDateTime + "\t" + "ERROR1: " + ex.ToString() + "   frame=" + frame.ToString() + System.Environment.NewLine);
                    }
                                       
                    //errorCount++;
                }
            catch (Exception ex)
            {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();

                    if (Properties.Settings.Default.debugMode == true)
                {
                    this.Invoke(new MethodInvoker(() => txtOutput.Text = "ERROR2: " + ex.Message + System.Environment.NewLine + txtOutput.Text));
                }

                if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                {
                    writeToDebugFile(localDateTime + "\t" + "ERROR2: " + ex.ToString() + "   frame=" + frame.ToString() + System.Environment.NewLine);
                }

                    //errorCount++;
                }
        }
            else
            {
                communicator.Break();
                BackgroundWorker1.CancelAsync();
                BackgroundWorker1.Dispose();
            }
        }

        public frmMain()
        {
           //if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            //{
            //    MessageBox.Show("The weather app is already running.");
            //    Close();
            //}

            // This call is required by the designer.
            InitializeComponent();
            this.Text = "Kevin's My AcuRite smartHUB Reader - Build:" + appBuild;

            dtWindData.Columns.Add("time_local", typeof(DateTime));
            dtWindData.Columns.Add("windSpeed", typeof(double));

            dtRainData.Columns.Add("time_local", typeof(DateTime));
            dtRainData.Columns.Add("time_acurite", typeof(DateTime));
            dtRainData.Columns.Add("rain", typeof(double));

            string loadPreviousWindRainDataInfo = loadWindAndRainData();

            if (loadPreviousWindRainDataInfo.Length > 0)
            {
                txtOutput.Text = loadPreviousWindRainDataInfo + System.Environment.NewLine + txtOutput.Text;
            }

            BackgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HandleWorkerCompleted); // Must have this for the event to fire!

            refreshStationInfoDisplay();

            timerCWOP.Interval = Properties.Settings.Default.cwUpdateMinutes * 60000;

            if (Properties.Settings.Default.networkDevice.Length > 0)
            {
                if (BackgroundWorker1.IsBusy == false)
                {
                    BackgroundWorker1.RunWorkerAsync();
                    timer1.Start();
                    timerCWOP.Start();
                    timerAW.Start();
                }
            }
        }
             

        private void HandleWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (backgroundWorkerRestart == true)
            {
                BackgroundWorker1.Dispose();

                GC.Collect();

                //this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "DEBUG: Resuming." + System.Environment.NewLine + txtOutput.Text));
                BackgroundWorker1.RunWorkerAsync();
            }

        }

        private decimal calcWindGust()
        {
            try
            {
                decimal windGustDec =
                        Convert.ToDecimal((from myRow in dtWindData.AsEnumerable()
                         where myRow.Field<DateTime>("time_local") > localDateTime.AddMinutes(-10)
                         select myRow.Field<double>("windSpeed")).DefaultIfEmpty().Max());
              
                return windGustDec;
            }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                {
                    writeToDebugFile(localDateTime + "\t" + "ERROR processing calcWindGust: " + ex.ToString() + System.Environment.NewLine);
                }

                this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR processing wind gust: " + ex.Message +
                   System.Environment.NewLine + txtOutput.Text));

                return (0);
            }
        }
        
        private decimal calcRainLast60Minutes(DateTime inputCurrentAcuriteDateTime, decimal currentRainTotalToday = 0) 
        {
            try
            {
                decimal minCumulRainLastHrYesterdayPortionDec = 0;
                decimal maxCumulRainLastHrYesterdayPortionDec = 0;
                decimal totalRainLastHrYesterdayPortionDec = 0;
                decimal totalRainLastHrTodayPortionDec = 0; 
                decimal rainInTheLast60Minutes = 0;

                if (inputCurrentAcuriteDateTime.Hour < 1)
                {
                    // Minimum cumulative rain 1 hour ago yesterday portion
                    minCumulRainLastHrYesterdayPortionDec =
                        Convert.ToDecimal(
                            (from myRow in dtRainData.AsEnumerable()
                             where myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.AddHours(-1)
                             && myRow.Field<DateTime>("time_acurite") < inputCurrentAcuriteDateTime.Date
                             select myRow.Field<double>("rain")).DefaultIfEmpty().Min());

                    // Maximum cumulative rain 1 hour ago yesterday portion
                    maxCumulRainLastHrYesterdayPortionDec =
                        Convert.ToDecimal(
                            (from myRow in dtRainData.AsEnumerable()
                             where myRow.Field<DateTime>("time_acurite") > inputCurrentAcuriteDateTime.AddHours(-1)
                             && myRow.Field<DateTime>("time_acurite") < inputCurrentAcuriteDateTime.Date
                             select myRow.Field<double>("rain")).DefaultIfEmpty().Max());
                }
                
                // Minimum cumulative rain 1 hour ago today portion
                decimal minCumulRainLastHrTodayPortionDec =
                        Convert.ToDecimal((from myRow in dtRainData.AsEnumerable()
                         where myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.Date
                         && myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.AddHours(-1)
                         select myRow.Field<double>("rain")).DefaultIfEmpty().Min());                

                if (maxCumulRainLastHrYesterdayPortionDec >= minCumulRainLastHrYesterdayPortionDec)
                {
                    totalRainLastHrYesterdayPortionDec = maxCumulRainLastHrYesterdayPortionDec - minCumulRainLastHrYesterdayPortionDec;
                }

                if (currentRainTotalToday >= minCumulRainLastHrTodayPortionDec)
                {
                    totalRainLastHrTodayPortionDec = currentRainTotalToday - minCumulRainLastHrTodayPortionDec;
                }

                rainInTheLast60Minutes = totalRainLastHrYesterdayPortionDec + totalRainLastHrTodayPortionDec;

                if (Properties.Settings.Default.debugMode == true)
                {
                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "Rain Last 60 Minutes Variables: " +
                        "  minCumulRainLastHrYesterdayPortionDec=" + minCumulRainLastHrYesterdayPortionDec +
                        "  maxCumulRainLastHrYesterdayPortionDec=" + maxCumulRainLastHrYesterdayPortionDec +
                        "  minCumulRainLastHrTodayPortionDec=" + minCumulRainLastHrTodayPortionDec +
                        "  totalRainLastHrYesterdayPortionDec=" + totalRainLastHrYesterdayPortionDec +
                        "  totalRainLastHrTodayPortionDec=" + totalRainLastHrTodayPortionDec +
                        "  inputCurrentAcuriteDateTime=" + inputCurrentAcuriteDateTime +
                        "  currentRainTotalToday=" + currentRainTotalToday +
                        System.Environment.NewLine + txtOutput.Text));

                    writeToDebugFile("Rain Last 60 Minutes Variables: " +
                        "  minCumulRainLastHrYesterdayPortionDec=" + minCumulRainLastHrYesterdayPortionDec +
                        "  maxCumulRainLastHrYesterdayPortionDec=" + maxCumulRainLastHrYesterdayPortionDec +
                        "  minCumulRainLastHrTodayPortionDec=" + minCumulRainLastHrTodayPortionDec +
                        "  totalRainLastHrYesterdayPortionDec=" + totalRainLastHrYesterdayPortionDec +
                        "  totalRainLastHrTodayPortionDec=" + totalRainLastHrTodayPortionDec +
                        "  inputCurrentAcuriteDateTime=" + inputCurrentAcuriteDateTime +
                        "  currentRainTotalToday=" + currentRainTotalToday +
                        System.Environment.NewLine);
                }

                return rainInTheLast60Minutes;
            }               

            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR processing rainLast60Minutes: " + ex.Message +
                   System.Environment.NewLine + txtOutput.Text));

                if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                {
                    writeToDebugFile(localDateTime + "\t" + "ERROR processing rainLast60Minutes: " + ex.ToString() + System.Environment.NewLine);
                }

                return 0;
            }
        }

        private decimal calcRainLast24Hours(DateTime inputCurrentAcuriteDateTime, decimal currentRainTotalToday = 0)
        {
            try
            {
                decimal minCumulRainLast24HrsYesterdayPortionDec = 0;
                decimal maxCumulRainLast24HrsYesterdayPortionDec = 0;
                decimal minCumulRainLast24HrsTodayPortionDec = 0;
                decimal totalRainLast24HrsYesterdayPortionDec = 0;
                decimal totalRainLast24HrsTodayPortionDec = 0;
                decimal rainInTheLast24Hours = 0;

                // Minimum cumulative rain 24 hours ago yesterday portion
                minCumulRainLast24HrsYesterdayPortionDec = 
                    Convert.ToDecimal(
                        (from myRow in dtRainData.AsEnumerable()
                         where  myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.AddHours(-24)
                         && myRow.Field<DateTime>("time_acurite") < inputCurrentAcuriteDateTime.Date
                         select myRow.Field<double>("rain")).DefaultIfEmpty().Min());

                // Maximum cumulative rain 24 hours ago yesterday portion
                maxCumulRainLast24HrsYesterdayPortionDec = 
                    Convert.ToDecimal(
                        (from myRow in dtRainData.AsEnumerable()
                         where myRow.Field<DateTime>("time_acurite") > inputCurrentAcuriteDateTime.AddHours(-24)
                         && myRow.Field<DateTime>("time_acurite") < inputCurrentAcuriteDateTime.Date
                         select myRow.Field<double>("rain")).DefaultIfEmpty().Max());

                // Minimum cumulative rain 24 hours ago today portion
                minCumulRainLast24HrsTodayPortionDec = 
                    Convert.ToDecimal(
                        (from myRow in dtRainData.AsEnumerable()
                         where myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.AddHours(-24)
                         && myRow.Field<DateTime>("time_acurite") >= inputCurrentAcuriteDateTime.Date
                         select myRow.Field<double>("rain")).DefaultIfEmpty().Min());

                if (maxCumulRainLast24HrsYesterdayPortionDec >= minCumulRainLast24HrsYesterdayPortionDec)
                {
                    totalRainLast24HrsYesterdayPortionDec = maxCumulRainLast24HrsYesterdayPortionDec - minCumulRainLast24HrsYesterdayPortionDec;
                }

                if(currentRainTotalToday >= minCumulRainLast24HrsTodayPortionDec)
                {
                    totalRainLast24HrsTodayPortionDec = currentRainTotalToday - minCumulRainLast24HrsTodayPortionDec;
                }
                else
                {
                    totalRainLast24HrsTodayPortionDec = minCumulRainLast24HrsTodayPortionDec; // zzz new 10/7/2016 7:05 PM
                }

                rainInTheLast24Hours = totalRainLast24HrsYesterdayPortionDec + totalRainLast24HrsTodayPortionDec;

                if (Properties.Settings.Default.debugMode == true)
                {
                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "Rain Last 24 Hours Variables: " +
                        "  minCumulRainLast24HrsYesterdayPortionDec=" + minCumulRainLast24HrsYesterdayPortionDec +
                        "  maxCumulRainLast24HrsYesterdayPortionDec=" + maxCumulRainLast24HrsYesterdayPortionDec +
                        "  minCumulRainLast24HrsTodayPortionDec=" + minCumulRainLast24HrsTodayPortionDec +
                        "  totalRainLast24HrsYesterdayPortionDec=" + totalRainLast24HrsYesterdayPortionDec +
                        "  totalRainLast24HrsTodayPortionDec=" + totalRainLast24HrsTodayPortionDec +
                        "  inputCurrentAcuriteDateTime=" + inputCurrentAcuriteDateTime +
                        "  currentRainTotalToday=" + currentRainTotalToday +
                        System.Environment.NewLine + txtOutput.Text));

                    writeToDebugFile("Rain Last 24 Hours Variables: " +
                        "  minCumulRainLast24HrsYesterdayPortionDec=" + minCumulRainLast24HrsYesterdayPortionDec +
                        "  maxCumulRainLast24HrsYesterdayPortionDec=" + maxCumulRainLast24HrsYesterdayPortionDec +
                        "  minCumulRainLast24HrsTodayPortionDec=" + minCumulRainLast24HrsTodayPortionDec +
                        "  totalRainLast24HrsYesterdayPortionDec=" + totalRainLast24HrsYesterdayPortionDec +
                        "  totalRainLast24HrsTodayPortionDec=" + totalRainLast24HrsTodayPortionDec +
                        "  inputCurrentAcuriteDateTime=" + inputCurrentAcuriteDateTime +
                        "  currentRainTotalToday=" + currentRainTotalToday +
                        System.Environment.NewLine);
                }

                return rainInTheLast24Hours;
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR processing rainLast24Hours: " + ex.Message  +
                   System.Environment.NewLine + txtOutput.Text));

                if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                {
                    writeToDebugFile(localDateTime + "\t" + "ERROR processing rainLast24Hours: " + ex.ToString() + System.Environment.NewLine);
                }

                return 0;
            }
        }


        public int progressBarValue(int pbarIncrement)
        {
            int pbarValue = 0;
            if (pbarProgressBar1.Value + pbarIncrement <= pbarProgressBar1.Maximum)
            {
                pbarValue = pbarProgressBar1.Value + pbarIncrement;
            }
            else
            {
                pbarValue = 5;
            }

            return pbarValue;
        }


        public void btnStart_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.networkDevice.Length > 0)
            {
                txtOutput.Text = waitMessage;
                txtOutput.Refresh();
                lblWuStationID.Text = Properties.Settings.Default.wuStation;
                lblWbStationID.Text = Properties.Settings.Default.wbStation;
                lblPwsStationID.Text = Properties.Settings.Default.pwsStation;
                lblAweatherStationID.Text = Properties.Settings.Default.awStation;

                pbarProgressBar1.Value = 5;

                if (BackgroundWorker1.IsBusy == false)
                {
                    BackgroundWorker1.RunWorkerAsync();
                    timer1.Start();
                    timerCWOP.Interval = Properties.Settings.Default.cwUpdateMinutes * 60000;
                    timerCWOP.Start();
                    timerAW.Start();
                    postDelayedCwopDataNow = true;
                    postDelayedAwDataNow = true;
                }               
            }
            else
            {
                this.Invoke(new MethodInvoker(() => txtOutput.Text = "PLEASE GO TO SETTINGS AND SPECIFY A NETWORK DEVICE." + System.Environment.NewLine + txtOutput.Text));
            }           
        }

        public void btnStop_Click_1(object sender, EventArgs e)
        {
            BackgroundWorker1.CancelAsync();
            BackgroundWorker1.Dispose();
            
            timer1.Stop();
            timerCWOP.Stop();
            timerAW.Stop();
            timerGetNetworkDevices.Stop();
            backgroundWorkerRestart = false;
            this.Invoke(new MethodInvoker(() => pbarProgressBar1.Value = 0));
            this.Invoke(new MethodInvoker(() => txtOutput.Text = txtOutput.Text.Replace(waitMessage, "")));
            signalFails = 0;
            txtSignal.BackColor = Control.DefaultBackColor;
            txtSignal.Text = "";
            txtBattery.BackColor = Control.DefaultBackColor;
            txtBattery.Text = "";
            txtLastUpdated.Text = "";
            txtAcuriteTime.Text = "";
            txtSignalFails.Text = "";
            aculinkData = "";
            aculinkDataBegin = false;
            aculinkDataEnd = false;

            dtRainData.Clear();
            dtWindData.Clear();
            dtRainData.Dispose();
            dtWindData.Dispose();
            aculinkDataBegin = false;
            aculinkDataEnd = false;
            aculinkData = "";

            errorCount = 0;            

            GC.Collect();
            
            this.Invoke(new MethodInvoker(() => txtOutput.Text = "Stopped." + System.Environment.NewLine + txtOutput.Text));
        }

        private void AboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Kevin's My AcuRite smartHUB to Weather Underground Rapid Fire and More" + System.Environment.NewLine + "Build: " +
                appBuild + System.Environment.NewLine + "© 2017  Kevin Key" +
                System.Environment.NewLine + "Comments/suggestions: " + System.Environment.NewLine + "http://kevin-key.blogspot.com/");
            }

        bool tryToGetListOfNetworkDevices(object sender, EventArgs e)
        {
            try
            {
                // Retrieve the device list from the local machine
                allDevices = LivePacketDevice.AllLocalMachine;
                return true;
            }
            catch (Exception)
            {
                Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" +
                   "Waiting for WinPcap to start. If this message doesn't go away after a couple minutes," +
                   " go to an administrator command prompt and type    net start npf    then press [Enter] to start the WinPcap service manually. If you see " +
                   "this message often, your WinPcap installation may be corrupt. Meanwhile, I will keep trying to resume every 15 seconds..." +
                   System.Environment.NewLine + txtOutput.Text));

                if (Properties.Settings.Default.debugMode == true)
                {
                    writeToDebugFile(localDateTime + "\t" +
                      "Waiting for WinPcap to start. If this message doesn't go away after a couple minutes," +
                      " go to an administrator command prompt and type    net start npf    then press [Enter] to start the WinPcap service manually. If you see " +
                      "this message often, your WinPcap installation may be corrupt. Meanwhile, I will keep trying to resume every 15 seconds..." +
                      System.Environment.NewLine);
                }

                if (timerGetNetworkDevices.Enabled == false)
                {
                    timerGetNetworkDevices.Start();
                }
                
                return false;
            }
        }

        private void BackgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            bool winpcapIsOk = false;

             if (string.IsNullOrEmpty(Properties.Settings.Default.networkDevice))
            {
                BackgroundWorker1.Dispose();
                pbarProgressBar1.Value = 0;
                this.Invoke(new MethodInvoker(() => txtOutput.Text = txtOutput.Text.Replace(waitMessage, "")));
                frmSetup setup = new frmSetup(this);

                if (setup.Visible == false)
                {
                    setup.ShowDialog();
                }
                
            }

            try
            {
                int deviceNumberToUse = 0;

                winpcapIsOk = tryToGetListOfNetworkDevices(this, EventArgs.Empty);
                
                if (winpcapIsOk == false)
                {
                    timerGetNetworkDevices.Start();

                    BackgroundWorker1.CancelAsync();
                    BackgroundWorker1.Dispose();

                    return;
                }


                if (timerGetNetworkDevices.Enabled)
                {
                    timerGetNetworkDevices.Stop();
                }
                
                int i = 0;
                
                while (i != allDevices.Count)
                {
                    LivePacketDevice device = allDevices[i];

                    if (Properties.Settings.Default.networkDevice.IndexOf(device.Name.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        deviceNumberToUse = i;
                    }

                    i += 1;
                }
                
                PacketDevice selectedDevice = allDevices[deviceNumberToUse];

                this.Invoke(new MethodInvoker(() => txtOutput.Text = waitMessage + System.Environment.NewLine + txtOutput.Text));
                this.Invoke(new MethodInvoker(() => txtOutput.Refresh()));
                this.Invoke(new MethodInvoker(() => pbarProgressBar1.Value = 5)); 
                                
                try
                {
                    // Open the device
                    // portion of the packet to capture
                    // 65536 guarantees that the whole packet will be captured on all the link layers
                    // promiscuous mode
                                        
                    using (communicator = selectedDevice.Open(1000, PacketDeviceOpenAttributes.NoCaptureLocal, 1000))
                                               
                   // using (PacketCommunicator communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                    {
                        // start the capture
                        communicator.ReceivePackets(0, PacketHandler); 
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new MethodInvoker(() => txtOutput.Text = localDateTime + "\t" + "ERROR 1: " + ex.Message + System.Environment.NewLine + txtOutput.Text));

                    if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                    {
                        writeToDebugFile(localDateTime + "\t" + "ERROR 1: " + ex.ToString() + System.Environment.NewLine);
                    }

                }
            }
            catch (System.InvalidOperationException ex)
            {
                errorHandler(ex);
            }
            catch (Exception ex)
            {
                errorHandler(ex);
                Close();
            }           

        }

        private void frmMain_Load_1(object sender, EventArgs e)
        {
            //notifyIcon1.ShowBalloonTip(1000);
        }



        // I store the wind gust, rain in the past 60 minutes, and rain in the past 24 hours values in XML files so that this data can still be
        // retrieved even if the application and/or comptuer is stopped and restarted due to Windows Update, power failure, version upgrades, etc.
        private string loadWindAndRainData()
        {
            string outputMessage = "";
                       
            if (File.Exists("WindData.xml"))
            {
                try
                {
                    DataSet theDataSetW = new DataSet();
                    theDataSetW.ReadXml("WindData.xml");

                    if (theDataSetW.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in theDataSetW.Tables[0].Rows)
                        {                        
                            DataRow workRow = dtWindData.NewRow();
                            workRow["time_local"] = Convert.ToDateTime(row["time_local"]);
                            workRow["windSpeed"] = Convert.ToDouble(row["windSpeed"]);
                            dtWindData.Rows.Add(workRow);
                        }
                    }

                    if (dtWindData.Rows.Count > 0)
                    {
                        outputMessage = outputMessage + "Restored " + dtWindData.Rows.Count.ToString() + " wind gust history records."
                            + System.Environment.NewLine;
                    }
                }

                catch (Exception ex)
                {                  
                    if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                    {
                        writeToDebugFile(localDateTime + "\t" + "ERROR with WindData.xml file. No big deal - going to attempt to create a new one.   " + ex.ToString() + System.Environment.NewLine);
                    }

                    File.Copy("WindData.xml", "WindDataERROR-" + localDateTime.Year.ToString() + localDateTime.Month.ToString() +
                        localDateTime.Day.ToString() + localDateTime.Hour.ToString() + localDateTime.Minute.ToString() +
                        localDateTime.Second.ToString() + ".xml", true);

                    File.Delete("WindData.xml");

                    outputMessage = outputMessage + "Found problems with WindData.xml. No big deal - creating new one. " +
                        "If you just upgraded to a new version of this app, this is expected behavior during the first launch." 
                        + System.Environment.NewLine;
                }
            }

            if (File.Exists("RainData.xml"))
            {
                try
                {
                    DataSet theDataSetR = new DataSet();
                    theDataSetR.ReadXml("RainData.xml");

                    if (theDataSetR.Tables[0].Rows.Count > 0)
                    {
                         foreach (DataRow row in theDataSetR.Tables[0].Rows)
                        {
                            DataRow workRow = dtRainData.NewRow();
                            workRow["time_local"] = Convert.ToDateTime(row["time_local"]);
                            workRow["time_acurite"] = Convert.ToDateTime(row["time_acurite"]);
                            workRow["rain"] = Convert.ToDouble(row["rain"]);
                            dtRainData.Rows.Add(workRow);
                        }
                    }

                    if (dtRainData.Rows.Count > 0)
                    {
                        outputMessage = outputMessage + "Restored " + dtRainData.Rows.Count.ToString() + 
                            " rain hour and rain 24 hours history records." + System.Environment.NewLine;
                    }
                }

                catch (Exception ex)
                {
                    if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                    {
                        writeToDebugFile(localDateTime + "\t" + "ERROR with RainData.xml file.  No big deal - going to attempt to create a new one.   " + ex.ToString() + System.Environment.NewLine);
                    }

                    File.Copy("RainData.xml", "RainDataERROR-" + localDateTime.Year.ToString() + localDateTime.Month.ToString() +
                        localDateTime.Day.ToString() + localDateTime.Hour.ToString() + localDateTime.Minute.ToString() +
                        localDateTime.Second.ToString() + ".xml", true);

                    File.Delete("RainData.xml");

                    outputMessage = outputMessage + "Found problems with RainData.xml. No big deal - creating new one. " +
                        "If you just upgraded to a new version of this app, this is expected behavior during the first launch." 
                        + System.Environment.NewLine;
                }
            }

            return outputMessage;
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (MessageBox.Show(this, "Are you sure you want to exit the weather app?", "Closing", MessageBoxButtons.YesNo) == DialogResult.No))
            {
                e.Cancel = true;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //if (Properties.Settings.Default.autoRestart)
            //{
            //    this.IsAutoRestarting = true;
            //    Application.Restart();
            //    this.IsAutoRestarting = false;
            //}
                        
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(250);
                this.ShowInTaskbar = false;
                //this.ShowDialog();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //this.WindowState = FormWindowState.Normal;
            //notifyIcon1.Visible = false;
            //this.ShowInTaskbar = true;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Normal;
            //notifyIcon1.Visible = false;
            //this.ShowInTaskbar = true;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timerCWOP_Tick(object sender, EventArgs e)
        {
            postDelayedCwopDataNow = true;
        }

        private void timerAW_Tik(object sender, EventArgs e)
        {
            postDelayedAwDataNow = true;
        }
         

        public static void errorHandler(Exception ex)
        {
            frmMain frmMainClass = new frmMain();

            if (ex.Message.IndexOf("pcapdotnet.base", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MessageBox.Show("ERROR: Can't find file PCAPDOTNET.BASE.DLL. Please make sure this file is in the same directory as the main " +
                    " program." + System.Environment.NewLine + System.Environment.NewLine +
                    "Error Details: " + ex.Message);
            }
            else if (ex.Message.IndexOf("pcapdotnet.core", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MessageBox.Show("ERROR: Can't find file PCAPDOTNET.CORE.DLL. Please make sure this file is in the same directory as the main " +
                    " program." + System.Environment.NewLine + System.Environment.NewLine +
                    "Error Details: " + ex.Message);
            }
            else if (ex.Message.IndexOf("pcapdotnet.packets", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MessageBox.Show("ERROR: Can't find file PCAPDOTNET.PACKETS.DLL. Please make sure this file is in the same directory as the main " +
                    " program." + System.Environment.NewLine + System.Environment.NewLine +
                    "Error Details: " + ex.Message);
            }
            else
            {
                if (Properties.Settings.Default.debugMode == true || Properties.Settings.Default.logSeriousErrors == true)
                {
                    frmMainClass.writeToDebugFile(frmMainClass.localDateTime + "\t" + "ERROR5: " + ex.ToString() + System.Environment.NewLine);
                }

                frmMainClass.txtOutput.Text = "ERROR5: " + ex.Message + System.Environment.NewLine + frmMainClass.txtOutput.Text;
            }            
        }

        public void refreshStationInfoDisplay()
        {
            Properties.Settings.Default.Reload();

            lblWuStationID.Text = Properties.Settings.Default.wuStation;
            lblWbStationID.Text = Properties.Settings.Default.wbPub;
            lblPwsStationID.Text = Properties.Settings.Default.pwsStation;
            lblAweatherStationID.Text = Properties.Settings.Default.awStation;
            lblOwmId.Text = Properties.Settings.Default.owStationId;
            lblCwopId.Text = Properties.Settings.Default.cwRegNum;
        }

        private void timerGetNetworkDevices_Tick(object sender, EventArgs e)
        {
            if (BackgroundWorker1.IsBusy == false)
            {
                BackgroundWorker1.RunWorkerAsync();
            }
        }

        public void writeToDebugFile(string textToWrite)
        {
            if (!File.Exists(NetworkDebugLog))
            {
                File.WriteAllText(NetworkDebugLog, textToWrite);
            } else
            {
                File.AppendAllText(NetworkDebugLog, textToWrite);
            }            
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                frmSetup setup = new frmSetup(this);

                if (setup.Visible == false)
                {
                    setup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }

    public class WindData
    {
        public DateTime time_local { get; set; }
        public decimal windSpeed { get; set; }

        public WindData(DateTime time_local, decimal windSpeed) 
        {
            this.time_local = time_local;
            this.windSpeed = windSpeed;
        }
    }

    public class RainData
    {
        public DateTime time_local { get; set; }
        public DateTime time_acurite { get; set; }
        public double rain { get; set; }

        public RainData(DateTime time_local, DateTime time_accurite, double rain)
        {
            this.time_local = time_local;
            this.time_acurite = time_acurite;
            this.rain = rain;
        }
    }

    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    }       
}
