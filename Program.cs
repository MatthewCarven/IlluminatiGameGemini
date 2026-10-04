using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Natural;

namespace IlluminatiIdle
{
    class Generator
    {
        public string Name;
        public string Category;
        public ApFloat Count;
        public ApFloat BaseCost;
        public ApFloat BaseOutput;
        public ConsoleKey Key;
        public string KeyName;

        static readonly ApFloat Discount = ApFloat.Parse("0.001", CultureInfo.InvariantCulture);
        static readonly ApFloat MultiplierScaling = ApFloat.Parse("0.000001", CultureInfo.InvariantCulture);

        public ApFloat GetCost(ApFloat multiplier)
        {
            ApFloat next = Count + 1;
            return BaseCost * next * next * (multiplier * MultiplierScaling) * Discount;
        }
    }

    class Initiative
    {
        public string Name;
        public string Category;
        public bool Owned;
        public ApFloat Cost;
        public ApFloat Multiplier;
        public ConsoleKey Key;
        public string KeyName;
    }

    public class Program
    {
        public static ApFloat TotalDP = 0;
        static ApFloat InfluenceShards = 0;
        static ApFloat TotalPrestiges = 0;
        
        static ApFloat InfoDamage = 0;
        static ApFloat EconDamage = 0;
        static ApFloat HealthDamage = 0;
        static ApFloat SocialDamage = 0;
        static ApFloat ControlDamage = 0;
        
        static ApFloat BaseClick = 1;
        
        static string SaveStatus = "";

        static List<Generator> Generators = new List<Generator>
        {
            new Generator { Name = "Troll Farms         ", Category = "Info   ", BaseCost = 10, BaseOutput = 1, Key = ConsoleKey.D1, KeyName = "1" },
            new Generator { Name = "Deepfake Anchors    ", Category = "Info   ", BaseCost = 500, BaseOutput = 25, Key = ConsoleKey.D2, KeyName = "2" },
            
            new Generator { Name = "Predatory Lenders   ", Category = "Econ   ", BaseCost = 25, BaseOutput = 2, Key = ConsoleKey.D3, KeyName = "3" },
            new Generator { Name = "Corporate Cartels   ", Category = "Econ   ", BaseCost = 1250, BaseOutput = 50, Key = ConsoleKey.D4, KeyName = "4" },
            
            new Generator { Name = "Processed Food      ", Category = "Health ", BaseCost = 50, BaseOutput = 4, Key = ConsoleKey.D5, KeyName = "5" },
            new Generator { Name = "VR Sedentarism      ", Category = "Health ", BaseCost = 2500, BaseOutput = 100, Key = ConsoleKey.D6, KeyName = "6" },
            
            new Generator { Name = "Agent Provocateurs  ", Category = "Social ", BaseCost = 100, BaseOutput = 8, Key = ConsoleKey.D7, KeyName = "7" },
            new Generator { Name = "Algorithmic Cults   ", Category = "Social ", BaseCost = 5000, BaseOutput = 200, Key = ConsoleKey.D8, KeyName = "8" },
            
            new Generator { Name = "Mass Surveillance   ", Category = "Control", BaseCost = 10000, BaseOutput = 500, Key = ConsoleKey.D9, KeyName = "9" },
            new Generator { Name = "Social Credit System", Category = "Control", BaseCost = 50000, BaseOutput = 2500, Key = ConsoleKey.D0, KeyName = "0" }
        };

        static List<Initiative> Initiatives = new List<Initiative>
        {
            new Initiative { Name = "Echo Chambers         ", Category = "Info", Cost = 1000, Multiplier = 5, Key = ConsoleKey.Q, KeyName = "Q" },
            new Initiative { Name = "Reality Revision      ", Category = "Info", Cost = 50000, Multiplier = 10, Key = ConsoleKey.A, KeyName = "A" },
            
            new Initiative { Name = "Planned Obsolescence  ", Category = "Econ", Cost = 5000, Multiplier = 5, Key = ConsoleKey.W, KeyName = "W" },
            new Initiative { Name = "Infinite Subscriptions", Category = "Econ", Cost = 250000, Multiplier = 10, Key = ConsoleKey.S, KeyName = "S" },
            
            new Initiative { Name = "Microplastics         ", Category = "Health", Cost = 25000, Multiplier = 5, Key = ConsoleKey.E, KeyName = "E" },
            new Initiative { Name = "Forever Chemicals     ", Category = "Health", Cost = 1250000, Multiplier = 10, Key = ConsoleKey.D, KeyName = "D" },
            
            new Initiative { Name = "Culture Wars          ", Category = "Social", Cost = 100000, Multiplier = 5, Key = ConsoleKey.R, KeyName = "R" },
            new Initiative { Name = "Hyper-Individualism   ", Category = "Social", Cost = 5000000, Multiplier = 10, Key = ConsoleKey.F, KeyName = "F" },
            
            new Initiative { Name = "Predictive Policing   ", Category = "Control", Cost = 500000, Multiplier = 5, Key = ConsoleKey.G, KeyName = "G" },
            new Initiative { Name = "Mandatory Biometrics  ", Category = "Control", Cost = 2500000, Multiplier = 10, Key = ConsoleKey.T, KeyName = "T" },
            new Initiative { Name = "Drone Enforcers       ", Category = "Control", Cost = 12500000, Multiplier = 15, Key = ConsoleKey.Y, KeyName = "Y" },
            new Initiative { Name = "Total Neural Override ", Category = "Control", Cost = 50000000, Multiplier = 20, Key = ConsoleKey.H, KeyName = "H" }
        };

        static ApFloat CurrentMultiplier()
        {
            ApFloat one = 1;
            ApFloat prestigeMult = one + InfluenceShards;
            return prestigeMult * (one + InfoDamage) * (one + EconDamage) * (one + HealthDamage) * (one + SocialDamage) * (one + ControlDamage);
        }
        
        static ApFloat GetPrestigeThreshold()
        {
            ApFloat threshold = 1000000;
            for (int i = 0; i < TotalPrestiges; i++)
            {
                threshold = threshold * 10;
            }
            return threshold;
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();
            
            LoadGame();
            
            DateTime lastTick = DateTime.Now;
            DrawUI();
            
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    var key = keyInfo.Key;
                    
                    if (key == ConsoleKey.Spacebar)
                    {
                        TotalDP = TotalDP + (BaseClick * CurrentMultiplier());
                        SaveStatus = "";
                        DrawUI();
                    }
                    else if (key == ConsoleKey.P && TotalDP >= GetPrestigeThreshold())
                    {
                        Prestige();
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        SaveGame();
                        break;
                    }
                    else if (key == ConsoleKey.F12)
                    {
                        var uiThread = new Thread(() => {
                            System.Windows.Forms.Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
                            System.Windows.Forms.Application.EnableVisualStyles();
                            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                            var form = new Natural_Playground.Form1();
                            form.BindToTotalDP(() => TotalDP);
                            System.Windows.Forms.Application.Run(form);
                        });
                        uiThread.SetApartmentState(ApartmentState.STA);
                        uiThread.Start();
                        SaveStatus = "ApFloat Playground launched!";
                        DrawUI();
                    }
                    else
                    {
                        var gen = Generators.FirstOrDefault(g => g.Key == key);
                        if (gen != null)
                        {
                            ApFloat cost = gen.GetCost(CurrentMultiplier());
                            if (TotalDP >= cost)
                            {
                                TotalDP = TotalDP - cost;
                                gen.Count = gen.Count + 1;
                                SaveStatus = "";
                                DrawUI();
                            }
                        }
                        
                        var init = Initiatives.FirstOrDefault(i => i.Key == key);
                        if (init != null && !init.Owned)
                        {
                            ApFloat cost = init.Cost;
                            if (TotalDP >= cost)
                            {
                                TotalDP = TotalDP - cost;
                                init.Owned = true;
                                SaveStatus = "";
                                DrawUI();
                            }
                        }
                    }
                }
                
                DateTime now = DateTime.Now;
                if ((now - lastTick).TotalMilliseconds >= 1000)
                {
                    lastTick = now;
                    
                    ApFloat infoTick = CalculateTick("Info");
                    ApFloat econTick = CalculateTick("Econ");
                    ApFloat healthTick = CalculateTick("Health");
                    ApFloat socialTick = CalculateTick("Social");
                    ApFloat controlTick = CalculateTick("Control");
                    
                    InfoDamage = InfoDamage + infoTick;
                    EconDamage = EconDamage + econTick;
                    HealthDamage = HealthDamage + healthTick;
                    SocialDamage = SocialDamage + socialTick;
                    ControlDamage = ControlDamage + controlTick;
                    
                    DrawUI();
                }
                
                Thread.Sleep(1);
            }
        }
        
        static ApFloat CalculateTick(string category)
        {
            ApFloat tick = 0;
            foreach (var gen in Generators.Where(g => g.Category.Trim() == category))
            {
                tick = tick + (gen.Count * gen.BaseOutput);
            }

            ApFloat multiplier = 1;
            foreach (var init in Initiatives.Where(i => i.Category == category && i.Owned))
            {
                multiplier *= init.Multiplier;
            }
            
            return tick * multiplier;
        }

        static void Prestige()
        {
            ApFloat threshold = GetPrestigeThreshold();
            ApFloat shardsToGain = TotalDP / threshold;
            InfluenceShards = InfluenceShards + shardsToGain;
            
            TotalPrestiges = TotalPrestiges + 1;
            
            TotalDP = 0;
            InfoDamage = 0;
            EconDamage = 0;
            HealthDamage = 0;
            SocialDamage = 0;
            ControlDamage = 0;
            
            foreach (var gen in Generators) gen.Count = 0;
            foreach (var init in Initiatives) init.Owned = false;
            
            SaveStatus = "Timeline Reset for the New World Order.";
            Console.Clear();
            DrawUI();
        }

        static void SaveGame()
        {
            try
            {
                var lines = new List<string> {
                    TotalDP.ToString("R", CultureInfo.InvariantCulture),
                    InfluenceShards.ToString("R", CultureInfo.InvariantCulture),
                    TotalPrestiges.ToString("R", CultureInfo.InvariantCulture),
                    InfoDamage.ToString("R", CultureInfo.InvariantCulture),
                    EconDamage.ToString("R", CultureInfo.InvariantCulture),
                    HealthDamage.ToString("R", CultureInfo.InvariantCulture),
                    SocialDamage.ToString("R", CultureInfo.InvariantCulture),
                    ControlDamage.ToString("R", CultureInfo.InvariantCulture),
                    string.Join(",", Generators.Select(g => g.Count.ToString("R", CultureInfo.InvariantCulture))),
                    string.Join(",", Initiatives.Select(i => i.Owned))
                };
                
                string content = string.Join("\n", lines);
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content + "IlluminatiSecretSalt"));
                    lines.Add(Convert.ToBase64String(hash));
                }
                File.WriteAllLines("savegame.sav", lines);
                SaveStatus = "Game saved successfully.";
            }
            catch (Exception ex)
            {
                SaveStatus = "Error saving game: " + ex.Message;
            }
        }

        static void LoadGame()
        {
            if (!File.Exists("savegame.sav"))
            {
                SaveStatus = "";
                return;
            }
            
            try
            {
                var lines = File.ReadAllLines("savegame.sav");
                if (lines.Length < 11)
                {
                    SaveStatus = "Save file is corrupted.";
                    return;
                }
                
                string content = string.Join("\n", lines.Take(10));
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content + "IlluminatiSecretSalt"));
                    string expectedHash = Convert.ToBase64String(hash);
                    if (expectedHash != lines[10])
                    {
                        SaveStatus = "Save file tampering detected! Loading aborted.";
                        return;
                    }
                }
                
                TotalDP = ApFloat.Parse(lines[0], CultureInfo.InvariantCulture);
                InfluenceShards = ApFloat.Parse(lines[1], CultureInfo.InvariantCulture);
                TotalPrestiges = ApFloat.Parse(lines[2], CultureInfo.InvariantCulture);
                InfoDamage = ApFloat.Parse(lines[3], CultureInfo.InvariantCulture);
                EconDamage = ApFloat.Parse(lines[4], CultureInfo.InvariantCulture);
                HealthDamage = ApFloat.Parse(lines[5], CultureInfo.InvariantCulture);
                SocialDamage = ApFloat.Parse(lines[6], CultureInfo.InvariantCulture);
                ControlDamage = ApFloat.Parse(lines[7], CultureInfo.InvariantCulture);
                
                var genCounts = lines[8].Split(',').Select(s => ApFloat.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                for (int i = 0; i < Generators.Count && i < genCounts.Length; i++)
                    Generators[i].Count = genCounts[i];
                    
                var initOwned = lines[9].Split(',').Select(bool.Parse).ToArray();
                for (int i = 0; i < Initiatives.Count && i < initOwned.Length; i++)
                    Initiatives[i].Owned = initOwned[i];
                    
                SaveStatus = "Game loaded successfully.";
            }
            catch (Exception ex)
            {
                SaveStatus = "Error loading game: " + ex.Message;
            }
        }

        static string FormatNum(ApFloat val)
        {
            string s;
            ApFloat trillion = ApFloat.Parse("1000000000000", System.Globalization.CultureInfo.InvariantCulture);
            if (val >= trillion)
            {
                s = val.ToString("E12");
            }
            else
            {
                s = val.ToString("F12");
                int dotIndex = s.IndexOf('.');
                if (dotIndex == -1) dotIndex = s.Length;
                
                string intPart = s.Substring(0, dotIndex);
                for (int i = intPart.Length - 3; i > (intPart.StartsWith("-") ? 1 : 0); i -= 3)
                {
                    intPart = intPart.Insert(i, ",");
                }
                
                s = intPart + (dotIndex < s.Length ? s.Substring(dotIndex) : "");
            }
            
            // Pad to a fixed width of 28 characters to keep the UI perfectly aligned
            return s.PadLeft(28);
        }

        static string Pad(string text, int width)
        {
            if (text.Length >= width) return text.Substring(0, width);
            return text + new string(' ', width - text.Length);
        }

        static int lastWidth = -1;
        static int lastHeight = -1;

        static void DrawUI()
        {
            int currentWidth = Console.WindowWidth;
            int currentHeight = Console.WindowHeight;
            
            // Clear the screen entirely if the console was resized
            if (currentWidth != lastWidth || currentHeight != lastHeight)
            {
                Console.Clear();
                lastWidth = currentWidth;
                lastHeight = currentHeight;
            }
            
            Console.SetCursorPosition(0, 0);
            
            // Limit drawing width to avoid implicit line wrapping (subtract 1)
            int w = Math.Max(currentWidth - 1, 50); 
            
            Console.WriteLine(Pad("=== ILLUMINATI IDLE ===", w));
            Console.WriteLine(Pad($"Total DP: {FormatNum(TotalDP)}", w));
            Console.WriteLine(Pad($"Multiplier: {FormatNum(CurrentMultiplier())}x", w));
            
            if (InfluenceShards > 0) 
                Console.WriteLine(Pad($"Prestige Shards: {FormatNum(InfluenceShards)} (Times Reset: {TotalPrestiges})", w));
            else 
                Console.WriteLine(Pad("", w));
            
            Console.WriteLine(Pad(new string('-', w), w));
            
            Console.WriteLine(Pad($"Info Damage:   {FormatNum(InfoDamage)}", w));
            Console.WriteLine(Pad($"Econ Damage:   {FormatNum(EconDamage)}", w));
            Console.WriteLine(Pad($"Health Damage: {FormatNum(HealthDamage)}", w));
            Console.WriteLine(Pad($"Social Damage: {FormatNum(SocialDamage)}", w));
            Console.WriteLine(Pad($"Control Damage:{FormatNum(ControlDamage)}", w));
            
            Console.WriteLine(Pad(new string('-', w), w));
            Console.WriteLine(Pad("GENERATORS:", w));
            
            foreach (var g in Generators)
            {
                string s = $"[{g.KeyName}] {g.Name} (Owned: {FormatNum(g.Count)}) | Cost: {FormatNum(g.GetCost(CurrentMultiplier()))} | Output: +{FormatNum(g.BaseOutput)} {g.Category}/sec";
                Console.WriteLine(Pad(s, w));
            }

            Console.WriteLine(Pad(new string('-', w), w));
            Console.WriteLine(Pad("INITIATIVES (Permanent Multipliers):", w));
            
            for (int i = 0; i < Initiatives.Count; i += 2)
            {
                var i1 = Initiatives[i];
                var i2 = Initiatives[i+1];
                string s1 = $"[{i1.KeyName}] {i1.Name} ({(i1.Owned ? "OWNED" : FormatNum(i1.Cost))})";
                string s2 = $"[{i2.KeyName}] {i2.Name} ({(i2.Owned ? "OWNED" : FormatNum(i2.Cost))})";
                
                int halfWidth = w / 2;
                Console.WriteLine(Pad($"{Pad(s1, halfWidth - 2)} | {s2}", w));
            }
            
            Console.WriteLine(Pad(new string('-', w), w));
            Console.WriteLine(Pad("[SPACE] Exert Influence (Click)", w));
            
            ApFloat currentThreshold = GetPrestigeThreshold();
            if (TotalDP >= currentThreshold)
                Console.WriteLine(Pad($"[P] Enact New World Order (Prestige for Shards!)", w));
            else
                Console.WriteLine(Pad($"[P] New World Order unlocks at {FormatNum(currentThreshold)} DP", w));
            
            Console.WriteLine(Pad("[F12] Inspect ApFloat Precision (Open Playground)", w));
            Console.WriteLine(Pad("[ESC] Save and Quit", w));
            
            if (!string.IsNullOrEmpty(SaveStatus))
            {
                Console.WriteLine(Pad(">>> " + SaveStatus, w));
            }
            else
            {
                Console.WriteLine(Pad("", w));
            }
        }
    }
}
