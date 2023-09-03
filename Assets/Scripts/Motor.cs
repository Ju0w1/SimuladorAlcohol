using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor
{
    public float max_rpm; // rpm maximo de motor
    public float min_rpm; // minimo de rpm para que el motor se mantenga encendido

    public float fuerza_base;

    public float rpm; // rpm actual
    public int cambio; // -1 es reversa y 0 es neutro
    public float aceleracion;
    public float embriague;
    public float freno;

    public Motor(float max_rpm, float min_rpm, float fuerza_base)
    {
        this.max_rpm = max_rpm;
        this.min_rpm = min_rpm;
        rpm = 0;
        cambio = 0;
        aceleracion = 0;
        embriague = 0;
        freno = 0;
        this.fuerza_base = fuerza_base;
    }

    internal float obtenerTorque(float rpm, float radius)
    {
        //throw new NotImplementedException();
        return aceleracion * 400;
    }

    internal float obtenerFreno(float rpm, float radius)
    {
        //throw new NotImplementedException();
        return freno;
    }

    internal void update(float rpm, float radius)
    {
        //throw new NotImplementedException();
    }
}
