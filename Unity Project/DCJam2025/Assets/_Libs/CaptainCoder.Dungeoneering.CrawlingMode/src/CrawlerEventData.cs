
using System.Collections.Generic;
using System.Linq;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    public class CrawlerEventData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: Expandable][field: SerializeField] public List<EventConditionData> Conditions { get; private set; }
        [field: Expandable][field: SerializeField] public List<EventActionData> Actions { get; private set; }

        public virtual bool AllConditionsMet() => Conditions.All(c => c.ConditionMet());
    }
}