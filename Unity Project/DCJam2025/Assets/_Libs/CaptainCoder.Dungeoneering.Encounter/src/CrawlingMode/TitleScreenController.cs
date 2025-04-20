
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
        public IEnumerator StartCrawlerRoutine()
        {
            yield return _screenHider.HideScreenCoroutine();
            AsyncOperation operation = SceneManager.LoadSceneAsync("DungeonCrawling");
            while (!operation.isDone) { yield return null; }
        }
    }
}