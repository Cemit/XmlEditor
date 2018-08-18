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
        //添加完枚举
        //还需要修改Enum2Struct方法
        //父结构体中必须有SetValue方法和GetValue方法
        Fruits, Computers, ArmsDatas
    }
    public static class PublicStruct
    {
        static bool _enum2Struct_isCreate = false;
        static Type[] _enum2Struct;
        
        public static Type Enum2Struct(this StructEnum @struct)
        {
            if (!_enum2Struct_isCreate)
            {
                _enum2Struct = new Type[Enum.GetValues(typeof(StructEnum)).Length];
                _enum2Struct[(int)StructEnum.Fruits] = typeof(Fruits);
                _enum2Struct[(int)StructEnum.Computers] = typeof(Computers);
                _enum2Struct[(int)StructEnum.ArmsDatas] = typeof(ArmsDatas);

                _enum2Struct_isCreate = true;
            }
            Type type = _enum2Struct[(int)@struct];
            return type;
        }

        public static object[][] XmlObj2Array(StructEnum @struct, object obj)
        {
            object[][] arrays;
            Type type = @struct.Enum2Struct();
            Type childType = type.GetChildStruct();
            FieldInfo[] fieldInfos = type.GetFields();
            if (fieldInfos.Length != 1)
            {
                throw new Exception("PublicStruct.XmlObj2Array: " +
                    "错误的类型" + @struct.ToString());
            }

            MethodInfo methodInfo = type.GetMethod("GetValue");
            object[] childArray = (object[])methodInfo.Invoke(obj, null);

            arrays = new object[childArray.Length][];
            int i = 0;
            foreach (var item in childArray)
            {
                fieldInfos = childType.GetFields();
                object[] array = new object[fieldInfos.Length];
                int j = 0;
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    array[j++] = fieldInfo.GetValue(item);
                }
                arrays[i++] = array; 
            }
            return arrays;
            /*
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
            */
        }

        public static object CreateObj(StructEnum @struct)
        {
            Type type = Enum2Struct(@struct);
            Type childType = GetChildStruct(type);
            object obj = Activator.CreateInstance(type);
            object[] childObj = new object[1] { Activator.CreateInstance(childType) };

            FieldInfo[] fieldInfos = childType.GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                fieldInfo.SetValue(childObj[0], Convert.ChangeType(0, fieldInfo.FieldType));
            }


            MethodInfo methodInfo = type.GetMethod("SetValue");
            methodInfo.Invoke(obj, new object[] { childObj });
            return obj;
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

        public static Type GetChildStruct(this Type type)
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

        public object[] GetValue()
        {
            object[] obj = new object[fruit.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i] = fruit[i];
            }
            return obj;
        }
    };

    public struct Fruit
    {
        public int id;
        public string name;
    };

    public struct Computers
    {
        public Computer[] computer;

        public void SetValue(object[] obj)
        {
            Computer[] f = new Computer[obj.Length];
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (Computer)obj[i];
            }
            computer = f;
        }

        public object[] GetValue()
        {
            object[] obj = new object[computer.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i] = computer[i];
            }
            return obj;
        }
    }
    public struct Computer
    {
        public int id;
        public string name;
        public int color;
    }

    public struct ArmsDatas
    {
        public ArmsData[] armsDatas;
        public void SetValue(object[] obj)
        {
            ArmsData[] f = new ArmsData[obj.Length];
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (ArmsData)obj[i];
            }
            armsDatas = f;
        }

        public object[] GetValue()
        {
            object[] obj = new object[armsDatas.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i] = armsDatas[i];
            }
            return obj;
        }
    }

    public struct ArmsData
    {
        public int id;
        public string name;
        public string attack; //攻击力
        public string protect; //防御力
    }
}
