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
    private CanvasGroup canvasGroup;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        Lua.RegisterFunction("Fade", this, SymbolExtensions.GetMethodInfo(() => FadeCommand((double) 0, (double) 0)));
        Lua.RegisterFunction("FadeEnd", this, SymbolExtensions.GetMethodInfo(() => FadeEndCommand((double) 0, (double) 0)));
        Lua.RegisterFunction("ShowIntro", this, SymbolExtensions.GetMethodInfo(() => ShowIntro((double) 0)));
        Lua.RegisterFunction("HideIntro", this, SymbolExtensions.GetMethodInfo(() => HideIntro()));

    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("Fade");
    }

    
    void FadeCommand(double alpha, double duration)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Fade((float) alpha, (float) duration);
    }
    
    void FadeEndCommand(double alpha, double duration)
    {
        HideIntro();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.SetAsLastSibling();
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
        float startAlpha = canvasGroup.alpha;
        Color startColor = image.color;
        float time = duration;
        

        while (time > 0)
        {
            time -= Time.unscaledDeltaTime;
            image.color = new Color(startColor.r, startColor.g, startColor.b, 1);
            canvasGroup.alpha = EasingFunctions.Ease(targetAlpha, startAlpha, time / duration, easeFunction);
            //image.color = EasingFunctions.Ease(color, startColor, time / duration, easeFunction);

            yield return null;
        }

        image.color = image.color = new Color(startColor.r, startColor.g, startColor.b, 1);
        canvasGroup.alpha = targetAlpha;
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


    public GameObject[] IntroDrawings;

    [Sirenix.OdinInspector.Button]
    private void ShowIntro(double index)
    {
        HideIntro();
        IntroDrawings[(int) index - 1].SetActive(true);
    }

    [Sirenix.OdinInspector.Button]
    void HideIntro()
    {
        foreach (var introDrawing in IntroDrawings)
        {
            introDrawing.SetActive(false);
        }
    }
}
