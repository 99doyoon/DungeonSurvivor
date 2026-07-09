using System.Collections;
using UnityEngine;

interface IHit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool Hit();

    IEnumerator SetIsHit();
}
