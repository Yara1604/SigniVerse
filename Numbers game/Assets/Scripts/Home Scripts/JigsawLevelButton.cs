using UnityEngine;
using UnityEngine.SceneManagement;

public class JigsawLevelButton : MonoBehaviour
{
    [Header("Level Configuration")]
    public string imagePathForThisLevel;      // e.g., "images/jigsaws/butterfly"
    public int columnsForThisLevel = 4;        // Control piece count per grid
    public string targetSignWord = "Butterfly"; // What the AI panel listens for
    public string displaySignText = "Butterfly"; // What the child reads on screen
    public UnityEngine.Video.VideoClip instructionalVideo;

    // Static variables to pass data safely between scenes
    public static string SelectedImagePath;
    public static int SelectedColumns;
    public static string SelectedTargetWord;
    public static string SelectedDisplayText;
    public static UnityEngine.Video.VideoClip SelectedVideo;

    public void OnClickPlayLevel()
    {
        // Save choices to temporary memory
        SelectedImagePath = imagePathForThisLevel;
        SelectedColumns = columnsForThisLevel;
        SelectedTargetWord = targetSignWord;
        SelectedDisplayText = displaySignText;
        SelectedVideo = instructionalVideo;

        // Load the shared template scene
        SceneManager.LoadScene("HomeGameTemplate");
    }
}