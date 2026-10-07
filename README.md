# Illuminati Idle

A sinister, infinitely scaling idle game where you play as a shadowy cabal aiming for total global subjugation. Enact policies, harvest data, and manipulate society across five distinct pillars of control to amass unimaginable power.

## Overview
Built as a C# Console prototype, this game utilizes the [Natural](https://github.com/MatthewCarven/Natural) library (`ApFloat`) to handle massive, arbitrary-precision numbers, ensuring that your Damage Points (DP) can scale infinitely without ever overflowing or capping out.
<img width="1312" height="894" alt="image" src="https://github.com/user-attachments/assets/ecbe0d37-31a3-4d49-9358-247b87d3ed32" />
as usual right click extract the zip file then in the first folder the exe for the program
https://drive.google.com/file/d/1mIQuzsGvnfgiVQpHv19sqfjnuuT0BK0J/view?usp=drive_link

## Features
* **5 Pillars of Control:** Information, Economics, Health, Social, and Control.
* **10 Generators:** Ranging from Troll Farms to Social Credit Systems, generating passive DP.
* **10+ Permanent Initiatives:** Expensive, one-off purchases like *Predictive Policing* and *Reality Revision* that grant massive permanent multipliers.
* **New World Order (Prestige):** Reset your timeline once you reach the threshold to gain **Influence Shards**, which act as a permanent global multiplier for all future runs. The required threshold scales exponentially with each reset!
* **Anti-Cheat Saving:** Automatically saves on exit and loads on start, protected by an SHA-256 checksum to prevent timeline tampering.

## How to Play
Ensure you have the .NET SDK installed.

1. Clone the repository with submodules (required for the `Natural` math library):
   ```bash
   git clone --recursive https://github.com/MatthewCarven/IlluminatiGameGemini.git
   ```
2. Navigate into the directory and run the game:
   ```bash
   cd IlluminatiGameGemini
   dotnet run
   ```

### Controls
* `[SPACE]` - Manually exert influence to generate DP based on your current multiplier.
* `[1]` to `[0]` - Purchase Tier 1 and Tier 2 generators across the 5 pillars.
* `[Q], [W], [E], [R], [G], [T], [Y], [H]` - Purchase permanent multiplier Initiatives.
* `[P]` - Enact the New World Order (Prestige) once you reach the required DP threshold.
* `[ESC]` - Save and Quit.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
