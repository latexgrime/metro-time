using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject[] npc;
    public bool activateEffect = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerTarget")
        {
            activateEffect = true;
            for (int i = 0; i < npc.Length; i++)
            {
                Destroy(npc[i]);
            }
        }
        StartCoroutine(DeactivateEffect());
    }

    IEnumerator DeactivateEffect()
    {
        yield return new WaitForSeconds(1);
        activateEffect = false;
    }
}
