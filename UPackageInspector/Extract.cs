using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPackageInspector
{
    public class Extract
    {
        static public void ExtractAssets()
        {
            int i = 0;
            FolderManager.CreateExtractFolder();
            foreach (Asset asset in Asset.assets)
            {
                i++;
                Console.CursorLeft = 0;
                Console.Write(
                    "アセットを抽出しています...\t" +
                    ((i / (float)Asset.assets.Count) * 100.0f).ToString("f0") + "%");

                string binaryFullPath = asset.fullPath + "\\";
                string extractFullPath = FolderManager.extractFolderPath + "\\" + asset.PathName.Replace("/", "\\");

                Directory.CreateDirectory(Path.GetDirectoryName(extractFullPath));
                if (File.Exists(binaryFullPath + "asset"))
                {
                    File.Copy(binaryFullPath + "asset", extractFullPath, true);
                }
                if (File.Exists(binaryFullPath + "asset.meta"))
                {
                    File.Copy(binaryFullPath + "asset.meta", extractFullPath + ".meta", true);
                }

            }
        }
    }
}
