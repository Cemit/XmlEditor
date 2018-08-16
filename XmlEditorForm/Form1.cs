using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlEditorForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Manager.Create(MassageLabel, structComboBox1, dataGridView1);
            Fruit[] obj = new Fruit[]
                {
                    new Fruit(){ id = 0, name = "apple" },
                    new Fruit(){ id = 1, name = "banana" }
                };
            Fruits fruits = new Fruits()
            {
                fruit = obj
            };
            Xml.Write(StructEnum.Fruits,"fruit.xml", fruits);

            FieldInfo[] fieldInfos = typeof(Object).GetFields();
            foreach (FieldInfo item in fieldInfos)
            {
                FieldInfo[] f = item.FieldType.GetElementType().GetFields();
                Console.WriteLine(item.FieldType);
                Console.WriteLine(f.Length);

                foreach (var i in f)
                {
                    Console.WriteLine(i);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Manager.GetManager().OpenFile(openFileDialog1);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Manager.GetManager().SaveFile();
        }
    }
}
