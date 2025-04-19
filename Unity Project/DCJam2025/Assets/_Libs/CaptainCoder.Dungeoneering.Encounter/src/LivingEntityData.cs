using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Living Entity Data")]
    public class LivingEntityData : ObservableSO
    {
        [field: SerializeField] public AttackData UnarmedAttack { get; private set; }
        public static bool RemoveEffectOnDamage(EffectData data) => data.RemoveIfDamaged;
        [field: SerializeField] public TraitDatabase TraitDatabase { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: ShowAssetPreview][field: SerializeField] public Sprite Portrait { get; private set; }
        [field: SerializeField] public int BaseHealth { get; protected set; }
        public int MaxHealth => BaseHealth + TraitEffects().Where(te => te.TraitType == TraitDatabase.HealthTrait).Sum(te => te.Value);
        [SerializeField] private int _wounds;
        public int Wounds
        {
            get => _wounds;
            set
            {
                int previous = _wounds;
                _wounds = Mathf.Clamp(value, 0, MaxHealth);
                if (_wounds == MaxHealth)
                {
                    Notify(EntityDeathEvent.Instance);
                }
                else if (previous < _wounds)
                {
                    Notify(new EntityDamagedEvent(_wounds - previous));
                    RemoveEffectsWhere(RemoveEffectOnDamage);
                }
                else if (previous > _wounds)
                {
                    Notify(new EntityHealedEvent(previous - _wounds));
                }
            }
        }
        public int Health => MaxHealth - Wounds;
        [field: SerializeField] public int BaseSpeed { get; protected set; }
        public int Speed => BaseSpeed + TraitEffects().Where(te => te.TraitType == TraitDatabase.SpeedTrait).Sum(te => te.Value);
        [field: SerializeField] public int BaseArmor { get; protected set; }
        public int Armor => BaseArmor + TraitEffects().Where(te => te.TraitType == TraitDatabase.ArmorTrait).Sum(te => te.Value);
        [field: SerializeField] public List<EffectData> Effects { get; private set; } = new();
        [field: Expandable][field: SerializeField] public AnimationData SpawnAnimation { get; private set; }
        [field: Expandable][field: SerializeField] public AnimationData AttackAnimation { get; private set; }
        [field: Expandable][field: SerializeField] public AnimationData IdleAnimation { get; private set; }
        [SerializeField] private string _shortName;
        public string ShortName => _shortName;

        public event System.Action<LivingEntityChangeEvent> OnChanged;
        protected void Notify(LivingEntityChangeEvent @event) => OnChanged?.Invoke(@event);
        public void ClearListeners()
        {
            Debug.LogWarning($"TODO: Consider not using clear listeners when switching scenes");
            OnChanged = null;
        }
        public virtual IEnumerable<TraitEffect> TraitEffects() => Effects.SelectMany(e => e.TraitEffects);

        protected static void CopyTo(LivingEntityData original, LivingEntityData copy)
        {
            copy.Name = original.Name;
            copy.Portrait = original.Portrait;
            copy.BaseHealth = original.BaseHealth;
            copy.Wounds = original.Wounds;
            copy.BaseSpeed = original.BaseSpeed;
            copy.BaseArmor = original.BaseArmor;
            copy.SpawnAnimation = original.SpawnAnimation;
            copy.AttackAnimation = original.AttackAnimation;
            copy.IdleAnimation = original.IdleAnimation;
        }

        void OnValidate()
        {
            if (UnarmedAttack == null)
            {
                Debug.LogWarning($"UnarmedAttack not set on {name}", this);
            }
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            OnChanged = null;
            _shortName = Name?.Split(null)[0];
        }

        internal virtual string TraitValueText(TraitTypeData traitTypeData)
        {
            if (traitTypeData == TraitDatabase.HealthTrait)
            {
                return $"{Health}/{MaxHealth}";
            }
            if (traitTypeData == TraitDatabase.ArmorTrait)
            {
                return Armor.ToString();
            }
            if (traitTypeData == TraitDatabase.SpeedTrait)
            {
                return Speed.ToString();
            }
            Debug.LogError($"Entity {this} does not have the trait {traitTypeData}", this);
            return null;
        }

        internal virtual string TraitDetails(TraitTypeData traitTypeData)
        {
            if (traitTypeData == TraitDatabase.HealthTrait)
            {
                return TraitDetails(BaseHealth, TraitDatabase.HealthTrait, TraitEffects());
            }
            if (traitTypeData == TraitDatabase.ArmorTrait)
            {
                return TraitDetails(BaseArmor, TraitDatabase.ArmorTrait, TraitEffects());
            }
            if (traitTypeData == TraitDatabase.SpeedTrait)
            {
                return TraitDetails(BaseSpeed, TraitDatabase.SpeedTrait, TraitEffects());
            }
            Debug.LogError($"Entity {this} does not have the trait {traitTypeData}", this);
            return null;
        }

        private static readonly StringBuilder StringBuilder = new();
        public static string TraitDetails(int baseValue, TraitTypeData traitType, IEnumerable<TraitEffect> effects)
        {
            StringBuilder.Clear();
            StringBuilder.Append($"{traitType.Name}: {baseValue}");
            var relaventEffects = effects.Where(te => te.TraitType == traitType);
            if (!relaventEffects.Any()) { return StringBuilder.ToString(); }
            int totalModifier = relaventEffects.Sum(te => te.Value);
            char sign = totalModifier < 0 ? '-' : '+';
            StringBuilder.Append($" {sign} {Mathf.Abs(totalModifier)}");
            foreach (var effect in relaventEffects)
            {
                StringBuilder.Append($"\n{effect.Source}: {effect.Value}");
            }
            return StringBuilder.ToString();
        }

        internal void AddEffect(EffectData effect)
        {
            Effects.Add(effect);
            OnChanged.Invoke(StatusChangedEvent.Instance);
        }

        internal void RemoveEffect(EffectData effect)
        {
            if (Effects.Remove(effect))
            {
                OnChanged.Invoke(StatusChangedEvent.Instance);
            }
        }

        internal void RemoveEffectsWhere(Predicate<EffectData> removeAfterAttacking)
        {
            if (Effects.RemoveAll(removeAfterAttacking) > 0)
            {
                OnChanged.Invoke(StatusChangedEvent.Instance);
            }
        }

        internal virtual IEnumerable<AttackAbilityData> GetAttackAbilities()
        {
            return Enumerable.Empty<AttackAbilityData>();
        }

        internal virtual IEnumerable<DefenderAbilityData> GetDefenderAbilities()
        {
            return Enumerable.Empty<DefenderAbilityData>();
        }

        internal static bool IsGuardEffect(EffectData effect) => effect.IsGuard;
    }

    public abstract record class LivingEntityChangeEvent;
    public sealed record class EquipmentChangedEvent : LivingEntityChangeEvent
    {
        public static readonly EquipmentChangedEvent Instance = new();
    }

    public sealed record class StatusChangedEvent : LivingEntityChangeEvent
    {
        public static readonly StatusChangedEvent Instance = new();
    }

    public sealed record class TraitChangedEvent : LivingEntityChangeEvent
    {
        public static readonly TraitChangedEvent Instance = new();
    }

    public sealed record class EntityDamagedEvent(int Amount) : LivingEntityChangeEvent;
    public sealed record class EntityHealedEvent(int Amount) : LivingEntityChangeEvent;

    public sealed record class EntityDeathEvent : LivingEntityChangeEvent
    {
        public static readonly EntityDeathEvent Instance = new();
    }
}