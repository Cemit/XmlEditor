using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace XmlEditorForm
{
    static class Xml
    {
        static public bool Write(StructEnum @struct, string path, object obj)
        {
            if(File.Exists(path))
                File.Delete(path);
            XmlSerializer xmlSerializer = PublicStruct.GetXmlSerializer(@struct);
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            xmlSerializer.Serialize(stream, obj);
            stream.Close();
            return true;
        }

        static public object Read(StructEnum @struct, string path)
        {
            object ret = null;
            XmlSerializer xmlSerializer = PublicStruct.GetXmlSerializer(@struct);
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            ret = xmlSerializer.Deserialize(stream);
            stream.Close();
            return ret;
        }

    }
}
