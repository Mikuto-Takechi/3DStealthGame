using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioDataBase", menuName = "ScriptableObjects/AudioDataBase", order = 1)]
public class AudioDataBase : ScriptableObject
{
    public string Name;
    public string Summary;
    public NameAudioPair[] Clips;
    
    [ContextMenu("EnumCreate")]
    void EnumCreate()
    {
        if (Name == string.Empty) return;
        List<string> filenames = new List<string>();
        int unassignedCount = 0;
        foreach (var pair in Clips)
        {
            if (pair.Clip && pair.Name != String.Empty)
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
    [Serializable]
    public class NameAudioPair
    {
        public string Name;
        public AudioClip Clip;
    }
}
