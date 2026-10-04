using Window = Terminal.Gui.Views.Window;
using System;
using Label = Terminal.Gui.Views.Label;
using Button = Terminal.Gui.Views.Button;
using View = Terminal.Gui.ViewBase.View;
using Application = Terminal.Gui.App.Application;
using Terminal.Gui.App;
using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;

namespace IlluminatiIdle
{
    public static class TuiEngine
    {
        private static Label lblTotalDP;
        private static Label lblMultiplier;
        private static Label lblStatus;
        private static View genView;
        private static View initView;

        public static void Run()
        {
            Application.Init();

            var top = new Window { Title = "Illuminati Idle", Width = Dim.Fill(), Height = Dim.Fill() };
            
            // 1. Header Frame (Top 20%)
            var headerFrame = new FrameView { Title = "=== ILLUMINATI IDLE ===",
                X = 0, Y = 0,
                Width = Dim.Fill(), Height = Dim.Percent(20)
            };
            
            lblTotalDP = new Label { Text = "Total DP: ", X = Pos.Center(), Y = Pos.Center() - 1 };
            lblMultiplier = new Label { Text = "Multiplier: ", X = Pos.Center(), Y = Pos.Center() };
            headerFrame.Add(lblTotalDP, lblMultiplier);

            // 2. Generators Frame (Middle 40%)
            var genFrame = new FrameView { Title = "Generators",
                X = 0, Y = Pos.Bottom(headerFrame),
                Width = Dim.Fill(), Height = Dim.Percent(40)
            };
            
            genView = new View {
                X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill()
            };
            RefreshGenerators();
            genFrame.Add(genView);

            // 3. Initiatives Frame (Bottom 40%)
            var initFrame = new FrameView { Title = "Initiatives (Permanent Multipliers)",
                X = 0, Y = Pos.Bottom(genFrame),
                Width = Dim.Fill(), Height = Dim.Fill(1) // Leave 1 line for status
            };
            
            initView = new View {
                X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill()
            };
            RefreshInitiatives();
            initFrame.Add(initView);

            // 4. Status Bar (Bottom 1 line)
            lblStatus = new Label { Text = "Press [S] to Save | [P] for New World Order (Prestige) | [ESC] to Quit",
                X = 0, Y = Pos.Bottom(initFrame),
                Width = Dim.Fill(), Height = 1
            };

            top.Add(headerFrame, genFrame, initFrame, lblStatus);

            // Setup tick timer
            Application.AddTimeout(TimeSpan.FromMilliseconds(100), () => {
                Program.InfoDamage = Program.InfoDamage + Program.CalculateTick("Info");
                Program.EconDamage = Program.EconDamage + Program.CalculateTick("Econ");
                Program.HealthDamage = Program.HealthDamage + Program.CalculateTick("Health");
                Program.SocialDamage = Program.SocialDamage + Program.CalculateTick("Social");
                Program.ControlDamage = Program.ControlDamage + Program.CalculateTick("Control");
                UpdateDynamicUI();
                return true;
            });

            // Keybindings
            top.KeyDown += (s, e) => {
                if (e.KeyCode == Terminal.Gui.Drivers.KeyCode.S)
                {
                    Program.SaveGame();
                    SetStatus(Program.SaveStatus);
                    e.Handled = true;
                }
                else if (e.KeyCode == Terminal.Gui.Drivers.KeyCode.Space)
                {
                    Program.TotalDP = Program.TotalDP + (Program.BaseClick * Program.CurrentMultiplier());
                    e.Handled = true;
                }
                else if (e.KeyCode == Terminal.Gui.Drivers.KeyCode.P)
                {
                    if (Program.TotalDP >= Program.GetPrestigeThreshold()) Program.Prestige();
                    RefreshGenerators();
                    RefreshInitiatives();
                    e.Handled = true;
                }
                else if (e.KeyCode == Terminal.Gui.Drivers.KeyCode.Esc)
                {
                    Program.SaveGame();
                    Application.RequestStop();
                    e.Handled = true;
                }
            };

            Application.Run(top);
            Application.Shutdown();
        }

        private static void UpdateDynamicUI()
        {
            lblTotalDP.Text = $"Total DP: {Program.FormatNum(Program.TotalDP)}";
            lblMultiplier.Text = $"Multiplier: {Program.FormatNum(Program.CurrentMultiplier())}x";
            
            // We could also dynamically update button text here, but replacing controls often causes flicker.
            // Better to just let them click and see it change.
            // For now, we update generator costs.
        }

        private static void RefreshGenerators()
        {
            genView.RemoveAll();
            for (int i = 0; i < Program.Generators.Count; i++)
            {
                var g = Program.Generators[i];
                var btn = new Button { Text = $"{g.Name} (Owned: {Program.FormatNum(g.Count)}) - Cost: {Program.FormatNum(g.GetCost(Program.CurrentMultiplier()))}",
                    X = 0, Y = i
                };
                btn.Accepted += (s, e) => {
                    var cost = g.GetCost(Program.CurrentMultiplier());
                    if (Program.TotalDP >= cost)
                    {
                        Program.TotalDP -= cost;
                        g.Count = g.Count + 1;
                        SetStatus($"Purchased {g.Name}!");
                        RefreshGenerators(); // Redraw button to update cost
                    }
                    else
                    {
                        SetStatus($"Not enough DP for {g.Name}. Need {Program.FormatNum(cost - Program.TotalDP)} more.");
                    }
                };
                genView.Add(btn);
            }
        }

        private static void RefreshInitiatives()
        {
            initView.RemoveAll();
            for (int i = 0; i < Program.Initiatives.Count; i++)
            {
                var init = Program.Initiatives[i];
                string status = init.Owned ? "OWNED" : Program.FormatNum(init.Cost);
                var btn = new Button { Text = $"{init.Name} - Cost: {status}",
                    X = (i % 2 == 0) ? 0 : Pos.Center(), 
                    Y = i / 2
                };
                
                if (init.Owned) {
                    btn.CanFocus = false; // Disable if owned
                }
                
                btn.Accepted += (s, e) => {
                    if (init.Owned) return;
                    
                    if (Program.TotalDP >= init.Cost)
                    {
                        Program.TotalDP -= init.Cost;
                        init.Owned = true;
                        SetStatus($"Unlocked Initiative: {init.Name}!");
                        RefreshInitiatives();
                        RefreshGenerators(); // Because multiplier changed, generator costs change
                    }
                    else
                    {
                        SetStatus($"Not enough DP for {init.Name}.");
                    }
                };
                initView.Add(btn);
            }
        }

        private static void SetStatus(string message)
        {
            lblStatus.Text = message;
        }
    }
}


