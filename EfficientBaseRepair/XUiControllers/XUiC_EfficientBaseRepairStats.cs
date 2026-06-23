using Audio;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class XUiC_EfficientBaseRepairStats : XUiController
{
	private static readonly Logging.Logger logger = Logging.CreateLogger<XUiC_EfficientBaseRepairStats>();

	private XUiController btnRefresh;

	private XUiV_Button btnRefresh_Background;

	private XUiController btnOn;

	private XUiV_Button btnOn_Background;

	private XUiController btnUpgrade;

	private XUiV_Button btnUpgrade_Background;

	private XUiV_Label lblOnOff;

	private XUiV_Sprite sprOnOff;

	private XUiV_Label lblUpgrade;

	private XUiV_Sprite sprUpgrade;

	private Color32 onColor = new Color32((byte)250, byte.MaxValue, (byte)163, byte.MaxValue);

	private Color32 offColor = (Color32)Color.white;

	private string turnOff => Localization.Get("xuiTurnOff");

	private string turnOn => Localization.Get("xuiTurnOn");

	private string UpgradeOnText => "Upgrade On";

	private string UpgradeOffText => "Upgrade Off";

	private bool lastOn;

	public int WindowWidth;

	public TEFeatureEBR TileEntity { get; set; }

	public override void Init()
	{
		base.Init();

		btnRefresh = GetChildById("btnRefresh");
		btnRefresh_Background = (XUiV_Button)btnRefresh.GetChildById("clickable").ViewComponent;
		btnRefresh_Background.Controller.OnPress += BtnRefresh_OnPress;

		btnOn = GetChildById("btnOn");
		btnOn_Background = (XUiV_Button)btnOn.GetChildById("clickable").ViewComponent;
		btnOn_Background.Controller.OnPress += BtnOn_OnPress;

		btnUpgrade = GetChildById("btnUpgrade");
		btnUpgrade_Background = (XUiV_Button)btnUpgrade.GetChildById("clickable").ViewComponent;
		btnUpgrade_Background.Controller.OnPress += BtnUpgrade_OnPress;

		lblOnOff = (XUiV_Label)GetChildById("lblOnOff").ViewComponent;
		sprOnOff = (XUiV_Sprite)GetChildById("sprOnOff").ViewComponent;

		lblUpgrade = (XUiV_Label)GetChildById("lblUpgrade").ViewComponent;
		sprUpgrade = (XUiV_Sprite)GetChildById("sprUpgrade").ViewComponent;

		((XUiV_Label)GetChildById("lblRefresh").ViewComponent).Text = Localization.Get("xuiServerBrowserRefreshList");
	}

	public override void Update(float _dt)
	{
		base.Update(_dt);

		if (TileEntity == null)
			return;

		if (lastOn != TileEntity.IsOn)
		{
			RefreshIsOn(TileEntity.IsOn);
		}

		RefreshUpgradeOn(TileEntity.UpgradeOn);
	}

	public override void OnOpen()
	{
		base.OnOpen();

		RefreshIsOn(TileEntity.IsOn);
		RefreshUpgradeOn(TileEntity.UpgradeOn);
		// RefreshBindings();
		// RefreshBindingsSelfAndChildren();

		logger.Info($"OnOpen, windowWidth: {WindowWidth}");
	}

	public override void OnClose()
	{
		// GameManager instance = GameManager.Instance;
		// Vector3i blockPos = TileEntity.ToWorldPos();
		// if (!XUiC_CameraWindow.hackyIsOpeningMaximizedWindow)
		// {
		// 	TileEntity.SetUserAccessing(_bUserAccessing: false);
		// 	instance.TEUnlockServer(TileEntity.GetClrIdx(), blockPos, TileEntity.entityId, false);
		// 	TileEntity.SetModified();
		// }

		var header = GetChildById("header");

		logger.Info($"OnClose - width: {ViewComponent.Width}, header: {header.ViewComponent.Width}");

		base.OnClose();
	}

	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		switch (_bindingName)
		{
			case "windowWidth":
				_value = WindowWidth.ToString();
				return true;

			case "lblBlocksToRepair":
				_value = $"{TileEntity?.DamagedBlockCount:N0} damaged blocks found.";
				return true;

			case "lblBlocksToUpgrade":
				_value = $"{TileEntity?.UpgradableBlockCount:N0} upgradable blocks found.";
				return true;

			case "lblTotalDamages":
				_value = $"{TileEntity?.TotalDamagesCount:N0} damages points to repair.";
				return true;

			case "lblVisitedBlocks":
				_value = $"{TileEntity?.VisitedBlocksCount:N0} blocks visited.";
				return true;

			case "lblTimer":
				_value = $"Repair time {TileEntity?.CalcRepairTime()}";
				return true;

			case "upgradeEnabled":
				_value = TileEntity?.UpgradeEnabled.ToString();
				return true;

			default:
				return base.GetBindingValueInternal(ref _value, _bindingName);
		}
	}

	private void BtnRefresh_OnPress(XUiController _sender, int _mouseButton)
	{
		TileEntity.ForceRefresh();
		Manager.PlayInsidePlayerHead("UseActions/chest_tier4_open");
	}

	private void BtnOn_OnPress(XUiController _sender, int _mouseButton)
	{
		TileEntity.Switch();
	}

	private void BtnUpgrade_OnPress(XUiController _sender, int _mouseButton)
	{
		TileEntity.SwitchUpgrade();
	}

	private void RefreshUpgradeOn(bool upgradeOn)
	{
		if (upgradeOn)
		{
			lblUpgrade.Text = UpgradeOffText;
			if (sprUpgrade != null)
			{
				sprUpgrade.Color = onColor;
			}
		}
		else
		{
			lblUpgrade.Text = UpgradeOnText;
			if (sprUpgrade != null)
			{
				sprUpgrade.Color = offColor;
			}
		}
	}

	private void RefreshIsOn(bool isOn)
	{
		lastOn = isOn;

		if (isOn)
		{
			lblOnOff.Text = PropTurnOff;
			if (sprOnOff != null)
			{
				sprOnOff.Color = onColor;
			}
		}
		else
		{
			lblOnOff.Text = PropTurnOn;
			if (sprOnOff != null)
			{
				sprOnOff.Color = offColor;
			}
		}
	}

}
