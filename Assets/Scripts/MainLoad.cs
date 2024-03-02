using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainLoad : MonoBehaviour
{
    private void Awake()
    {
        DOVirtual.DelayedCall(0.5f, () => StartCoroutine(LoadScene()));
    }

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        while (!operation.isDone)
            yield return null;
    }
}
