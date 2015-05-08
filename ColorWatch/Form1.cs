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
        private String trInputNumber;
        private Boolean openMeasureForm { get; set; }
        private Boolean dontCalibrate { get; set; }
        private String calibrationData;
        private double hValue;
        private Boolean firstEntryForHValue = true;
        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
            richTextBox1.Enabled = false;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
        }



        /*
         * Connect, Disconnect the selected port
         */
        private void button1_Click(object sender, EventArgs e)
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
                    connectPort.Close();
                    button1.Text = "Connect";
                    label1.Text = "";
                    button1.BackColor = default(Color);
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
                richTextBox1.Text = connectPort.ReadExisting();
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
         * Send 'S' to micro-controller and write response
         * to rich text
         */
        /*
         * private void button6_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write(button6.Text);
                //To do : find out exact time needed
                Thread.Sleep(200); //more time because it gives more response 
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }
         * */

        /**
         * Controls the length of the text to be 3 in trNumber input box.
         * Doesn't allow any characters besides the number.
         * **/

        private void trNumberInputBox_TextChanged(object sender, EventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(trNumberInputBox.Text, "^[0-9]+$")
                || trNumberInputBox.TextLength > 3)//true = not only number in inputtext or inputtext length greater than 3
            {
                if (trNumberInputBox.TextLength > 1)
                {//greater than 1 set inputtext as old data
                    trNumberInputBox.Text = trInputNumber;
                    //setting cursor at the end for inputext length greater than 1
                    trNumberInputBox.Select(trNumberInputBox.Text.Length, 0);
                }
                else
                {
                    trNumberInputBox.Text = "";
                }
            }
            else
            {//only number in the inputtext, do nothing but store the data in trInputNumber
                trInputNumber = trNumberInputBox.Text;
                if (trNumberInputBox.Text.Length > 1)
                {//setting cursor at the end for inputext length greater than 1
                    trNumberInputBox.Select(trNumberInputBox.Text.Length, 0);
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
                connectPort.Close();
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
                if (calibrationData != null)
                {
                    if (calibrationData.Length > 0)
                    {
                        String[] calibrationDataArray = BaseFunctions.extractCalibrationFactor(calibrationData);
                        textBox1.Text = calibrationDataArray[0];
                        textBox2.Text = calibrationDataArray[1];
                        textBox3.Text = calibrationDataArray[2];
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
                //Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
                backgroundWorker2.CancelAsync();
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
                    for (int i = 0; i < chartHeightPoints.Length; i++)
                    {
                        if (chartHeightPoints[i] <= 50)
                        {//Border_Clear
                            chart1.Series[3].Points.AddY(chartHeightPoints[i]);
                        }
                        else if (chartHeightPoints[i] <= 100)
                        {//Border_Yellow
                            chart1.Series[2].Points.AddY(chartHeightPoints[i]);
                        }
                        else if (chartHeightPoints[i] <= 300)
                        {//Border_Green
                            chart1.Series[1].Points.AddY(chartHeightPoints[i]);
                        }
                        else
                        {//Border_Pink
                            chart1.Series[0].Points.AddY(chartHeightPoints[i]);
                        }
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
                connectPort.Write("S");
                Thread.Sleep(2000);
                String manualStartResponse = connectPort.ReadExisting();
                if (manualStartResponse != null)
                {
                    if (manualStartResponse.Length > 0)
                    {
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
                connectPort.Write("B");
                //Cancel the asynchronous operation.
                backgroundWorker2.CancelAsync();
            }
        }

        //This event handler is where the time-consuming work ist done.
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
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
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (firstEntryForHValue)
            {
                chart1.Series[4].Points.AddY(hValue);
                firstEntryForHValue = false;
            }
            else
            {
                String manualStartResponseData = connectPort.ReadExisting();
                if (manualStartResponseData != null)
                {
                    if (manualStartResponseData.Length > 0)
                    {
                        richTextBox1.Text = manualStartResponseData;
                        chart1.Series[4].Points.AddY(BaseFunctions.hValue(manualStartResponseData));
                    }
                }
            }
        }

        //This event handler deals with the results of the background operation.
        private void backgroundWorker2_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

    }
}
