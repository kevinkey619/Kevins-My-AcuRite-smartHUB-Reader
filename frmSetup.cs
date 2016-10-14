using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PcapDotNet.Core;
using System.IO;


namespace AcuLink_Bridge_Reader_CSharp
{
    public partial class frmSetup : Form
    {
        frmMain _owner;
        IList<LivePacketDevice> allDevices;

        public frmSetup(frmMain owner)
        {
            // This call is required by the designer.
            {
                InitializeComponent();

                _owner = owner;
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSetup_FormClosing);

                try
                {
                    // Retrieve the device list from the local machine
                    allDevices = LivePacketDevice.AllLocalMachine;
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to get a list of network devices. This is most likely due to WinPcap not running. If you are certain that " +
                        "you've already installed WinPcap from www.winpcap.org, please to to an administrator command prompt and type    net start npf    " +
                        "then press [Enter] to start the WinPcap service manually.");
                    this.Close();
                         
                }

                string itemDescription = null;

                if (allDevices.Count == 0)
                {
                    cmbDeviceId.Items.Add("No interfaces found! Make sure WinPcap is installed and running.");
                    return;
                }

                // Print the list
                int i = 0;
                while (i != allDevices.Count)
                {
                    LivePacketDevice device = allDevices[i];
                    itemDescription = device.Name;
                    if (device.Description != null)
                    {
                        itemDescription += " (" + device.Description.ToString() + ")";
                    }
                    else
                    {
                        itemDescription += " (No description available)";
                    }

                    cmbDeviceId.Items.Add(itemDescription);

                    i += 1;
                }

                cmbSensorType.Items.Add("");
                cmbSensorType.Items.Add("5n1");
                cmbSensorType.Items.Add("3n1");
                cmbSensorType.Items.Add("tower");
                cmbSensorType.Items.Add("water");

                cmbSensorTypeHumidity.Items.Add("");
                cmbSensorTypeHumidity.Items.Add("5n1");
                cmbSensorTypeHumidity.Items.Add("3n1");
                cmbSensorTypeHumidity.Items.Add("tower");

                cmbSensorTypeTemperature.Items.Add("");
                cmbSensorTypeTemperature.Items.Add("5n1");
                cmbSensorTypeTemperature.Items.Add("3n1");
                cmbSensorTypeTemperature.Items.Add("tower");
                cmbSensorTypeTemperature.Items.Add("water");

                cmbSensorTypeWind.Items.Add("");
                cmbSensorTypeWind.Items.Add("5n1");
                cmbSensorTypeWind.Items.Add("3n1");

                cmbSensorTypeRain.Items.Add("");
                cmbSensorTypeRain.Items.Add("5n1");
                cmbSensorTypeRain.Items.Add("3n1");
                cmbSensorTypeRain.Items.Add("rain");

                cmbSensorTypeSoil.Items.Add("");
                cmbSensorTypeSoil.Items.Add("5n1");
                cmbSensorTypeSoil.Items.Add("3n1");
                cmbSensorTypeSoil.Items.Add("tower");
                cmbSensorTypeSoil.Items.Add("water");

                cmbCwUpdateMinutes.Items.Add("5");
                cmbCwUpdateMinutes.Items.Add("6");
                cmbCwUpdateMinutes.Items.Add("7");
                cmbCwUpdateMinutes.Items.Add("8");
                cmbCwUpdateMinutes.Items.Add("9");
                cmbCwUpdateMinutes.Items.Add("10");
                cmbCwUpdateMinutes.Items.Add("15");
                cmbCwUpdateMinutes.Items.Add("20");
                cmbCwUpdateMinutes.Items.Add("25");
                cmbCwUpdateMinutes.Items.Add("30");
                cmbCwUpdateMinutes.Items.Add("45");
                cmbCwUpdateMinutes.Items.Add("60");
                
                // Add any initialization after the InitializeComponent() call.
                initializeSettings();
            }
        }

        private void initializeSettings()
        {
            txtWuID.Text = Properties.Settings.Default.wuStation;
            txtWuPwd.Text = Properties.Settings.Default.wuPwd;
            txtPressureOffset.Text = Properties.Settings.Default.pressureOffset.ToString();
            cmbDeviceId.SelectedItem = Properties.Settings.Default.networkDevice;
            cbPostToWunderground.Checked = Properties.Settings.Default.postToWunderground;
            cbPostToWeatherbug.Checked = Properties.Settings.Default.postToWeatherBug;
            txtWbPubID.Text = Properties.Settings.Default.wbPub;
            txtWbStationNum.Text = Properties.Settings.Default.wbStation;
            txtWbPassword.Text = Properties.Settings.Default.wbPwd;
            cbWriteToCSV.Checked = Properties.Settings.Default.writeToCSV;
            cbPostToPWS.Checked = Properties.Settings.Default.postToPws;
            txtPwsStationId.Text = Properties.Settings.Default.pwsStation;
            txtPwsPassword.Text = Properties.Settings.Default.pwsPwd;
            txtFilterOnSensorId.Text = Properties.Settings.Default.filterOnSensorId;
            cmbSensorType.SelectedItem = Properties.Settings.Default.sensorType;
            txtAwID.Text = Properties.Settings.Default.awStation;
            txtAwPwd.Text = Properties.Settings.Default.awPwd;
            cbPostToAWeather.Checked = Properties.Settings.Default.postToAWeather;
            txtSensorIdHumidity.Text = Properties.Settings.Default.sensorIdHumidity;
            txtSensorIdRain.Text = Properties.Settings.Default.sensorIdRain;
            txtSensorIdTemperature.Text = Properties.Settings.Default.sensorIdTemp;
            txtSensorIdWind.Text = Properties.Settings.Default.sensorIdWind;
            cmbSensorTypeHumidity.SelectedItem = Properties.Settings.Default.sensorTypeHumidity;
            cmbSensorTypeRain.SelectedItem = Properties.Settings.Default.sensorTypeRain;
            cmbSensorTypeTemperature.SelectedItem = Properties.Settings.Default.sensorTypeTemp;
            cmbSensorTypeWind.SelectedItem = Properties.Settings.Default.sensorTypeWind;
            txtCsvFilePath.Text = Properties.Settings.Default.csvFilePath;
            txtTempOffset.Text = Properties.Settings.Default.tempOffset.ToString();
            cbDebugMode.Checked = Properties.Settings.Default.debugMode;
            cmbSensorTypeSoil.SelectedItem = Properties.Settings.Default.sensorTypeSoil;
            txtSensorIdSoil.Text = Properties.Settings.Default.sensorIdSoil;
            txtWindOffsetPct.Text = Properties.Settings.Default.windOffsetPct.ToString();
            //cbAutoRestart.Checked = Properties.Settings.Default.autoRestart;
            txtSoilTempOffset.Text = Properties.Settings.Default.soilTempOffset.ToString();
            cbPostToOpenWeatherMap.Checked = Properties.Settings.Default.postToOw;
            txtOwUsername.Text = Properties.Settings.Default.owUsername;
            txtOwPwd.Text = Properties.Settings.Default.owPwd;
            txtOwLat.Text = Properties.Settings.Default.owLat;
            txtOwLon.Text = Properties.Settings.Default.owLon;
            txtOwAlt.Text = Properties.Settings.Default.owAlt;
            txtOwStationName.Text = Properties.Settings.Default.owStationName;
            cbPostToCw.Checked = Properties.Settings.Default.postToCw;
            txtCwRegNum.Text = Properties.Settings.Default.cwRegNum;
            txtCwHostName.Text = Properties.Settings.Default.cwHostName;
            txtCwLat.Text = Properties.Settings.Default.cwLat;
            txtCwLon.Text = Properties.Settings.Default.cwLon;
            txtCwPasscode.Text = Properties.Settings.Default.cwPasscode;
            cmbCwUpdateMinutes.SelectedItem = Properties.Settings.Default.cwUpdateMinutes.ToString();
            txtHumidityOffset.Text = Properties.Settings.Default.humidityOffset.ToString();
            txtCwComment.Text = Properties.Settings.Default.cwComment.ToString();
            cbPostToVera.Checked = Properties.Settings.Default.veraPostTo;
            txtVeraIp.Text = Properties.Settings.Default.veraIp;
            txtVeraHumidDevId.Text = Properties.Settings.Default.veraHumidDevId;
            txtVeraHumidSvc.Text = Properties.Settings.Default.veraHumidSvc;
            txtVeraTempDevId.Text = Properties.Settings.Default.veraTempDevId;
            txtVeraTempSvc.Text = Properties.Settings.Default.veraTempSvc;
            txtVeraHumidVar.Text = Properties.Settings.Default.veraHumidVar;
            txtVeraTempVar.Text = Properties.Settings.Default.veraTempVar;
            cbLogSeriousErrors.Checked = Properties.Settings.Default.logSeriousErrors;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            double temp = 0;

            try
            {
                if (cmbDeviceId.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Network Device.");
                    cmbDeviceId.Focus();
                    return;
                }


                if (!double.TryParse(txtPressureOffset.Text, out temp))
                {
                    MessageBox.Show("Please enter a Pressure Offset value between -9.99 and 9.99.");
                    txtPressureOffset.Focus();
                    return;
                }
                else if (!(double.Parse(txtPressureOffset.Text) >= -9.99) & double.Parse(txtPressureOffset.Text) <= 9.99)
                {
                    MessageBox.Show("Please enter a Pressure Offset value between -9.99 and 9.99.");
                    txtPressureOffset.Focus();
                    return;
                }

                temp = 0;

                if (!double.TryParse(txtTempOffset.Text, out temp))
                {
                    MessageBox.Show("Please enter a Temperature Offset value between -99.9 and 99.9.");
                    txtTempOffset.Focus();
                    return;
                }
                else if (!(double.Parse(txtTempOffset.Text) >= -99.9) & double.Parse(txtTempOffset.Text) <= 99.9)
                {
                    MessageBox.Show("Please enter a Temperature Offset value between -99.9 and 99.9.");
                    txtTempOffset.Focus();
                    txtTempOffset.Focus();
                    txtTempOffset.Focus();
                    return;
                }

                if (!double.TryParse(txtSoilTempOffset.Text, out temp))
                {
                    MessageBox.Show("Please enter a Soil/Water Temperature Offset value between -99.9 and 99.9.");
                    txtSoilTempOffset.Focus();
                    return;
                }
                else if (!(double.Parse(txtSoilTempOffset.Text) >= -99.9) & double.Parse(txtSoilTempOffset.Text) <= 99.9)
                {
                    MessageBox.Show("Please enter a Soil/Water Temperature Offset value between -99.9 and 99.9.");
                    txtSoilTempOffset.Focus();
                    return;
                }

                if (!double.TryParse(txtHumidityOffset.Text, out temp))
                {
                    MessageBox.Show("Please enter a Humidity Offset value between -99.9 and 99.9.");
                    txtHumidityOffset.Focus();
                    return;
                }
                else if (!(double.Parse(txtHumidityOffset.Text) >= -99.9) & double.Parse(txtHumidityOffset.Text) <= 99.9)
                {
                    MessageBox.Show("Please enter a Humidity Offset value between -99.9 and 99.9.");
                    txtHumidityOffset.Focus();
                    return;
                }

                var _with1 = Properties.Settings.Default;
                _with1.wuStation = txtWuID.Text.ToUpper();
                _with1.wuPwd = txtWuPwd.Text;
                _with1.pressureOffset = double.Parse(txtPressureOffset.Text);
                _with1.networkDevice = cmbDeviceId.SelectedItem.ToString();
                _with1.postToWunderground = cbPostToWunderground.Checked;
                _with1.postToWeatherBug = cbPostToWeatherbug.Checked;
                _with1.wbPub = txtWbPubID.Text;
                _with1.wbPwd = txtWbPassword.Text;
                _with1.wbStation = txtWbStationNum.Text;
                _with1.writeToCSV = cbWriteToCSV.Checked;
                _with1.postToPws = cbPostToPWS.Checked;
                _with1.pwsStation = txtPwsStationId.Text;
                _with1.pwsPwd = txtPwsPassword.Text;
                _with1.filterOnSensorId = txtFilterOnSensorId.Text;
                _with1.sensorType = cmbSensorType.SelectedItem.ToString();
                _with1.awStation = txtAwID.Text;
                _with1.awPwd = txtAwPwd.Text;
                _with1.postToAWeather = cbPostToAWeather.Checked;
                _with1.sensorIdHumidity = txtSensorIdHumidity.Text;
                _with1.sensorIdRain = txtSensorIdRain.Text;
                _with1.sensorIdTemp = txtSensorIdTemperature.Text;
                _with1.sensorIdWind = txtSensorIdWind.Text;
                _with1.sensorTypeHumidity = cmbSensorTypeHumidity.SelectedItem.ToString();
                _with1.sensorTypeRain = cmbSensorTypeRain.SelectedItem.ToString();
                _with1.sensorTypeTemp = cmbSensorTypeTemperature.SelectedItem.ToString();
                _with1.sensorTypeWind = cmbSensorTypeWind.SelectedItem.ToString();
                _with1.csvFilePath = txtCsvFilePath.Text;
                _with1.tempOffset = decimal.Parse(txtTempOffset.Text);
                _with1.debugMode = cbDebugMode.Checked;
                _with1.sensorTypeSoil = cmbSensorTypeSoil.SelectedItem.ToString();
                _with1.sensorIdSoil = txtSensorIdSoil.Text;
                _with1.windOffsetPct = decimal.Parse(this.txtWindOffsetPct.Text);
                _with1.soilTempOffset = decimal.Parse(txtSoilTempOffset.Text);
                _with1.postToOw = cbPostToOpenWeatherMap.Checked;
                _with1.owUsername = txtOwUsername.Text;
                _with1.owPwd = txtOwPwd.Text;
                _with1.owLat = txtOwLat.Text;
                _with1.owLon = txtOwLon.Text;
                _with1.owAlt = txtOwAlt.Text;
                _with1.owStationName = txtOwStationName.Text;
                _with1.postToCw = cbPostToCw.Checked;
                _with1.cwRegNum = txtCwRegNum.Text;
                _with1.cwHostName = txtCwHostName.Text;
                _with1.cwLat = txtCwLat.Text;
                _with1.cwLon = txtCwLon.Text;
                _with1.cwPasscode = txtCwPasscode.Text;
                _with1.cwUpdateMinutes = int.Parse(cmbCwUpdateMinutes.SelectedItem.ToString());
                _with1.humidityOffset = decimal.Parse(txtHumidityOffset.Text);
                _with1.cwComment = txtCwComment.Text;
                _with1.veraPostTo = cbPostToVera.Checked;
                _with1.veraTempDevId = txtVeraTempDevId.Text;
                _with1.veraTempSvc = txtVeraTempSvc.Text;
                _with1.veraHumidDevId = txtVeraHumidDevId.Text;
                _with1.veraHumidSvc = txtVeraHumidSvc.Text;
                _with1.veraIp = txtVeraIp.Text;
                _with1.veraTempVar = txtVeraTempVar.Text;
                _with1.veraHumidVar = txtVeraHumidVar.Text;
                _with1.logSeriousErrors = cbLogSeriousErrors.Checked;
                
                Properties.Settings.Default.Save();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Problem encountered while validating and/or saving settings. Details: " + ex.ToString());
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSetup_Load(object sender, EventArgs e)
        {
        }

        private void cbGetPreviousVersionSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Upgrade();
            MessageBox.Show("Attempting to pull your settings from the previous version.");
            initializeSettings();
            //cbGetPreviousVersionSettings.Checked = false;            
        }

        private void cbGetPreviousVersionSettings_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void frmSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _owner.refreshStationInfoDisplay();
            }
            catch (Exception)
            {
            }
        }
    }
}
