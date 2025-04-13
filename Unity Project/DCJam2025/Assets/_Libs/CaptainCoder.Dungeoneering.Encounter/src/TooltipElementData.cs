using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Tooltip Element Data")]
    public class TooltipElementData : ObservableSO
    {
        private TooltipController _tooltip;
        public TooltipController Tooltip => _tooltip == null ? (_tooltip = FindFirstObjectByType<TooltipController>()) : _tooltip;
        private DiceTooltipController _diceTooltipController;
        public DiceTooltipController DiceTooltipController => _diceTooltipController == null ? (_diceTooltipController = FindFirstObjectByType<DiceTooltipController>()) : _diceTooltipController;

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _tooltip = null;
            _diceTooltipController = null;
        }
    }
}