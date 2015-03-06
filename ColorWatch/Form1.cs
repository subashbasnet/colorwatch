using Microsoft.Win32.SafeHandles;
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
    public partial class Form1 : Form
    {
        public SerialPort connectPort { get; set; }
        private String trInputNumber;
        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = GetAllPorts();
            richTextBox1.Enabled = false;
        }

        /*
         * Get all serial ports list
         */
        public Array GetAllPorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
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
            else {
                if (button1.Text.Equals("Connect"))
                {
                    connectPort = new SerialPort(comboBox1.Text, 19200, Parity.None, 8, StopBits.One);
                    try
                    {
                        connectPort.Open();
                        button1.Text = "Disconnect";
                        button1.BackColor = Color.Green;
                        label1.Text = "";
                    }
                    catch (Exception)
                    {
                        label1.Text = comboBox1.Text + " is being used by another application!!";
                        button1.BackColor = Color.Red;
                        throw;
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
            //List<String> x = new List<string>();
            comboBox1.DataSource = GetAllPorts();
        }

        /*
         * Send 'W' to micro-controller and write response
         * to rich text
         */
        private void button3_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write(button3.Text);
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /*
         * Send 'L' to micro-controller and write response
         * to rich text
         */
        private void button4_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write(button4.Text);
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
                connectPort.Write(button5.Text+"<"+trNumberInputBox.Text+">");
                Thread.Sleep(20);
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        /*
         * Send 'S' to micro-controller and write response
         * to rich text
         */
        private void button6_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Disconnect"))
            {
                connectPort.Write(button6.Text);
                //To do : find out exact time needed
                Thread.Sleep(100); //more time because it gives more response 
                richTextBox1.Text = connectPort.ReadExisting();
            }
        }

        private void trNumberInputBox_TextChanged(object sender, EventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(trNumberInputBox.Text, "^[0-9]+$")
                ||trNumberInputBox.TextLength>3)//true = not only number in inputtext or inputtext length greater than 3
            {
                if (trNumberInputBox.TextLength > 1)
                {//greater than 1 set inputtext as old data
                    trNumberInputBox.Text = trInputNumber;
                    //setting cursor at the end for inputext length greater than 1
                    trNumberInputBox.Select(trNumberInputBox.Text.Length, 0);
                }
                else {
                    trNumberInputBox.Text = "";
                }
            }
            else {//only number in the inputtext, do nothing but store the data in trInputNumber
                trInputNumber = trNumberInputBox.Text;
                if (trNumberInputBox.Text.Length > 1)
                {//setting cursor at the end for inputext length greater than 1
                    trNumberInputBox.Select(trNumberInputBox.Text.Length, 0);
                }
            }
        }
    }
}
