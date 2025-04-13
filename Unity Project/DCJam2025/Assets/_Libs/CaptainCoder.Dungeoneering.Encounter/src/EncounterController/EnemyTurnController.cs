

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EnemyTurnController : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        internal void TakeEnemyTurn()
        {
            StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            foreach ((Vector2Int position, EncounterFigureController figure) in State.Figures)
            {
                if (figure.Figure.EntityData is EnemyEntityData enemyData)
                {
                    Debug.Log($"Taking {enemyData.Name}'s turn");
                    Controller.Select(figure);
                    figure.Figure.Attacks = 1;
                    figure.Figure.Movement = figure.Figure.EntityData.Speed;
                    yield return new WaitForSeconds(2f);
                    HashSet<MoveInfo> possibleMoves = figure.Figure.FindMoves(State, Controller.EncounterData);
                    Debug.Log(possibleMoves.Count);
                    Controller.TileHighlighter.Highlight(possibleMoves.Select(p => p.Position));
                    yield return new WaitForSeconds(2f);
                    Controller.TileHighlighter.Clear();
                }
            }
        }
    }
}