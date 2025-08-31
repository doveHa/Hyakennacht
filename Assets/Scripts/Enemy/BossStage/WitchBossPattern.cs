using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.BossStage
{
    public class WitchBossPattern : ABossPattern
    {
        /*
            CircularTargeting = 1
            Scratch = 2
         */

        public GameObject ClockBullet;
        private bool _isMotionEnd;

        protected override void Start()
        {
            base.Start();
        }

        protected override void SetAction()
        {
            Actions.Add(() => StartCoroutine(PatternCircularTargeting()));
            Actions.Add(() => StartCoroutine(PatternScratch()));
        }

        public void MotionEnd()
        {
            _isMotionEnd = true;
        }

        public float interval = 1f;
        public float bulletSpeed = 2f;

        private IEnumerator PatternCircularTargeting()
        {
            float duration = 4f; // 패턴의 지속 시간
            float timer = 0f;

            int bulletCount = 20; // 발사할 총알의 개수
            float radius = 5f; // 원의 반지름
            float angleStep = 360f / bulletCount; // 각 오브젝트가 차지할 각도
            float angle = 0f; // 시작 각도

            List<GameObject> bullets = new List<GameObject>(); // 생성된 총알들을 저장할 리스트
            List<Vector3> directions = new List<Vector3>(); // 각 총알이 향해야 할 방향

            Vector3 targetPosition = _target.position;
            // 원형 탄막을 만들어서 순차적으로 생성하는 패턴
            for (int i = 0; i < bulletCount; i++)
            {
                // 회전하는 각도를 계산
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                Vector3 bulletPosition = targetPosition + new Vector3(x, y, 0f); // 원 위치에 총알 배치

                // 플레이어를 향한 방향 계산
                Vector3 direction = (targetPosition - bulletPosition).normalized;

                // 총알 생성 후 리스트에 추가
                GameObject bullet = Instantiate(ClockBullet, bulletPosition, Quaternion.identity);

                float bulletRotateAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(bulletRotateAngle, Vector3.forward);
                bullets.Add(bullet);
                directions.Add(direction);

                angle += angleStep; // 시계방향으로 각도 증가

                // 간격만큼 기다린 후 생성
                yield return new WaitForSeconds(interval);
            }

            // 모든 총알 생성 완료 후 일제히 발사
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] != null)
                {
                    // 각 총알의 속도 적용 및 회전 적용
                    Rigidbody2D rb = bullets[i].GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        // 총알을 플레이어 방향으로 발사
                        rb.linearVelocity = directions[i] * bulletSpeed;

                        // 회전 (총알이 발사되는 방향에 맞춰서 회전)
                        float BulletAngle = Mathf.Atan2(directions[i].y, directions[i].x) * Mathf.Rad2Deg;
                        bullets[i].transform.rotation = Quaternion.AngleAxis(BulletAngle, Vector3.forward);
                    }
                }
            }

            _controller.CanChangeState();
        }

        public GameObject scratchPrefab; // 스크레치 프리팹
        public float speed = 5f;
        public Transform[] scratchPoints;

        private IEnumerator PatternScratch()
        {
            yield return new WaitUntil(() => _isMotionEnd);
            _isMotionEnd = false;

            Vector3 direction = (_target.position - scratchPoints[1].position).normalized;
            foreach (Transform point in scratchPoints)
            {
                GameObject scratch = Instantiate(scratchPrefab, point.position, Quaternion.identity);
                scratch.GetComponent<SpriteRenderer>().flipX = _controller.IsLeftSight;
                scratch.GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
            }

            _controller.CanChangeState();
        }
    }
}