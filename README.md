# Help2Swagger (.NET)
Converts Riot Games' native help format to Swagger and OpenAPI.

# Sponsors
A big thank you for your support!

[![Sentry Logo](https://raw.githubusercontent.com/mikaeldui/riot-games-dotnet-client/main/sponsors/sentry.svg)](https://sentry.io/for/good/)
[![JetBrains Logo (Main) logo](https://raw.githubusercontent.com/mikaeldui/riot-games-dotnet-client/main/sponsors/jetbrains.svg)](https://jb.gg/OpenSourceSupport)

# How it works

1. It downloads the `/Help` in two different formats, [`Full`](https://www.mingweisamuel.com/lcu-schema/lcu/help.json) and [`Console`](https://www.mingweisamuel.com/lcu-schema/lcu/help.console.json).
    - The information needed is spread out over both representations.
2. The Helps are parsed.
3. The OpenAPI spec is constructed using parts from both Helps.
4. The spec gets serialized to JSON.
