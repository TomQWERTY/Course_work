using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FloydForms
{
    struct MatrixElement
    {
        public int edge_weight, num_of_symbols;
        public string start_of_hint;
        public bool changed;
        public MatrixElement(int edge_weight_)
        {
            edge_weight = edge_weight_;
            changed = false;
            start_of_hint = "";
            num_of_symbols = 1;
        }
        public override string ToString()
        {
            return "<td>" + start_of_hint + (changed ? ("*") : "") +
                (edge_weight < int.MaxValue ? ("" + edge_weight) : "∞") +
                (changed ? ("*") : "") + (changed ? ("</strong></span>") : "") + "</td>";
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static int Plus(int a, int b)               //функція для суми шоб уникнуть переповнення інта
        {
            if (a == int.MaxValue || b == int.MaxValue)
            {
                return int.MaxValue;
            }
            else
            {
                return a + b;
            }
        }

        static int CountSymbols(int a)   //кількість знаків довжини ребра
        {
            int num_of_symbols_local = 0;
            if (a < 0)
            {
                num_of_symbols_local++;
            }
            if (a == 0 || a == int.MaxValue)
            {
                num_of_symbols_local = 1;
            }
            else
            {
                while (a != 0)
                {
                    a /= 10;
                    num_of_symbols_local++;
                }
            }
            return num_of_symbols_local;
        }

        static int FindMax(MatrixElement[,]matrix)            //пошук найдовшого значення в матриці
        {
            int max_num_of_symbols = 1;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j].num_of_symbols > max_num_of_symbols)
                    {
                        max_num_of_symbols = matrix[i, j].num_of_symbols;
                    }
                }
            }
            return max_num_of_symbols;
        }

        void CreateTable()                                                   //створення таблиці
        {
            try
            {
                dataGridView1.Visible = false;
                int num_of_vertex = Convert.ToInt32(textBox2.Text);
                int current_num_in_table = dataGridView1.Columns.Count;
                for (int i = 0; i < current_num_in_table - num_of_vertex; i++)
                {
                    dataGridView1.Columns.RemoveAt(current_num_in_table - i - 1);
                }
                for (int i = 0; i < current_num_in_table - num_of_vertex; i++)
                {
                    dataGridView1.Rows.RemoveAt(current_num_in_table - i - 1);
                }
                current_num_in_table = dataGridView1.Columns.Count;
                for (int i = current_num_in_table; i < num_of_vertex; i++)
                {
                    dataGridView1.Columns.Add("" + i, "");
                    dataGridView1.Columns[i].Width = 50;
                }
                if (num_of_vertex - current_num_in_table > 0)
                {
                    dataGridView1.Rows.Add(num_of_vertex - current_num_in_table);
                }
                for (int i = 0; i < num_of_vertex; i++)
                {
                    dataGridView1.Rows[i].Cells[i].Value = 0;
                    dataGridView1.Rows[i].Cells[i].ReadOnly = true;
                }
                dataGridView1.Width = num_of_vertex * 50 + 5;
                dataGridView1.Height = num_of_vertex * 22 + 5;
                this.MaximumSize = DefaultMaximumSize;
                this.Height = 200 + dataGridView1.Height;
                if (num_of_vertex > 7)
                {
                    this.Width = 34 + dataGridView1.Width;
                }
                else
                {
                    this.Width = 417;
                }
                this.MaximumSize = this.Size;
                dataGridView1.Visible = true;
                label3.Text = "To mark an infinity, type \"&&&\"";
                button3.Visible = true;
                button2.Visible = true;
                button1.Visible = true;
            }
            catch
            {
                MessageBox.Show("You entered incorrect number of vertex!");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)     //щоб кнопки не були активні коли нічого не введено
        {
            if (textBox2.Text != "")
            {
                button4.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
            }
        }

        private void button4_MouseClick(object sender, MouseEventArgs e)     //клік на create table
        {
            CreateTable();
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)      //клік на start
        {
            try
            {
                int num_of_vertex = dataGridView1.Columns.Count;
                MatrixElement[,] matrix = new MatrixElement[num_of_vertex, num_of_vertex];
                for (int i = 0; i < num_of_vertex; i++)
                {
                    for (int j = 0; j < num_of_vertex; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString() == "&")
                        {
                            matrix[i, j] = new MatrixElement(int.MaxValue);
                        }
                        else
                        {
                            matrix[i, j] = new MatrixElement(Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value.ToString()));
                        }
                    }
                }
                DoIt(matrix);
            }
            catch
            {
                MessageBox.Show("Something went wrong. Maybe you entered wrong values into table or left it empty. Please enter only numbers or \"&\" and don't left cells empty.");
            }
        }

        private void button5_Click(object sender, EventArgs e)   //клік на import
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string address = openFileDialog1.FileName;
                    StreamReader strrd = new StreamReader(address);
                    int num_of_vertex = Convert.ToInt32(strrd.ReadLine());
                    textBox2.Text = Convert.ToString(num_of_vertex);
                    CreateTable();
                    for (int i = 0; i < num_of_vertex; i++)
                    {
                        string[] temp = strrd.ReadLine().Split(' ');
                        for (int j = 0; j < num_of_vertex; j++)
                        {
                            dataGridView1.Rows[i].Cells[j].Value = temp[j];
                        }
                    }
                    strrd.Close();
                }
                catch
                {
                    MessageBox.Show("You have choosed incorrect file!");
                }
            }
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)    //клік на clean table
        {
            int num_of_vertex = dataGridView1.Columns.Count;
            for (int i = 0; i < num_of_vertex; i++)
            {
                for (int j = 0; j < num_of_vertex; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = "";
                }
                dataGridView1.Rows[i].Cells[i].Value = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)   //клік на export
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string address = saveFileDialog1.FileName;
                    StreamWriter strwr = new StreamWriter(address);
                    int num_of_vertex = dataGridView1.Columns.Count;
                    strwr.WriteLine(num_of_vertex);
                    for (int i = 0; i < num_of_vertex; i++)
                    {
                        for (int j = 0; j < num_of_vertex; j++)
                        {
                            strwr.Write(dataGridView1.Rows[i].Cells[j].Value + " ");
                        }
                        strwr.WriteLine();
                    }
                    strwr.Close();
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Maybe you entered wrong values into table or left it empty. Please enter only numbers or \"&\" and don't left cells empty.");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)    //клік на read directly from file
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader strrd = new StreamReader(openFileDialog1.FileName);
                    int num_of_vertex = Convert.ToInt32(strrd.ReadLine());
                    MatrixElement[,] matrix = new MatrixElement[num_of_vertex, num_of_vertex];
                    for (int i = 0; i < num_of_vertex; i++)
                    {
                        string[] temp = strrd.ReadLine().Split(' ');
                        for (int j = 0; j < num_of_vertex; j++)
                        {
                            if (temp[j] == "&")
                            {
                                matrix[i, j] = new MatrixElement(int.MaxValue);
                            }
                            else
                            {
                                matrix[i, j] = new MatrixElement(Convert.ToInt32(temp[j]));
                            }
                        }
                    }
                    strrd.Close();
                    DoIt(matrix);
                }
                catch
                {
                    MessageBox.Show("You have choosed incorrect file!");
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog2.ShowDialog();
        }

        void DoIt(MatrixElement[,] matrix)          //основна частина
        {
            int num_of_vertex = matrix.GetLength(0);
            int max_num_of_symbols = 1;
            for (int i = 0; i < num_of_vertex; i++)
            {
                for (int j = 0; j < num_of_vertex; j++)
                {
                    matrix[i, j].num_of_symbols = CountSymbols(matrix[i, j].edge_weight);
                    if (matrix[i, j].num_of_symbols > max_num_of_symbols)
                    {
                        max_num_of_symbols = matrix[i, j].num_of_symbols;
                    }
                }
            }

            string address = saveFileDialog2.FileName;
            StreamWriter strwr = new StreamWriter(address);
            strwr.Write("<html>\n<head>\n\t<meta charset = \"utf-8\">\n\t<title>Застосування алгоритму Флойда</title>\n</head><body>\n\n");
            strwr.Write("<kbd><big>Початковий вигляд матрицi, пiдготовленої для алгоритма Флойда:<br>");       //<kbd> - моноширинний шрифт, <big> - великий шрифт
            strwr.Write("\n<table border=\"1\">\n<col span = \"" + num_of_vertex + "\" width = \"" + max_num_of_symbols * 9 + "\">\n");

            for (int a = 0; a < num_of_vertex; a++)
            {
                strwr.Write("<tr>\n");
                for (int b = 0; b < num_of_vertex; b++)
                {
                    strwr.Write(matrix[a, b] + "\t");
                }
                strwr.Write("\n</tr>\n");
            }
            strwr.Write("\n</table>\n");

            for (int k = 0; k < num_of_vertex; k++)
            {
                strwr.Write("<p>\n\tВигляд матрицi наприкiнцi " + (k + 1) + "-ої iтерацiї алгоритму Флойда:\n</p>");
                for (int i = 0; i < num_of_vertex; i++)
                {
                    for (int j = 0; j < num_of_vertex; j++)
                    {
                        if (Plus(matrix[i, k].edge_weight, matrix[k, j].edge_weight) < matrix[i, j].edge_weight)
                        {
                            int old_ik = matrix[i, k].edge_weight;
                            int old_kj = matrix[k, j].edge_weight;
                            matrix[i, j].edge_weight = Plus(matrix[i, k].edge_weight, matrix[k, j].edge_weight);
                            matrix[i, j].changed = true;
                            matrix[i, j].start_of_hint = "<span title=\"v[" + i + "]->(" + matrix[i, j].edge_weight + ")->v[" + j + "] отримано з v[" + i + "]->(" + old_ik + ")->v[" + k + "]->(" + old_kj + ")->v[" + j + "]\"><strong>";
                            matrix[i, j].num_of_symbols = CountSymbols(matrix[i, j].edge_weight) + 2;
                        }
                    }
                }

                max_num_of_symbols = FindMax(matrix);
                strwr.Write("\n<table border=\"1\">\n<col span = \"" + num_of_vertex + "\" width = \"" + max_num_of_symbols * 9 + "\">\n");
                for (int a = 0; a < num_of_vertex; a++)
                {
                    strwr.Write("<tr>\n");
                    for (int b = 0; b < num_of_vertex; b++)
                    {
                        strwr.Write(matrix[a, b]);
                        if (matrix[a, b].changed)
                        {
                            matrix[a, b].changed = false;
                            matrix[a, b].start_of_hint = "";
                            matrix[a, b].num_of_symbols -= 2;
                        }
                    }
                    strwr.Write("\n</tr>\n");
                }
                strwr.Write("\n</table>\n");
            }
            strwr.Write("</big></kbd>\n</body>\n</html>");
            strwr.Close();
            System.Diagnostics.Process.Start(saveFileDialog2.FileName);
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button5, "Import matrix from file to table on form");
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button2, "Delete all values from table");
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button1, "Save matrix from table to file");
        }

        private void button6_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button6, "Start algorithm using the matrix from file as an input");
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button4, "Create clear table on form with sizes specified");

        }

        private void button7_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button7, "Choose where output file will be saved");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(button3, "Start algorithm using the matrix from table as an input");
        }
    }
}
