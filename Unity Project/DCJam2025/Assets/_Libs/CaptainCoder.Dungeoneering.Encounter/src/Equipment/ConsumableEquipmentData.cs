using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Equipment/Consumable Equipment Data")]
    public sealed class ConsumableEquipmentData : EquipmentData
    {
        public ConsumedEffect[] ConsumedEffects;

        public void OnConsumed(HeroEntityData hero)
        {
            Debug.LogWarning("TODO: Implement consumable");
            foreach (ConsumedEffect effect in ConsumedEffects)
            {
                effect.Apply(hero);
            }
        }
    }

    [System.Serializable]
    public struct ConsumedEffect
    {
        public int Health;
        public int BaseHealth;
        public int Stamina;
        public int BaseStamina;
        public int BaseSpeed;
        public int BaseArmor;
        public EffectData[] AddEffect;
        public DieData[] MeleeSkill;
        public DieData[] MagicSkill;
        public DieData[] RangedSkill;

        internal void Apply(HeroEntityData hero)
        {
            hero.Wounds -= Health;
            hero.Exertion -= Stamina;
            hero.LevelUp(BaseHealth, BaseSpeed, BaseStamina, BaseArmor);
            foreach (EffectData effect in AddEffect)
            {
                hero.AddEffect(effect);
            }
            foreach (var die in MeleeSkill)
            {
                hero.AddMeleeDie(die);
            }
            foreach (var die in MagicSkill)
            {
                hero.AddMagicDie(die);
            }
            foreach (var die in RangedSkill)
            {
                hero.AddRangedDie(die);
            }
        }
    }
}