using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorWatch
{
    public partial class Form1 : Form
    {
        public SerialPort connectPort { get; set; }
        private String trInputNumber, tpInputNumber, tcInputNumber, twInputNumber;
        private Boolean openMeasureForm { get; set; }
        private Boolean dontCalibrate { get; set; }
        private String calibrationData;
        private double hValue;
        private Boolean firstEntryForHValue = true;
        private Boolean _restart1;
        private Boolean onLoad = true;
        private double testForGraph = 0;
        //Stopwatch sw = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            chart1.Series[0].Color = Color.Pink;
            domainUpDown1.SelectedIndex = 0;
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
            richTextBox1.Enabled = false;
            textBox6.Enabled = false;
            textBox5.Enabled = false;
            textBox8.Enabled = false;
            textBox11.Enabled = false;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker4.WorkerSupportsCancellation = true;
        }



        /*
         * Connect, Disconnect the selected port
         */
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true && backgroundWorker2.IsBusy != true) //otherwise it shows exception of port closed while 
            {
                if (comboBox1.Items.Count < 1)
                { //if no items then connection cannot be done
                    label1.Text = "Empty Port List";
                    button1.BackColor = Color.Red;
                }
                else
                {
                    if (button1.Text.Equals("Connect"))
                    {
                        connectPort = new SerialPort(comboBox1.Text, 19200, Parity.None, 8, StopBits.One);
                        try
                        {
                            connectPort.Open();
                            button1.Text = "Disconnect";
                            button1.BackColor = Color.Green;
                            label1.Text = "";
                            dontCalibrate = false;
                        }
                        catch (Exception)
                        {
                            label1.Text = comboBox1.Text + " is being used by another application!!";
                            button1.BackColor = Color.Red;
                            //throw;
                        }
                    }
                    else
                    {//Disconnect pressed
                        if (connectPort != null)
                        { //for the case when USB is already taken out before disconnect pressed
                            //and disconnect is pressed later then throws error on close as there is no connection.
                            connectPort.Close();
                        }
                        button1.Text = "Connect";
                        label1.Text = "";
                        button1.BackColor = default(Color);
                    }
                }
            }
        }

        /*
         * Refresh the portlist
         */
        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
        }

        /*
         * Send 'W' to micro-controller and write response
         * to rich text,
         * Set the 'Data Output Low' button to Active-->Green color 
         * or Inactive-->Red color
         */
        private void button3_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("W");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
                if (BaseFunctions.dataOutWrite(richTextBox1.Text))
                {
                    button9.BackColor = Color.Green;
                    button9.Text = "Data Output active";
                }
                else
                {
                    button9.BackColor = Color.Red;
                    button9.Text = "Data Output inactive";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].Color = Color.Transparent;
            chart1.Series[0].Points.AddXY(15, 40);
            if (this.Owner != null)
            {
                this.Owner.Hide();
            }
        }



        /*
         * Send 'L' to micro-controller and write response
         * to rich text
         */
        private void button4_Click(object sender, EventArgs e)
        {//8 second for response it takes
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("L");
                Thread.Sleep(20);
                //richTextBox1.Text = connectPort.ReadExisting();
                backgroundWorker3.RunWorkerAsync();
            }
        }

        /*
         * Send 'K' to micro-controller and write response
         * to rich text and change button color
         */
        private void button26_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("K");
                Thread.Sleep(20);
                string digitalOuptResponse = connectPort.ReadExisting();
                richTextBox1.Text = digitalOuptResponse;
                if (digitalOuptResponse.Equals("unfiltered_mode<1>"))
                {
                    button26.BackColor = Color.Green;
                }
                else
                {
                    button26.BackColor = default(Color);
                }
            }
        }

        /*
         * Send 'R' to micro-controller and write response
         * to rich text and change button color
         */
        private void button27_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("R");
                Thread.Sleep(20);
                string digitalOuptResponse = connectPort.ReadExisting();
                richTextBox1.Text = digitalOuptResponse;
                if (digitalOuptResponse.Equals("Raw_Data_Output<1>"))
                {
                    button27.BackColor = Color.Green;
                }
                else
                {
                    button27.BackColor = default(Color);
                }
            }
        }

        //working on receiving response
        /*
         * Send 'E<1> or E<2>' to micro-controller and write response
         * to rich text and change button color
         */
        private void button25_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("E<" + domainUpDown1.Text + ">");
                Thread.Sleep(20);
                string digitalOuptResponse = connectPort.ReadExisting();
                richTextBox1.Text = digitalOuptResponse;
                //if (digitalOuptResponse.Equals("0"))
                //{
                //    button25.BackColor = Color.Green;
                //}
                //else
                //{
                //    button25.BackColor = default(Color);
                //}
            }
        }

        /*
         * Send 'TR' to micro-controller and write response
         * to rich text
         */
        private void button5_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("TR<" + trNumberInputBox.Text.Trim() + ">");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /*
         * Send 'TP' to micro-controller and write response
         * to rich text
         */
        private void button22_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("TP<" + textBox12.Text.Trim() + ">");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /*
         * Send 'TC' to micro-controller and write response
         * to rich text
         */
        private void button24_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("TC<" + textBox14.Text.Trim() + ">");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /*
         * Send 'TW' to micro-controller and write response
         * to rich text
         */
        private void button23_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("TW<" + textBox13.Text.Trim() + ">");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /**
         * Event called on Reference-timer text changed
         * **/
        private void trNumberInputBox_TextChanged(object sender, EventArgs e)
        {
            textBoxFilter(trNumberInputBox, "TR");
        }

        /**
         * Event called on Probe-timer text changed
         * **/
        private void tpInputChanged(object sender, EventArgs e)
        {
            textBoxFilter(textBox12, "TP");
        }

        /**
         * Event called on Conductivity-timer text changed
         * **/
        private void tcInputChanged(object sender, EventArgs e)
        {
            textBoxFilter(textBox14, "TC");
        }

        /**
         * Event called on PWM-cycle text changed
         * **/
        private void twInputChanged(object sender, EventArgs e)
        {
            textBoxFilter(textBox13, "TW");
        }

        /**
         * Controls the length of the text to be not greater than 4 in input box.
         * Doesn't allow any characters besides the number.
         * **/
        public void textBoxFilter(TextBox input, string inputType)
        {

            if (!System.Text.RegularExpressions.Regex.IsMatch(input.Text, "^[0-9]+$")
                || input.TextLength > 4)//true = not only number in inputtext or inputtext length greater than 3
            {
                if (input.TextLength > 1)
                {//greater than 1 set inputtext as old data
                    switch (inputType)
                    {
                        case "TR":
                            input.Text = trInputNumber;
                            break;
                        case "TP":
                            input.Text = tpInputNumber;
                            break;
                        case "TC":
                            input.Text = tcInputNumber;
                            break;
                        case "TW":
                            input.Text = twInputNumber;
                            break;
                    }
                    //setting cursor at the end for inputext length greater than 1
                    input.Select(input.Text.Length, 0);
                }
                else
                {
                    input.Text = "";
                }
            }
            else
            {//only number in the inputtext, do nothing but store the data in trInputNumber
                switch (inputType)
                {
                    case "TR":
                        trInputNumber = input.Text;
                        break;
                    case "TP":
                        tpInputNumber = input.Text;
                        break;
                    case "TC":
                        tcInputNumber = input.Text;
                        break;
                    case "TW":
                        twInputNumber = input.Text;
                        break;
                }
                if (input.Text.Length > 1)
                {//setting cursor at the end for inputext length greater than 1
                    input.Select(input.Text.Length, 0);
                }
            }

        }

        /**
         * Call measure window,
         * Close the port if it's in a open state, so that after navigation
         * next form doesn't indicate port busy status
         * **/
        private void button12_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("B");
                connectPort.Close();
                button1.Text = "Connect";
                button1.BackColor = default(Color);
            }
            if (this.Owner.Owner != null)
            {
                openMeasureForm = true;
                this.Hide();
                this.Owner.Owner.Show();
            }
            else
            {
                Form2 f2 = new Form2(); //Measure Form
                f2.Show();
            }
        }

        /**
         *closing form 2 i.e. measure form on cross button click
         * **/
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (this.Owner.Owner != null && !openMeasureForm)
            {
                this.Owner.Owner.Close();
            }
            //
        }

        /**
         *listen button, send 'I' and receive response
         * **/
        private void button7_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("I");
                if (dontCalibrate)// dontCalibrate = true means Calibrate button clicked.
                {
                    Thread.Sleep(1600);//simultaneous pressing calibrate then listen 
                    //gives argument exception error, so to get rid of this time lapse added
                }
                else
                {
                    Thread.Sleep(20);
                }
                calibrationData = connectPort.ReadExisting();
                richTextBox1.Text = calibrationData;
                if (calibrationData != null)
                {
                    if (calibrationData.Length > 0)
                    {
                        if(onLoad){
                            chart1.Series[0].Points.Clear(); //to clear the initial point set to show the chart
                            chart1.Series[0].Color = Color.Pink;
                            onLoad = false;
                        }
                        String[] calibrationDataArray = BaseFunctions.extractCalibrationFactor(calibrationData);
                        textBox1.Text = calibrationDataArray[0];
                        textBox2.Text = calibrationDataArray[1];
                        textBox3.Text = calibrationDataArray[2];
                        //code for borders
                        double[] borderPoints = BaseFunctions.extractBorderValues(calibrationData);
                        //Border_Pink
                        textBox6.Text = borderPoints[0].ToString();
                        //Border_Green
                        textBox5.Text = borderPoints[1].ToString();
                        //Border_Yellow
                        textBox8.Text = borderPoints[2].ToString();
                        //Border_Clear
                        textBox11.Text = borderPoints[3].ToString();
                        if (backgroundWorker1.IsBusy != true)
                        {
                            //Start the asynchronous operation.
                            backgroundWorker1.RunWorkerAsync();
                        }
                    }
                }
                richTextBox1.Text = calibrationData;
                dontCalibrate = false;
            }
        }

        /**
         * Stop Button
         * Cancel both background async process1 and process2 
         * **/
        private void button8_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("B");
                setRestart1(false);
                //Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
                backgroundWorker2.CancelAsync();
                backgroundWorker4.CancelAsync();
            }
        }

        /**
         *calibrate button send 'C'
         * **/
        private void button13_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                if (!dontCalibrate)
                {
                    connectPort.Write("C");
                    Thread.Sleep(20);
                    String calibrationData = connectPort.ReadExisting();
                    if (calibrationData != null)
                    {
                        if (calibrationData.Length > 0)
                        {
                            String[] calibrationDataArray = BaseFunctions.extractCalibrationNumerics(calibrationData);
                            textBox1.Text = calibrationDataArray[0];
                            textBox2.Text = calibrationDataArray[1];
                            textBox3.Text = calibrationDataArray[2];
                        }
                    }
                    richTextBox1.Text = calibrationData;
                    dontCalibrate = true;
                }
            }
        }

        /**
         * Output_Test:--> D
            Digital Ausgang 1--> rot/grun
            Digital Ausgang 2--> rot/grun
            Fehler --> rot/grun
         ***/
        private void button6_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("D");
                Thread.Sleep(20);
                String outPutTest = connectPort.ReadExisting();
                if (outPutTest != null)
                {
                    if (outPutTest.Length > 0)
                    {
                        Boolean[] outputTestBoolean = BaseFunctions.outPutTest(outPutTest);
                        if (outputTestBoolean[0])
                        {
                            button10.BackColor = Color.Green;
                        }
                        else
                        {
                            button10.BackColor = Color.Red;
                        }
                        if (outputTestBoolean[1])
                        {
                            button14.BackColor = Color.Green;
                        }
                        else
                        {
                            button14.BackColor = Color.Red;
                        }
                        if (outputTestBoolean[2])
                        {
                            button11.BackColor = Color.Green;
                        }
                        else
                        {
                            button11.BackColor = Color.Red;
                        }
                    }
                }
                richTextBox1.Text = outPutTest;
            }
        }

        //This event handler is where the time-consuming work ist done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            for (int i = 1; i <= 1000; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        //This event handler updates the progress
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (calibrationData != null)
            {
                if (calibrationData.Length != 0)
                {
                    double[] chartHeightPoints = BaseFunctions.extractBorderValues(calibrationData);
                    //Border_Clear
                    chart1.Series[3].Points.AddY(chartHeightPoints[3]);
                    //Border_Yellow
                    chart1.Series[2].Points.AddY(chartHeightPoints[2]);
                    //Border_Green
                    chart1.Series[1].Points.AddY(chartHeightPoints[1]);
                    //Border_Pink
                    chart1.Series[0].Points.AddY(chartHeightPoints[0]);
                    if (!isRestart1())
                    {
                        chart1.Series[4].Points.AddY(1);
                    }

                }
            }
        }

        //This event handler deals with the results of the background operation
        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /**
        * Manual Start Button
        * Send 'S' 
        * **/
        private void button16_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                backgroundWorker4.CancelAsync();
                connectPort.Write("S");
                Thread.Sleep(2000);
                String manualStartResponse = connectPort.ReadExisting();
                if (manualStartResponse != null)
                {
                    if (manualStartResponse.Length > 0)
                    {
                        if (onLoad)
                        {
                            chart1.Series[0].Points.Clear(); //to clear the initial point set to show the chart
                            chart1.Series[0].Color = Color.Pink;
                            onLoad = false;
                        }
                        richTextBox1.Text = manualStartResponse;
                        hValue = BaseFunctions.hValue(manualStartResponse);
                        firstEntryForHValue = true;
                        if (backgroundWorker2.IsBusy != true)
                        {
                            //Start the asynchronous operation.
                            backgroundWorker2.RunWorkerAsync();
                        }
                    }
                }
                else
                {//sometimes microcontroller takes more time to respond
                    if (backgroundWorker2.IsBusy != true)
                    {
                        //Start the asynchronous operation.
                        backgroundWorker2.RunWorkerAsync();
                    }
                }
            }
        }

        /**
          * Manual Stop Button
          * Send 'B' 
          * **/
        private void button15_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                backgroundWorker2.CancelAsync();
                connectPort.Write("B");
                Thread.Sleep(1000);
                chart1.Series[4].Points.AddY(2);
                setRestart1(false);
                if (backgroundWorker4.IsBusy != true)
                {
                    //Start the asynchronous operation.
                    backgroundWorker4.RunWorkerAsync();
                }
                else
                {
                    backgroundWorker4.CancelAsync();
                }
            }
        }

        //This event handler is where the time-consuming work ist done.
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker4.CancelAsync();
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            for (int i = 1; i <= 1000; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        //This event handler updates the progress
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            setRestart1(true);
            //double xAxisMaximum = chart1.ChartAreas[0].AxisX.Maximum;
            if (firstEntryForHValue)
            {
                testForGraph += 2; //the time elapse for reading the data
                //testForGraph = sw.ElapsedMilliseconds/200;
                //chart1.Series[4].Points.AddXY(testForGraph, hValue);
                chart1.Series[4].Points.AddY(hValue);
                //richTextBox1.Text = "Xmax 1st----->" + chart1.ChartAreas[0].AxisX.Maximum.ToString();
                //richTextBox1.Text = "Xmax 1st----->" + chart1.ChartAreas[0].AxisX.Maximum.ToString()
                //    + "elapsed milli seconds 1st----->" + sw.ElapsedMilliseconds;
                firstEntryForHValue = false;
            }
            else
            {
                testForGraph += 3; //the time elapse for reading the data
                String manualStartResponseData = connectPort.ReadExisting();
                if (manualStartResponseData != null)
                {
                    if (manualStartResponseData.Length > 0)
                    {
                        testForGraph += 1;
                        richTextBox1.Text = manualStartResponseData;
                        //chart1.Series[4].Points.AddXY(testForGraph, BaseFunctions.hValue(manualStartResponseData));
                        chart1.Series[4].Points.AddY(BaseFunctions.hValue(manualStartResponseData));
                        //richTextBox1.Text = "Xmax----->" + chart1.ChartAreas[0].AxisX.Maximum.ToString();
                        //richTextBox1.Text = "Xmax----->" + chart1.ChartAreas[0].AxisX.Maximum.ToString() +
                        //   "elapsed milli seconds----->" + sw.ElapsedMilliseconds;
                    }
                }
            }
        }

        //This event handler deals with the results of the background operation.
        private void backgroundWorker2_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /**
         * Only the number entry is allowed but be more than one '.' is also allowed and backspace
         * **/
        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            BaseFunctions.restrictCharsFunction(sender, e);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            BaseFunctions.restrictCharsFunction(sender, e);
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            BaseFunctions.restrictCharsFunction(sender, e);
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            BaseFunctions.restrictCharsFunction(sender, e);
        }

        /**
         * send 'MP<XX>' where ‘XX’ is the number entered to microcontroller and 
         *  now change the greyed text box with this value and the other text box empty as before.
         * **/
        private void button17_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                if (textBox9.Text.Length > 0)
                {
                    String borderValue = BaseFunctions.trimForMoreThanOneDotInInputNumber(textBox9.Text);
                    if (borderValue.Length == 1 && borderValue.Equals("."))
                    {
                        textBox9.Clear();
                    }
                    else
                    {
                        connectPort.Write("MP<" + borderValue + ">");
                        textBox6.Text = borderValue;
                        textBox9.Clear();
                    }
                }
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                if (textBox4.Text.Length > 0)
                {
                    String borderValue = BaseFunctions.trimForMoreThanOneDotInInputNumber(textBox4.Text);
                    if (borderValue.Length == 1 && borderValue.Equals("."))
                    {
                        textBox4.Clear();
                    }
                    else
                    {
                        connectPort.Write("MP<" + borderValue + ">");
                        textBox5.Text = borderValue;
                        textBox4.Clear();
                    }
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                if (textBox7.Text.Length > 0)
                {
                    String borderValue = BaseFunctions.trimForMoreThanOneDotInInputNumber(textBox7.Text);
                    if (borderValue.Length == 1 && borderValue.Equals("."))
                    {
                        textBox7.Clear();
                    }
                    else
                    {
                        connectPort.Write("MP<" + borderValue + ">");
                        textBox8.Text = borderValue;
                        textBox7.Clear();
                    }
                }
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                if (textBox10.Text.Length > 0)
                {
                    String borderValue = BaseFunctions.trimForMoreThanOneDotInInputNumber(textBox10.Text);
                    if (borderValue.Length == 1 && borderValue.Equals("."))
                    {
                        textBox10.Clear();
                    }
                    else
                    {
                        connectPort.Write("MP<" + borderValue + ">");
                        textBox11.Text = borderValue;
                        textBox10.Clear();
                    }
                }
            }
        }

        /**
         * Clear the chart values
         * **/
        private void button21_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                backgroundWorker1.CancelAsync();
                backgroundWorker2.CancelAsync();
                Thread.Sleep(100);
                chart1.Series[3].Points.Clear();
                chart1.Series[2].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart1.Series[0].Points.Clear();
                chart1.Series[4].Points.Clear();
                //calibrationData = null;
            }
        }

        //This event handler is where the time-consuming work ist done.
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            for (int i = 1; i <= 50; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        /**
         * Cancel background process of reading response for led
         * **/
        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            String ledResponseData = connectPort.ReadExisting();
            if (ledResponseData != null)
            {
                if (ledResponseData.Length > 0)
                {
                    richTextBox1.Text = ledResponseData;
                    backgroundWorker3.CancelAsync();
                }
            }
        }

        private void backgroundWorker3_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /**
     * @return the _restart1
     */
        public Boolean isRestart1()
        {
            return _restart1;
        }

        /**
         * @param _restart1 the _restart1 to set
         */
        public void setRestart1(Boolean _restart1)
        {
            this._restart1 = _restart1;
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            for (int i = 1; i <= 1000; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        private void backgroundWorker4_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            chart1.Series[4].Points.AddY(1);
        }

        private void backgroundWorker4_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /*
        * Send 'A' i.e. self-test to micro-controller and write response
        * to rich text and change button color
        */
        private void button30_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("A");
                Thread.Sleep(20);
                string digitalOuptResponse = connectPort.ReadExisting();
                richTextBox1.Text = digitalOuptResponse;
                if (digitalOuptResponse.Equals("Pass"))
                {
                    button27.BackColor = Color.Green;
                }
                else if (digitalOuptResponse.Equals("Fail"))
                {
                    button27.BackColor = Color.Red;
                }
                else
                {
                    button27.BackColor = default(Color);
                }
            }
        }

        /*
        * Send 'G' i.e. measuring conductivity to micro-controller and write response
        * to rich text and change button color
        */
        private void button28_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("A");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /*
        * Send 'P' i.e. calibration conductivity to micro-controller and write response
        * to rich text and change button color
        */
        private void button29_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write("P");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }
    }
}
