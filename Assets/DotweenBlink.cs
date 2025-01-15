using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotweenBlink : MonoBehaviour
{
    public float blinkDuration = 0.5f; // Duración de cada ciclo de parpadeo (tiempo en el que se desvanece y aparece)
    public float targetAlpha = 0f; // Alpha hacia el que se desvanece (0 = completamente transparente)
    public int loops = -1; // Número de repeticiones del parpadeo. -1 para ciclo infinito

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Intentar obtener CanvasGroup, si no existe, se agrega uno
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Iniciar el parpadeo
        StartBlinking();
    }

    private void StartBlinking()
    {
        // Parpadeo infinito
        canvasGroup.DOFade(targetAlpha, blinkDuration)
                   .SetLoops(loops, LoopType.Yoyo) // "Yoyo" hace que el fade sea reversible, es decir, va de 1 a 0 y luego de 0 a 1
                   .SetEase(Ease.InOutSine); // Establece una curva de animación suave
    }
}
