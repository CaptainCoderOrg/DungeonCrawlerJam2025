using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Default Refresh Heroes Action")]
    public class RefreshHeroesActionData : EventActionData
    {
        [SerializeField] private PartyData _partyData;
        public override void Execute(CrawlerLogicController logicController)
        {
            _partyData.RefreshAllHeroes();
        }
    }
}