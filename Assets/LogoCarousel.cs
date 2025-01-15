using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoCarousel : MonoBehaviour
{
    public List<Image> logos; // Lista de im�genes
    public float entryDuration = 1f; // Duraci�n de la entrada
    public float pauseDuration = 2f; // Duraci�n de la pausa en el centro
    public float exitDuration = 1f; // Duraci�n de la salida
    public Vector3 entryPosition; // Posici�n inicial (fuera del borde derecho)
    public Vector3 centerPosition; // Posici�n central (en pantalla)
    public Vector3 exitPosition; // Posici�n final (fuera del borde izquierdo)

    private void Start()
    {
        if (logos == null || logos.Count == 0)
        {
            Debug.LogError("No se han asignado logos.");
            return;
        }

        foreach (var logo in logos)
        {
            RectTransform rectTransform = logo.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = entryPosition;
        }

        StartCarousel();
    }

    private void StartCarousel()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var logo in logos)
        {
            RectTransform rectTransform = logo.GetComponent<RectTransform>();

            // Animaci�n de entrada, movimiento hacia el centro
            sequence.Append(rectTransform.DOAnchorPos(centerPosition, entryDuration).SetEase(Ease.OutCubic))
                    // Pausa en el centro
                    .AppendInterval(pauseDuration)
                    // Animaci�n de salida, movimiento hacia la izquierda
                    .Append(rectTransform.DOAnchorPos(exitPosition, exitDuration).SetEase(Ease.InCubic))
                    // Resetea la posici�n inicial para el siguiente ciclo
                    .AppendCallback(() => rectTransform.anchoredPosition = entryPosition);
        }

        // Hace que el ciclo sea infinito
        sequence.SetLoops(-1);
    }
}
