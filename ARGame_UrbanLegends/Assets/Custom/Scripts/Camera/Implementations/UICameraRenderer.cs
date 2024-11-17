using UnityEngine;
using UnityEngine.UI;
using System;

// Implementación del renderizado usando UI de Unity
public class UICameraRenderer : ICameraRenderer
{
    private readonly RawImage _image;
    private readonly AspectRatioFitter _aspectRatioFitter;

    public UICameraRenderer(RawImage image, AspectRatioFitter aspectRatioFitter)
    {
        _image = image ?? throw new ArgumentNullException(nameof(image));
        _aspectRatioFitter = aspectRatioFitter ?? throw new ArgumentNullException(nameof(aspectRatioFitter));
        _aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
    }

    public void UpdateRotation(float angle)
    {
        // Corregir la rotación según la orientación
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            // Ajustar el ángulo para orientación portrait
            angle = 0f;
        }
        else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            angle = 180f;
        }
        
        _image.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    public void UpdateAspectRatio(float ratio)
    {
        if (Screen.orientation == ScreenOrientation.Portrait || 
            Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            ratio = 1f / ratio;
        }
        _aspectRatioFitter.aspectRatio = ratio;
    }


    public void UpdateUVRect(bool isMirrored)
    {
        // Ajustar las coordenadas UV
        if (Screen.orientation == ScreenOrientation.Portrait || 
            Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            _image.uvRect = new Rect(0, 0, 1, 1);
        }
        else
        {
            _image.uvRect = isMirrored ? new Rect(1, 0, -1, 1) : new Rect(0, 0, 1, 1);
        }
    }

    public void SetTexture(Texture texture)
    {
        _image.texture = texture;
    }
}