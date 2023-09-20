using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkEffects : MonoBehaviour
{
    public Shader shader_doble_vision;
    private Material material_doble_vision;
    public Shader shader_blur;
    private Material material_blur;

    public float intencidad_vision_doble = 0.01f;
    public int intencidad_blur = 2;

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
        material_doble_vision.SetFloat(Shader.PropertyToID("_IntencidadVisionDoble"), intencidad_vision_doble);
        material_blur.SetInt(Shader.PropertyToID("_IntencidadBlur"), intencidad_blur);
    }

    private RenderTexture intermedio;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intermedio == null || source.width != intermedio.width || source.height != intermedio.height)
            intermedio = new RenderTexture(source);
        Graphics.Blit(source, intermedio, material_doble_vision);
        Graphics.Blit(intermedio, destination, material_blur);
    }
}
