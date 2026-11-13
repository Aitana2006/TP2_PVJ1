TP2 - Programación de Videojuegos 1

Motor: Unity 6 (6000.3.19f1).

Descripción del Juego

Juego de acción y exploración en primera persona en 3D. El jugador debe explorar el terreno, defenderse de un enemigo con inteligencia artificial, recolectar potenciadores (*power-ups*), recuperar salud en la estación de curación y llevar la **moneda** hasta la plataforma final para completar el nivel.

Controles del Jugador
W, A, S, D = Moverse por el escenario
Mouse = Mirar alrededor en primera persona
Espacio = Saltar
E = Interactuar a distancia
Q = Soltar el ítem llevado en la mano
P = Pausar
R = Reiniciar nivel

Victoria
- El jugador debe buscar la moneda ubicada en el mapa y recogerla.
- Debe transportarla hasta la plataforma azul
- Al pisar la plataforma con la moneda en mano, la moneda queda fijada en la meta y se activa el estado de ¡VICTORIA!.
Nota: Si el jugador llega a la plataforma sin la moneda, el juego le avisará en pantalla que debe buscarla y no le otorgará la victoria.

Game Over
- El enemigo patrulla y persigue al jugador. Si lo alcanza, le inflige daño continuo por contacto y mediante ataques.
- Si la vida del jugador llega a 0 HP, se activa la pantalla de GAME OVER y el nivel finaliza.

Implementaciones Técnicas y Rúbrica Cumplida

1. Detección e Interacción por Raycast y LayerMask
Chequeo de Suelo:
  - Proyecta un rayo desde los pies del personaje hacia abajo.
  - Utiliza una configurada con las capas de suelo para validar cuándo puede saltar de forma limpia y precisa.
  - Incorpora visualización Gizmo: Verde al tocar suelo, Rojo en el aire.
Interacción a Distancia:
  - Emite un Raycast frontal desde la cámara con alcance de 4.5 metros.
  - Filtra estrictamente usando de la capa Interactable.
  - Permite activar consolas/terminales de datos y recoger ítems/monedas  pulsando la tecla E con interfaz contextual en pantalla.
Garantía contra Autodaño:
  - En los rayos y colisionadores de ataque, se ignora la capa del jugador y se descarta cualquier objeto perteneciente a su jerarquía.
  - La función de daño previene explícitamente cualquier daño entre el jugador y sus componentes o armas.

2. Estación de Curación:
- Diseño Identificable: Plataforma cilíndrica con material verde brillante y una luz de punto verde visible a distancia en la zona base del jugador.
- Detección por Collider: Colisionador tipo Trigger que detecta la presencia del jugador.
- Lógica Progresiva: Otorga +10 HP cada 1.5 segundos mientras el jugador herido esté dentro.
Límite de Carga y Cooldown:
  - Capacidad máxima de 50 HP de carga dispensable.
  - Al agotarse la carga, entra en tiempo de enfriamiento durante 5 segundos.
Retroalimentación Visual e Interfaz:
  - Durante el enfriamiento apaga su luz verde, cambia su material a gris atenuado y muestra en pantalla y consola
  - Al finalizar el enfriamiento, reactiva la luz y reestablece su carga máxima.

3. Inteligencia Artificial del Enemigo
- Máquina de estados:
  Patrol: Patrulla entre puntos de ruta designados.
  Chase: Detecta al jugador y lo persigue. Reacciona inmediatamente si el jugador lo ataca primero.
  Attack: Ataca al jugador cuando entra en su radio de ataque.
  Daño por Contacto: Aplica daño directo tanto por colisiones físicas como por proximidad horizontal.

4. Sistema de Salud y HUD
- Barra de vida en pantalla en tiempo real con indicador numérico.
- Efecto de destello rojo al recibir daño y destello verde al regenerar salud en la estación.
- Mensajes centrales de fin de partida (Victoria en Cyan, Game Over en Rojo) con soporte para reinicio instantáneo con R.

Estructura Principal de Scripts

Control de locomoción, gravedad y chequeo de suelo por Raycast.
Raycast frontal con LayerMask para interactuar a distancia con la tecla E.
Interfaz polimórfica para objetos interactivos.
Terminal interactivo de consola para interacción por Raycast.
Componente para interactuar y recoger ítems/moneda por Raycast.
Sistema de equipamiento en manos y soltar con Q.
Attack: Lógica de ataque cuerpo a cuerpo y a distancia con protección total contra autodaño.
HealthSystem: Gestión de salud, daño, curación y barra de vida en OnGUI.
HealingStation: Estación de curación con recarga progresiva, límite de carga y cooldown.
enemy: Inteligencia artificial del enemigo.
zonaFinal: Detección de meta y depósito de la moneda para activar la victoria.
GameManager: Administrador global de estados de juego.
power up: Potenciadores de velocidad con temporizador visual.
