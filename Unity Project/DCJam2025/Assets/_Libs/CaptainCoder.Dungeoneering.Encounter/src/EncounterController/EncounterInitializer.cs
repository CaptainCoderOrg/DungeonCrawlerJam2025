using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterInitializer : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [AssertIsSet][SerializeField] private EncounterFigureController _enemyFigurePrefab;
        [AssertIsSet][SerializeField] private Transform _enemyFigureParent;
        [AssertIsSet][field: SerializeField] private EnemyFigurePanel _enemyFigurePanel;

        public void Init(EncounterData encounterData)
        {
            State.Figures.Clear();
            State.Heroes.Clear();
            foreach (EnemyFigure f in encounterData.EnemyFigures)
            {
                if (State.Figures.ContainsKey(f.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {f.Position}");
                }
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.Figure = FigureData.CopyEnemyAndCreate(f.EnemyEntityTemplate, f.Position);
                controller.Figure.EntityData.OnChanged += @event => HandleEntityChanged(controller, @event);
                controller.name = $"{f.EnemyEntityTemplate.Name}'s Figure";

                State.Figures[f.Position] = controller;
                controller.OnSelected.AddListener(() => _enemyFigurePanel.Render(controller.Figure));
                controller.OnDeselected.AddListener(_enemyFigurePanel.Hide);
                controller.OnClick.AddListener(_controller.Select);
            }

            int ix = 0;
            for (; ix < encounterData.HeroFigures.Count; ix++)
            {
                HeroFigure h = encounterData.HeroFigures[ix];
                if (State.Figures.ContainsKey(h.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {h.Position}");
                }
                State.Heroes.Add(h.HeroEntity);
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.name = $"{h.HeroEntity.Name}'s Figure";
                controller.Figure = FigureData.Create(h.HeroEntity, h.Position);

                // TODO: This is a hack that ensure there are no left over listeners from the previous scene
                h.HeroEntity.ClearListeners();

                State.Figures[h.Position] = controller;
                HeroFigurePanel panel = Controller.HeroPanels[ix];
                panel.FigureController = controller;
                controller.OnClick.AddListener(_controller.Select);
                controller.OnSelected.AddListener(panel.Select);
                controller.OnDeselected.AddListener(panel.Deselect);
            }
            for (; ix < Controller.HeroPanels.Length; ix++)
            {
                Controller.HeroPanels[ix].gameObject.SetActive(false);
            }
        }

        private void HandleEntityChanged(EncounterFigureController controller, LivingEntityChangeEvent @event)
        {
            if (@event is EntityDeathEvent)
            {
                State.Figures.Remove(controller.Figure.Position);
                Destroy(controller.gameObject);
                Controller.CheckForEndOfCombat();
            }
        }
    }
}