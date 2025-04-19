using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class ContainerController : MonoBehaviour
    {
        [SerializeField] private ContainerData _containerData;
        [AssertIsSet][SerializeField] private EquipmentSlotRenderer[] _equipmentSlotRenderers;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        public event System.Action OnClose;
        public ContainerData ContainerData
        {
            get => _containerData;
            set
            {
                _containerData = value;
                RenderData();
            }
        }

        void Awake()
        {
            if (_containerData != null) { RenderData(); }
        }

        public void RenderData()
        {
            for (int ix = 0; ix < _equipmentSlotRenderers.Length; ix++)
            {
                EquipmentSlotRenderer renderer = _equipmentSlotRenderers[ix];
                if (ix < _containerData.EquipmentSlots.Length)
                {
                    renderer.Render(_containerData.EquipmentSlots[ix]);
                }
                else
                {
                    Debug.LogWarning("TODO: Hide unused equipment slot");
                }
            }
        }

        public void Open()
        {
            _toggleablePanel.IsEnabled = true;
            RenderData();
        }

        public void Toggle()
        {
            _toggleablePanel.Toggle();
        }

        public void Close()
        {
            _toggleablePanel.IsEnabled = false;
            OnClose?.Invoke();
        }
    }
}