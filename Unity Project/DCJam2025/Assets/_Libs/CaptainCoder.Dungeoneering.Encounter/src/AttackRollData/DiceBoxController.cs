using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DiceBoxController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] GameObject _diceCameraPivot;
        [AssertIsSet][SerializeField] Camera _diceCamera;
        [SerializeField] private DieController[] _dice;
        private int _count = 0;
        private readonly List<DieResult> _results = new();

        public event System.Action<IEnumerable<DieResult>> OnResult;
        public event System.Action<DieController> OnClicked;

        private void CenterCamera()
        {
            float minX = float.PositiveInfinity;
            float minZ = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxZ = float.NegativeInfinity;

            foreach (DieController dieController in _dice.Where(d => d.gameObject.activeInHierarchy))
            {
                maxX = Mathf.Max(maxX, dieController.transform.position.x);
                maxZ = Mathf.Max(maxZ, dieController.transform.position.z);
                minX = Mathf.Min(minX, dieController.transform.position.x);
                minZ = Mathf.Min(minZ, dieController.transform.position.z);
            }

            _diceCameraPivot.transform.position = new Vector3(
            ((maxX - minX) * 0.5f) + minX,
            _diceCameraPivot.transform.position.y,
            ((maxZ - minZ) * 0.5f) + minZ
        );
        }

        public void SetDice(IEnumerable<DieData> dice)
        {

            int ix = 0;
            _count = 0;
            _results.Clear();
            foreach (DieData die in dice)
            {
                _count++;
                DieController dieController = _dice[ix++];
                dieController.Die = die;
                dieController.gameObject.SetActive(true);
                dieController.OnResult -= HandleDieResult;
                dieController.OnResult += HandleDieResult;

            }

            for (; ix < _dice.Length; ix++)
            {
                DieController dieController = _dice[ix];
                dieController.gameObject.SetActive(false);
                dieController.OnResult -= HandleDieResult;
            }

            CenterCamera();
        }

        public bool AddDie(DieData die)
        {
            if (_count >= _dice.Length) { return false; }
            DieController next = _dice[_count];
            next.Die = die;
            next.gameObject.SetActive(true);
            next.OnResult -= HandleDieResult;
            next.OnResult += HandleSingleDieResult;
            next.Roll();
            _count++;
            CenterCamera();
            return true;
        }

        private void HandleSingleDieResult(DieResult result)
        {
            _results.Add(result);
            OnResult?.Invoke(_results);
        }

        private void HandleReRollResults(DieResult result)
        {
            IEnumerable<DieController> rolled = _dice.Where(d => d.gameObject.activeInHierarchy);
            if (rolled.Any(d => d.IsRolling)) { return; }
            _results.Clear();
            _results.AddRange(rolled.Select(d => d.Result));
            OnResult?.Invoke(_results);
        }

        private void HandleDieResult(DieResult result)
        {
            _results.Add(result);
            if (_results.Count == _count)
            {
                OnResult?.Invoke(_results);
            }
            StartCoroutine(RotateResults());
        }

        [SerializeField] private Vector3 _startingRotation = new(60, 15, 0);
        private Quaternion _endRotation = Quaternion.Euler(90, 0, 0);
        [SerializeField] private float _animationDuration = 1f;
        private bool _awaitingResults;

        private IEnumerator RotateResults()
        {
            Quaternion startRotation = Quaternion.Euler(_startingRotation);
            float elapsedTime = 0;
            while (elapsedTime < _animationDuration)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
                _diceCamera.transform.rotation = Quaternion.Lerp(startRotation, _endRotation, elapsedTime / _animationDuration);
            }
            _diceCamera.transform.rotation = _endRotation;
            _awaitingResults = false;
        }

        void Awake()
        {
            _dice = GetComponentsInChildren<DieController>(true);
            foreach (DieController die in _dice)
            {
                die.OnClick += HandleDieClicked;
            }
        }

        private void HandleDieClicked(DieController controller)
        {
            OnClicked?.Invoke(controller);
        }

        internal void ReRoll(DieController controller)
        {
            controller.gameObject.SetActive(true);
            controller.OnResult -= HandleDieResult;
            controller.OnResult += HandleReRollResults;
            controller.Roll();
            CenterCamera();
        }

        [Button]
        public IEnumerator Roll()
        {
            _diceCamera.transform.rotation = Quaternion.Euler(_startingRotation);
            foreach (var die in _dice.Where(d => d.isActiveAndEnabled))
            {
                die.Roll();
            }
            _awaitingResults = true;
            yield return StartCoroutine(WaitForRoll());
        }

        private IEnumerator WaitForRoll()
        {
            while (_awaitingResults)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}