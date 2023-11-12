using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPackageInspector
{

    public class FolderManager
    {
        public static string tempFolderPath = "";
        public static string extractFolderPath = "";
        public static string exportPackagePath = "";

        public static String CreateTempFolder()
        {
            tempFolderPath = Path.Combine(AppContext.BaseDirectory + "Temp\\" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(tempFolderPath);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            return tempFolderPath;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Directory.Delete(tempFolderPath,true);
        }

        public static String CreateExtractFolder()
        {
            extractFolderPath = Path.Combine(
                AppContext.BaseDirectory +
                "Extract\\" +
                Path.GetFileNameWithoutExtension(Package.currentPackage.fileName) +
                "_" +
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
            );

            Directory.CreateDirectory(extractFolderPath);

            return extractFolderPath;
        }

        public static String CreateExportFolder()
        {
            exportPackagePath = Path.Combine(
                AppContext.BaseDirectory +
                "Export\\" +
                Path.GetFileNameWithoutExtension(Package.currentPackage.fileName)
            );

            Directory.CreateDirectory(exportPackagePath);

            return exportPackagePath;
        }
    }
}
