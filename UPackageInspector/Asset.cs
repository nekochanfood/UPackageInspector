using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPackageInspector
{
    public enum SizeUnit
    {
        B,KB,MB,GB
    }
    public class Asset
    {
        public static List<Asset> assets = new List<Asset>();
        public string PathName { get; set; } = "";
        public string fullPath { get; set; } = "";
        public string FileName { get; set; } = "";
        public long FileSize { get; set; } = 0;
        public GUID? Guid { get; set; }
        public bool IsFolder { get; set; } = false;

        public static float ConvertTo(SizeUnit su, long size)
        {
            float f = 0;
            switch (su)
            {
                case SizeUnit.B:
                    f = size;
                    return f;
                case SizeUnit.KB:
                    f = (float)size / 1024;
                    return f;
                case SizeUnit.MB:
                    f = (float)size / 1024 / 1024;
                    return f;
                case SizeUnit.GB:
                    f = (float)size / 1024 / 1024 / 1024;
                    return f;
                default:
                    f = size;
                    return f;
            }
        }

        private static Asset CreateAsset(string assetPath)
        {
            Asset asset = new Asset();
            asset.PathName = GetPathName(assetPath);
            asset.fullPath = assetPath;
            asset.FileName = Path.GetFileName(asset.PathName);
            asset.FileSize = GetAssetSize(assetPath);
            asset.IsFolder = asset.FileSize == 0 ? true : false;
            asset.Guid = new GUID
            {
                ID = GUID.findGuid(assetPath)
            };

            return asset;
        }

        private static long GetAssetSize(string path)
        {
            long size = 0;
            try
            {
                FileInfo fileInfo = new FileInfo(path + "\\asset");
                size = fileInfo.Length;
            }
            catch (FileNotFoundException)
            {
                size = 0;
            }
            
            return size;
        }

        private static string GetPathName(string path)
        {
            StreamReader sr = new StreamReader(path + "\\pathname", Encoding.UTF8);
            string str = sr.ReadToEnd();
            sr.Close();

            return str;
        }

        public static List<Asset> CollectAssets()
        {
            assets = new List<Asset>();
            String[] Directories = Directory.GetDirectories(FolderManager.tempFolderPath);
            int i = 0;
            foreach (String directory in Directories)
            {
                i++;
                Console.CursorLeft = 0;
                Console.Write(
                    "アセットを収集しています...\t" +
                    ((i / (float)Directories.Length) * 100.0f).ToString("f0") + "%");

                Asset asset = Asset.CreateAsset(directory);
                assets.Add(asset);
            }
            Console.WriteLine();
            return assets;
        }

        public static void WriteToConsole(Asset asset)
        {
            Console.WriteLine(
                    $"ファイル名:\t\t{Path.TrimEndingDirectorySeparator(asset.PathName)}\n" +
                    $"GUID:\t\t\t{asset.Guid.ID}\n" +
                    $"ファイルサイズ:\t\t{Math.Ceiling(ConvertTo(SizeUnit.KB, asset.FileSize)) + " KB"}\n" +
                    $"フォルダーかどうか:\t{(asset.IsFolder == true?"はい":"いいえ")}\n");
        }
    }
}
