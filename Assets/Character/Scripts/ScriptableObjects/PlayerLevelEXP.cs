using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//data save template
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerLevelEXP", order = 1)]
public class PlayerLevelEXP : ScriptableObject
{
    // EXP per level
    public List<int> totalEXPNeeded = new List<int> { 0, 50 };
}
