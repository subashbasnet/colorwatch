using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorWatch
{
    public partial class Form3 : Form
    {

        private Form2 mForm;
        public Form3(ref Form2 measureForm)
        {
            mForm = measureForm;
        }

        private void closeMeasureForm()
        {
            if (this.mForm != null)
                this.mForm.Close();
            this.mForm = null;
        }
        public Form3()
        {
            InitializeComponent();
        }

        /**
         * Go to next form i.e. Test-Admin form
         * **/
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Equals("c"))
            {
                if (this.Owner != null)
                {
                    //this.Hide(); //hides current window
                    this.Owner.Hide(); //hides current and it's parent window
                    
                }
                if (this.ActiveMdiChild != null)
                {
                    this.ActiveMdiChild.ShowDialog();
                }
                else {//for the first time, when not hidden
                    //ActiveForm.Close();
                    Form1 f1 = new Form1(); //Test form
                    f1.ShowDialog(this);
                }
            }
        }

        /**
         * Take 'Enter' also as 'Button Click'
         * **/
        private void passwordTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                button1_Click(sender, e);
            }
        }
    }
}
