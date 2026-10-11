# Perk Fixes and Rebalance

Mount & Blade II: Bannerlord mod — fixes broken and misleading perks, clarifies tooltips, and rebalances selected effects. Configurable in MCM.

- **Game:** Bannerlord v1.4.8 (War Sails and RBM optional)
- **Nexus listing copy:** see [NEXUS.md](NEXUS.md)

## Build

```bash
dotnet build -c Release
```

Deploys `PerkFixesAndRebalance.dll` and `SubModule.xml` to the game `Modules\PerkFixesAndRebalance` folder (see the csproj `DeployToBannerlord` target).
