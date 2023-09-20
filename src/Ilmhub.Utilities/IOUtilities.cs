﻿namespace Ilmhub.Utilities;

public class IOUtilities
{
    public static string CreateTemporaryDirectory(string rootFolder = "")
    {
        var randomName = Guid.NewGuid().ToString();
        var root = string.IsNullOrWhiteSpace(rootFolder) || Directory.Exists(rootFolder) is false
        ? Path.Combine(Path.GetTempPath(), randomName)
        : Path.Combine(rootFolder, randomName);

        var tempFolder = Path.GetFullPath(root); 
        Directory.CreateDirectory(tempFolder);

        return tempFolder;
    }

    public static void CreateEmptyFile(string filePath)
    {
        var containingFolder = Path.GetDirectoryName(filePath);
        if(string.IsNullOrWhiteSpace(containingFolder) is false)
            Directory.CreateDirectory(containingFolder);
        File.Create(filePath).Dispose();
    }
}
