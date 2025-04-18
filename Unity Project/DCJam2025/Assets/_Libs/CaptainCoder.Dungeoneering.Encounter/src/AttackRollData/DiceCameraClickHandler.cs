using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DiceCameraClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private Camera _diceBoxCamera;
        private RectTransform Rect => (RectTransform)transform;
        private Vector3[] _corners = new Vector3[4];
        public void OnPointerClick(PointerEventData eventData)
        {
            Rect.GetWorldCorners(_corners);
            Debug.Log(eventData);
            Debug.Log(eventData.position - new Vector2(_corners[0].x, _corners[0].y));
        }
    }
}