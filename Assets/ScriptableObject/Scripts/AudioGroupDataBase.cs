using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioGroupDataBase", menuName = "ScriptableObjects/AudioGroupDataBase", order = 1)]
public class AudioGroupDataBase : ScriptableObject
{
    public string Name;
    public string Summary;
    public NameAudioGroupPair[] AudioGroups;
    
    #if UNITY_EDITOR
    [ContextMenu("EnumCreate")]
    void EnumCreate()
    {
        if (Name == string.Empty) return;
        List<string> filenames = new List<string>();
        int unassignedCount = 0;
        foreach (var pair in AudioGroups)
        {
            bool assignedPair = true;
            foreach (var clip in pair.Clips)
            {
                if (!clip)
                {
                    assignedPair = false;
                    break;
                }
            }
            if (assignedPair && pair.Name != String.Empty)
            {
                filenames.Add(pair.Name);
            }
            else
            {
                filenames.Add($"unassigned___{unassignedCount}");
                unassignedCount++;
            }
        } 
        if(Summary == String.Empty)
            EnumCreator.Create(Name, filenames, $"Assets/ScriptableObject/Enum/{Name}.cs");
        else
            EnumCreator.Create(Name, filenames, $"Assets/ScriptableObject/Enum/{Name}.cs", Summary);
    }
    #endif
    [Serializable]
    public class NameAudioGroupPair
    {
        public string Name;
        public AudioClip[] Clips;
    }
}