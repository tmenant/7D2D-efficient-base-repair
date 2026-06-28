using UnityEngine.Assertions;

public static class EBRUtils
{
    public static void Assert(bool condition, string message = "")
    {
        if (!condition)
        {
            throw new AssertionException(message.ToString(), "");
        }
    }

    public static int GetMissingItemCount(ItemStack stack, int stackSize)
    {
        if (stack.IsEmpty())
        {
            return stackSize;
        }

        return stackSize - stack.count;
    }

    public static T GetXuiController<T>() where T : XUiController
    {
        return GameManager.Instance.World.GetPrimaryPlayer().playerUI.xui.GetChildByType<T>();
    }
}