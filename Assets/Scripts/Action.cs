using UnityEngine;
using System.Collections;

public class Action 
{
    public Texture2D icon;
    public enum actionType { move, settle, attack };
    public actionType type;
    public string tooltip;

    public Action(Texture2D _icon, Action.actionType _type)
    {
        icon = _icon;
        type = _type;
    }

    public Action(Texture2D _icon, Action.actionType _type, string _tooltip)
    {
        icon = _icon;
        type = _type;
        tooltip = _tooltip;
    }
}
