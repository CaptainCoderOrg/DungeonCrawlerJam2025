
using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Game State Data")]
    public sealed class GameStateData : ScriptableObject
    {
        public PartyData PartyData;
        public OptionsSettings OptionsSettings;
        public DungeonEvents[] DungeonEvents;
    }
}