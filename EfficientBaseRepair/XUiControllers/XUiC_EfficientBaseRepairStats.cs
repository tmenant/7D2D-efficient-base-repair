using Audio;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class XUiC_EfficientBaseRepairStats : XUiController
{
	private static readonly Logging.Logger logger = Logging.CreateLogger<XUiC_EfficientBaseRepairStats>();

	private static readonly Color32 onColor = new Color32(250, 255, 163, 255);
	private static readonly Color32 offColor = Color.white;

	private XUiV_Label lblOnOff;
	private XUiV_Sprite sprOnOff;
	private XUiV_Label lblUpgrade;
	private XUiV_Sprite sprUpgrade;

	private string RefreshSound => "UseActions/chest_tier4_open";
	private string PropTurnOff => Localization.Get("xuiTurnOff");
	private string PropTurnOn => Localization.Get("xuiTurnOn");
	private string PropUpgradeOn => Localization.Get("ebrUpgradeOn");
	private string PropUpgradeOff => Localization.Get("ebrUpgradeOff");

	public TEFeatureEBR TileEntity { get; set; }
	public int WindowWidth { get; set; }

	public override void Init()
	{
		base.Init();

		GetChildById("btnOn").OnPress += BtnOn_OnPress;
		GetChildById("btnRefresh").OnPress += BtnRefresh_OnPress;
		GetChildById("btnUpgrade").OnPress += BtnUpgrade_OnPress;

		lblOnOff = GetChildById("lblOnOff").ViewComponent as XUiV_Label;
		sprOnOff = GetChildById("sprOnOff").ViewComponent as XUiV_Sprite;

		lblUpgrade = GetChildById("lblUpgrade").ViewComponent as XUiV_Label;
		sprUpgrade = GetChildById("sprUpgrade").ViewComponent as XUiV_Sprite;
	}

	public override void Update(float _dt)
	{
		base.Update(_dt);

		lblOnOff.SetTextImmediately(TileEntity.IsOn ? PropTurnOff : PropTurnOn);
		sprOnOff.SetColorImmediately(TileEntity.IsOn ? onColor : offColor);

		lblUpgrade.SetTextImmediately(TileEntity.UpgradeOn ? PropUpgradeOff : PropUpgradeOn);
		sprUpgrade.SetColorImmediately(TileEntity.UpgradeOn ? onColor : offColor);

		RefreshBindingsSelfAndChildren();
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
		Manager.BroadcastPlay(RefreshSound);
	}

	private void BtnOn_OnPress(XUiController _sender, int _mouseButton)
	{
		TileEntity.Switch();
	}

	private void BtnUpgrade_OnPress(XUiController _sender, int _mouseButton)
	{
		TileEntity.SwitchUpgrade();
	}
}
