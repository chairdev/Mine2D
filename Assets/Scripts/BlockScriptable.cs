using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/BlockScriptable", order = 1)]
public class BlockScriptable : ScriptableObject
{
    public Sprite sprite;
    public Color lineColor;
}