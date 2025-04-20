using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroFigurePanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ConsumableDropZone _consumableDropZone;
        [AssertIsSet][SerializeField] private ToggleablePanel _knockedOutPanel;
        [AssertIsSet][field: SerializeField] public RectTransform TopLeftPivot { get; private set; }
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Image _backgroundColor;
        [SerializeField] private EncounterController _encounterController;
        [SerializeField] private EncounterFigureController _figureController;
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        [SerializeField] private ILivingEntityRenderer[] _entityRenderers;
        [AssertIsSet][SerializeField] private HeroActionButtons _heroActionButtons;
        [SerializeField] private bool _isCrawlingMode = false;

        public event System.Action<HeroFigurePanel> OnMoved;

        public bool IsActive => _encounterController != null && _encounterController.HeroTurnController.FigureController == _figureController;

        public EncounterFigureController FigureController
        {
            get => _figureController;
            set
            {
                _figureController = value;
                if (_figureData != null)
                {
                    _figureData.OnChanged -= HandleFigureChanged;
                    _figureData.EntityData.OnChanged -= HandleEntityChanged;
                }
                if (_figureController == null) { return; }
                _consumableDropZone.HeroEntityData = (HeroEntityData)_figureController.Figure.EntityData;
                _figureData = _figureController.Figure;
                _figureData.OnChanged += HandleFigureChanged;
                _figureData.EntityData.OnChanged += HandleEntityChanged;
                UpdateRenderers();
                _heroActionButtons.UpdateButtons(_figureData, IsActive);
                CheckForDeath();
            }
        }

        void OnDestroy()
        {
            if (_figureData != null)
            {
                _figureData.OnChanged -= HandleFigureChanged;
                _figureData.EntityData.OnChanged -= HandleEntityChanged;
            }
        }

        private void CheckForDeath()
        {
            if (_figureData.EntityData.Health <= 0)
            {
                _encounterController.SoundDatabase.Play("ko");
                _knockedOutPanel.IsEnabled = true;
                _figureData.HasTakenTurn = true;
                _encounterController?.EnsureCharacterIsDead(this);
            }
            else
            {
                _knockedOutPanel.IsEnabled = false;
            }
        }

        private void HandleEntityChanged(LivingEntityChangeEvent @event)
        {
            CheckForDeath();
            if (@event is TraitChangedEvent)
            {
                StartCoroutine(UpdateRenderersAtEndOfFrame());
            }
        }

        private IEnumerator UpdateRenderersAtEndOfFrame()
        {
            yield return null;
            UpdateRenderers();
        }

        private void HandleFigureChanged(FigureDataChangedEvent @event)
        {
            _heroActionButtons.UpdateButtons(_figureData, IsActive);
        }

        void Awake()
        {
            if (!_isCrawlingMode)
            {
                _encounterController = GetComponentInParent<EncounterController>();
                Debug.Assert(_encounterController != null, "Could not find Encounter Controller", this);
                // Do not allow changing equipment during combat.
                foreach (EquipmentSlotRenderer slot in GetComponentsInChildren<EquipmentSlotRenderer>())
                {
                    slot.CanDrag = false;
                }
                // CanDrag = FindAnyObjectByType<EncounterController>() == null;
            }
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
            FigureController = _figureController;
        }

        void Start()
        {
            Rebuild();
        }


        public void SelectFigure()
        {
            _encounterController.Select(_figureController);
        }

        private void UpdateRenderers()
        {
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
            foreach (IFigureRenderer renderer in _figureRenderers)
            {
                renderer.Render(_figureData);
            }
            foreach (ILivingEntityRenderer renderer in _entityRenderers)
            {
                renderer.Render(_figureData.EntityData);
            }
        }

        public void Exert()
        {
            if (_encounterController.AwaitingConfirmation) { return; }
            if (_figureData.EntityData is HeroEntityData hero && hero.Stamina > 0)
            {
                hero.Exertion++;
                _figureData.Movement++;
                _encounterController.HeroTurnController.ShowMove();
            }
        }

        public void TakeTurn()
        {
            if (_encounterController.AwaitingConfirmation) { return; }
            _encounterController.SoundDatabase.Play("accept");
            _encounterController.SelectTactics(this);
            _encounterController.Tutorial.ShowTutorialIfNeverSeen(TutorialData._01AttackAndMove);
        }

        public void Select()
        {
            _backgroundColor.color = _selectedColor;
        }
        public void Deselect()
        {
            _backgroundColor.color = _defaultColor;
        }

        public void Rebuild() => StartCoroutine(RebuildAtEndOfFrame());

        public IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            OnMoved?.Invoke(this);
        }

        public void Move()
        {
            if (_encounterController.AwaitingConfirmation) { return; }
            _encounterController.HeroTurnController.ShowMove();
        }

        public void EndTurn()
        {
            if (_encounterController.AwaitingConfirmation) { return; }
            if (_figureData.Movement > 0 || _figureData.Attacks > 0)
            {
                _encounterController.ShowConfirmation($"{_figureData.EntityData.Name} has action points remaining, are you sure you want to end this {_figureData.EntityData.Name}'s turn?", _encounterController.HeroTurnController.EndTurn);
            }
            else
            {
                _encounterController.HeroTurnController.EndTurn();
            }
        }

        public void SelectAttack() => _encounterController.HeroTurnController.StartAttack();

        /// <summary>
        /// Called when any figure is starting their turn
        /// </summary>
        /// <param name="figureController"></param>
        internal void StartTurn(EncounterFigureController figureController)
        {
            if (_figureController?.Figure == null) { return; }
            if (figureController == _figureController)
            {
                _heroActionButtons.EndTurnButton.Show();
                _heroActionButtons.EndTurnButton.Enabled = true;
                _heroActionButtons.TakeTurnButton.Hide();
            }
            else
            {
                _heroActionButtons.TakeTurnButton.Enabled = false;
                _heroActionButtons.EndTurnButton.Hide();
            }
        }

        /// <summary>
        /// Called when any figure is ending their turn
        /// </summary>
        internal void TurnEnded()
        {
            if (_figureController?.Figure == null) { return; }
            _heroActionButtons.TakeTurnButton.Show();
            _heroActionButtons.EndTurnButton.Hide();
            _heroActionButtons.TakeTurnButton.Enabled = !_figureController.Figure.HasTakenTurn;
        }
    }
}