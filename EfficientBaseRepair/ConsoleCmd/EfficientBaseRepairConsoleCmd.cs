using System;
using System.Collections.Generic;

public class EfficientBaseRepairConsoleCmd : ConsoleCmdAbstract
{
    private static readonly Logging.Logger logger = Logging.CreateLogger("EBRConsoleCmd");

    public static readonly List<string> activeBoxNames = new List<string>();

    public override string[] getCommands()
    {
        return new string[] { "efficientbaserepair", "ebr" };
    }

    public override string getDescription()
    {
        return "efficientbaserepair ebr => command line tools for the mod EfficientBaseRepair.";
    }

    public override string getHelp()
    {
        return @"EfficientBaseRepair commands:
            - isChild: log the selected blockValue.isChild
            - neighbors: select all neighbors of the selected block
            - clear: clear all selection boxes added by command 'neighbors'
            - material, mat: fill the opened EfficientBaseRepair crate with required materials
            - setfuel <value>: set the given fuel amount into the opened powerSource item. If no value is given, a random value is choosen.
            - getconfig <name>: show the value of the given ebr parameter. The name is case-sensitive and must be defined in ModConfig.xml, ex: `get repairRate`
            - setconfig <name> <value>: set the value of the given ebr parameter, ex: `set repairRate 100`
            - setdamage, sd: set a given damage value to all blocks inside the selection box
        ";
    }

    public static SelectionCategory GetSelectionCategory()
    {
        var selectionBoxCategory = "BlockSelectionUtils";
        var sbm = SelectionBoxManager.Instance;

        if (!sbm.categories.ContainsKey(selectionBoxCategory))
        {
            sbm.CreateCategory(
                _name: selectionBoxCategory,
                _colSelected: new UnityEngine.Color(0f, 0f, 1f, 0.5f),
                _colUnselected: new UnityEngine.Color(0f, 0f, 1f, 0.5f),
                _colFaceSelected: new UnityEngine.Color(1f, 1f, 0f, 0.4f),
                _bCollider: false,
                _tag: null
            );
        }

        return sbm.categories[selectionBoxCategory];
    }

    private void SelectBlock(Vector3i pos)
    {
        var selectionCat = GetSelectionCategory();
        var boxName = pos.ToString();

        SelectionBox box = selectionCat.AddBox(boxName, pos, Vector3i.one);
        box.SetVisible(true);
        box.SetSizeVisibility(_visible: true);

        selectionCat.SetVisible(true);

        activeBoxNames.Add(boxName);
    }

    private IEnumerable<Vector3i> GetSelectionBoxPositions()
    {
        var selection = BlockToolSelection.Instance;

        var start = selection.m_selectionStartPoint;
        var end = selection.m_SelectionEndPoint;

        int y = start.y;
        while (true)
        {
            int x = start.x;
            while (true)
            {
                int z = start.z;
                while (true)
                {
                    yield return new Vector3i(x, y, z);

                    if (z == end.z) break;
                    z += Math.Sign(end.z - start.z);
                }
                if (x == end.x) break;
                x += Math.Sign(end.x - start.x);
            }
            if (y == end.y) break;
            y += Math.Sign(end.y - start.y);
        }

        yield break;
    }

    private void CmdIsChild()
    {
        var position = BlockToolSelection.Instance.m_selectionStartPoint;
        var isChild = GameManager.Instance.World.GetBlock(position).ischild;

        logger.Info(isChild);
    }

    private void CmdNeighbors()
    {
        var position = BlockToolSelection.Instance.m_selectionStartPoint;
        var blockValue = GameManager.Instance.World.GetBlock(position);

        SelectionBoxManager.Instance.Deactivate();

        foreach (var pos in TEFeatureEBR.GetNeighbors(position, blockValue))
        {
            SelectBlock(pos);
        }
    }

    private void CmdClearBoxes()
    {
        var selectionCat = GetSelectionCategory();

        foreach (var name in activeBoxNames)
        {
            selectionCat.RemoveBox(name);
        }

        activeBoxNames.Clear();
    }

    private void CmdMaterial()
    {
        var xuiController = EBRUtils.GetXuiController<XUiC_EfficientBaseRepair>();

        if (xuiController is null || !xuiController.IsOpen)
        {
            logger.Error("No EfficientBaseRepair crate is open.");
            return;
        }

        var tileEntity = xuiController.TileEntity;

        foreach (var material in tileEntity.requiredMaterials)
        {
            var itemName = material.Key;
            var itemCount = material.Value;

            var itemType = ItemClass.nameToItem[itemName].Id;
            var itemValue = new ItemValue(itemType);
            var itemStack = new ItemStack(itemValue, itemCount);

            tileEntity.AddItem(itemStack);
        }
    }

    private void CmdSetFuel(string[] args)
    {
        var xuiController = EBRUtils.GetXuiController<XUiC_PowerSourceStats>();

        if (xuiController is null || !xuiController.IsOpen)
        {
            logger.Error("No power source item is open");
            return;
        }

        if (args.Length < 2 || !int.TryParse(args[1], out var value))
        {
            value = new System.Random().Next(xuiController.tileEntity.MaxFuel);
        }

        xuiController.tileEntity.CurrentFuel = (ushort)value;
    }

    private void CmdSetConfig(string[] args)
    {
        ModConfig.SetField<Config>(args[1], args[2]);
    }

    private void CmdGetConfig(string[] args)
    {
        var fieldName = args.Length > 1 ? args[1] : "";
        var value = ModConfig.GetField<Config>(fieldName);

        if (value != null)
        {
            Log.Out(value.ToString());
        }
    }

    private void CmdSetDamage(string[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("missing argument: 'damage'");
        }

        if (!int.TryParse(args[1], out int damage))
        {
            throw new ArgumentException($"Invalid argument: '{args[0]}'. Integer value is required");
        }

        var blockChangeInfos = new List<BlockChangeInfo>();

        foreach (var blockPos in GetSelectionBoxPositions())
        {
            var blockValue = GameManager.Instance.World.GetBlock(blockPos);
            var maxDamage = blockValue.Block.MaxDamage;

            if (!blockValue.isair)
            {
                blockValue.damage = Math.Clamp(maxDamage - damage, 0, maxDamage);
                blockChangeInfos.Add(new BlockChangeInfo(blockPos, blockValue));
            }
        }

        GameManager.Instance.World.SetBlocksRPC(blockChangeInfos);
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        var args = _params.ToArray();

        if (args.Length == 0)
        {
            Log.Out(getHelp());
            return;
        }

        switch (args[0].ToLower())
        {
            case "ischild":
                CmdIsChild();
                break;

            case "neighbor":
            case "neighbors":
                CmdNeighbors();
                break;

            case "clear":
                CmdClearBoxes();
                break;

            case "material":
            case "mat":
                CmdMaterial();
                break;

            case "setfuel":
                CmdSetFuel(args);
                break;

            case "setconfig":
            case "set":
                CmdSetConfig(args);
                break;

            case "getconfig":
            case "get":
                CmdGetConfig(args);
                break;

            case "setdamage":
            case "sd":
                CmdSetDamage(args);
                break;

            default:
                logger.Error($"Invalid or not implemented command: '{_params[0]}'");
                break;
        }
    }
}