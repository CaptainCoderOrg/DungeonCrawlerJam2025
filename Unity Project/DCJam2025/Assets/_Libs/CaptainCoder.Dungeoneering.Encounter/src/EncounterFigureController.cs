using System;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterFigureController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private MeshRenderer _baseMeshRenderer;
        [AssertIsSet][SerializeField] private float _flickerSpeed = 0.5f;
        [SerializeField] private bool _isSelected = false;
        [AssertIsSet][SerializeField] private Transform _figureQuad;
        [AssertIsSet][SerializeField] private Transform _scalePivot;
        [SerializeField] private FigureData _figureData;
        [SerializeField] private bool _isCrawlingMode;
        public FigureData Figure
        {
            get => _figureData;
            set
            {
                _figureData = value;
                Initialize();
            }
        }
        [AssertIsSet][SerializeField] private QuadAnimator _animator;
        private EncounterController _controller;
        [field: SerializeField] public UnityEvent<EncounterFigureController> OnClick { get; private set; }
        [field: SerializeField] public UnityEvent OnSelected { get; private set; }
        [field: SerializeField] public UnityEvent OnDeselected { get; private set; }

        public void Click() => OnClick.Invoke(this);

        public void Select()
        {
            OnSelected.Invoke();
            _isSelected = true;
        }

        void Awake()
        {
            if (!_isCrawlingMode)
            {
                _controller = GetComponentInParent<EncounterController>();
                Debug.Assert(_controller != null, $"Could not find {nameof(_controller)}", this);
            }
        }

        [Button]
        public void AdjustPivot()
        {
            Debug.Log(Camera.main.transform.rotation.eulerAngles);
            Vector3 result = Camera.main.transform.rotation * _figureData.EntityData.IdleAnimation.SpriteSheet.TileOffset;
            Debug.Log(result);
            result.y = 0;
            _scalePivot.localPosition = result;
        }

        private void Initialize()
        {
            _animator.Play(_figureData.EntityData.SpawnAnimation);
            transform.localPosition = _figureData.LocalPosition;
            _scalePivot.localScale = new Vector3(_figureData.EntityData.IdleAnimation.SpriteSheet.XRatio, _figureData.EntityData.IdleAnimation.SpriteSheet.YRatio, _figureData.EntityData.IdleAnimation.SpriteSheet.XRatio);
            AdjustPivot();
        }

        void OnEnable()
        {
            _controller?.EncounterCamera?.ObserveCamera(FollowCamera);
        }

        void OnDisable()
        {
            _controller?.EncounterCamera?.RemoveObserver(FollowCamera);
        }

        private void FollowCamera(Camera camera)
        {
            Vector3 eulers = camera.transform.rotation.eulerAngles;
            eulers.x *= 0.25f;
            eulers.z = 0;
            _figureQuad.rotation = Quaternion.Euler(eulers);
            Vector3 position = _figureQuad.localPosition;
            position.y = 0.7f + (eulers.x * .01f);
            _figureQuad.localPosition = position;
            AdjustPivot();
        }

        public void Update()
        {
            if (!_isSelected)
            {
                _baseMeshRenderer.enabled = false;
                return;
            }
            _baseMeshRenderer.enabled = true;
            Color c = _baseMeshRenderer.material.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI * _flickerSpeed)) * 0.25f + 0.20f;
            _baseMeshRenderer.material.color = c;
        }

        internal void Deselect()
        {
            _isSelected = false;
            OnDeselected.Invoke();
        }

        internal void Hide()
        {
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }
    }
}