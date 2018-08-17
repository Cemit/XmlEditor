using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        static bool _enum2Struct_isCreate = false;
        static Type[] _enum2Struct;
        public static Type Enum2Struct(StructEnum @struct)
        {
            if (!_enum2Struct_isCreate)
            {
                _enum2Struct = new Type[Enum.GetValues(typeof(StructEnum)).Length];
                _enum2Struct[(int)StructEnum.Fruits] = typeof(Fruits);
                _enum2Struct[(int)StructEnum.Computers] = typeof(Computers);
                _enum2Struct_isCreate = true;
            }
            Type type = _enum2Struct[(int)@struct];
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
            Type type = Enum2Struct(@struct);
            Type childType = GetChildStruct(type);
            object obj = Activator.CreateInstance(type);
            object[] childObj = new object[dataTable.Rows.Count];
            int i;
            for (i = 0; i < childObj.Length; i++)
            {
                childObj[i] = Activator.CreateInstance(childType);
            }
            i = 0;
            foreach (DataRow item in dataTable.Rows)
            {
                FieldInfo[] fieldInfos = childType.GetFields();
                int j = 0;
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    fieldInfo.SetValue(childObj[i], Convert.ChangeType(item[j++], fieldInfo.FieldType));
                }
                i++;
            }
            FieldInfo[] parentFieldInfos = type.GetFields();
            if (parentFieldInfos.Length != 1)
                throw new Exception("PublicStruct:DataTable2xmlObj 错误的类型" +
                    type.ToString());
            MethodInfo methodInfo = type.GetMethod("SetValue");
            methodInfo.Invoke(obj, new object[] { childObj });
            return obj; 

            /**
            switch (@struct)
            {
                case StructEnum.Fruits:

                    Fruit[] fruits = new Fruit[dataTable.Rows.Count];
                    i = 0;
                    foreach (DataRow item in dataTable.Rows)
                    {

                        Fruit fruit = new Fruit()
                        {
                            id = Convert.ToInt32(item[0]),
                            name = (string)item[1]
                        };
                        fruits[i++] = fruit;
                    }
                    return new Fruits() { fruit = (object[])fruits };
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
            **/
        }

        public static XmlSerializer GetXmlSerializer(StructEnum @struct)
        {
            XmlSerializer ret;
            ret = new XmlSerializer(Enum2Struct(@struct));
            return ret;
        }

        //将Byte转换为结构体类型
        public static byte[] StructToBytes(object structObj, int size)
        {
            int num = 2;
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }

        //将Byte转换为结构体类型
        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            //分配结构体内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public static Type GetChildStruct(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields();
            if (fieldInfos.Length != 1)
                throw new Exception("PublicStruct:GetChildStruct 格式错误的结构体 " +
                    type.ToString());
            return fieldInfos[0].FieldType.GetElementType();
        }

    }

    public struct Fruits
    {
        public Fruit[] fruit;

        public void SetValue(object[] obj)
        {
            Fruit[] f = new Fruit[obj.Length];
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (Fruit)obj[i];
            }
            fruit = f;
        }
    };

    public struct Fruit
    {
        public int id;
        public string name;

        //public Fruit GetValue()
        //{
        //    return new Fruit() { id = id, name = name };
        //}
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
