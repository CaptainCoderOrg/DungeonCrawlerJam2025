

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/On New Game")]
    public class GameStartEventData : CrawlerEventData
    {
        public bool SkipInPlayMode = false;
        public bool HasTriggered = false;

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            HasTriggered = false;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            HasTriggered = false;
        }
    }
}