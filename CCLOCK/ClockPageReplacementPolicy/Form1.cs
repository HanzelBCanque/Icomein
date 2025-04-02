using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClockPageReplacementPolicy
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Form2 simulationForm = new Form2();
            simulationForm.Opacity = 0;
            simulationForm.Show();

            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 20;
            fadeTimer.Tick += (s, ev) =>
            {
                if (simulationForm.Opacity < 1)
                    simulationForm.Opacity += 0.1;
                else
                    fadeTimer.Stop();
            };
            fadeTimer.Start();

            this.Hide();
        }
    }
    }

