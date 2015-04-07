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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null) //password form null checking
            {
                this.ActiveMdiChild.ShowDialog();
            }
            else {
                Form3 f3 = new Form3(); //Password Entry Form
                f3.ShowDialog(this); //this makes the current Form 'Form2' as owner of 'f3'
            }
        }
    }
}
//ctrl+K+D for format code
//ctrl+C+V to duplicate line