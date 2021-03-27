﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

namespace Parallel.Sample
{
    public class ColliderSizeUpdater : MonoBehaviour
    {
        ParallelBoxCollider2D _boxCollider;
        FFloat scale = FFloat.FromDivision(12, 10);

        // Start is called before the first frame update
        void Start()
        {
            _boxCollider = GetComponent<ParallelBoxCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Plus))
            {
                _boxCollider.UpdateShape(_boxCollider.size * scale);
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                _boxCollider.UpdateShape(_boxCollider.size / scale);
            }
        }
    }
}