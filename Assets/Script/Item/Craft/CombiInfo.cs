using UnityEngine;
using System.Collections.Generic;

public class CombineInfo 
{
    internal string MainProperty;
    internal string SubProperty;
    internal HashSet<string> ResultProperty;
    public string Result;
    internal Sprite ResultSprite;

    public CombineInfo(string m, string s, string res ,HashSet<string> properties, Sprite sprite)
    {
        MainProperty = m;
        SubProperty = s;
        ResultProperty = properties;
        Result = res;
        ResultSprite = sprite;
    }

}
