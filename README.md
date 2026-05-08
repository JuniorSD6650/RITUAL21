# 🃏 Ritual 21: Deck Chronicles 🔮

**Ritual 21** es un motor de juego de cartas de duelo y azar para multijugador local desarrollado en .NET. El sistema está diseñado para ser **totalmente escalable**, permitiendo intercambiar "Paquetes de Invocación" (Skins) sin alterar la lógica central del ritual.

---

## 🎨 Arquitectura de Paquetes (Escalabilidad)
El juego separa la **Lógica** de la **Apariencia**. Actualmente, el sistema soporta el cambio dinámico de texturas:
- **Paquete Inicial:** *Demorfitos Ancestrales* (Monstruos graciosos con trajes).
- **Próximos Paquetes:** *Meme Cats Edition*, *Cyber-Demorfos*, entre otros.

---

## 🎮 Mecánicas Globales

- **Multijugador Local:** De 2 a 5 invocadores. Sistema de turnos rotativo por nombre.
- **El Límite del Ritual:** Sumar 21 puntos. Superar este valor provoca una "Expulsión del Vacío" (Derrota).
- **Habilidades de Grito:** Activación de efectos mediante **Polimorfismo** al invocar cartas Legendarias.

---

## 👺 El Mazo de Demorfitos (52 Cartas)

| Rango | Cantidad | Valor | Especialidad |
| :--- | :--- | :--- | :--- |
| **Comunes (Sbirros)** | 30 | 2 - 9 | Estabilizan el ritual sin efectos secundarios. |
| **Guardianes (Élite)** | 11 | 10 | Poseen *Escudo Inmune*: Protegidos contra habilidades rivales. |
| **Legendarios** | 10 | Q, K, As | **Habilidades de Grito** (Alteran las reglas del juego). |
| **EL ARCHIDEMORFO (Jefe)** | 1 | 1 - 11 | *Mirada de Espejo*: Copia el valor de la carta más fuerte en mesa. |

### ✨ Poderes de Invocación (Polimorfismo)

1. **💀 El Espectro (Q):** *Grito de Pánico*. [cite_start]Obliga al invocador anterior a descartar su última criatura[cite: 196, 204].
2. **😇 El Chamán (As):** *Sanación Crítica*. [cite_start]Si el total supera 21, resta 5 puntos para evitar la derrota[cite: 165].
3. **🐍 La Quimera (K):** *Polimorfia*. El jugador decide si su valor es 5 o 10 al entrar en juego.

---

## 🛠️ Tecnologías y Pilares POO

Este proyecto es una vitrina de ingeniería en C#:
- [cite_start]**Herencia:** Clases especializadas que expanden la base de `Carta`[cite: 889, 892].
- [cite_start]**Polimorfismo:** Un mismo método `ActivarHabilidad()` con múltiples resultados[cite: 902, 908].
- [cite_start]**Manejo de Excepciones:** Uso de `try-catch` y `throw` para reglas de juego y errores de usuario[cite: 937, 945].
- [cite_start]**Multiplataforma:** Compatible con Windows, Linux, Mac y listo para escalar a Móvil vía Unity/.NET MAUI[cite: 1416, 1427].

---
*Diseñado y programado por Junior. El ritual ha comenzado.* 