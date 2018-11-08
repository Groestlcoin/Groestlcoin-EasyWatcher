using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Documents;

namespace WatchOnlyGroestlcoinWallet.Services
{
    public static class DataManager
    {
        static DataManager()
        {
            mainFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Groestlcoin Sentinel Wallet";
        }


        private static readonly string mainFolderPath;


        public enum FileType
        {
            Wallet,
            Settings,
        }

        /// <summary>
        /// Reads file from disk. Returns a new instance (empty) if file not found.
        /// </summary>
        /// <typeparam name="T">Type of the file being read.</typeparam>
        /// <param name="f">Name of the folder containing file to read.</param>
        /// <returns>Read data.</returns>
        public static T ReadFile<T>(FileType f)
        {
            string myFilePath = string.Empty;
            switch (f)
            {
                case FileType.Wallet:
                    myFilePath = mainFolderPath + @"\Wallet.json";
                    break;
                case FileType.Settings:
                    myFilePath = mainFolderPath + @"\Settings.json";
                    break;
            }
            T result;
            if (File.Exists(myFilePath))
            {
                using (StreamReader st = File.OpenText(myFilePath)){
                    result = (T)JsonConvert.DeserializeObject(st.ReadToEnd(), typeof(T));
                }
                return result;
            }
            return (T)Activator.CreateInstance(typeof(T));
        }


        /// <summary>
        /// Writes data to disk.
        /// </summary>
        /// <typeparam name="T">Type of the data to save.</typeparam>
        /// <param name="dataToSave">Data to save.</param>
        /// <param name="f">Name of the folder to place the file.</param>
        public static void WriteFile<T>(T dataToSave, FileType f)
        {
            string myFilePath = string.Empty;
            switch (f)
            {
                case FileType.Wallet:
                    myFilePath = mainFolderPath + @"\Wallet.json";
                    break;
                case FileType.Settings:
                    myFilePath = mainFolderPath + @"\Settings.json";
                    break;
            }

            if (!Directory.Exists(mainFolderPath))
            {
                Directory.CreateDirectory(mainFolderPath);
            }

            using (StreamWriter str = File.CreateText(myFilePath)){
                string json = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
                str.Write(json);
            }
        }


        /// <summary>
        /// Opens up default open dialog and tries reading the file.
        /// </summary>
        /// <returns>returns null on failure</returns>
        public static Response<string[]> OpenFileDialog()
        {
            Response<string[]> resp = new Response<string[]>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    resp.Result = File.ReadAllLines(ofd.FileName);
                }
                catch (Exception)
                {
                    resp.Errors.Add("Could not read file!");
                }
            }
            return resp;
        }

    }
}
