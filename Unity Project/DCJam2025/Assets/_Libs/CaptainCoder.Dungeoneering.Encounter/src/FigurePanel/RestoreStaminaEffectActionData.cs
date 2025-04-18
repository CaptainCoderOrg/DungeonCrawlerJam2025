using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Effects/Restore Stamina")]
    public class RestoreStaminaEffectActionData : EncounterEffectActionData
    {

        public override void Apply(EncounterFigureController figure, EncounterController _)
        {
            if (figure.Figure.EntityData is HeroEntityData hero)
            {
                hero.Exertion = 0;
            }
        }
    }
}