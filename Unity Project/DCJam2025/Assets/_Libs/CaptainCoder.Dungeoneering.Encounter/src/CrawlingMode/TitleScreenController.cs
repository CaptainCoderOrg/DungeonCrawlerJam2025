
using System.Collections;

using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.SceneManagement;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class TitleScreenController : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] private ScreenHider _screenHider;

        void Awake()
        {
            _screenHider.Show();
        }

        public void StartCrawling() => StartCoroutine(StartCrawlerRoutine());
        public void TitleScreen() => StartCoroutine(StartTitleScreenRoutine());
        public void Credits() => StartCoroutine(StartCreditsRoutine());

        public IEnumerator StartCreditsRoutine()
        {
            yield return _screenHider.HideScreenCoroutine();
            AsyncOperation operation = SceneManager.LoadSceneAsync("Credits");
            while (!operation.isDone) { yield return null; }
        }

        public IEnumerator StartTitleScreenRoutine()
        {
            yield return _screenHider.HideScreenCoroutine();
            AsyncOperation operation = SceneManager.LoadSceneAsync("Title Screen");
            while (!operation.isDone) { yield return null; }
        }
        public IEnumerator StartCrawlerRoutine()
        {
            yield return _screenHider.HideScreenCoroutine();
            AsyncOperation operation = SceneManager.LoadSceneAsync("DungeonCrawling");
            while (!operation.isDone) { yield return null; }
        }

        
    }
}