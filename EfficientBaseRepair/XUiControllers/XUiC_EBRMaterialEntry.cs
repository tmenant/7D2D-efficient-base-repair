using UnityEngine;

public class XUiC_EBRMaterialEntry : XUiController
{
    private static Color colorEnought = Color.green;
    private static Color colorMissing = Color.yellow;
    private static Color colorEmpty = Color.red;

    private XUiV_Label Label { get; set; }
    private XUiV_Sprite Sprite { get; set; }
    private XUiV_Sprite Background { get; set; }

    public override void Init()
    {
        base.Init();

        Label = GetChildById("label").viewComponent as XUiV_Label;
        Sprite = GetChildById("icon").viewComponent as XUiV_Sprite;
        Background = GetChildById("background").ViewComponent as XUiV_Sprite;

        EBRUtils.Assert(Label != null);
        EBRUtils.Assert(Sprite != null);
        EBRUtils.Assert(Background != null);
    }

    public void SetMaterial(ItemClass itemClass, int available, int required)
    {
        ViewComponent.ToolTip = itemClass.GetLocalizedItemName();
        Label.Text = $"{available} / {required}";
        Label.Color = GetLabelColor(available, required);
        Sprite.SetSpriteImmediately(itemClass.GetIconName());
    }

    private Color GetLabelColor(int available, int required)
    {
        if (available >= required)
        {
            return colorEnought;
        }
        else if (available > 0)
        {
            return colorMissing;
        }

        return colorEmpty;
    }

    public void SetEmpty()
    {
        ViewComponent.ToolTip = null;
        Label.SetTextImmediately(null);
        Sprite.SetSpriteImmediately(null);
    }

    public override void OnHovered(bool _isOver)
    {
        if (_isOver)
        {
            Background?.SetColorImmediately(new Color32(96, 96, 96, byte.MaxValue));
        }
        else
        {
            Background?.SetColorImmediately(new Color32(64, 64, 64, byte.MaxValue));
        }
    }
}