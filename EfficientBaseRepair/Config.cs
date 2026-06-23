public class Config
{
    private static readonly ModConfig config = new ModConfig(version: 0, save: false);

    public static int maxBfsIterations = config.GetInt("maxBfsIterations");

    public static bool needsMaterialsForRepair = config.GetBool("needsMaterialsForRepair");

    public static bool needsMaterialsForUpgrade = config.GetBool("needsMaterialsForUpgrade");

    public static bool activeDuringBloodMoon = config.GetBool("activeDuringBloodMoon");

    public static int repairRate = config.GetInt("repairRate");

    public static int refreshRate = config.GetInt("refreshRate");

    public static bool playRepairSound = config.GetBool("playRepairSound");

    public static bool autoTurnOff = config.GetBool("autoTurnOff");

    public static bool turnOnAfterReload = config.GetBool("turnOnAfterReload");

    public static bool turnOnAfterRefuel = config.GetBool("turnOnAfterRefuel");

    public static int upgradeRate = config.GetInt("upgradeRate");

    public static bool keepPaintAfterUpgrade = config.GetBool("keepPaintAfterUpgrade");

    public static string upgradeSound = config.GetString("upgradeSound");

    [ModConfig.ReadOnly]
    public static Vector2i lootSize = config.GetVector2i("lootSize");

    [ModConfig.ReadOnly]
    public const TileEntityType tileEntityType = (TileEntityType)191;
}