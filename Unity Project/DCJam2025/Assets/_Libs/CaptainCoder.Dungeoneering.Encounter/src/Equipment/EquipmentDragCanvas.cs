using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentDragCanvas : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public Canvas DragCanvas { get; private set; }
    }
}