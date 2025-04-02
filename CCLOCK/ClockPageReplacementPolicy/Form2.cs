using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClockPageReplacementPolicy
{
    public partial class Form2: Form
    {
        private int frameCount;
        private List<string> pageRequests;
        private List<string> frames = new List<string>();
        private Dictionary<string, int> usageTracker = new Dictionary<string, int>();
        private int successCount = 0, failureCount = 0;
        private int currentStep = -1;
        public Form2()
        {
            InitializeComponent();
        }

        private void dgvFrames_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtFrames_TextChanged(object sender, EventArgs e)
        {
            3
        }

        private void lblFrames_Click(object sender, EventArgs e)
        {

        }

        private void lblPages_Click(object sender, EventArgs e)
        {

        }

        private void txtPages_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSuccess_Click(object sender, EventArgs e)
        {

        }

        private void lblFailure_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtFrames.Text = "";
            txtPages.Text = "";
            dgvFrames.Columns.Clear();
            dgvFrames.Rows.Clear();
            lblSuccess.Text = "Success Rate (%) = ";
            lblFailure.Text = "Failure Rate (%) = ";
            frameCount = 0;
            pageRequests = new List<string>();
            frames.Clear();
            usageTracker.Clear();
            successCount = 0;
            failureCount = 0;
            currentStep = -1;
            MessageBox.Show("Simulation has been restarted.");
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (currentStep == -1)
            {
                if (!int.TryParse(txtFrames.Text, out frameCount) || frameCount < 1 || frameCount > 3)
                {
                    MessageBox.Show("Invalid frame count! Enter a number between 1 and 3.");
                    return;
                }

                // Validate page request sequence (only uppercase letters and comma-separated format like A,B,C)
                string input = txtPages.Text.ToUpper();
                if (!Regex.IsMatch(input, "^([A-Z],)*[A-Z]$"))
                {
                    MessageBox.Show("Invalid page request sequence! Enter letters (A-Z) separated by commas, e.g., A,B,C.");
                    return;
                }

                pageRequests = input.Split(',').ToList();

                StartSimulation();
            }
            else
            {
                NextStep();
            }
        }

        private void StartSimulation()
        {
            successCount = 0;
            failureCount = 0;
            currentStep = 0;
            frames.Clear();
            usageTracker.Clear();
            SetupGrid();
        }

        private void SetupGrid()
        {
            dgvFrames.Columns.Clear();
            dgvFrames.Rows.Clear();
            dgvFrames.DefaultCellStyle.Font = new Font("Mongolian Baiti", 13);
            dgvFrames.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFrames.ColumnHeadersVisible = false;
            dgvFrames.RowHeadersVisible = true;
            dgvFrames.BackgroundColor = Color.Magenta;
            dgvFrames.ForeColor = Color.Black;

            dgvFrames.Columns.Add("Step", "Step");
            foreach (var page in pageRequests)
            {
                dgvFrames.Columns.Add(page, page);
            }

            dgvFrames.Rows.Add("Sequence");
            dgvFrames.Rows[0].DefaultCellStyle.Font = new Font("Mongolian Baiti", 13, FontStyle.Bold);

            for (int i = 0; i < pageRequests.Count; i++)
            {
                dgvFrames.Rows[0].Cells[i + 1].Value = pageRequests[i];
            }

            // Add frame rows and reference bit rows together
            for (int i = 0; i < frameCount; i++)
            {
                dgvFrames.Rows.Add("Frame " + (i + 1));
                dgvFrames.Rows.Add("RefBit " + (i + 1)); // Add reference bit row after each frame
            }

            dgvFrames.Rows.Add("Page Fault");
            dgvFrames.Rows.Add("Clock Hand");

            for (int i = 1; i < dgvFrames.Rows.Count; i++)
            {
                for (int j = 1; j < dgvFrames.Columns.Count; j++)
                {
                    dgvFrames.Rows[i].Cells[j].Value = "";
                }
            }

            dgvFrames.ClearSelection();
            dgvFrames.CurrentCell = null;
        }

        private void NextStep()
        {
            if (currentStep < pageRequests.Count)
            {
                ProcessPage(pageRequests[currentStep]);
                UpdateGrid(currentStep);
                currentStep++;
            }

            if (currentStep >= pageRequests.Count)
            {
                ShowFinalResults();
                MessageBox.Show("Simulation completed!");
            }
        }
        private int clockHand = 0; // Points to the next frame to be checked
        private Dictionary<string, int> referenceBits = new Dictionary<string, int>();

        private void ProcessPage(string page)
        {
            bool isPageFault = false;

            if (frames.Contains(page))
            {
                successCount++;
                referenceBits[page] = 1;
            }
            else
            {
                failureCount++;
                isPageFault = true;

                if (frames.Count < frameCount)
                {
                    frames.Add(page);
                    referenceBits[page] = 1;
                }
                else
                {
                    while (true)
                    {
                        string candidate = frames[clockHand];

                        if (referenceBits[candidate] == 0)
                        {
                            frames[clockHand] = page;
                            referenceBits.Remove(candidate);
                            referenceBits[page] = 1;
                            break;
                        }
                        else
                        {
                            referenceBits[candidate] = 0;
                            clockHand = (clockHand + 1) % frameCount;
                        }
                    }
                }
                clockHand = (clockHand + 1) % frameCount;
            }

            // Update page fault indicator
            int pageFaultRow = 1 + (frameCount * 2);
            dgvFrames.Rows[pageFaultRow].Cells[currentStep + 1].Value = isPageFault ? "*" : "";
            if (isPageFault)
            {
                dgvFrames.Rows[pageFaultRow].Cells[currentStep + 1].Style.BackColor = Color.Red;
            }
        }

        private void UpdateGrid(int step)
        {
            dgvFrames.Rows[0].Cells[step + 1].Value = pageRequests[step];

            // Update frame contents and reference bits
            for (int i = 0; i < frameCount; i++)
            {
                // Frame values (row 1, 3, 5, etc.)
                int frameRow = 1 + (i * 2);
                dgvFrames.Rows[frameRow].Cells[step + 1].Value = i < frames.Count ? frames[i] : "";

                // Reference bits (row 2, 4, 6, etc.)
                if (i < frames.Count && frames[i] != null && referenceBits.ContainsKey(frames[i]))
                {
                    dgvFrames.Rows[frameRow + 1].Cells[step + 1].Value = referenceBits[frames[i]];
                }
                else
                {
                    dgvFrames.Rows[frameRow + 1].Cells[step + 1].Value = "";
                }
            }

            // Update clock hand position
            int clockHandRow = 1 + (frameCount * 2) + 1; // After frames + refbits + page fault
            dgvFrames.Rows[clockHandRow].Cells[step + 1].Value = clockHand + 1; // Show as 1-based index
        }

        private void ShowFinalResults()
        {
            int totalOperations = successCount + failureCount;
            if (totalOperations == 0) return;

            double successRate = ((double)successCount / totalOperations) * 100;
            double failureRate = ((double)failureCount / totalOperations) * 100;

            lblSuccess.Text = "Success Rate (%) = (No. of No Interrupt / No. of Operations) × 100 = ("
                              + successCount + " / " + totalOperations + ") × 100 = "
                              + successRate.ToString("F2") + "%";

            lblFailure.Text = "Failure Rate (%) = (No. of Interrupts / No. of Operations) × 100 = ("
                              + failureCount + " / " + totalOperations + ") × 100 = "
                              + failureRate.ToString("F2") + "%";
        }
    }
    }

