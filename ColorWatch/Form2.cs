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

namespace ColorWatch
{
    public partial class Form2 : Form
    {
        public SerialPort connectPort { get; set; }
        
        public Form2()
        {
            InitializeComponent();
            comboBox1.DataSource = BaseFunctions.GetAllPorts();
        }

        /**
         * Connect, Disconnect the selected port
         * **/
        private void button3_Click(object sender, EventArgs e)
        {
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
                {//Disconnect pressed
                    connectPort.Close();
                    button3.Text = "Connect";
                    label2.Text = "";
                    button3.BackColor = default(Color);
                }
            }
        }

        /**
         * Open password form
         * **/
        private void button1_Click(object sender, EventArgs e)
        {
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
         * Manual Start Button
         * Send 'S' 
         * **/
        private void button10_Click(object sender, EventArgs e)
        {
            connectPort.Write("S"); 
        }

        /**
         * Manual Stop Button
         * Send 'B' 
         * **/
        private void button11_Click(object sender, EventArgs e)
        {
            connectPort.Write("B");
        }
    }
}
