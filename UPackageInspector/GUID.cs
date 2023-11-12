using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPackageInspector
{
    public class GUID
    {
        public string ID = "";
        public string alternateID = "";

        public GUID() {
            if (alternateID == "")
            {
                alternateID = Guid.NewGuid().ToString("N");
            }
        }

        public static List<GUID> CollectDefinedGUID()
        {
            List<GUID> guids = new List<GUID>();
            return guids;
        }

        public static string findGuid(string path)
        {
            string guid = "";

            StreamReader sr = new StreamReader(path + "\\asset.meta", Encoding.UTF8);
            string str = sr.ReadToEnd();
            sr.Close();

            guid = str.Substring(str.IndexOf("guid:") + 6, 32);
            return guid;
        }
    }
}
