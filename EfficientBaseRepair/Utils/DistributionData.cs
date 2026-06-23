using System.Linq;
using UnityEngine.Assertions;

public class DistributionData
{
    public readonly TileEntity tileEntity;

    public readonly ItemClass itemClass;

    public readonly int stackSize;

    public readonly int stackCount;

    public int ItemCount { get; set; }

    public int TotalAdded { get; set; }

    public DistributionData(TileEntity tileEntity, ItemClass itemClass, int itemCount, int stackSize, int stackCount)
    {
        EBRUtils.Assert(stackCount > 0, "stackCount must be greater than 0");

        this.tileEntity = tileEntity;
        this.itemClass = itemClass;
        this.stackSize = stackSize;
        this.stackCount = stackCount;
        this.ItemCount = itemCount;
    }

    public ItemStack[] ToItemStacks()
    {
        var remainingItems = ItemCount;
        var itemStackSize = itemClass.Stacknumber.Value;
        var itemValue = new ItemValue(itemClass.Id);
        var itemStacks = new ItemStack[stackCount];

        for (int i = 0; i < stackCount; i++)
        {
            var itemCount = Utils.FastMin(remainingItems, itemStackSize);
            remainingItems -= itemCount;

            itemStacks[i] = new ItemStack(itemValue, itemCount);
        }

        EBRUtils.Assert(remainingItems == 0, $"remaining items: {remainingItems}");

        return itemStacks;
    }

    public string ItemStacksToString()
    {
        var values = ToItemStacks()
            .Select(stack => stack.count.ToString())
            .ToArray();

        return $"[{string.Join(", ", values)}]";
    }

    public override string ToString()
    {
        return $"[{tileEntity.ToWorldPos(),12}] item: {itemClass.Name}, size: {stackSize}, count: {ItemCount}, added: {TotalAdded}, stacks: {ItemStacksToString()}";
    }
}
