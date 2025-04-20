
using System;

using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class BuyEquipmentButton : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private SoundDatabase _soundDatabase;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private PartyData _partyData;
        [AssertIsSet][SerializeField] private LootTableData _lootTableData;
        [AssertIsSet][SerializeField] private CrawlerLogicController _crawlerLogicController;
        [SerializeField] private int _cost = 200;
        private bool _inCombat;

        void Awake()
        {
            _crawlerLogicController = FindFirstObjectByType<CrawlerLogicController>();
            Debug.Assert(_crawlerLogicController != null, $"Could not find CrawlerLogicController", this);
            _inCombat = FindAnyObjectByType<EncounterController>();
            if (_inCombat)
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0.5f;
            }
            HandleGoldChanged(_partyData.Gold);
        }

        void OnEnable()
        {
            _partyData.OnGoldChanged += HandleGoldChanged;
        }

        void OnDisable()
        {
            _partyData.OnGoldChanged -= HandleGoldChanged;
        }

        private void HandleGoldChanged(int amount)
        {
            if (_inCombat) { return; }
            if (amount >= _cost)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 1f;
            }
            else
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0.5f;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_partyData.Gold >= _cost)
            {
                _partyData.Gold -= _cost;
                _crawlerLogicController.GainItem(_lootTableData.GetRandomItem());
                _soundDatabase.Play("buy");
            }
        }
    }
}