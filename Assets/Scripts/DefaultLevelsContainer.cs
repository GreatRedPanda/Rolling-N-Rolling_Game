using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DefaultLevels/Levels")]
public class DefaultLevelsContainer : ScriptableObject
{
    public List<LevelDataDefault> Levels;
   
}