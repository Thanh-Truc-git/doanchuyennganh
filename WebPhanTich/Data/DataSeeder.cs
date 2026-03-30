using System;
using System.IO;
using System.Linq;
using System.Text;
using WebPhanTich.Models;

namespace WebPhanTich.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            Console.WriteLine(">>> 🔹 Bắt đầu seed dữ liệu JSUT Ver.1.1 ...");

            if (context.PronunciationSamples.Any())
            {
                Console.WriteLine(">>> ⚠️ Dữ liệu đã có sẵn, bỏ qua seeding.");
                return;
            }

            SeedJSUTVer11(context);
        }

        private static void SeedJSUTVer11(AppDbContext context)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audios", "jsut_ver1.1");

            Console.WriteLine($">>> 📂 Đường dẫn thư mục gốc: {basePath}");

            if (!Directory.Exists(basePath))
            {
                Console.WriteLine(">>> ❌ Không tìm thấy thư mục jsut_ver1.1");
                return;
            }

            var subFolders = Directory.GetDirectories(basePath);

            Console.WriteLine($">>> 🔍 Tìm thấy {subFolders.Length} thư mục con.");

            foreach (var folder in subFolders)
            {
                string folderName = Path.GetFileName(folder);
                string transcriptPath = Path.Combine(folder, "transcript_utf8.txt");
                string wavFolder = Path.Combine(folder, "wav");

                Console.WriteLine($"--- 📁 Đang xử lý: {folderName}");

                if (!File.Exists(transcriptPath))
                {
                    Console.WriteLine($"❌ Không có file transcript: {transcriptPath}");
                    continue;
                }

                if (!Directory.Exists(wavFolder))
                {
                    Console.WriteLine($"❌ Không có thư mục wav: {wavFolder}");
                    continue;
                }

                var lines = File.ReadAllLines(transcriptPath, Encoding.UTF8);

                Console.WriteLine($"✅ Đọc được {lines.Length} dòng transcript.");

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(':', '|');
                    if (parts.Length < 2) continue;

                    string fileName = parts[0].Trim();
                    string text = parts[1].Trim();
                    string wavPath = Path.Combine(wavFolder, fileName + ".wav");

                    if (!File.Exists(wavPath))
                    {
                        // Kiểm tra xem tên file trong transcript có chứa đuôi không
                        if (File.Exists(Path.Combine(wavFolder, fileName)))
                            wavPath = Path.Combine(wavFolder, fileName);
                        else
                        {
                            Console.WriteLine($"⚠️ Không tìm thấy file wav cho: {fileName}");
                            continue;
                        }
                    }

                    string relativePath = wavPath
    .Replace(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "wwwroot", "")
    .Replace("\\", "/");

                    if (!relativePath.StartsWith("/"))
                        relativePath = "/" + relativePath.TrimStart('/');


                    context.PronunciationSamples.Add(new PronunciationSample
                    {
                        Word = text,
                        IPA = "",
                        Description = $"Sample from {folderName}",
                        AudioPath = relativePath
                    });
                }
            }

            int count = context.SaveChanges();
            Console.WriteLine($">>> ✅ Đã thêm {count} mẫu phát âm vào CSDL.");
        }
    }
}
