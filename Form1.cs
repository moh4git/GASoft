using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int val;

            if (radioButton2.Checked)
            {
                Form2 f2 = new Form2();

                int number;

                if (textBox1.Text == null || !int.TryParse(textBox1.Text, out number))
                {
                    MessageBox.Show("Nombre des tâches non valide ou la valeur null");
                    return;
                }
                else
                {
                    val = int.Parse(textBox1.Text);
                }

                f2.dataGridView1.Columns.Clear();

                f2.dataGridView1.ColumnCount = val;

                int i;

                for (i = 0; i < val; i++)
                {
                    f2.dataGridView1.Columns[i].HeaderCell.Value = (i + 1).ToString();
                }

                f2.dataGridView1.RowHeadersWidth = 80;


                Random rnd = new Random();

                //Calculate Pi           
                string[] row = new string[val];
                for (int j = 0; j < val; j++)
                {
                    row[j] = rnd.Next(1, 101).ToString();
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[0].HeaderCell.Value = "Pi";

                //Calculate Wi
                for (int j = 0; j < val; j++)
                {
                    row[j] = rnd.Next(1, 11).ToString();
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[1].HeaderCell.Value = "Wi";

                //Calculate Di

                i = 0;
                int result = 0;
                for (i = 0; i < f2.dataGridView1.ColumnCount; i++)
                {
                    result += Convert.ToInt32(f2.dataGridView1.Rows[0].Cells[i].Value);
                }

                for (int j = 0; j < val; j++)
                {
                    row[j] = calc_di(result, rnd).ToString();
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[2].HeaderCell.Value = "Di";

                f2.dataGridView1.AllowUserToAddRows = false;

                textBox1.Text = null;
                this.Hide();
                f2.RefToForm1 = this;
                f2.ShowDialog();
                
            }
            else if (radioButton1.Checked)
            {

                Form2 f2 = new Form2();

                int number;

                if (textBox1.Text == null || !int.TryParse(textBox1.Text, out number))
                {
                    MessageBox.Show("Nombre des tâches non valide ou la valeur null");
                    return;
                }
                else
                {
                    val = int.Parse(textBox1.Text);
                }

                f2.dataGridView1.Columns.Clear();

                f2.dataGridView1.ColumnCount = val;

                int i;

                for (i = 0; i < val; i++)
                {
                    f2.dataGridView1.Columns[i].HeaderCell.Value = (i + 1).ToString();
                }

                f2.dataGridView1.RowHeadersWidth = 80;


                Random rnd = new Random();

                //Calculate Pi           
                string[] row = new string[val];
                for (int j = 0; j < val; j++)
                {
                    row[j] = "";
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[0].HeaderCell.Value = "Pi";

                //Calculate Wi
                for (int j = 0; j < val; j++)
                {
                    row[j] = "";
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[1].HeaderCell.Value = "Wi";

                //Calculate Di

                for (int j = 0; j < val; j++)
                {
                    row[j] = "";
                }
                f2.dataGridView1.Rows.Add(row);
                f2.dataGridView1.Rows[2].HeaderCell.Value = "Di";

                f2.dataGridView1.AllowUserToAddRows = false;

                textBox1.Text = null;
                this.Hide();
                f2.RefToForm1 = this;
                f2.ShowDialog();
            }
        }


        public double calc_di(int p, Random rnd)
        {
            double TF = 0.4;
            double RDD = 0.2;

            double min = p * (1 - TF - (RDD / 2));
            double max = p * (1 - TF + (RDD / 2));
            double di = Math.Round(rnd.NextDouble() * (max - min) + min, 0);
            return di;
        }

    }
}
