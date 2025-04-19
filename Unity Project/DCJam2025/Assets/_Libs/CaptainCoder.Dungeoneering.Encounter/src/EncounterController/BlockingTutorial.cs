using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class BlockingTutorial : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ToggleablePanel _raycastBlocker;
        [AssertIsSet][SerializeField] private EncounterTutorialController _tutorial;
        [SerializeField] private string _nextTutorial;

        public void Activate()
        {
            _raycastBlocker.IsEnabled = true;
            _tutorial.OnClick += HandleNext;
        }

        private void HandleNext()
        {
            _tutorial.OnClick -= HandleNext;
            _raycastBlocker.IsEnabled = false;
            if (_nextTutorial == null || _nextTutorial.Trim() == string.Empty)
            {
                _tutorial.HideTutorial();
                return;
            }
            _tutorial.ShowTutorialIfNeverSeen(_nextTutorial);
        }
    }
}