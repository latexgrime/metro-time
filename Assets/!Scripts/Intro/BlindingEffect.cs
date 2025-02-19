using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
public class BlindingEffect : MonoBehaviour
{
    public Volume volume;
    public CanvasGroup alphaController;
    [SerializeField] private NPC effect;
    private bool _isLightOn;

    private void Update()
    {
        if (effect.activateEffect == true)
        {
            Flashbang();   
        }

        if (effect.activateEffect == false)
        {
            Time.timeScale = .05f;

            alphaController.alpha -= Time.deltaTime * (int)1.5;
            volume.weight -= Time.deltaTime * (int)1.5;

            if (alphaController.alpha <= 0)
            {
                volume.weight = 0;
                alphaController.alpha = 0;
                Time.timeScale = 1;
                _isLightOn = false;
            }
        }
    }

    private void Flashbang()
    {
        volume.weight = 1;
        _isLightOn = true;
        alphaController.alpha = 1;
    }
}
