using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    private Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        
        Lua.RegisterFunction("Fade", this, SymbolExtensions.GetMethodInfo(() => FadeCommand((double) 0, (double) 0)));

    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("Fade");
    }

    
    void FadeCommand(double alpha, double duration)
    {
        Fade((float) alpha, (float) duration);
    }

    [Sirenix.OdinInspector.Button]
    public void Fade(float targetAlpha, float duration, EasingFunctions.EaseFunction easeFunction = EasingFunctions.EaseFunction.Linear, System.Action callback = null)
    {
        Fade(targetAlpha, duration, Color.black, easeFunction, callback);
    }
    
    public void Fade(float targetAlpha, float duration, Color color, EasingFunctions.EaseFunction easeFunction = EasingFunctions.EaseFunction.Linear, System.Action callback = null)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, color, easeFunction, callback));
    }

    public bool IsFadeActive => fadeRoutine != null;
    private Coroutine fadeRoutine = null;

    private IEnumerator FadeRoutine(float targetAlpha, float duration, Color color, EasingFunctions.EaseFunction easeFunction = EasingFunctions.EaseFunction.Linear, System.Action callback = null)
    {
        float startAlpha = image.color.a;
        Color startColor = image.color;
        float time = duration;
        

        while (time > 0)
        {
            time -= Time.unscaledDeltaTime;
            image.color = new Color(startColor.r, startColor.g, startColor.b, EasingFunctions.Ease(targetAlpha, startAlpha, time / duration, easeFunction));
            //image.color = EasingFunctions.Ease(color, startColor, time / duration, easeFunction);

            yield return null;
        }

        image.color = image.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        callback?.Invoke();

        fadeRoutine = null;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetAlpha(float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
}
