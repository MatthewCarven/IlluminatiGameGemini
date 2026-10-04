using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Natural;

class Program {
    static void Main() {
        var lines = File.ReadAllLines("savegame.sav");
        string content = string.Join("\n", lines.Take(10));
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content + "IlluminatiSecretSalt"));
            string expectedHash = Convert.ToBase64String(hash);
            Console.WriteLine(expectedHash == lines[10]);
            if (expectedHash != lines[10]) Console.WriteLine($"Expected: {expectedHash}, Actual: {lines[10]}");
        }
        
        try {
            ApFloat.Parse(lines[0], CultureInfo.InvariantCulture);
            Console.WriteLine("Parse successful!");
        } catch (Exception e) {
            Console.WriteLine("Parse failed: " + e.Message);
        }
    }
}
