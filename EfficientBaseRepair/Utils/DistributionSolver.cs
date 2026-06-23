using System.Collections.Generic;
using System.Linq;


class DistributionSolver
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<DistributionSolver>();

    private readonly List<DistributionData> distribDatas = new List<DistributionData>();

    public readonly ItemClass itemClass;

    private readonly int stackSize;

    public DistributionSolver(ItemClass itemClass, int stackSize)
    {
        this.itemClass = itemClass;
        this.stackSize = stackSize;
    }

    public void AddDatas(TileEntity tileEntity, int itemCount, int stackCount)
    {
        var distribData = new DistributionData(tileEntity, itemClass, itemCount, stackSize, stackCount);

        distribDatas.Add(distribData);
    }

    public List<DistributionData> CalcDistributionDatas(int amountToAdd)
    {
        while (amountToAdd > 0)
        {
            var entriesNotFull = distribDatas.Where(data => data.ItemCount < stackSize).ToArray();

            if (entriesNotFull.Length == 0)
                break;

            int minValue = entriesNotFull.Min(data => data.ItemCount);
            var minLevelDatas = entriesNotFull.Where(stack => stack.ItemCount == minValue).ToArray();
            int minLevelDatasCount = minLevelDatas.Length;

            if (amountToAdd >= minLevelDatasCount)
            {
                foreach (var data in minLevelDatas)
                {
                    data.ItemCount++;
                    data.TotalAdded++;
                }

                amountToAdd -= minLevelDatasCount;
            }
            else
            {
                foreach (var data in minLevelDatas.Take(amountToAdd))
                {
                    data.ItemCount++;
                    data.TotalAdded++;
                }

                amountToAdd = 0;
            }
        }

        return distribDatas;
    }
}