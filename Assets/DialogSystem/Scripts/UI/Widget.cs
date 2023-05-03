using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class Widget : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Coroutine _fade;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Fade(float opacity, float duration, Action onFinished)
    {
        if (duration <= 0)
        {
            _canvasGroup.alpha = opacity;
            onFinished?.Invoke();
            return;
        }
        if(_fade != null)
            StopCoroutine(_fade);
        _fade = StartCoroutine(CanvasFade(opacity, duration, onFinished));
    }

    IEnumerator CanvasFade(float opacity, float duration, Action onFinished)
    {
        var startAlpha = _canvasGroup.alpha;
        var time = 0f;
        while (time < duration)
        {
            time = Mathf.Min(duration, time + Time.unscaledDeltaTime);
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, opacity, curve.Evaluate(time / duration));
            yield return null;
        }
        onFinished?.Invoke();
    }
}
