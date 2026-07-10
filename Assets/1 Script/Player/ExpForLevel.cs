using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class ExpForLevel : ScriptableObject
{
    public List<int> nextExpRequired;
}
