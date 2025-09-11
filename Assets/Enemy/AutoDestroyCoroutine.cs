using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyCoroutine : MonoBehaviour
{
    public float lifeTime = 2f;

    private IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("Destroy");
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitDestroy());
    }
}
