using System.Collections.Generic;
using UnityEngine;

public static class Phys2DCompat
{
    // mask.value == 0 �̸� "��� ���̾� ���"���� ó��
    static ContactFilter2D BuildFilter(LayerMask mask, bool includeTriggers)
    {
        var f = new ContactFilter2D();
        f.useTriggers = includeTriggers;

        if (mask.value != 0)
        {
            f.useLayerMask = true;
            f.SetLayerMask(mask);
        }
        else
        {
            // ���̾� ���� ����
            f.useLayerMask = false;
            f.layerMask = Physics2D.DefaultRaycastLayers; // ������
        }

        f.useDepth = false;
        f.useOutsideDepth = false;
        return f;
    }

    public static int OverlapCircle(Vector2 center, float radius, Collider2D[] results, LayerMask mask, bool includeTriggers = true)
    {
        var filter = BuildFilter(mask, includeTriggers);
        return Physics2D.OverlapCircle(center, radius, filter, results);
    }

    public static int OverlapBox(Vector2 point, Vector2 size, float angleDeg, Collider2D[] results, LayerMask mask, bool includeTriggers = true)
    {
        var filter = BuildFilter(mask, includeTriggers);
        return Physics2D.OverlapBox(point, size, angleDeg, filter, results);
    }

    public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D dir, float angleDeg, Collider2D[] results, LayerMask mask, bool includeTriggers = true)
    {
        var filter = BuildFilter(mask, includeTriggers);
        return Physics2D.OverlapCapsule(point, size, dir, angleDeg, filter, results);
    }
}
