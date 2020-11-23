using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularCollider : MonoBehaviour
{
    public float m_width = 0.30f;
    public float m_hight = 0.30f;

    public Vector2 m_pos = new Vector2();
    public Vector2 m_previousPos = new Vector2();

    private void Start()
    {
        m_pos = transform.position;
        m_previousPos = m_pos;
    }

    private void FixedUpdate()
    {
        m_previousPos = m_pos;
        m_pos = transform.position;
    }
}
