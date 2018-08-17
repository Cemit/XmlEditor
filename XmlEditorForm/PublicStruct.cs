using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlEditorForm
{
    public enum StructEnum
    {
        Fruits, Computers
    }
    public class PublicStruct
    {
        public static Type Enum2Struct(StructEnum @struct)
        {
            Type type;
            switch (@struct)
            {
                case StructEnum.Fruits:
                    type = typeof(Fruits);
                    break;
                case StructEnum.Computers:
                    type = typeof(Computers);
                    break;
                default:
                    throw new Exception("PublicStruct:Enum2Struct 错误的类型：" 
                        + @struct.ToString());
            }
            return type;
        }

        public static object[][] XmlObj2Array(StructEnum @struct, object obj)
        {
            object[][] arrays;
            switch (@struct)
            {
                case StructEnum.Fruits:
                    Fruits fruits = (Fruits)obj;
                    arrays = new object[fruits.fruit.Length][];
                    int i = 0;
                    foreach (Fruit item in fruits.fruit)
                    {
                        arrays[i++] = new object[] { item.id,item.name };
                    }
                    break;
                case StructEnum.Computers:
                    Computers computers = (Computers)obj;
                    arrays = new object[computers.computer.Length][];
                    i = 0;
                    foreach (Computer item in computers.computer)
                    {
                        arrays[i++] = new object[] { item.id, item.name, item.color };
                    }
                    break;
                default:
                    throw new Exception("PublicStruct:XmlObjToArray 错误的类型：" 
                        + @struct.ToString());
            }
            return arrays;
        }

        public static object DataTable2xmlObj(StructEnum @struct, DataTable dataTable)
        {
            switch (@struct)
            {
                case StructEnum.Fruits:
                    Fruit[] fruits = new Fruit[dataTable.Rows.Count];
                    int i = 0;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        Fruit fruit = new Fruit()
                        {
                            id = Convert.ToInt32(item[0]),
                            name = (string)item[1]
                        };
                        fruits[i++] = fruit;
                    }
                    return new Fruits() { fruit = fruits };
                case StructEnum.Computers:

                    Computer[] computers = new Computer[dataTable.Rows.Count];
                    i = 0;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        Computer computer = new Computer()
                        {
                            id = Convert.ToInt32(item[0]),
                            name = (string)item[1],
                            color = Convert.ToInt32(item[2]),
                        };
                        computers[i++] = computer;
                    }
                    return new Computers() { computer = computers };
                default:
                    throw new Exception("PublicStruct:dataTable2xmlObj 错误的类型："
                     + @struct.ToString());
            }
        }

        public static XmlSerializer GetXmlSerializer(StructEnum @struct)
        {
            XmlSerializer ret;
            ret = new XmlSerializer(Enum2Struct(@struct));
            return ret;
        }
    }

    public struct Fruits
    {
        public Fruit[] fruit;
    };
    public struct Fruit
    {
        public int id;
        public string name;
    };

    public struct Computers
    {
        public Computer[] computer;
    }
    public struct Computer
    {
        public int id;
        public string name;
        public int color;
    }
}
