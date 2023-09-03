using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor
{
    public float rpm_velocidad = 1;
    public float fuerza_base;

    public float max_rpm; // rpm maximo de motor
    public float min_rpm; // minimo de rpm para que el motor se mantenga encendido

    public float rpm = 0; // rpm actual
    public int cambio = 0; // -1 es reversa y 0 es neutro
    public float aceleracion = 0;
    public float embriague = 0;
    public float freno = 0;

    public Motor(float max_rpm, float min_rpm, float fuerza_base)
    {
        this.max_rpm = max_rpm;
        this.min_rpm = min_rpm;
        this.fuerza_base = fuerza_base;

        // debug
        rpm = min_rpm;
    }

    internal float obtenerTorque(float rpm, float radius)
    {
        //throw new NotImplementedException();
        return aceleracion * 400 * cambio;
    }

    internal float obtenerFreno(float wheel_rpm, float wheel_radius)
    {
        //throw new NotImplementedException();
        return freno;
    }

    internal void update(float wheel_rpm, float wheel_radius)
    {
        if (encendido())
        {
            float base_aceleracion = max_rpm / min_rpm;
            rpm = Mathf.MoveTowards(rpm, aceleracion * max_rpm, rpm_velocidad * Time.deltaTime);
        }
    }

    private bool encendido()
    {
        bool valor = rpm >= min_rpm && rpm <= max_rpm;
        if (!valor)
            rpm = 0;
        return valor;
    }
}
