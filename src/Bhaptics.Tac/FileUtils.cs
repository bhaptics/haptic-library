using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace Bhaptics.Tac
{
    /// <summary>
    /// FileUtils
    /// </summary>
    public class FileUtils
    {
        /// <summary>
        /// Writes the specified dictionary to the file path.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="filePath">The file path.</param>
        public static void Write(Dictionary<string, string> dictionary, string filePath)
        {
            using (FileStream fs = File.OpenWrite(filePath))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(dictionary.Count);
                // Write pairs.
                foreach (var pair in dictionary)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        /// <summary>
        /// Writes the specified string content to file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="file">The file.</param>
        public static void Write(string content, string file)
        {
            try
            {
                using (StreamWriter fileWr =
                new StreamWriter(file))
                {
                    fileWr.Write(content);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(file + " " + content + ", " + exception);
            }
        }

        /// <summary>
        /// Reads the string from file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static string ReadString(string file)
        {
            if (!File.Exists(file))
            {
                return "";
            }

            return File.ReadAllText(file);
        }

        /// <summary>
        /// Reads dictionary from file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Read(string file)
        {
            var result = new Dictionary<string, string>();
            if (!File.Exists(file))
            {
                return new Dictionary<string, string>();
            }
            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Get count.
                int count = reader.ReadInt32();
                // Read in all pairs.
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
