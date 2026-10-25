using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongLibrary", menuName = "Encore/Song Library")]
public class SongLibrary : ScriptableObject
{
    public List<SongData> songs = new List<SongData>();
}