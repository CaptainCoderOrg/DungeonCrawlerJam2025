
using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class BuyEquipmentButton : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private PartyData _partyData;
        [AssertIsSet][SerializeField] private LootTableData _lootTableData;
        [AssertIsSet][SerializeField] private CrawlerLogicController _crawlerLogicController;
        [SerializeField] private int _cost = 200;

        void Awake()
        {
            _crawlerLogicController = FindFirstObjectByType<CrawlerLogicController>();
            Debug.Assert(_crawlerLogicController != null, $"Could not find CrawlerLogicController", this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_partyData.Gold >= _cost)
            {
                _partyData.Gold -= _cost;
                _crawlerLogicController.GainItem(_lootTableData.GetRandomItem());
            }
        }
    }
}