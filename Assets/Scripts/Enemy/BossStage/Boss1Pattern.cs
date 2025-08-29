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
            Actions.Add(PatternRadial);    
            Actions.Add(PatternSpiral);    
            Actions.Add(PatternWave);    
        }
        
        void PatternRadial()
        {
            radialTimer += Time.deltaTime;
            if (radialTimer >= radialInterval)
            {
                float angleStep = 360f / radialBulletCount;
                float angle = 0f;

                for (int i = 0; i < radialBulletCount; i++)
                {
                    float x = Mathf.Cos(angle * Mathf.Deg2Rad);
                    float y = Mathf.Sin(angle * Mathf.Deg2Rad);
                    Vector3 dir = new Vector3(x, y, 0f);

                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
                    bullet.transform.parent = transform.GetChild(1);

                    angle += angleStep;
                }

                radialTimer = 0f;
            }
        }

        // --- 패턴 2: 나선 탄막 ---
        private float spiralTimer = 0f;
        private float spiralInterval = 0.05f;
        private float rotateSpeed = 10f;
        private float spiralAngle = 0f;

        void PatternSpiral()
        {
            spiralTimer += Time.deltaTime;
            if (spiralTimer >= spiralInterval)
            {
                float dirX = Mathf.Cos(spiralAngle * Mathf.Deg2Rad);
                float dirY = Mathf.Sin(spiralAngle * Mathf.Deg2Rad);

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dirX, dirY) * bulletSpeed;

                spiralAngle += rotateSpeed;
                spiralTimer = 0f;
            }
        }

        // --- 패턴 3: 파도형 탄막 ---
        private float waveTimer = 0f;
        private float waveInterval = 0.2f;
        private float waveFrequency = 5f;
        private float waveAmplitude = 2f;
        private float waveOffset = 0f;

        void PatternWave()
        {
            if (_target == null) return;

            waveTimer += Time.deltaTime;
            if (waveTimer >= waveInterval)
            {
                Vector2 dir = (_target.position - transform.position).normalized;
                Vector2 perp = new Vector2(-dir.y, dir.x);

                waveOffset += waveFrequency * Time.deltaTime;
                Vector2 finalDir = (dir + perp * Mathf.Sin(waveOffset) * waveAmplitude).normalized;

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = finalDir * bulletSpeed;

                waveTimer = 0f;
            }
        }
    }
}