using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "SampleScene"; 

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
