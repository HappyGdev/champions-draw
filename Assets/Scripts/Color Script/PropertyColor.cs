using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class PropertyColor : PropertyAttribute
{
    public Color color{ get; }
    ///<summary>
    ///set the color of the property field (HTML string e.g. "red" or "#FF0000")
    /// </summary>
    /// 

    public PropertyColor(string htmlColorString)
    {
        if(ColorUtility.TryParseHtmlString(htmlColorString,out Color c))
        {
            color = c;
            return;
        }
        color = Color.white;
    }
}

