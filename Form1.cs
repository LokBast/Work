using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Work
{
    public partial class Form1 : Form
    {
        int i = 0;
        public Form1()
        {
            InitializeComponent();
            RadioButton MyRadioButton;
            TextBox txt;
            Label MyLabel;

            int Xr = 12, Yr = 25, Xb = 110, Yb = 21, Xl = 12, Yl = 25;

            foreach (PropertyInfo prop in typeof(DataObject).GetProperties())
            {
                //СОЗДАНИЕ СТОЛБЦОВ              
                listView1.Columns.Add(prop.Name, 100);
           

                //СОЗДАНИЕ РАДИО КНОПКИ
                MyRadioButton = new RadioButton();
                MyRadioButton.Click += RadioButtonSelect;
                MyRadioButton.Tag = i;
                MyRadioButton.Name = "Field" + i;
                MyRadioButton.Text = "" + prop.Name;
                MyRadioButton.Location = new Point(Xr, Yr);
                MyRadioButton.AutoSize = true;
                groupBox1.Controls.Add(MyRadioButton);
                Yr += 25;


                //СОЗДАНИЕ ТЕКСТОВОГО ПОЛЯ
                txt = new TextBox();
                txt.Leave += LeaveTxt;
                txt.Enter += EnterTxt;
                txt.Tag = i;
                txt.Name = "textBox" + i;
                txt.Text = "";
                txt.Location = new Point(Xb, Yb);
                txt.AutoSize = true;
                groupBox2.Controls.Add(txt);
                Yb += 25;


                //СОЗДАНИЕ ЛЕЙБЛА
                MyLabel = new Label();
                MyLabel.Name = "Label" + i;
                MyLabel.Text = "" + prop.Name;
                MyLabel.Location = new Point(Xl, Yl);
                MyLabel.AutoSize = true;
                groupBox2.Controls.Add(MyLabel);
                Yl += 25;


                i++;
            }

            Type t = typeof(DataObject); int j = 0;
            foreach (var prop in t.GetProperties())
            {
                groupBox2.Controls.Find("Label" + j, false)[0].Text = "" + prop.Name + " " + prop.PropertyType.Name;
                j++;
            }
        }

        public class DataObject
        {
            private string _field10;
            private DateTime _field3;
            private int _field6;
            private long _field8;

            public string Field10
            {
                get { return _field10; }
                set { _field10 = value; }
            }

            public DateTime Field3
            {
                get { return _field3; }
                set { _field3 = value; }
            }

            public int Field6
            {
                get { return _field6; }
                set { _field6 = value; }
            }

            public long Field8
            {
                get { return _field8; }
                set { _field8 = value; }
            }

            public decimal Field7 { get; set; }
        }




        //ДОБАВЛЕНИЕ ДАННЫХ В LISTVIEW
        DataObject dataObject = new DataObject();
        private void Button1_Click(object sender, EventArgs e)
        {
            ListViewItem item1 = new ListViewItem("" + (groupBox2.Controls.Find("TextBox0", false))[0].Text);
            for (int m = 1; m < i; m++) { item1.SubItems.Add("" + (groupBox2.Controls.Find("TextBox" + m, false))[0].Text); }

            listView1.Items.AddRange(new ListViewItem[] { item1 });
            sort();
        }




        //ЗАМЕНА ДАННЫХ ЧЕРЕЗ ТЕКСТБОКС
        string txtEnt; int ent;
        public void EnterTxt(object sender, EventArgs e)
        {
            PropertyInfo[] props = typeof(DataObject).GetProperties();
            for (int v = 0; v < i; v++)
            {
                if (groupBox2.Controls.Find("TextBox" + v, false)[0].Focused == true)
                {
                    txtEnt = groupBox2.Controls.Find("TextBox" + v, false)[0].Text;
                    ent = v;
                }
            }
        }
        public void LeaveTxt(object sender, EventArgs e)
        {
            //ПРОВЕРКА ВВОДИМЫХ ДАННЫХ
            PropertyInfo[] props = typeof(DataObject).GetProperties();
            try
            {
                var t = Convert.ChangeType(groupBox2.Controls.Find("TextBox" + ent, false)[0].Text, props[ent].PropertyType);
            }
            catch
            {
                groupBox2.Controls.Find("TextBox" + ent, false)[0].Focus();

                MessageBox.Show(
                "Невозможно привести к типу " + props[ent].PropertyType,
                "Ошибка введенных данных",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                return;
            }


            //ЗАМЕНА ДАННЫХ В LISTVIEW
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                ListViewItem item = listView1.SelectedItems[0];
                for (int j = 0; j < i; j++)
                {
                    item.SubItems[j].Text = groupBox2.Controls.Find("TextBox" + j, false)[0].Text;
                }
            }
            sort();
        }





        //ПЕРЕНОС ДАННЫХ ТАБЛИЦЫ В ТЕКСТБОКС
        int ind;
        public void ListView1_ItemSelectionChanged_1(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ind = e.ItemIndex;

            for (int j = 0; j < i; j++)
            {
                if (((RadioButton)groupBox1.Controls[j]).Checked == true) { g = j; }
            }
            this.listView1.ListViewItemSorter = new ListViewItemComparer(g, listView1.Sorting);
        }
        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                ListViewItem item = listView1.SelectedItems[0];

                for (int j = 0; j < i; j++) { groupBox2.Controls.Find("TextBox" + j, false)[0].Text = (item.SubItems[j].Text); }
            }
        }



        //УДАЛЕНИЕ СТРОКИ
        private void Button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(eachItem);
            }
            sort();
        }



        //СОРТИРОВКА
        int g = 0;
        public void sort()
        {
            ListView lv1 = new ListView();
            for (int j = 0; j < i; j++)
            {
                if (((RadioButton)groupBox1.Controls[j]).Checked == true) { g = j; }
            }
            this.listView1.ListViewItemSorter = new ListViewItemComparer(g, listView1.Sorting);
        }

        public void RadioButtonSelect(object sender, EventArgs e)
        {
            sort();
        }

        public class ListViewItemComparer : IComparer
        {

            private int col;
            private SortOrder order;
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                return returnVal;
            }
        }
    }
}