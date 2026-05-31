using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct JigsawLevelData
{
    public string levelName;
    public string imagePath;          // e.g., "images/jigsaws/car"
    public int columns;                // e.g., 4
    public string targetSignWord;     // What AI looks for
    public string displaySignText;    // What child reads
    public VideoClip instructionalVideo;
}