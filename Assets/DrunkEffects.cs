using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkEffects : MonoBehaviour
{
    public Shader shader_doble_vision;
    private Material material_doble_vision;
    public Shader shader_blur;
    private Material material_blur;

    public float alcolemia = 1;
    private float intencidad_vision_doble = 0.01f;
    private float intencidad_blur = 1;

    private void Awake()
    {
        material_doble_vision = new Material(shader_doble_vision);
        material_blur = new Material(shader_blur);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        intencidad_vision_doble = alcolemia / 1000;
        intencidad_blur = alcolemia;

        material_doble_vision.SetFloat(Shader.PropertyToID("_IntencidadVisionDoble"), intencidad_vision_doble);
        material_blur.SetFloat(Shader.PropertyToID("_IntencidadBlur"), intencidad_blur);
    }

    private RenderTexture intermedio;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intermedio == null || source.width != intermedio.width || source.height != intermedio.height)
            intermedio = new RenderTexture(source);
        if (alcolemia != 0)
        {
            Graphics.Blit(source, intermedio, material_doble_vision);
            Graphics.Blit(intermedio, destination, material_blur);
        }
        else
            Graphics.Blit(source, destination);
    }
}
