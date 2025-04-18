using CaptainCoder.Dungeoneering.Encounter;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/PartyData")]
    public class PartyData : ObservableSO
    {
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