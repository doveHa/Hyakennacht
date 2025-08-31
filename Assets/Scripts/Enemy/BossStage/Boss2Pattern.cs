using System.Collections;
using Manager;
using UnityEngine;

namespace Enemy.BossStage
{
    public class Boss2Pattern : ABossPattern
    {
        public GameObject bulletPrefab;

        public float bulletSpeed;

        protected override void SetAction()
        {
            Actions.Add(() => StartCoroutine(PatternAcceleratingSpiral()));
            Actions.Add(() => StartCoroutine(PatternHorizontalWall()));
            Actions.Add(() => StartCoroutine(PatternSniper()));
        }

        public IEnumerator PatternSniper()
        {
            float timer = 0f;
            float elapsed = 0f;
            float duration = 4f;
            float interval = 0.2f;

            while (elapsed < duration)
            {
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;

                if (timer >= interval)
                {
                    timer = 0f;
                    Vector3 dir = (_target.position - transform.position).normalized;
                    SpawnBullet(dir);
                }

                yield return null;
            }

            _controller.CanChangeState();
        }


        public IEnumerator PatternHorizontalWall()
        {
            float timer = 0f;
            float elapsed = 0f;
            float duration = 4f;
            float interval = 2f;
            int bulletCount = 10;
            float spacing = 1f;

            while (elapsed < duration)
            {
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;

                if (timer >= interval)
                {
                    timer = 0f;
                    for (int i = 0; i < bulletCount; i++)
                    {
                        Vector3 pos = transform.position +
                                      new Vector3(i * spacing - (bulletCount / 2f) * spacing, 0, 0);
                        SpawnBullet(pos, false);
                    }
                }

                yield return null;
            }

            _controller.CanChangeState();
        }

        public IEnumerator PatternAcceleratingSpiral()
        {
            float timer = 0f;
            float elapsed = 0f;
            float angle = 0f;
            float duration = 4f;
            float interval = 2f;
            int bulletCount = 20;

            while (elapsed < duration)
            {
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;

                if (timer >= interval)
                {
                    timer = 0f;

                    for (int i = 0; i < bulletCount; i++)
                    {
                        float currentAngle = angle + (360f / bulletCount) * i;
                        Vector3 dir = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                            Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                        SpawnBullet(dir);
                    }

                    angle += 15f;
                    interval = Mathf.Max(0.05f, interval - 0.01f); // 점점 빨라짐
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