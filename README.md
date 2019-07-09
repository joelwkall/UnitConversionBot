# UnitConversionBot
Bot that scans imgur.com for posts that include measurements, and converts them between various systems.

## How to get it running
1. Copy appsettings_sample.json to appsettings.json
2. Insert the values required in appsettings.json. See https://apidocs.imgur.com/?version=latest#authorization-and-oauth for details.
3. If you want the bot to actually post comments (not just lurk) you need to add a file prod.txt in the executable directory.

NOTE: I've only tested this on Win 10 with .NET Core installed. Please let me know if you'd be willing to get it working on other systems.
