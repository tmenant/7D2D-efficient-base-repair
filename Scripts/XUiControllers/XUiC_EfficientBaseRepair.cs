using Audio;

public class XUiC_EfficientBaseRepair : XUiController
{
	private static readonly Logging.Logger logger = Logging.CreateLogger<XUiC_EfficientBaseRepair>();

	private const string ID = "EfficientBaseRepair";

	private const string openSound = "UseActions/chest_tier4_open";

	private XUiC_LootWindow lootWindow;

	private XUiC_EfficientBaseRepairStats statsWindow;

	private XUiC_EfficientBaseRepairMaterials MaterialsWindow;

	private XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	public TEFeatureEBR TileEntity { get; private set; }

	public override void Init()
	{
		base.Init();

		lootWindow = GetChildByType<XUiC_LootWindow>();
		nonPagingHeaderWindow = GetChildByType<XUiC_WindowNonPagingHeader>();

		statsWindow = GetChildById("windowEfficientBaseRepairStats") as XUiC_EfficientBaseRepairStats;
		MaterialsWindow = GetChildById("windowEfficientBaseRepairMaterials") as XUiC_EfficientBaseRepairMaterials;
	}

	public override void OnOpen()
	{
		base.OnOpen();

		nonPagingHeaderWindow.SetHeader("Base Repair");

		Manager.BroadcastPlayByLocalPlayer(TileEntity.ToWorldPos(), openSound);
	}

	public override void OnClose()
	{
		base.OnClose();

		xui.playerUI.windowManager.Close("backpack");

		TileEntity.SetModified();
		TileEntity.SetUserAccessing(false);

		LockManager.Instance.UnlockRequestLocal();
	}

	private void SetTileEntity(TEFeatureEBR tileEntity)
	{
		TileEntity = tileEntity;

		lootWindow.SetTileEntityChest(ID, tileEntity);

		MaterialsWindow.TileEntity = tileEntity;

		statsWindow.SetUpgradeEnabled(TileEntity.UpgradeEnabled);
		statsWindow.WindowWidth = lootWindow.windowWidth;
		statsWindow.TileEntity = tileEntity;

		logger.Info($"lootWindow width: {statsWindow.WindowWidth}");
	}

	public static void Open(LocalPlayerUI playerUI, TEFeatureEBR tileEntity)
	{
		var controller = playerUI.xui.FindWindowGroupByName(ID) as XUiC_EfficientBaseRepair;

		if (controller == null)
		{
			logger.Error($"Window '{ID}' not found.");
			return;
		}

		tileEntity.ForceRefresh();

		controller.SetTileEntity(tileEntity);

		playerUI.windowManager.Open(ID, _bModal: true);
	}
}