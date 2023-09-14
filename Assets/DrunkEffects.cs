using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkEffects : MonoBehaviour
{
    public Material material;
    public Shader shader;

    private void Awake()
    {
        material = new Material(shader);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat(Shader.PropertyToID("_IntencidadVisionDoble"), 0.5f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
