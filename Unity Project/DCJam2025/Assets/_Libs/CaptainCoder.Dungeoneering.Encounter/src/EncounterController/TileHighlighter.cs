using System;
using System.Collections.Generic;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class TileHighlighter : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);

        public void ClearAll()
        {
            foreach (EncounterTileSelector selector in Controller.TileSelectors.Values)
            {
                selector.ClearEvents();
                selector.Hide();
            }
        }

        public void Clear(IEnumerable<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                var selector = Controller.TileSelectors[position];
                selector.ClearEvents();
                selector.Hide();
            }
        }

        public void Highlight(IEnumerable<Vector2Int> positions)
        {
            foreach (Vector2Int position in positions)
            {
                if (Controller.TileSelectors.TryGetValue(position, out var selector))
                {
                    selector.Highlight();
                }
            }
        }

        internal void Selected(IEnumerable<Vector2Int> positions)
        {
            foreach (Vector2Int position in positions)
            {
                if (Controller.TileSelectors.TryGetValue(position, out var selector))
                {
                    selector.Selected();
                }
            }
        }

    }
}