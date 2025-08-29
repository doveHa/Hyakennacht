using System.Collections;
using UnityEngine;

namespace Enemy.BossStage
{
    public class MiddleBossPattern : ABossPattern
    {
        private EnemyController _controller;

        protected override void Start()
        {
            base.Start();
            _controller = GetComponent<EnemyController>();
        }

        protected override void SetAction()
        {
            _actions.Add(PatternLightning);
        }

        // --- 패턴 4: 번개 낙하 (경고 → 번개 → 연쇄 십자가) ---
        private float lightningInterval = 2.5f;
        private float lightningRange = 3f;
        public GameObject warningPrefab; // 경고 표시 프리팹
        public GameObject lightningPrefab; // 번개 프리팹
        private bool isLightningActive = false;
        public float fallingSpeed = 1f;
        void PatternLightning()
        {
            if (!isLightningActive)
            {
                StartCoroutine(SpawnLightningSequence());
            }
        }

        IEnumerator SpawnLightningSequence()
        {
            isLightningActive = true;
         
            // 타일맵 그리드 스냅
            Vector3Int cellPos = _controller.stage.WorldToCell(_target.position);
            Vector3 spawnPos = _controller.stage.GetCellCenterWorld(cellPos);

            // 1단계: 경고 표시
            GameObject warn = Instantiate(warningPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(fallingSpeed); // 1초 뒤 낙하

            Destroy(warn);
            Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            // 플레이어 충돌 체크
            Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Debug.Log("플레이어가 번개에 맞음!");
                }
            }

            // 2단계: 십자가 방향 연쇄 낙뢰
            Vector3[] directions = new Vector3[]
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

            foreach (var dir in directions)
            {
                Vector3Int crossCell = _controller.stage.WorldToCell(spawnPos + dir * 3f);
                Vector3 crossPos = _controller.stage.GetCellCenterWorld(crossCell);

                // 경고 표시
                GameObject crossWarn = Instantiate(warningPrefab, crossPos, Quaternion.identity);

                StartCoroutine(DelayedLightning(crossWarn, crossPos, fallingSpeed));
            }

            yield return new WaitForSeconds(lightningInterval);
            isLightningActive = false;
        }

        IEnumerator DelayedLightning(GameObject warning, Vector3 pos, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (warning != null) Destroy(warning);

            Instantiate(lightningPrefab, pos, Quaternion.identity);

            // 충돌 판정
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Debug.Log("플레이어가 연쇄 번개에 맞음!");
                }
            }
        }
    }
}