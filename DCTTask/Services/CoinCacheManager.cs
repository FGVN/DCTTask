using DCTTask.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DCTTask.Services
{

    public static class CoinCacheManager
    {
        private static string CacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheDirectory");

        public static List<CoinCapData> LoadCachedData(string fileName)
        {
            string filePath = Path.Combine(CacheDirectory, fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<CoinCapData>>(json);
            }

            return new List<CoinCapData>();
        }

        public static void CacheData<T>(string fileName, T data)
        {
            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }

            string filePath = Path.Combine(CacheDirectory, fileName);
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, json);
        }
    }
}
