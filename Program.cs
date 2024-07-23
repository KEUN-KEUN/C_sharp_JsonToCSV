using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Newtonsoft.Json;
//using CsvWriter;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;

/// <summary>
using System.Diagnostics;
/// </summary>

namespace WindowsFormsApp1
{
    static class Program_BAK
    {
        public class ItemFlow
        {
            public string usernickname { get; set; }
            public int cnt { get; set; }
            public int testKey { get; set; }
        }

        public class Root
        {
            public ItemFlow ItemFlow { get; set; }
        }

        static void Main()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string rootDirectory = @"D:\JsonData";

            if (Directory.Exists(rootDirectory))
            {
                var subDirectories = Directory.GetDirectories(rootDirectory);

                foreach (string directoryPath in subDirectories)
                {
                    var records = new List<ItemFlow>();
                    string[] files = Directory.GetFiles(directoryPath, "*.json");

                    foreach (string file in files)
                    {
                        string jsonData = File.ReadAllText(file);

                        // JSON 데이터의 루트가 객체인지 확인
                        var token = JToken.Parse(jsonData); // 객체란,, json 포맷에서 의미하는 범위는 무엇입니까 ? 
                                                            // object 가 많으면 ??? ,,, 현재는 하나의 object만 가능
                        if (token is JObject) // JSON 데이터가 객체인 경우
                        {
                            var root = JsonConvert.DeserializeObject<Root>(jsonData); // DeserializeObject 사용 이유 
                                                                                      // 중복 제거 TestKey_typeseqno 기준
                                                                                      // 하나의 object에 여러 itemFlow 존재할 경우 
                            if (root != null && root.ItemFlow != null)
                            {
                                records.Add(root.ItemFlow);
                            }
                        }
                        else if (token is JArray)
                        {
                            var rootArray = JsonConvert.DeserializeObject<List<Root>>(jsonData);

                            foreach (var root in rootArray)
                            {
                                if (root != null && root.ItemFlow != null)
                                {
                                    records.Add(root.ItemFlow);
                                }
                            }
                        }
                    }

                    if (records.Count > 0)
                    {
                        // 현재 폴더명을 가져와서 CSV 파일명으로 사용
                        string folderName = new DirectoryInfo(directoryPath).Name;
                        string csvFilePath = Path.Combine(directoryPath, $"{folderName}.csv");

                        // CSV 파일 생성
                        using (var writer = new StreamWriter(csvFilePath))
                        {
                            writer.WriteLine("usernickname,cnt");
                            foreach (var record in records)
                            {
                                writer.WriteLine($"{record.usernickname},{record.cnt}");
                            }
                        }

                        Console.WriteLine($"CSV 파일이 '{csvFilePath}'에 성공적으로 생성되었습니다.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"디렉터리 '{rootDirectory}'가 존재하지 않습니다.");
            }

            stopwatch.Stop();
            Console.WriteLine($"Time 1 : {stopwatch.ElapsedMilliseconds}ms");
            // 1.3GB > 81810ms
            // 병렬 처리 , 다른 함수 , 
        }
    }
}
