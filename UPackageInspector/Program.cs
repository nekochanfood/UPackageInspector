using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UPackageInspector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Package.LoadPackageFromPath(args[0]);

            Package.ExtractPackage();

            Package.WriteToConsole(Package.currentPackage);

            Asset.CollectAssets();

            foreach(Asset asset in Asset.assets)
            {
                Asset.WriteToConsole(asset);
            }

            Extract.ExtractAssets();
        }
    }
}