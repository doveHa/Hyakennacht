using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.BossStage
{
    public class MiddleBossPattern : ABossPattern
    {
        /*
            LightningIndex = 1
            DashIndex = 2
         */

        private bool _isMotionEnd;

        protected override void Start()
        {
            base.Start();
        }

        protected override void SetAction()
        {
            Actions.Add(PatternLightning);
            Actions.Add(PatternDash);
        }

        public void MotionEnd()
        {
            _isMotionEnd = true;
        }

        // --- 패턴 4: 번개 낙하 (경고 → 번개 → 연쇄 십자가) ---
        private float lightningInterval = 2.5f;
        private float lightningRange = 3f;
        public GameObject warningPrefab; // 경고 표시 프리팹
        public GameObject lightningPrefab; // 번개 프리팹
        public float fallingSpeed = 1f;

        void PatternLightning()
        {
            StartCoroutine(SpawnLightningSequence());
        }

        IEnumerator SpawnLightningSequence()
        {
            yield return new WaitUntil(() => _isMotionEnd);

            _isMotionEnd = false;
            // 타일맵 그리드 스냅
            Vector3Int cellPos = _controller.Stage.WorldToCell(_target.position);
            Vector3 spawnPos = _controller.Stage.GetCellCenterWorld(cellPos);

            // 1단계: 경고 표시
            GameObject warn = Instantiate(warningPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(fallingSpeed); // 1초 뒤 낙하

            Destroy(warn);
            Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

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
                Vector3Int crossCell = _controller.Stage.WorldToCell(spawnPos + dir * 3f);
                Vector3 crossPos = _controller.Stage.GetCellCenterWorld(crossCell);

                // 경고 표시
                GameObject crossWarn = Instantiate(warningPrefab, crossPos, Quaternion.identity);

                StartCoroutine(DelayedLightning(crossWarn, crossPos, fallingSpeed));
            }
            _controller.IsChangeState = true;
        }

        IEnumerator DelayedLightning(GameObject warning, Vector3 pos, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (warning != null) Destroy(warning);

            Instantiate(lightningPrefab, pos, Quaternion.identity);
        }

        // --- 패턴 5: 기 모으기 후 돌진 ---
        public float chargeTime = 1.0f; // 기 모으는 시간
        public float dashSpeed = 12f; // 돌진 속도
        private float dashDuration = 0.4f; // 돌진 지속 시간

        void PatternDash()
        {
            StartCoroutine(DashSequence());
        }

        IEnumerator DashSequence()
        {
            yield return new WaitUntil(() => _isMotionEnd);
            _isMotionEnd = false;
            
            if (_target != null)
            {
                Vector3 dir = (_target.position - transform.position).normalized;
                float timer = 0f;

                // 맵 경계 가져오기
                Bounds stageBounds = _controller.Stage.localBounds;

                while (timer < dashDuration)
                {
                    // 이동 예정 좌표
                    Vector3 nextPos = transform.position + dir * dashSpeed * Time.deltaTime;

                    // Clamp 적용
                    float clampedX = Mathf.Clamp(nextPos.x, stageBounds.min.x, stageBounds.max.x);
                    float clampedY = Mathf.Clamp(nextPos.y, stageBounds.min.y, stageBounds.max.y);
                    Vector3 clampedPos = new Vector3(clampedX, clampedY, nextPos.z);

                    // 만약 Clamp 결과가 달라졌다면 (= 벽에 닿음)
                    if (clampedPos.x != nextPos.x || clampedPos.y != nextPos.y)
                    {
                        transform.position = clampedPos;
                        Debug.Log("보스가 벽에 부딪혀 돌진 종료!");
                        break; // 돌진 즉시 종료
                    }

                    transform.position = clampedPos;

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            _controller.IsChangeState = true;
            _controller.Animator.SetBool("EndDash",true);
        }
    }
}