using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JigsawCanvasManager : MonoBehaviour
{
    [Header("Level Sequence Playlist")]
    [Tooltip("List all the levels for this area in their exact gameplay order.")]
    public List<JigsawLevelData> areaLevels = new List<JigsawLevelData>();

    // Internal tracker to remember which level from the playlist is currently running
    private int currentLevelIndex = -1;

    private void Start()
    {
        DetermineCurrentLevelIndex();
    }

    // Looks at what image is currently loaded to find its position in our playlist
    private void DetermineCurrentLevelIndex()
    {
        if (string.IsNullOrEmpty(JigsawLevelButton.SelectedImagePath)) return;

        for (int i = 0; i < areaLevels.Count; i++)
        {
            if (areaLevels[i].imagePath == JigsawLevelButton.SelectedImagePath)
            {
                currentLevelIndex = i;
                break;
            }
        }
    }

    // RETRY BUTTON: Resets  current level using same configuration data
    public void OnClickRetryButton()
    {
        Time.timeScale = 1f;
        // Simply reload the current template scene; the handshake variables remain safely in memory!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // NEXT LEVEL BUTTON: Advances  player to next level in the area playlist
    public void OnClickNextLevelButton()
    {
        Time.timeScale = 1f;

        // Calculate next index position
        int nextLevelIndex = currentLevelIndex + 1;

        // Safety Check: Have they beaten the final level of this area?
        if (nextLevelIndex >= areaLevels.Count)
        {
            Debug.Log("Area Completed! Returning to Level Selection Menu.");
            ExitToLevelSelect();
            return;
        }

        // Extract the next level's configuration payload
        JigsawLevelData nextLevel = areaLevels[nextLevelIndex];

        // Load the data directly into the static handshake fields
        JigsawLevelButton.SelectedImagePath = nextLevel.imagePath;
        JigsawLevelButton.SelectedColumns = nextLevel.columns;
        JigsawLevelButton.SelectedTargetWord = nextLevel.targetSignWord;
        JigsawLevelButton.SelectedDisplayText = nextLevel.displaySignText;
        JigsawLevelButton.SelectedVideo = nextLevel.instructionalVideo;

        // Reload the template scene to build the next puzzle!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// BACK/EXIT BUTTON: Routes the player safely back to the select screen
    public void ExitToLevelSelect()
    {
        Time.timeScale = 1f;
        if (JigsawManager.Instance != null)
        {
            SceneManager.LoadScene(JigsawManager.Instance.areaLevelSelectSceneName);
        }
        else
        {
            SceneManager.LoadScene("BedroomScene"); // Fallback safety
        }
    }
}
