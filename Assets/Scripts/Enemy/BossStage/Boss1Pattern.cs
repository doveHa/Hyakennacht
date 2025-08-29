using System.Collections;
using UnityEngine;

namespace Enemy.BossStage
{
    public class Boss1Pattern : ABossPattern
    {
        public GameObject bulletPrefab;

        public float bulletSpeed;

        // --- 패턴 1: 원형 확산탄 ---
        private float radialTimer = 0f;
        private float radialInterval = 2f;
        private int radialBulletCount = 20;

        protected override void SetAction()
        {
            Actions.Add(() => StartCoroutine(PatternRadial()));
            Actions.Add(() => StartCoroutine(PatternSpiral()));
            Actions.Add(() => StartCoroutine((PatternWave())));
        }

        // --- 패턴 1: 원형 확산탄 ---
        private IEnumerator PatternRadial()
        {
            float duration = 3f;
            float timer = 0f;

            float interval = 2f;
            float localTimer = 0f;
            int bulletCount = 20;

            while (timer < duration)
            {
                localTimer += Time.deltaTime;
                if (localTimer >= interval)
                {
                    float angleStep = 360f / bulletCount;
                    float angle = 0f;

                    for (int i = 0; i < bulletCount; i++)
                    {
                        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
                        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
                        Vector3 dir = new Vector3(x, y, 0f);

                        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
                        bullet.transform.parent = transform.GetChild(1);

                        angle += angleStep;
                    }

                    localTimer = 0f;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _controller.CanChangeState(); // 패턴 끝나면 상태 전환 허용
        }

        // --- 패턴 2: 나선 탄막 ---
        private IEnumerator PatternSpiral()
        {
            float duration = 3f;
            float timer = 0f;

            float interval = 0.05f;
            float localTimer = 0f;
            float angle = 0f;
            float rotateSpeed = 10f;

            while (timer < duration)
            {
                localTimer += Time.deltaTime;
                if (localTimer >= interval)
                {
                    float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                    float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);

                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dirX, dirY) * bulletSpeed;

                    angle += rotateSpeed;
                    localTimer = 0f;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _controller.CanChangeState();
        }

        // --- 패턴 3: 파도형 탄막 ---
        private IEnumerator PatternWave()
        {
            float duration = 3f;
            float timer = 0f;

            float interval = 0.2f;
            float localTimer = 0f;
            float waveFrequency = 5f;
            float waveAmplitude = 2f;
            float waveOffset = 0f;

            while (timer < duration)
            {
                localTimer += Time.deltaTime;
                if (localTimer >= interval)
                {
                    Vector2 dir = (_target.position - transform.position).normalized;
                    Vector2 perp = new Vector2(-dir.y, dir.x);

                    waveOffset += waveFrequency * Time.deltaTime;
                    Vector2 finalDir = (dir + perp * Mathf.Sin(waveOffset) * waveAmplitude).normalized;

                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = finalDir * bulletSpeed;

                    localTimer = 0f;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _controller.CanChangeState();
        }
    }
}
