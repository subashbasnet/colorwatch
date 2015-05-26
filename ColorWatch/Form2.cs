using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ColorWatch
{
    public partial class Form2 : Form
    {
        public SerialPort connectPort { get; set; }
        private String[] rLabData;
        Boolean firstEntryForGraph = true;
        private int count;
        public Form2()
        {
            InitializeComponent();
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        /**
         * Connect, Disconnect the selected port
         * **/
        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true) //otherwise it shows exception of port closed while 
            {                                     //while background process is running
                if (comboBox1.Items.Count < 1)
                { //if no items then connection cannot be done
                    label2.Text = "Empty Port List";
                    button3.BackColor = Color.Red;
                }
                else
                {
                    if (button3.Text.Equals("Connect"))
                    {
                        connectPort = new SerialPort(comboBox1.Text, 19200, Parity.None, 8, StopBits.One);
                        try
                        {
                            connectPort.Open();
                            button3.Text = "Disconnect";
                            button3.BackColor = Color.Green;
                            label2.Text = "";
                        }
                        catch (Exception)
                        {
                            label2.Text = comboBox1.Text + " is being used by another application!!";
                            button3.BackColor = Color.Red;
                            //throw;
                        }
                    }
                    else
                    {   //Disconnect pressed
                        connectPort.Close();
                        button3.Text = "Connect";
                        label2.Text = "";
                        button3.BackColor = default(Color);
                    }
                }
            }
        }

        /**
         * Open password form
         * **/
        private void button1_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("B");
                connectPort.Close();
                button3.Text = "Connect";
                button3.BackColor = default(Color);
            }

            if (this.ActiveMdiChild != null) //password form null checking
            {
                this.ActiveMdiChild.ShowDialog();
            }
            else
            {
                Form3 f3 = new Form3(); //Password Entry Form
                f3.ShowDialog(this); //this makes the current Form 'Form2' as owner of 'f3'
            }
        }

        /*
         * Refresh the portlist
         */
        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
        }

        /**
         *listen button, send 'I' and receive response
         * **/
        private void button7_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("I");
                Thread.Sleep(20);
                String calibrationData = connectPort.ReadExisting();
            }
        }

        /**
         * sets 'input low' button as on-->Green and off-->Red color.
         * **/
        private void inputLow(string inputLow)
        {
            button4.Text = inputLow;
            if (inputLow.Equals("ON"))
            {
                button4.BackColor = Color.Green;
            }
            else
            {
                button4.BackColor = Color.Red;
            }
        }

        /**
         * for D01(digital output measurement input one) 
         * and D02(digital output measurement input two)
         * Sauber-->D01 High und D02 High, Pink farbe ziegen
         * Not Clean--> D01 Low und D02 High, Grun farbe ziegen
         * D01 High und D02 Low, Gelb farbe ziegen
         * Undefined -->D01 Low und D02 Low
         * **/
        private void digitalOutputMeasurement(string digitalOutputMeasurement)
        {
            switch (digitalOutputMeasurement)
            {
                case "undefiniert":
                    button6.Text = digitalOutputMeasurement;
                    break;
                case "gelb":
                    button6.Text = "Not Clean";
                    button6.BackColor = Color.Yellow;
                    break;
                case "grun":
                    button6.Text = "Not Clean";
                    button6.BackColor = Color.Green;
                    break;
                case "pink":
                    button6.Text = "Sauber";
                    button6.BackColor = Color.Pink;
                    break;
                default:
                    break;
            }
        }

        /**
         * Manual Start Button
         * Send 'S' 
         * **/
        private void button10_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("S");
                Thread.Sleep(2000);
                String display = connectPort.ReadExisting();
                if (display != null)
                {
                    if (display.Length > 0)
                    {
                        //richTextBox1.Text = manualStartResponse;
                        string[] manualStartColors = BaseFunctions.manualStart(display);
                        //for input low
                        inputLow(manualStartColors[0]);
                        //for D01(digital output measurement input one) 
                        //and D02(digital output measurement input two)
                        digitalOutputMeasurement(manualStartColors[1]);
                        rLabData = BaseFunctions.RLab(display);
                        //richTextBox1.Text = manualStartColors[0] + " and " + manualStartColors[1] + display;
                        firstEntryForGraph = true;
                        if (backgroundWorker1.IsBusy != true)
                        {
                            //Start the asynchronous operation.
                            backgroundWorker1.RunWorkerAsync();
                        }
                    }
                }
                else
                { //sometimes microcontroller takes more time to respond
                    if (backgroundWorker1.IsBusy != true)
                    {
                        //Start the asynchronous operation.
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
        }

        /**
          * Manual Stop Button
          * Send 'B' 
          * **/
        private void button11_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("B");
                //Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        /*
        * Send 'W' to micro-controller and write response
        * to rich text,
        * Set the 'Data Output Low' button to Active-->Green color 
        * or Inactive-->Red color
        */
        private void button12_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("W");
                Thread.Sleep(5);
                String wResponse = connectPort.ReadExisting();
                if (wResponse != null)
                {
                    if (wResponse.Length > 0)
                    {
                        if (BaseFunctions.dataOutWrite(wResponse))
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
            }
        }



        //This event handler is where the time-consuming work ist done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            for (int i = 1; i <= 5000; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //solution to data point addition at end by sorting
                    //chart1.Series[0].Sort(PointSortOrder.Ascending, "X");
                    //chart1.DataManipulator.Sort(System.Windows.Forms.DataVisualization.Charting.PointSortOrder.Ascending, chart1.Series[0]);
                    
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress(i * 10);
                }
            }
        }

        //This event handler updates the progress
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (firstEntryForGraph)
            {
                chart1.Series[0].Points.AddXY(rLabData[0], rLabData[1]);
                firstEntryForGraph = false;
            }
            else
            {
                String manualStartResponseData = connectPort.ReadExisting();
                if (manualStartResponseData != null)
                {
                    if (manualStartResponseData.Length > 0)
                    {
                        //if(firstEntryForDigitalOutput){ //setting the digital output colors in buttons only in first entry
                        //richTextBox1.Text = manualStartResponse;
                        string[] manualStartColors = BaseFunctions.manualStart(manualStartResponseData);
                        //for input low
                        inputLow(manualStartColors[0]);
                        //for D01(digital output measurement input one) 
                        //and D02(digital output measurement input two)
                        digitalOutputMeasurement(manualStartColors[1]);
                        //richTextBox1.Text = manualStartColors[0] + " and " + manualStartColors[1] + manualStartResponseData;
                        //firstEntryForDigitalOutput = false;
                        // }
                        rLabData = BaseFunctions.RLab(manualStartResponseData);
                        //chart1.Series[0].Points.AddXY(rLabData[0], rLabData[1]);
                        count++;
                        //richTextBox1.Text = "rLabData[0]-->" + rLabData[0] + "/n  rLabData[1]-->" + rLabData[1]+"/n  count-->"+count;
                        //new code
                        DataPoint dp = chart1.Series[0].Points.FirstOrDefault(p => p.XValue > Double.Parse(rLabData[0]));
                        if (dp != null)
                        {
                            // got it, so we insert before it..
                            int index = chart1.Series[0].Points.IndexOf(dp);
                            if (index >= 0) chart1.Series[0].Points.InsertXY(index, Double.Parse(rLabData[0]), Double.Parse(rLabData[1]));
                        }
                        // no, so we add to the end
                        else chart1.Series[0].Points.AddXY(Double.Parse(rLabData[0]), Double.Parse(rLabData[1]));
                    }
                }
            }
        }

        //This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /**
         * Cancel background process on 'stop' button click
         * **/
        private void button8_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                connectPort.Write("B");
                //Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        /**
         * Clear the chart
         * **/
        private void button13_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                backgroundWorker1.CancelAsync();
                Thread.Sleep(100);
                chart1.Series[0].Points.Clear();
            }
        }

        /**
         * Reset the values in the chart i.e. to the default condition
         * 
         * **/
        private void button14_Click(object sender, EventArgs e)
        {
            if (button3.Text.Equals("Disconnect"))
            {
                backgroundWorker1.CancelAsync();
                connectPort.Write("B");
                Thread.Sleep(2000);
                chart1.Series[0].Points.Clear();
                //connectPort.Close();
                //button3.Text = "Connect";
                //button3.BackColor = default(Color);
                button4.Text = "Input Low";
                button4.BackColor = default(Color);
                button5.Text = "Digital Output Measurement";
                button5.BackColor = default(Color);
                button6.Text = "Initial State";
                button6.BackColor = default(Color);
                //comboBox1.DataSource = BaseFunctions.GetAllPorts();
            }
        }
    }
}
