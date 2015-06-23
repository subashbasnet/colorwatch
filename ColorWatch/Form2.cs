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
        //Boolean firstEntryForG = true;
        Boolean manualStart = true;
        Boolean onLoad = true;
        //private int count;
        public Form2()
        {
            InitializeComponent();
            chart1.Series[0].Color = Color.Black;
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            //richTextBox1.SelectionStart = richTextBox1.Text.Length;
            //richTextBox1.ScrollToCaret();
        }

        /**
         * Connect, Disconnect the selected port
         * **/
        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true && backgroundWorker2.IsBusy != true) //otherwise it shows exception of port closed while 
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
                            String display = connectPort.ReadExisting();//for continuous messung, there is default response
                            if (display == null || display.Length <= 0)
                            {
                                Thread.Sleep(300);
                                display = connectPort.ReadExisting();
                            }
                            if (display != null)
                            {
                                if (display.Length > 0)
                                {
                                    manualStart = false; //as this one is continuous messung
                                    if (backgroundWorker2.IsBusy != true)
                                    {
                                        //Start the asynchronous operation.
                                        backgroundWorker2.RunWorkerAsync();
                                    }
                                    else
                                    {
                                        backgroundWorker2.CancelAsync();
                                    }
                                    if (backgroundWorker1.IsBusy != true)
                                    {
                                        //Start the asynchronous operation.
                                        backgroundWorker1.RunWorkerAsync();
                                    }
                                    else
                                    {
                                        backgroundWorker1.CancelAsync();
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            label2.Text = comboBox1.Text + " is being used by another application!!";
                            button3.BackColor = Color.Red;
                            //throw;
                        }
                    }
                    else
                    {
                        //Disconnect pressed
                        connectPort.Close();
                        button3.Text = "Connect";
                        label2.Text = "";
                        button3.BackColor = default(Color);
                        //firstEntryWriteG = true;
                    }
                }
            }
            else//to kill the background processes
            {
                System.Windows.Forms.MessageBox.Show("Please Press 'Stop' before Disconnect");
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
                richTextBox1.AppendText(connectPort.ReadExisting());
                richTextBox1.ScrollToCaret();
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
                        if (onLoad)
                        {
                            chart1.Series[0].Points.Clear(); //to clear the initial point set to show the chart
                            chart1.Series[0].Color = Color.Black;
                            onLoad = false;
                        }
                        //string[] manualStartColors = BaseFunctions.manualStart(display);
                        //for input low
                        //inputLow(manualStartColors[0]);
                        //for D01(digital output measurement input one) 
                        //and D02(digital output measurement input two)
                        //digitalOutputMeasurement(manualStartColors[1]);
                        //rLabData = BaseFunctions.RLab(display);
                        //firstEntryForGraph = true;
                        if (backgroundWorker1.IsBusy != true)
                        {
                            //Start the asynchronous operation.
                            backgroundWorker1.RunWorkerAsync();
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Please Press 'Stop' to end current process");
                            connectPort.Write("B");//to end the continuous manual start response
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
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please Press 'Stop' to end current process");
                        connectPort.Write("B");//to end the continuous manual start response
                    }
                }
            }
        }

        //This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (onLoad)
            {
                chart1.Series[0].Points.Clear(); //to clear the initial point set to show the chart
                chart1.Series[0].Color = Color.Black;
                onLoad = false;
            }
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            //for (int i = 1; i <= 5000; i++)
            //{
            int i = 0;
            while (true)
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
                i++;
            }
            //}
        }

        //This event handler updates the progress
        /**
         * Plot's graph and if manual start then 
         * changes color DI1 and DO1 green/red
         * if Manual start do everything, otherwise only plot the graph
         * **/
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            String manualStartResponseData = connectPort.ReadExisting();
            if (manualStartResponseData != null)
            {
                if (manualStartResponseData.Length > 0)
                {
                    if (manualStart)
                    {
                        rLabData = BaseFunctions.RLabManualStart(manualStartResponseData);
                        richTextBox1.AppendText(manualStartResponseData);
                        richTextBox1.ScrollToCaret();
                    }
                    else
                    {// for continuous messung only graph 
                        rLabData = BaseFunctions.RLabForGraphOnly(manualStartResponseData);
                    }
                    //data to plot graph
                    double n0;
                    bool isNumeric0 = double.TryParse(rLabData[0], out n0);
                    double n1;
                    bool isNumeric1 = double.TryParse(rLabData[1], out n1);
                    double n2 = 0;
                    bool isNumeric2 = false;
                    double n3 = 0;
                    bool isNumeric3 = false;
                    if (manualStart)
                    {
                        isNumeric2 = double.TryParse(rLabData[2], out n2);
                        isNumeric3 = double.TryParse(rLabData[3], out n3);
                    }
                    if (firstEntryForGraph)
                    {
                        if (isNumeric0 && isNumeric1)
                        {
                            chart1.Series[0].Points.AddXY(n0, n1);
                            firstEntryForGraph = false;
                        }
                        if (manualStart)
                        {
                            if (isNumeric2 && isNumeric3)
                            {
                                if (n2 == 0)
                                    label3.BackColor = Color.Green;
                                else
                                    label3.BackColor = Color.Red;//1
                                if (n3 == 0)
                                    label4.BackColor = Color.Green;
                                else
                                    label4.BackColor = Color.Red;//1
                            }
                        }
                    }
                    else
                    {
                        //if(firstEntryForDigitalOutput){ //setting the digital output colors in buttons only in first entry
                        //richTextBox1.Text = manualStartResponse;
                        //string[] manualStartColors = BaseFunctions.manualStart(manualStartResponseData);
                        //for input low
                        //inputLow(manualStartColors[0]);
                        //for D01(digital output measurement input one) 
                        //and D02(digital output measurement input two)
                        //digitalOutputMeasurement(manualStartColors[1]);
                        //richTextBox1.Text = manualStartColors[0] + " and " + manualStartColors[1] + manualStartResponseData;
                        //firstEntryForDigitalOutput = false;
                        // }

                        //chart1.Series[0].Points.AddXY(rLabData[0], rLabData[1]);
                        //count++;
                        //richTextBox1.Text = "rLabData[0]-->" + rLabData[0] + "/n  rLabData[1]-->" + rLabData[1]+"/n  count-->"+count;
                        //new code
                        //double n0;
                        isNumeric0 = double.TryParse(rLabData[0], out n0);
                        //double n1;
                        isNumeric1 = double.TryParse(rLabData[1], out n1);
                        if (isNumeric0 && isNumeric1)
                        {
                            DataPoint dp = chart1.Series[0].Points.FirstOrDefault(p => p.XValue > n0);
                            if (dp != null)
                            {
                                // got it, so we insert before it..
                                int index = chart1.Series[0].Points.IndexOf(dp);
                                if (index >= 0) chart1.Series[0].Points.InsertXY(index, n0, n1);
                            }
                            // no, so we add to the end
                            else chart1.Series[0].Points.AddXY(n0, n1);
                        }
                        if (manualStart)
                        {
                            isNumeric2 = double.TryParse(rLabData[2], out n2);
                            isNumeric3 = double.TryParse(rLabData[3], out n3);
                            if (isNumeric2 && isNumeric3)
                            {
                                if (n2.Equals("0"))
                                    label3.BackColor = Color.Green;
                                else
                                    label3.BackColor = Color.Red;//1
                                if (n3.Equals("0"))
                                    label4.BackColor = Color.Green;
                                else
                                    label4.BackColor = Color.Red;//1
                            }
                        }
                    }
                    
                }
            }
            //}
        }

        //This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
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
                Thread.Sleep(5);//find out correct time for the response capture
                //System.Windows.Forms.MessageBox.Show(connectPort.ReadExisting());
                String wResponse = connectPort.ReadExisting();
                if (wResponse != null)
                {
                    if (wResponse.Length > 0)
                    {
                        wResponse = BaseFunctions.dataOutWrite(wResponse);
                        if (wResponse.Equals("true"))
                        {
                            button9.BackColor = Color.Green;
                            button9.Text = "Data Output active";
                        }
                        else if (wResponse.Equals("false"))
                        {
                            button9.BackColor = Color.Red;
                            button9.Text = "Data Output inactive";
                        }
                        else
                        {
                            button9.BackColor = default(Color);
                            button9.Text = "Unknown";
                        }
                    }
                }
            }
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
                backgroundWorker2.CancelAsync();
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
                Form2_Load(sender, e);
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
                //backgroundWorker1.CancelAsync();
                //backgroundWorker2.CancelAsync();
                //connectPort.Write("B");
                //Thread.Sleep(2000);
                richTextBox1.Clear();
                chart1.Series[0].Points.Clear();
                Form2_Load(sender, e);
                label3.BackColor = default(Color);
                label4.BackColor = default(Color);
                label5.BackColor = default(Color);
                label6.BackColor = default(Color);
                //connectPort.Close();
                //button3.Text = "Connect";
                //button3.BackColor = default(Color);
                //button4.Text = "Input Low";
                //button4.BackColor = default(Color);
                //button5.Text = "Digital Output Measurement";
                //button5.BackColor = default(Color);
                //button6.Text = "Initial State";
                //button6.BackColor = default(Color);
                //comboBox1.DataSource = BaseFunctions.GetAllPorts();
            }
        }

        /**
         * get's called only after the constructor
         * **/
        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.Series[0].Color = Color.Transparent;
            chart1.Series[0].Points.AddXY(15, -40);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //String wResponse = connectPort.ReadExisting();
            //Thread.Sleep(50);
            //if (wResponse != null)
            //{
            //    if (wResponse.Length > 0)
            //    {
            //        if (BaseFunctions.dataOutWrite(wResponse))
            //        {
            //            button9.BackColor = Color.Green;
            //            button9.Text = "Data Output active";
            //            System.Windows.Forms.MessageBox.Show("Data output active 1");
            //        }
            //        else
            //        {
            //            connectPort.Write("W");
            //            wResponse = connectPort.ReadExisting();
            //            if (wResponse != null)
            //            {
            //                if (wResponse.Length > 0)
            //                {
            //                    if (BaseFunctions.dataOutWrite(wResponse))
            //                    {
            //                        button9.BackColor = Color.Green;
            //                        button9.Text = "Data Output active";
            //                        System.Windows.Forms.MessageBox.Show("Data output active 2");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            int i = 0;
            while (true)
            {
                //for (int i = 1; i <= 10000; i++)
                //{
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //Perform a time consuming operation and report progress
                    System.Threading.Thread.Sleep(100);
                    worker.ReportProgress(i * 10);
                }
                // }
                i++;
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            //if(firstentrywriteg){
            //    connectport.write("g");
            //    firstentrywriteg = false;
            //}
            string responseData = connectPort.ReadExisting();
            if (responseData != null)
            {
                if (responseData.Length > 0)
                {
                    List<string> extractedSauberData = BaseFunctions.extractSauberCondition(responseData);
                    if (extractedSauberData != null)
                    {
                        if (extractedSauberData[0].Equals("All"))//continuous messung
                        {
                            setLabelColor(label3, extractedSauberData[1]);
                            setLabelColor(label4, extractedSauberData[2]);
                            setLabelColor(label5, extractedSauberData[3]);
                            setLabelColor(label6, extractedSauberData[4]);
                        }
                        else if (extractedSauberData[0].Equals("S"))//manual start 
                        {
                            setLabelColor(label3, extractedSauberData[1]);
                            setLabelColor(label4, extractedSauberData[2]);
                        }
                        else if (extractedSauberData[0].Equals("G"))//measuring conductivity
                        {
                            setLabelColor(label5, extractedSauberData[1]);
                            setLabelColor(label6, extractedSauberData[2]);
                        }
                        richTextBox1.AppendText(responseData);
                        richTextBox1.ScrollToCaret();
                    }
                    //richTextBox1.AppendText(responseData);
                    ////richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    //richTextBox1.ScrollToCaret();
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /**
         * Conductivity activity start
         * **/
        private void button15_Click(object sender, EventArgs e)
        {
            connectPort.Write("G");
            Thread.Sleep(200);
            String response = connectPort.ReadExisting();
            if (response != null)
            {
                if (response.Length > 0)
                {
                    if (backgroundWorker2.IsBusy != true)
                    {
                        //Start the asynchronous operation.
                        backgroundWorker2.RunWorkerAsync();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please Press 'Stop' to end current process");
                        connectPort.Write("B");// as the 'G' sends continuous response
                    }
                }
            }
        }

        /**
         * Setting the label color according to the response
         * **/
        public void setLabelColor(Label label, String extractedSauberData)
        {
            if (extractedSauberData.Equals("0"))
                label.BackColor = Color.Green;
            else
                label.BackColor = Color.Red;//1
        }
    }
}
