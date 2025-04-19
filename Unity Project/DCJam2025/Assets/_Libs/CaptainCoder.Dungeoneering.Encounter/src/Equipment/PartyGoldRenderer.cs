using System;

using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class PartyGoldRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private PartyData _partyData;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _goldLabel;

        public void OnEnable()
        {
            _partyData.OnGoldChanged += HandleGoldChanged;
            HandleGoldChanged(_partyData.Gold);
        }

        public void OnDisable()
        {
            _partyData.OnGoldChanged -= HandleGoldChanged;
        }

        private void HandleGoldChanged(int value)
        {
            _goldLabel.text = $"<sprite name=\"coin\"> {_partyData.Gold}";
        }
    }
}