using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlEditorForm
{
    class Manager
    {
        static Manager managerObj;
        static bool isCreate = false;

        string dataPath = string.Empty;
        StructEnum DataType
        {
            get => (StructEnum)typeComboBox.SelectedIndex;
        }

        ToolStripComboBox typeComboBox;
        ToolStripLabel massageBox;
        DataGridView dataGrid;

        string _massage = "初始化";
        public string Massage
        {
            get => _massage;
            private set
            {
                massageBox.Text = _massage = value;
            }
        }

        Manager() { }

        public static Manager Create(ToolStripLabel massageBox, ToolStripComboBox typeComboBox, DataGridView dataGrid)
        {
            if (!isCreate)
            {
                isCreate = true;
                managerObj = new Manager
                {
                    massageBox = massageBox,
                    typeComboBox = typeComboBox,
                    dataGrid = dataGrid,
                    Massage = "就绪"
                };
                managerObj.typeComboBox.Items.AddRange(Enum.GetNames(typeof(StructEnum)));

            }
            return managerObj;
        }

        public static Manager GetManager()
        {
            if (!isCreate)
            {
                throw new Exception("Manager:GetManager " +
                    "未使用Create对单例进行初始化");
            }
            return managerObj;
        }

        public bool OpenFile(OpenFileDialog fileDialog)
        {
            if (typeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择结构类型！");
                return false;
            }
            bool ret;
            string oldMassage = Massage;
            Massage = "文件选择对话框已开启";
            string path = fileDialog.FileName;
            fileDialog.ShowDialog();
            ret = path == fileDialog.FileName && path != dataPath;
            if (ret)
            {
                path = string.Empty;
                Massage = oldMassage;
            }
            else
            {
                path = fileDialog.FileName;
                Massage = "成功导入文件：" + Path.GetFileName(path);
                dataPath = path;
                CreateForm();
            }
            return !ret;
        }

        void CreateForm()
        {
            object obj = Xml.Read(DataType, dataPath);
            dataGrid.Columns.Clear();

            Type sturctType = PublicStruct.Enum2Struct(DataType);

            DataTable dataTable = new DataTable();

            FieldInfo[] fieldInfos = PublicStruct.GetChildStruct(sturctType).GetFields();

            foreach (var item in fieldInfos)
            {
                dataTable.Columns.Add(item.Name);
            }
            

            object[][] objArray  = PublicStruct.XmlObj2Array(DataType, obj);

            foreach (var item in objArray)
            {
                dataTable.Rows.Add(item);
            }

            dataGrid.DataSource = dataTable;
        }

        public void SaveFile()
        {
            if (typeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择结构类型！");
                return;
            } else if (dataPath == string.Empty)
            {
                MessageBox.Show("请先选择文件！");
                return;
            }
            
            DataTable dataTable = (DataTable)dataGrid.DataSource;
            object obj = PublicStruct.DataTable2xmlObj(DataType, dataTable);
            bool isSuccess = Xml.Write(DataType, dataPath, obj);
            string messageStr = isSuccess ? "保存成功！" : "保存失败";
            MessageBox.Show(messageStr);
        }
    }
}
