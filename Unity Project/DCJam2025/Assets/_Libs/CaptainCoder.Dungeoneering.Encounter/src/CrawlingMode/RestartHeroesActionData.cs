using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Default Restart Heroes Action")]
    public class RestartHeroesActionData : EventActionData
    {
        [SerializeField] private PartyData _partyData;
        public override void Execute(CrawlerLogicController logicController)
        {
            foreach (HeroEntityData hero in _partyData.Heroes)
            {
                _partyData.Gold = 0;
                hero.ResetToNewCharacter();
            }
        }
    }
}