using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{

    [CreateAssetMenu(menuName = "DC/Defender Ability Data")]
    public class DefenderAbilityData : ObservableSO
    {
        [field: ShowAssetPreview][field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: TextArea(3, 5)][field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public bool AllowsReroll { get; private set; } = false;
        [field: SerializeField] public bool IsPassive { get; private set; } = false;

    }

}