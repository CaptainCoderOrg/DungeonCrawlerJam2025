using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Equipment/Container Data")]
    public sealed class ContainerData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public EquipmentData[] EquipmentData { get; private set; } = new EquipmentData[16];
        public ContainerSlotReference[] EquipmentSlots { get; private set; }

        [Button]
        public void Clear()
        {
            for (int ix = 0; ix < EquipmentData.Length; ix++)
            {
                EquipmentData[ix] = null;
            }
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            EquipmentSlots = new ContainerSlotReference[EquipmentData.Length];
            for (int ix = 0; ix < EquipmentData.Length; ix++)
            {
                EquipmentSlots[ix] = new ContainerSlotReference(ix, this);
                if (ix < EquipmentData.Length)
                {
                    EquipmentSlots[ix].Data = EquipmentData[ix];
                }
            }
        }
    }
}