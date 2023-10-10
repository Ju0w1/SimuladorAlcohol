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
    public float min_tot_rpm; // minimo de rpm, pero de verdad, si no se pasa se apaga
    public float max_tot_rpm; // maximos rpm, pero de verdad, si se pasa se apaga

    public float rpm = 0; // rpm actual
    public int cambio = -1; // -1 es reversa y 0 es neutro
    public float aceleracion = 0;
    public float embrague = 0;
    public float freno = 0;

    public Motor(float min_rpm, float max_rpm, float fuerza_base)
    {
        this.min_rpm = min_rpm;
        this.max_rpm = max_rpm;
        this.min_tot_rpm = 25;//min_rpm / 2;
        this.max_tot_rpm = max_rpm + 1000;
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
    /*
    internal float obtenerTorque(float rpm, float radius)
    {
        // float efecto_fuerza = obtener_fuerza_necesaria(rpm, radius);
        float efecto_fuerza = cambio < 0 ? Mathf.Min(obtener_fuerza_necesaria(rpm, radius), 0):Mathf.Max(obtener_fuerza_necesaria(rpm, radius), 0);
        // Debug.Log(efecto_fuerza);
        return efecto_fuerza * efecto_embrague();
    }

    public float efecto_embrague()
    {
        return 1 - embrague;
    }

    internal float obtenerFreno(float wheel_rpm, float radius)
    {
        // float efecto_fuerza = Mathf.Abs(cambio < 0 ? Mathf.Min(obtener_fuerza_necesaria(rpm, radius), 0):Mathf.Max(obtener_fuerza_necesaria(rpm, radius), 0));
        float effect = Mathf.Max((wheel_rpm - obtener_rpm_objetivo_rueda()) * (cambio > 0 ? 1:-1), 0) * efecto_embrague() * 0.01f;
        // Debug.Log(effect + " " + obtener_rpm_objetivo_rueda());
        return freno + effect;// + efecto_fuerza * 1000 * efecto_embrague();
    }
    */

    internal void update(float wheel_rpm, float wheel_radius, float jump_size)
    {
        if (encendido())
        {
            if (cambio != 0)
            {
                // acomoda las revoluciones del motor a las del las ruedas
                rpm = Mathf.MoveTowards(rpm, obtener_rpm_objetivo_motor(wheel_rpm), 2000 * efecto_embrague());
            }

            // acomoda las revoluciones del motor hacia la que indica el acelerador
            float rpm_objetivo = aceleracion * (max_rpm - min_rpm) + min_rpm;
            rpm = Mathf.MoveTowards(rpm, rpm_objetivo, rpm_velocidad * Time.deltaTime * 3);
            // esto apaga el motor si las revoluciones son muy bajas, mezcla las revoluciones del motor con las de las ruedas para obtener un valor mas realista
            if (Mathf.Lerp(obtener_rpm_objetivo_motor(wheel_rpm), rpm, 1 - efecto_embrague()) < min_tot_rpm)
                rpm = 0;
            // esto permite apagar el motor si el cambio de cambios fue muy mal hecho
            if (jump_size > 500)
                rpm = 0;
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

        // if ((rpm_objetivo - wheel_rpm) * efecto_embrague() > 60 * Mathf.Abs(cambio))
        //     rpm = 0;
        float calculo_primario = 200.0f - Mathf.Min(Mathf.Abs(rpm_objetivo - wheel_rpm) / 100, 200.0f);
        calculo_primario *= Mathf.Abs(6 - cambio);
        //float calculo_primario = Mathf.Abs(rpm_objetivo - wheel_rpm);
        //calculo_primario *= Mathf.Abs(6 - cambio);
        return calculo_primario * (rpm_objetivo > wheel_rpm ? 1 : -1);
    }

    public bool encendido()
    {
        // esta variable sirve para que el no acelerar apague el motor en segunda marcha
        float factor_por_cambio = Mathf.Abs(cambio) * 3.2f * efecto_embrague();
        bool valor = rpm >= min_tot_rpm * factor_por_cambio && rpm <= max_tot_rpm;
        if (!valor)
            rpm = 0;
        return valor;
    }

    public void encender()
    {
        rpm = min_rpm;
    }
}
