using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MvvmMapsProject.Utility
{
    using System.IO;

    public static class FileHelper
    {
        public static void WriteFile(IGenerateNameOfFile filePath,string fileName, string json)
        {
            string backingFile = filePath.GetAbsolutePathToFile(fileName);

            if (backingFile == null)
                return;

            File.WriteAllText(backingFile, json);
        }

        public static string ReadFile(IGenerateNameOfFile filePath, string fileName)
        {
            string backingFile = filePath.GetAbsolutePathToFile(fileName);

            if (backingFile == null || !File.Exists(backingFile))
                return string.Empty;

            return File.ReadAllText(backingFile);
        }
    }
}