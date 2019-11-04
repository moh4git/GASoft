using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form2 : Form
    {
        public Form RefToForm1 { get; set; }
        public Form RefToForm2 { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private double f(int i)
        {
            var f1 = 59894 - (8128 * i) + (262 * i * i) - (1.6 * i * i * i);
            return f1;
        }

        public int calc_Ti(int Ci, int Di)
        {
            return Math.Max(0, (Ci - Di));
        }

        public int calc_fitness(string cromosome)
        {
            int fitness = 0;
            int Ci = 0;
            int Wi = 0;
            int Di = 0;

            var array = cromosome.Split('-');

            for (int i = 0; i < array.Length; i++)
            {
                Ci = 0;
                for (int j = i + 1; j > 0; j--)
                {
                    Ci += int.Parse(dataGridView1.Rows[0].Cells[int.Parse(array[j - 1]) - 1].Value.ToString());
                }

                Wi = int.Parse(dataGridView1.Rows[1].Cells[int.Parse(array[i]) - 1].Value.ToString());
                Di = int.Parse(dataGridView1.Rows[2].Cells[int.Parse(array[i]) - 1].Value.ToString());

                fitness += Wi * calc_Ti(Ci, Di);
            }
            return fitness;
        }

        public string[] crossover(string[] cromosome_list)
        {
            string[] cromosome_array = cromosome_list;

            //Create list from an array
            List<string> crom_lst = new List<string>(cromosome_list);
            List<string> crossover_list = new List<string>();
            List<string> crossover_result = new List<string>();

            Random rnd = new Random();

            for (int i = 0; i < crom_lst.Count / 2; i++)
            {
                string[] output = new string[2];

                for (int j = 0; j < 2; j++)
                {
                    int k = rnd.Next(0, crom_lst.Count);
                    foreach (var item in output)
                    {
                        while (item == crom_lst[k])
                        {
                            k = rnd.Next(0, crom_lst.Count);
                        }
                    }
                    output[j] = crom_lst[k];
                }


                //Get parent crom to crossover
                foreach (string two_crom_lst in output.Take(2))
                {
                    crossover_list.Add(two_crom_lst);
                }

                //Get parent just to select the best between parent and child after crossover
                foreach (string two_parent_crom_lst in output.Take(2))
                {
                    crossover_list.Add(two_parent_crom_lst);
                }

                int cut_position = rnd.Next(1, dataGridView1.ColumnCount);

                string crom_a = get_sub_string(crossover_list[0], cut_position);
                string crom_b = get_sub_string(crossover_list[1], cut_position);

                string crossovera = crossover_list[0].Replace(crom_a, crom_b);
                string crossoverb = crossover_list[1].Replace(crom_b, crom_a);


                crossover_list[0] = check_and_replace_duplicate(crossovera);
                crossover_list[1] = check_and_replace_duplicate(crossoverb);

                //check mutation
                crossover_list[0] = check_mutation(crossover_list[0],Convert.ToDouble(textBox3.Text),rnd);
                crossover_list[1] = check_mutation(crossover_list[1], Convert.ToDouble(textBox3.Text), rnd);

                Cromosome_list[] array = new Cromosome_list[]
	            {
	                new Cromosome_list(){Cromosome_fitness = calc_fitness(crossover_list[0]), Cromosome = crossover_list[0]},
	                new Cromosome_list(){Cromosome_fitness = calc_fitness(crossover_list[1]), Cromosome = crossover_list[1]},
	                new Cromosome_list(){Cromosome_fitness = calc_fitness(crossover_list[2]), Cromosome = crossover_list[2]},
	                new Cromosome_list(){Cromosome_fitness = calc_fitness(crossover_list[3]), Cromosome = crossover_list[3]}
	            };

                // Highest Cromosome_fitness.
                var result = from em in array
                             orderby em.Cromosome_fitness
                             select em;

                foreach (var em in result.Take(2))
                {
                    crossover_result.Add(em.Cromosome);
                }

                crossover_list.Clear();
            }
            return crossover_result.ToArray();
        }

        public string get_sub_string(string cromosome, int cut_position)
        {
            var array = cromosome.Split('-');

            //double cut_position = rnd.Next(1, array.Length - 1);
            //cut_position = Math.Round(cut_position);
            
            List<string> crom_part_lst = new List<string>(array);
            cromosome = string.Join("-", crom_part_lst.GetRange(0, Convert.ToInt32(cut_position)).ToArray());
            
            return cromosome;
        }

        public string check_and_replace_duplicate(string cromosome)
        {
            List<string> list = new List<string>(cromosome.Split('-'));
            string new_cromosome = null;
            IEnumerable<int> duplicates = null;

            foreach (var item in list.ToArray())
            {
                duplicates = list.Select((value, index) => new { value, index })
                                    .Where(a => string.Equals(a.value, item))
                                    .Select(a => a.index);

                if (duplicates != null)
                {
                    duplicates = duplicates.ToArray();

                    for (int i = 1; i <= list.Count; i++)
                    {
                        if (!list.Contains(i.ToString()))
                        {
                            foreach (var it in duplicates)
                            {
                                list[it] = i.ToString();
                                break;
                            }
                        }
                    }
                    duplicates = null;
                }
            }
            new_cromosome = string.Join("-", list);
            return new_cromosome;
        }


        public string check_mutation(string cromosome, double valeur_mutation, Random rnd)
        {
            if ((rnd.NextDouble() * (1 - 0)) + 0 < valeur_mutation)
            {
                List<string> list = new List<string>(cromosome.Split('-'));

                int first_position  = rnd.Next(0, list.Count);
                int second_position = rnd.Next(0, list.Count);

                while (first_position == second_position)
                {
                    second_position = rnd.Next(0, list.Count);
                }

                string old_first_position_value  = list[first_position];
                string old_second_position_value = list[second_position];

                list.RemoveAt(first_position);
                list.Insert(first_position, old_second_position_value);

                list.RemoveAt(second_position);
                list.Insert(second_position, old_first_position_value);

                cromosome = string.Join("-", list.ToArray());
                return cromosome;
            }
            return cromosome;
        }

        public string get_edd_cromosome(int cromosome_lenth, DataGridViewRowCollection row)
        {
            string cromosome;
            List<Edd> array_best_edd_gen = new List<Edd>();

            for (int i = 0; i < cromosome_lenth; i++)
            {
                array_best_edd_gen.Add(new Edd() { Di_value = Convert.ToInt32(row[2].Cells[i].Value), Di_position = i + 1 });                
            }

            // .
            var result = from em in array_best_edd_gen
                         orderby em.Di_value
                         select em;

            List<string> cromosome_array = new List<string>();
            foreach (var em in result)
            {
                cromosome_array.Add(em.Di_position.ToString()); 
            }
            cromosome = string.Join("-", cromosome_array);

            return cromosome;
        }

        public string taux_amelioration(int init_fitness,int ga_fitness)
        {
            double taux = 0;

            if (init_fitness > ga_fitness)
            {
                taux = init_fitness - ga_fitness;
            }
            else
            {
                taux = ga_fitness - init_fitness;
            }

            taux = Math.Round((taux / init_fitness), 2);
            taux = taux * 100;
            return taux.ToString();
        }

        class Cromosome_list
        {
            public int Cromosome_fitness { get; set; }
            public string Cromosome { get; set; }
        }

        class Edd
        {
            public int Di_value { get; set; }
            public int Di_position { get; set; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int population = int.Parse(textBox1.Text);
            int columncount = dataGridView1.ColumnCount;

            string[] a = new string[population];
            string crom_part = rnd.Next(1, columncount + 1).ToString();
            int progress_val = population / 100;
            progressBar1.Maximum = population + progress_val;
            progressBar1.Value = 1;

            for (int i = 0; i < population; i++)
            {
                string[] b = new string[columncount];
                string cromosome = "";
                bool exist_cromosome = true;

                while (exist_cromosome)
                {
                    for (int k = 0; k < b.Length; k++)
                    {
                        b[k] = "";
                    }
                    exist_cromosome = false;
                    cromosome = "";
                    for (int j = 0; j < columncount; j++)
                    {
                        while (b.Contains(crom_part))
                        {
                            crom_part = rnd.Next(1, columncount + 1).ToString();
                        }
                        b[j] = crom_part;
                    }

                    cromosome = string.Join("-", b);

                    foreach (var item in a)
                    {
                        if (item == cromosome)
                        {
                            exist_cromosome = true;
                            break;
                        }
                        else
                        {
                            exist_cromosome = false;
                        }
                    }
                }

                a[i] = cromosome;
                cromosome = "";
                Thread.Sleep(10);
                progressBar1.PerformStep();
                int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)14, FontStyle.Bold), Brushes.White, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
            }

            List<Cromosome_list> array_population = new List<Cromosome_list>();

            foreach (var item in a)
            {
                array_population.Add(new Cromosome_list() { Cromosome_fitness = calc_fitness(item), Cromosome = item });
            }

            // Highest Cromosome_fitness.
            var result = from em in array_population
                         orderby em.Cromosome_fitness
                         select em;

            foreach (var em in result.Take(1))
            {
                label5.Text = em.Cromosome_fitness.ToString();
                int init_fit = em.Cromosome_fitness;
            }

            int deacrease = Convert.ToInt32(textBox2.Text) - 1;

            for (int i = 1; i <= Convert.ToInt32(textBox2.Text); i++)
            {
                a = crossover(a);

                List<Cromosome_list> array_best_gen = new List<Cromosome_list>();

                foreach (var item in a)
                {
                    array_best_gen.Add(new Cromosome_list() { Cromosome_fitness = calc_fitness(item), Cromosome = item });
                }

                // Highest Cromosome_fitness.
                var result_two = from em in array_best_gen
                                 orderby em.Cromosome_fitness
                                 select em;

                foreach (var em in result_two.Take(1))
                {
                    label7.Text = em.Cromosome_fitness.ToString();
                    richTextBox1.Text = em.Cromosome;
                }

                int te1 = Convert.ToInt32(label5.Text);
                int te2 = Convert.ToInt32(label7.Text);

                label9.Text = taux_amelioration(te1, te2) + " %";

                string add_sequence = get_edd_cromosome(columncount, dataGridView1.Rows);
                label13.Text = calc_fitness(add_sequence).ToString();

                int te3 = Convert.ToInt32(label13.Text);
                int te4 = Convert.ToInt32(label5.Text);

                label14.Text = taux_amelioration(te3, te4) + " %";
                richTextBox2.Text = add_sequence;

                textBox4.Text = deacrease.ToString();

                deacrease--;
                Update();

                Thread.Sleep(100);
                Update();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefToForm1.Show();
            this.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefToForm1.Show();
            this.Dispose();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.RefToForm1.Show();
            this.Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Text = "GASoft";
            about.labelProductName.Text = "Nom de produit : GASoft";
            about.labelCopyright.Text = "Tous les droits réservés";
            about.labelCompanyName.Text = "Au nom de :";
            about.ShowInTaskbar = false;
            about.ShowDialog();
        }

    }
}
