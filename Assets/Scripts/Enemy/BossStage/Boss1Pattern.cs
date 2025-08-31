using System.Collections;
using Manager;
using UnityEngine;

namespace Enemy.BossStage
{
    public class Boss1Pattern : ABossPattern
    {
        public GameObject bulletPrefab;

        public float bulletSpeed;

        protected override void SetAction()
        {
            Actions.Add(() => StartCoroutine(PatternRadial()));
            Actions.Add(() => StartCoroutine(PatternSpiral()));
            Actions.Add(() => StartCoroutine(PatternFan()));
        }

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

                        // SpawnBullet 활용
                        SpawnBullet(dir, true);

                        angle += angleStep;
                    }

                    localTimer = 0f;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _controller.CanChangeState();
        }

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
                    Vector3 dir = new Vector3(dirX, dirY, 0f);

                    // SpawnBullet 활용
                    SpawnBullet(dir, true);

                    angle += rotateSpeed;
                    localTimer = 0f;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _controller.CanChangeState();
        }

        public IEnumerator PatternFan()
        {
            Debug.Log("fan");
            float timer = 0f;
            float elapsed = 0f;
            float duration = 4f;
            float interval = 2f;
            int bulletCount = 20;
            float angleRange = 120f;

            while (elapsed < duration)
            {
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;

                if (timer >= interval)
                {
                    timer = 0f;

                    Vector3 targetDir = (_target.position - transform.position).normalized;
                    float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                    float step = angleRange / (bulletCount - 1);

                    for (int i = 0; i < bulletCount; i++)
                    {
                        float angle = baseAngle - angleRange / 2f + step * i;
                        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                        SpawnBullet(dir);
                    }
                }

                yield return null;
            }

            _controller.CanChangeState();
        }

        public void SpawnBullet(Vector3 direction, bool isDirection = true)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 moveDir;

                if (isDirection)
                {
                    moveDir = direction.normalized;
                }
                else
                {
                    bullet.transform.position = direction;
                    moveDir = (_target.position - direction).normalized;
                }

                rb.linearVelocity = moveDir * bulletSpeed;

                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}