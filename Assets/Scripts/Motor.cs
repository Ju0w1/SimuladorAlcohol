using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Motor
{
    public float rpm_velocidad = 1000;
    public float fuerza_base;

    public float max_rpm; // rpm maximo de motor
    public float min_rpm; // minimo de rpm para que el motor se mantenga encendido

    public float rpm = 0; // rpm actual
    public int cambio = -1; // -1 es reversa y 0 es neutro
    public float aceleracion = 0;
    public float embrague = 0;
    public float freno = 0;

    public Motor(float min_rpm, float max_rpm, float fuerza_base)
    {
        this.min_rpm = min_rpm;
        this.max_rpm = max_rpm;
        this.fuerza_base = fuerza_base;

        // debug
        rpm = min_rpm;
    }

    internal float obtenerTorque(float rpm, float radius)
    {
        return obtener_fuerza_necesaria(rpm, radius) * efecto_embrague();
    }

    public float efecto_embrague()
    {
        return 1 - embrague;
    }

    internal float obtenerFreno(float wheel_rpm, float wheel_radius)
    {
        return freno;
    }

    internal void update(float wheel_rpm, float wheel_radius)
    {
        if (encendido())
        {
            if (efecto_embrague() == 1)
                rpm = Mathf.MoveTowards(rpm, obtener_rpm_objetivo_motor(wheel_rpm), 2000);

            float base_aceleracion = min_rpm / max_rpm;
            rpm = Mathf.MoveTowards(rpm, (aceleracion + base_aceleracion) / (1 - base_aceleracion) * max_rpm, rpm_velocidad * Time.deltaTime);
            rpm = Mathf.Min(rpm, max_rpm);
    
        }
    }

    // Inverso de obtener_rpm_objetivo_rueda
    public float obtener_rpm_objetivo_motor(float wheel_rpm)
    {
        return Mathf.Abs(wheel_rpm / (0.2f * cambio * cambio));
    }

    public float obtener_rpm_objetivo_rueda()
    {
        float rpm_objetivo = rpm * cambio * cambio * 0.2f;
        return rpm_objetivo * (cambio > 0 ? 1 : -1);
    }

    private float obtener_fuerza_necesaria(float wheel_rpm, float wheel_radius)
    {
        if (cambio == 0)
            return 0;
        float rpm_objetivo = obtener_rpm_objetivo_rueda();

        if ((rpm_objetivo - wheel_rpm) * efecto_embrague() > 70)
            rpm = 0;
        float calculo_primario = 200.0f - Mathf.Min(Mathf.Abs(rpm_objetivo - wheel_rpm) / 100, 200.0f);
        calculo_primario *= Mathf.Abs(6 - cambio);
        //float calculo_primario = Mathf.Abs(rpm_objetivo - wheel_rpm);
        //calculo_primario *= Mathf.Abs(6 - cambio);
        return calculo_primario * (rpm_objetivo > wheel_rpm ? 1 : -1);
    }

    public bool encendido()
    {
        bool valor = rpm >= min_rpm && rpm <= max_rpm;
        if (!valor)
            rpm = 0;
        return valor;
    }

    public void encender()
    {
        rpm = min_rpm;
    }
}
