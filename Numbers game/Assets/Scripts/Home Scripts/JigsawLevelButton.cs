using UnityEngine;
using UnityEngine.SceneManagement;

public class JigsawLevelButton : MonoBehaviour
{

    [SerializeField] private string sceneName;

    public void Load()
    {
        SceneManager.LoadScene(sceneName);
    }
}