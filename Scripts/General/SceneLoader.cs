using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance {  get; private set; }

    private float _delay;
    private string _sceneName;
    private void Awake()
    {
        if(Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void LoadScene(string sceneName, float delay = 0f)
    { 
        _delay = delay;
        _sceneName = sceneName;
        StartCoroutine(Load());
    }
    private IEnumerator Load()
    { 
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(_sceneName);
    }
}
