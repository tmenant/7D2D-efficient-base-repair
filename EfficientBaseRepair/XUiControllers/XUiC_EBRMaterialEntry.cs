using UnityEngine;

public class XUiC_EBRMaterialEntry : XUiController
{
    private static Color validColor = Color.green;

    private static Color invalidColor = Color.red;

    private XUiV_Label Label { get; set; }

    private XUiV_Sprite Sprite { get; set; }

    public override void Init()
    {
        base.Init();

        Label = GetChildById("label").viewComponent as XUiV_Label;
        Sprite = GetChildById("icon").viewComponent as XUiV_Sprite;

        EBRUtils.Assert(Label != null);
        EBRUtils.Assert(Sprite != null);
    }

    public void SetMaterial(ItemClass itemClass, int available, int required)
    {
        ViewComponent.ToolTip = itemClass.GetLocalizedItemName();
        Label.Text = $"{available} / {required}";
        Label.Color = available >= required ? validColor : invalidColor;
        Sprite.SetSpriteImmediately(itemClass.GetIconName());
    }

    public void SetEmpty()
    {
        ViewComponent.ToolTip = null;
        Label.SetTextImmediately(null);
        Sprite.SetSpriteImmediately(null);
    }

    public override bool ParseAttribute(string name, string value)
    {
        if (base.ParseAttribute(name, value))
            return true;

        switch (name)
        {
            case "valid_materials_color":
                validColor = StringParsers.ParseColor32(value);
                return true;

            case "invalid_materials_color":
                invalidColor = StringParsers.ParseColor32(value);
                return true;
        }

        return false;
    }

    public override void OnHovered(bool _isOver)
    {
        var background = GetChildById("background").ViewComponent as XUiV_Sprite;

        if (_isOver)
        {
            background.SetColorImmediately(new Color32(96, 96, 96, byte.MaxValue));
        }
        else
        {
            background.SetColorImmediately(new Color32(64, 64, 64, byte.MaxValue));
        }
    }
}