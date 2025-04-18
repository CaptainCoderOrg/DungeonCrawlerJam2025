using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    // [CreateAssetMenu(menuName = "DC/Effect Data")]
    public class EncounterEffectActionData : ScriptableObject
    {

        public virtual void Apply(EncounterFigureController figure, EncounterController encounterController)
        {

        }
    }
}