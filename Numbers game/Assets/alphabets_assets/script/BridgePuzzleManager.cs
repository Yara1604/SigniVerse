using UnityEngine;

public class BridgePuzzleManager : MonoBehaviour
{
    public GameObject puzzlePanel;
    public SpriteRenderer bridgeRenderer;
    public PolygonCollider2D walkCollider;
    public CarController carController;

    private bool bridgeUnlocked = false;

    void Start()
    {
        bridgeRenderer.color = Color.black;
        walkCollider.enabled = false;
        puzzlePanel.SetActive(false);
    }

    public void OpenPuzzlePanel()
    {
        if (bridgeUnlocked) return;
        //puzzlePanel.SetActive(true);
    }

    public void ContinueGame()
    {
        bridgeUnlocked = true;
        puzzlePanel.SetActive(false);

        bridgeRenderer.color = Color.white;
        walkCollider.enabled = true;

        if (carController != null)
        {
            carController.ResumeFromBridge();
        }
        Debug.Log("Continue pressed");
        Debug.Log("walkCollider enabled: " + walkCollider.enabled);
    }
}