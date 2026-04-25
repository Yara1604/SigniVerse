//using UnityEngine;

//public class BridgeClickable : MonoBehaviour
//{
//    public BridgePuzzleManager puzzleManager;
//    private bool playerNearby = false;

//    private void OnMouseDown()
//    {
//        if (playerNearby && puzzleManager != null)
//        {
//            puzzleManager.OpenPuzzlePanel();
//        }
//    }

//    public void SetPlayerNearby(bool value)
//    {
//        playerNearby = value;
//    }
//}
using UnityEngine;

public class BridgeClickable : MonoBehaviour
{
    public BridgePuzzleManager puzzleManager;
    private bool playerNearby = false;

    // Check if the click is even being registered
    private void OnMouseDown()
    {
        Debug.Log("1. Mouse Down Detected on Bridge Object!");

        if (playerNearby)
        {
            Debug.Log("2. Player is close. Opening Puzzle...");
            if (puzzleManager != null)
            {
                puzzleManager.OpenPuzzlePanel();
            }
        }
        else
        {
            Debug.Log("2. Click registered, but player is TOO FAR.");
        }
    }

    public void SetPlayerNearby(bool value)
    {
        playerNearby = value;
        Debug.Log("Player Near Bridge: " + value);
    }
}