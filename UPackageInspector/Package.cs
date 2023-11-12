using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace UPackageInspector
{
    public class Package
    {
        public static Package? currentPackage;
        public string pathName { get; set; } = "";
        public string fileName { get; set; } = "";
        public long fileSize { get; set; } = 0;

        public static Package LoadPackageFromPath(string path)
        {
            Package package = new Package();
            package.pathName = path;
            package.fileName = Path.GetFileName(path);

            FileInfo fileInfo = new FileInfo(path);
            package.fileSize = fileInfo.Length;

            currentPackage = package;
            return package;
        }

        public static string ExtractPackage()
        {
            string path = "";
            using (var tgzStream = File.OpenRead(currentPackage.pathName))
            using (var gzStream = new GZipStream(tgzStream, CompressionMode.Decompress))
            using (var tarArchive = TarArchive.CreateInputTarArchive(gzStream,Encoding.UTF8))
            {
                path = FolderManager.CreateTempFolder();
                tarArchive.ExtractContents(path);
            }
            return path;
        }

        public static void WriteToConsole(Package package)
        {
            Console.WriteLine(
                    $"パッケージのパス:\t{package.pathName}\n" +
                    $"ファイルのサイズ:\t{Math.Ceiling(Asset.ConvertTo(SizeUnit.KB, package.fileSize)) + " KB"}\n");
        }

        public static void ExportPackage()
        {

            string exportPackagePath = FolderManager.CreateExportFolder() + "\\" + Package.currentPackage.fileName;

            using (FileStream fsOut = new FileStream(exportPackagePath, FileMode.Create,FileAccess.Write))
            {
                using (GZipOutputStream gzipStream = new GZipOutputStream(fsOut))
                {
                    using (TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzipStream))
                    {
                        AddDirectoryFilesToTar(FolderManager.tempFolderPath, tarArchive, "");
                    }
                }
            }
        }
        static void AddDirectoryFilesToTar(string sourceDirectory, TarArchive tarArchive, string relativePath)
        {
            string[] files = Directory.GetFiles(sourceDirectory);
            string[] subdirectories = Directory.GetDirectories(sourceDirectory);

            foreach (var file in files)
            {
                string entryName = relativePath + Path.GetFileName(file);
                TarEntry tarEntry = TarEntry.CreateEntryFromFile(file);
                tarEntry.Name = entryName;
                tarArchive.WriteEntry(tarEntry, true);
            }

            foreach (var subdirectory in subdirectories)
            {
                string entryName = relativePath + Path.GetFileName(subdirectory) + "/";
                TarEntry tarEntry = TarEntry.CreateEntryFromFile(subdirectory);
                tarEntry.Name = entryName;
                tarArchive.WriteEntry(tarEntry, true);

                AddDirectoryFilesToTar(subdirectory, tarArchive, entryName);
            }
        }
    }
}
