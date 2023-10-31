using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TwoBitMachines
{
        public class Storage
        {
                public static string path { get { return Path.Combine(Application.persistentDataPath, "TwoBitMachines"); } }
                public static bool encrypt = false;

#if UNITY_EDITOR
                [MenuItem("Window/Flare Save Folder")]
                static void ShowSaveFolder()
                {
                        EditorUtility.RevealInFinder(path);
                }
#endif

                public static bool Delete(string folder, string key)
                {
                        string currentPath = Path.Combine(Path.Combine(path, folder), key);
                        if (!File.Exists(currentPath))
                        {
                                #region Debug
#if UNITY_EDITOR
                                Debug.LogWarning("Path does not exist: " + currentPath);
#endif
                                #endregion
                                return false;
                        }
                        File.Delete(currentPath);
                        return true;
                }

                public static bool DeleteAll(string folder)
                {
                        string currentPath = Path.Combine(path, folder);
                        if (Directory.Exists(currentPath))
                        {
                                Directory.Delete(currentPath, true);
                        }
                        Directory.CreateDirectory(currentPath);
                        return true;
                }

                public static void Save(object data, string folder, string key)
                {
                        string jsonData = JsonUtility.ToJson(data);
                        string directory = Path.Combine(path, folder);
                        string filePath = Path.Combine(directory, key);

                        try
                        {
                                Directory.CreateDirectory(directory);
                        }
                        catch
                        {

                        }

                        using (StreamWriter writer = new StreamWriter(filePath, false))
                        {
                                try
                                {
                                        writer.Write(EncryptDecrypt(jsonData));
                                        writer.Flush();
                                }
                                catch
                                {

                                }
                        }
                }

                public static T Load<T>(T defaultValue, string folder, string key) where T : class
                {
                        string data = null;
                        string currentPath = Path.Combine(Path.Combine(path, folder), key);

                        if (!File.Exists(currentPath))
                        {
                                return defaultValue;
                        }

                        using (StreamReader reader = new StreamReader(currentPath, false))
                        {
                                try
                                {
                                        data = reader.ReadToEnd();
                                        data = EncryptDecrypt(data);
                                }
                                catch
                                {
                                        data = null;
                                }
                        }
                        return data == null ? defaultValue : JsonUtility.FromJson<T>(data);
                }

                public static string EncryptDecrypt(string data)
                {
                        if (!encrypt) return data;

                        StringBuilder input = new StringBuilder(data);
                        StringBuilder output = new StringBuilder(data.Length);
                        char character;

                        for (int i = 0; i < data.Length; i++)
                        {
                                character = input[i];
                                character = (char)(character ^ 333);
                                output.Append(character);
                        }
                        return output.ToString();
                }

                // //https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
                // public static void CopyFolder (string source, string destination)
                // {
                //         string sourceFolder = Path.Combine (path, source);
                //         string destinationFolder = Path.Combine (path, destination);

                //         if (!Directory.Exists (destinationFolder))
                //         {
                //                 Directory.CreateDirectory (destinationFolder);
                //         }

                //         foreach (string dirPath in Directory.GetDirectories (sourceFolder, "*", SearchOption.AllDirectories))
                //         {
                //                 string newDirPath = dirPath.Replace (sourceFolder, destinationFolder);
                //                 Directory.CreateDirectory (newDirPath);
                //         }

                //         foreach (string filePath in Directory.GetFiles (sourceFolder, "*", SearchOption.AllDirectories))
                //         {
                //                 string newFilePath = filePath.Replace (sourceFolder, destinationFolder);
                //                 File.Copy (filePath, newFilePath, true);
                //         }
                // }

                //   https: //learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
                public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
                {
                        sourceDir = Path.Combine(path, sourceDir);
                        destinationDir = Path.Combine(path, destinationDir);

                        // Get information about the source directory
                        var dir = new DirectoryInfo(sourceDir);

                        // Check if the source directory exists
                        if (!dir.Exists)
                        {
                                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
                        }

                        // Cache directories before we start copying
                        DirectoryInfo[] dirs = dir.GetDirectories();

                        // Create the destination directory
                        Directory.CreateDirectory(destinationDir);

                        // Get the files in the source directory and copy to the destination directory
                        foreach (FileInfo file in dir.GetFiles())
                        {
                                string targetFilePath = Path.Combine(destinationDir, file.Name);
                                file.CopyTo(targetFilePath);
                        }

                        // If recursive and copying subdirectories, recursively call this method
                        if (recursive)
                        {
                                foreach (DirectoryInfo subDir in dirs)
                                {
                                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                                        CopyDirectory(subDir.FullName, newDestinationDir, true);
                                }
                        }
                }
        }

}