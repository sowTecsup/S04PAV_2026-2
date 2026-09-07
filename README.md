# S04PAV — Semana 4

**Programación Aplicada a Videojuegos** · 2.º ciclo · C26S
Sesión 4: *Abstracción y modelado del dominio del juego*

La semana 2 iba de **qué guarda un objeto dentro**. La 3, de **cómo nace**.
Esta va de **quién es quién**: qué cosas de tu juego son entidades, cuáles son
solo sus números, y —sobre todo— **quién decide**. Cuando lo decide todo el
atacante, cada enemigo nuevo te obliga a abrirlo. Cuando lo decide cada
entidad, un enemigo nuevo es un archivo nuevo y cero archivos tocados.

## Arrancar

1. **Unity Hub → Add → Add project from disk** → esta carpeta → **Unity 6** (`6000.3.9f1`)
2. **Tools → S04PAV → Preparar escena Semana 4** (si no la preparas, el editor te la ofrece al abrir)
3. **Play**

Salen los cinco botones a la izquierda. **Ten la consola abierta y desactiva
*Clear on Play*:** esta semana casi todo se lee ahí.

> La escena no se versiona a propósito: los GUID dependen de la versión exacta
> del editor. El script la arma igual en las 20 máquinas del laboratorio.

## Los cinco casos

Cada uno monta su situación, la ejecuta y la narra por consola. No hay huecos
que rellenar: se leen, se ejecutan y se rompen a propósito. Son pocos y largos
porque cada uno atraviesa **varias etapas del mismo problema**, no un ejemplo
suelto.

| # | Caso | Qué demuestra |
|---|---|---|
| 01 | El contrato que obliga | `virtual` es una sugerencia, `abstract` es un contrato. Y el olvido que compila |
| 02 | **Datos o comportamiento** ⭐ | por qué `BaseStats` es clase pura y **no** puede ser `MonoBehaviour` |
| 03 | **El switch que crece** ⭐ | quién decide cuánto duele. Cierra la limitación que quedó abierta en la S3 |
| 04 | **De la etiqueta al collider** ⭐ | por qué el hito 3 cambia de tags a colliders, en milisegundos. Y sus dos trampas |
| 05 | Recoger no es chocar | el trigger avisa de todo, y `Destroy` cobra dos veces |

Los casos **04 y 05 necesitan Play**: sin Play no hay pasos de físicas ni
`Destroy` diferido, y la lección saldría al revés. Te avisan y se paran.

> **Los casos usan nombres propios a propósito** — `BichoDelCaso`,
> `StatsDeEjemplo`, `TipoDeGolpe`, `RecolectableDelCaso`— y **no** `Entity`,
> `BaseStats`, `Elements` ni `Collectable`. Así puedes escribir los tuyos con
> los nombres que pide el laboratorio sin que el proyecto deje de compilar.

### Las dos trampas del caso 04, que valen la sesión entera

1. **Sin `Rigidbody2D` en uno de los dos, `OnTriggerEnter2D` no se llama nunca.**
   No hay error ni aviso: no pasa nada. El caso pone los dos sensores al lado.
2. **El enemigo que muere dentro del radio no sale andando: desaparece.** El
   aviso de salida no te llega en ese frame, tu ataque sí corre en ese frame, y
   la lista todavía sujeta el cadáver → `MissingReferenceException`. Es la fuga
   del caso 07 de la semana 3, ahora con collider.

## La pista de entrenamiento — el avance del proyecto

`Assets/Scripts/Proyecto/` es lo que **no** te piden y sin lo cual no puedes
probar lo que sí te piden: dianas y el círculo dibujado.

- `PistaDeEntrenamiento.cs` — coloca maniquíes en un anillo, **la mitad dentro
  del radio de ataque y la mitad fuera**, y dibuja ese radio con un gizmo. Si tu
  detección le pega a todos, tu radio no es el que crees.
- `ManiquiDeEntrenamiento.cs` — un saco que recibe golpes y los canta. **No
  hereda de nada** a propósito: no es una entidad de tu dominio, es un aparato
  de gimnasio para probar el ataque antes de tener enemigos.

Botón derecho sobre `PistaDeEntrenamiento` (en el objeto *Centro*) → **Colocar
maniquíes**. Cuando tu `Enemy` funcione, borra los dos archivos: no forman
parte de la entrega.

**Una limitación queda escrita y sin parchear a propósito:** el `switch` de
elementos se repite dentro de cada entidad (caso 03, ronda C). Con doce
enemigos, un elemento nuevo son doce archivos. Eso **no** lo arregla la
abstracción: se arregla con polimorfismo (semana 7) y sacando los números a
datos (semana 13). Hoy no tienes esa herramienta, y está bien: hoy toca ver por
qué hace falta.

## Tu laboratorio, caso por caso

| Hito | Dónde mirar |
|---|---|
| 1️⃣ (3p) `Entity` abstracta y `BaseStats` por constructor | casos **01** y **02** |
| 2️⃣ (4p) daño elemental por `enum`, y la vida solo por métodos | caso **03** · el `Set` del caso **02** |
| 3️⃣ (3p) detección por colliders y ataque cada cierto tiempo | caso **04** · `PistaDeEntrenamiento` para ver el radio |
| 4️⃣ (4p) `Collectable`, esferas de XP y pociones | caso **05** |

> El ataque **cada cierto intervalo** se hace con
> `InvokeRepeating("AutoAttack", 1f, 1f)` en el `Start`. No hace falta corrutina
> ni acumular `Time.deltaTime`.

## El techo de la semana

Se resuelve todo con clases, objetos, encapsulación, constructores, **clases y
métodos abstractos** y triggers 2D. La herencia entra aquí como herramienta; la
semana 5 va de cuándo **no** usarla.

**No hace falta** interfaces (S8), composición sobre herencia (S9), físicas 2D
de verdad —fuerzas, rebotes— (S10), eventos (S12), `ScriptableObject` (S13) ni
Singleton (S14). Tampoco corrutinas ni patrones con nombre propio. Si te ves
buscando esas palabras, te estás complicando.
