using UnityEngine;

[ExecuteAlways]
public class YSortOrder : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sr != null)
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}

