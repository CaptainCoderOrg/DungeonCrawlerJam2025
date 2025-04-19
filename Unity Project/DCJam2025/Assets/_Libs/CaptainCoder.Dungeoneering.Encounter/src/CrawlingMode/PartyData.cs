using CaptainCoder.Dungeoneering.Encounter;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/PartyData")]
    public class PartyData : ObservableSO
    {
        [SerializeField] private int _gold;
        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                OnGoldChanged?.Invoke(_gold);
            }
        }
        public event System.Action<int> OnGoldChanged;
        [field: Expandable][field: SerializeField] public HeroEntityData[] Heroes { get; private set; }
        [field: Expandable][field: SerializeField] public ContainerData PartyInventory { get; private set; }
        [field: Expandable][field: SerializeField] public PlayerViewData PlayerView { get; private set; }

        [Button]
        public void RefreshAllHeroes()
        {
            foreach (HeroEntityData hero in Heroes)
            {
                hero.Wounds = 0;
                hero.Exertion = 0;
            }
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            OnGoldChanged = null;
        }

        void OnValidate()
        {
            OnGoldChanged?.Invoke(_gold);
        }

        [Button]
        public void ReviveAllHeroes()
        {
            foreach (HeroEntityData hero in Heroes)
            {
                hero.Wounds = Mathf.Min(hero.Wounds, hero.MaxHealth - 1);
            }
        }
    }
}