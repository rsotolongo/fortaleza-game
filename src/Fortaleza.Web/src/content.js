// Long-form texts from the original game's "Introducción" and "Consejos" menu items.
// Block syntax: '## ' heading, '- ' list item, '> ' quote, '' ignored, anything else a paragraph.

const intro1 = {
  es: [
    'Usted es un hombre común, sumergido en una vida rutinaria y mediocre. Día tras día espera que algo interesante aparezca en su camino. Una mañana, luego de haber leído el periódico, encuentra una carta junto a la puerta. Desde la sala se acerca un ruido de tacones. Usted rompe el sobre y guarda nerviosamente la carta en un bolsillo. Su mujer acaba de pasar.',
    'Ahora está solo. Usted despliega el papel y comienza a leer.',
    '> Estimado señor:',
    '> Tengo el alto honor de comunicarle que usted ha sido designado para cumplir una de las misiones más importantes de la historia del hombre: Eliminar a la Bestia.',
    'Usted levanta los ojos y se pregunta "¿Quién será la Bestia esa? Debe ser algún presidiario que ha escapado, o un narcotraficante". Usted continúa leyendo.',
    '> La Bestia es una de las criaturas más temidas y misteriosas del universo. Según varios informes que nos han llegado últimamente, habita un lugar llamado la Fortaleza, aunque hay quienes dicen que la Bestia y la Fortaleza son una misma cosa. Nadie ha visto a la Bestia (quienes lo hicieron no pudieron contarlo nunca). Solo sabemos que la Fortaleza aparece todos los martes 24 de abril en los años bisiestos, o sea, casi nunca.',
    '> Su misión es: adentrarse en la Fortaleza y matar a la Bestia. Nada más.',
    '> Tenga preparada una muda de ropa (ropa de trabajo preferiblemente) para cuando vayamos a buscarlo.',
    '> General X — Jefe de la Oficina de Casualidades',
    'Usted guarda el papel lentamente. No puede creer lo que está ocurriendo. ¿Por qué yo?, se pregunta.',
    'Hay que ver las vueltas que da la vida. Varios minutos antes usted estaba ansioso de que ocurriese algo especial. Debía estar satisfecho. Uno no sabe con cuál se gana y con cuál se pierde.'
  ],
  en: [
    'You are an ordinary man, sunk in a routine and mediocre life. Day after day you wait for something interesting to cross your path. One morning, after reading the newspaper, you find a letter by the door. The click of heels approaches from the living room. You tear open the envelope and nervously stuff the letter into a pocket. Your wife has just walked by.',
    'Now you are alone. You unfold the paper and begin to read.',
    '> Dear Sir:',
    '> I have the high honor of informing you that you have been chosen to carry out one of the most important missions in the history of mankind: Eliminate the Beast.',
    'You look up and wonder, "Who is this Beast? Must be some escaped convict, or a drug trafficker." You keep reading.',
    '> The Beast is one of the most feared and mysterious creatures in the universe. According to several reports that have reached us lately, it dwells in a place called La Fortaleza, although some say that the Beast and La Fortaleza are one and the same. Nobody has seen the Beast (those who did never lived to tell). All we know is that La Fortaleza appears every Tuesday the 24th of April in leap years, that is, almost never.',
    '> Your mission is: enter La Fortaleza and kill the Beast. Nothing more.',
    '> Have a change of clothes ready (work clothes, preferably) for when we come to fetch you.',
    '> General X — Head of the Office of Coincidences',
    'You slowly put the paper away. You cannot believe what is happening. Why me? you ask yourself.',
    'Life certainly takes its turns. Only minutes ago you were eager for something special to happen. You should be satisfied. You never know which way luck will fall.'
  ]
}

const intro2 = {
  es: [
    'Bien, esta es la primera secuela de La Fortaleza. Pudiéramos llamarla "El regreso de la Fortaleza" por ser una segunda parte (¿nunca fueron buenas?) y porque su misión no es otra que regresar al mundo exterior.',
    'Ante todo haremos un pequeño resumen de la primera parte, para que los que no tuvieron la buena (o mala) fortuna de adentrarse en ella:',
    'Usted, un pobre terrícola sumido en el anonimato, fue elegido para eliminar a la Bestia, una de las criaturas más horribles y temidas del universo. La Bestia habitaba la Fortaleza de una forma muy singular. Estaba embotada en la Fortaleza misma, en cada pared, en cada habitación, en el aire y en la luz. Su misión era eliminar a la Bestia.',
    'Hasta aquí el resumen. Veamos que tiene que hacer ahora.',
    'Si bien en la primera parte usted debía acabar con una vida, ahora está obligado a salvar otra: la suya.',
    'La Bestia había planeado una venganza terrible. En el mismo instante en que usted la destruyó, ella echó a andar su diabólico plan. Usted quedó profundamente dormido mientras la Fortaleza cambiaba. Cada cosa se descompuso en un caudal único y luego, a partir de esa masa gigantesca, reapareció la Fortaleza.',
    'Usted despierta en un cuarto, habiendo perdido toda noción de tiempo y espacio, y debe tratar de salir. La Bestia ha dejado una persona (si se le puede llamar así) encargada de que usted perezca irremediablemente.',
    'Mantenga la mente abierta.'
  ],
  en: [
    'Well, this is the first sequel to La Fortaleza. We could call it "The Return of La Fortaleza", being a second part (were they ever any good?) and because your mission is none other than to return to the outside world.',
    'First, a short summary of the first part, for those who did not have the good (or bad) fortune of venturing into it:',
    'You, a poor earthling sunk in anonymity, were chosen to eliminate the Beast, one of the most horrible and feared creatures in the universe. The Beast dwelt in La Fortaleza in a very singular way. It was embedded in La Fortaleza itself, in every wall, in every room, in the air and in the light. Your mission was to eliminate the Beast.',
    'That is the end of the summary. Let us see what you have to do now.',
    'Whereas in the first part you had to put an end to a life, now you are forced to save another: your own.',
    'The Beast had planned a terrible revenge. The very instant you destroyed it, it set its diabolical plan in motion. You fell into a deep sleep while La Fortaleza changed. Everything dissolved into a single stream and then, from that gigantic mass, La Fortaleza reappeared.',
    'You wake up in a room, having lost all notion of time and space, and you must try to get out. The Beast has left behind a person (if it can be called that) in charge of making sure you perish beyond all remedy.',
    'Keep an open mind.'
  ]
}

const advice = {
  es: [
    'La Fortaleza es un juego hecho a la medida de su inteligencia (esto no es un spot de TV en TV). Se supone que mientras más inteligente sea usted, más fácil le será dominar los objetivos del juego. Sin embargo, hay otro aspecto que puede compensar la falta de inteligencia: el método (desgraciadamente, en la vida real ocurre lo mismo). Ambas fortalezas están concebidas de forma que ningún detalle escape al ojo metódico.',
    'El método surge de la misma práctica, pero la práctica puede ser desagradable hasta no tener un método elaborado. Es un círculo vicioso. Como la Fortaleza es un juego y se supone que la gente se entretenga, he decidido brindar algo equivalente a un método para los principiantes. Se parece mucho a una receta de cocina, o a un texto de ingeniería. En realidad, no son más que algunos consejos.',
    '## Gran Método Infalible Para Resolver Fortalezas',
    '- Regla 1: Tenga siempre lápiz y papel a mano.',
    '- Regla 2: Asegúrese de que el lápiz tenga punta en todo momento.',
    '- Regla 3: Vaya llevando un diagrama donde represente los distintos lugares que visite y las relaciones entre ellos (puertas, etc). De no seguir esta regla puede perderse en la Fortaleza hasta el punto de no encontrar la forma de pararse de la silla y apagar la máquina.',
    '- Regla 4: Trate de observar todas las cosas. Hágalo teniendo en cuenta el siguiente orden de prioridad: 1) cosas inanimadas, 2) cosas vivas, 3) puertas y uniones en general.',
    '- Regla 5: Algunas bases de cómo diferenciar las cosas con que usted se puede tropezar dentro de la Fortaleza. Estos métodos funcionan siempre, con algunas excepciones.',
    '- (1) Cosas inanimadas: se pueden coger, a no ser que pesen demasiado. Entonces se muestra el mensaje "Sería mucho peso." Otra forma, que nunca falla, es preguntarles o tratar de matarlas.',
    '- (2) Cosas que esconden otras cosas: son como las anteriores, pero no pueden cogerse. Usted debe romperlas con algún objeto específico (que debe descubrir) para tener acceso al objeto que esconden.',
    '- (3) Las cosas vivas pueden dividirse en dos grandes grupos: las que están de su parte y las que están contra usted. A los primeros usted debe interrogarlos (entre otras cosas) para determinar en qué lo pueden ayudar. Siempre la ayuda se reduce a entregarles cierto objeto, el problema es descubrir qué es lo que necesitan. Después de habérselo entregado, vuelva a interrogarlos y obtendrá alguna información valiosa. A los integrantes del segundo grupo debe matarlos. No tenga piedad. Generalmente cuando están agonizando confiesan algo interesante. La mayoría de las veces usted debe descubrir el arma que debe usar, puesto que cualquier objeto no sirve. Cuando se les interroga nunca responden.',
    '- (4) Las uniones (puertas, puentes, caminos, etc): si no logra reconocerlas por el nombre, use el comando "terminar" y no vuelva a tocar la Fortaleza otra vez.',
    '- Regla 6: Las uniones pueden estar abiertas o cerradas. Para abrirlas use el comando "abrir". Algunas necesitan palabras mágicas para abrirlas, y otras veces se verá obligado a resolver acertijos.',
    '- Regla 7: Existen algunas uniones que deben cruzarse cumpliendo algún requerimiento. Unas veces no puede cruzar si no lleva cierto objeto, y otras veces un objeto se puede hacer indispensable para poder atravesar la unión. Cuál objeto usar y cuál no, es algo que usted debe inferir a partir de sus observaciones y de las cosas que le hayan dicho.',
    '- Regla 8: Trate de acumular las cosas que encuentre en lugares céntricos. Así, si se ve necesitado de algún objeto que no lleva consigo, no se verá obligado a viajar tanto.',
    '## Vocabulario',
    'Los artículos "el" y "la", y la preposición "por" se desprecian siempre. Las tildes pueden ser omitidas.',
    '- Para moverse: ir, atravesar, cruzar. Luego el nombre de la unión. Si la puerta no necesita de alguna palabra clave, se abre sin necesidad de ordenarlo explícitamente. Ejemplos: ir puerta / ir por la puerta / cruzar la puerta',
    '- Para mirar el lugar: mirar, observar.',
    '- Para mirar una cosa: mirar, ver, leer. Luego el nombre del objeto. Ejemplos: mirar tal cosa / leer la carta',
    '- Para romper: romper, forzar, destrozar. Luego va el objeto que se quiere romper, después la palabra "con" y al final el objeto que se usará para romper el otro. Ejemplos: romper el busto con la maza / romper vaso con piedra',
    '- Para coger algo: coger, tomar. Ejemplos: coger tal cosa / tomar tal cosa',
    '- Para dejar: dejar, soltar. Si se pone "todo", usted se imagina qué pasa. Ejemplos: dejar tal cosa / dejar todo',
    '- Para interrogar: interrogar, preguntar. Luego el nombre del individuo. Ejemplos: interrogar al mudo / preguntar al sordo',
    '- Para abrir: abrir. Luego la unión que desea abrir. Si va a usar palabras mágicas añada "diciendo" y ponga luego las palabras. Si va a responder un acertijo use "respondiendo" antes de la respuesta. En realidad, "diciendo" es equivalente a "respondiendo" y ambos pueden ser sustituidos por "con". Ejemplos: abrir tal puerta diciendo abra cadabra / abrir tal puerta con el numero del carnet',
    '- Para matar: matar, asesinar. La sintaxis es similar a la de romper. El arma homicida puede ser obviada. Ejemplos: asesinar caperucita roja / matar al lobo con la escopeta',
    '- Para dar: dar, regalar. Luego el objeto que se va a dar y después a quién se le va a entregar, separados por una "a". El objeto debe estar entre sus pertenencias. Ejemplos: dar veneno a la abuelita / dar botella de vino a el borracho (en el último ejemplo el arcaísmo es inevitable, recuerde que el uso de "a" es obligatorio).',
    '- Para listar sus pertenencias: inventario. Al lado del nombre de cada objeto aparece cuánto pesa, expresado en "bolsas de arena del Río Negro". Para que tenga una idea exacta, una "bolsa" es equivalente a la octava parte de un tubo de luz fría lleno de agua.',
    '- Para pesar un objeto: pesar, y luego el nombre del objeto. Como es lógico, debe tener una balanza.',
    '- Para tener una idea de cuánto le falta: porciento. Le indica qué porciento de la Fortaleza ha recorrido hasta el momento (habitaciones visitadas entre total de habitaciones, por cien). Este porciento no es un indicador exacto de cuán cerca de la solución se encuentra: usted puede haber recorrido la Fortaleza entera y no tener idea de qué hacer para ganar. Eso pasa.',
    '- Para terminar: terminar, abandonar. Este comando termina provocando su muerte.',
    '## Rastros',
    'Desde el menú (⋯) puede guardar un rastro, que almacena todos los comandos que ha tecleado y que afectan el estado de la Fortaleza (no cuentan mirar, ver, inventario, interrogar, etc.), y volver a cargarlo en cualquier momento. Como se trata de un fichero de texto, puede editarse; se permiten comentarios tipo Pascal "{ }". Los rastros son una de las herramientas más útiles cuando se está resolviendo una Fortaleza.',
    '## Máquina de escribir',
    'Si activa la interacción "Máquina de escribir" en Ajustes, el texto se escribe letra a letra sobre una hoja de papel (toque la hoja o pulse Esc para verlo completo). Pulse Ctrl+L en la línea de órdenes para arrancar un papiro nuevo, como en el original.',
    '## El comercial de siempre',
    'La Fortaleza es un juego abierto. Por esto, todas las sugerencias son bienvenidas. Es probable que usted desee crear su propio juego, con características similares a las de la Fortaleza. Puede contactar al autor: Miguel Enrique Cepero, Instituto de Informática, Universidad Central de Las Villas.',
    'La Fortaleza es un producto de Merchise Software, que no se dedica a hacer jueguitos solamente.'
  ],
  en: [
    'La Fortaleza is a game made to the measure of your intelligence (this is not a TV spot on TV). The smarter you are, the easier it is supposed to be to master the goals of the game. However, there is another quality that can make up for a lack of intelligence: method (unfortunately, real life works the same way). Both fortresses are designed so that no detail escapes the methodical eye.',
    'Method comes from practice itself, but practice can be unpleasant until you have a method worked out. It is a vicious circle. Since La Fortaleza is a game and people are supposed to enjoy themselves, I have decided to offer something equivalent to a method for beginners. It looks a lot like a cooking recipe, or an engineering textbook. In truth, it is nothing more than a few pieces of advice.',
    '## Great Infallible Method for Solving Fortresses',
    '- Rule 1: Always keep a pencil and paper at hand.',
    '- Rule 2: Make sure the pencil is sharp at all times.',
    '- Rule 3: Keep a diagram of the different places you visit and the relations between them (doors, etc). If you do not, you may get so lost in La Fortaleza that you cannot find a way to get up from the chair and switch off the machine.',
    '- Rule 4: Try to observe everything. Do so in this order of priority: 1) inanimate things, 2) living things, 3) doors and links in general.',
    '- Rule 5: Some basics on telling apart the things you may stumble upon inside La Fortaleza. These methods always work, with a few exceptions.',
    '- (1) Inanimate things: they can be taken, unless they weigh too much. Then the message "That would be too heavy." is shown. Another way, which never fails, is to ask them questions or try to kill them.',
    '- (2) Things that hide other things: like the above, but they cannot be taken. You must break them with some specific object (which you must discover) to reach the object they hide.',
    '- (3) Living things fall into two large groups: those on your side and those against you. The former you must question (among other things) to find out how they can help you. The help always comes down to handing them a certain object; the problem is discovering what they need. After handing it over, question them again and you will obtain some valuable information. The members of the second group you must kill. Show no mercy. Usually, when they are dying they confess something interesting. Most of the time you must discover the weapon to use, since not just any object will do. When questioned, they never answer.',
    '- (4) Links (doors, bridges, paths, etc): if you cannot recognize them by name, use the "quit" command and never touch La Fortaleza again.',
    '- Rule 6: Links can be open or closed. To open them use the "open" command. Some need magic words to open, and at other times you will be forced to solve riddles.',
    '- Rule 7: Some links can only be crossed by meeting a requirement. Sometimes you cannot cross without carrying a certain object, and at other times an object becomes indispensable to get through the link. Which object to use and which not is something you must infer from your observations and from what you have been told.',
    '- Rule 8: Try to gather the things you find in central places. That way, if you need an object you are not carrying, you will not have to travel so far.',
    '## Vocabulary',
    'The articles "the" and "an", and the prepositions "by", "into" and "through" are always ignored.',
    '- To move: go, walk, traverse, cross, enter. Then the name of the link. If the door needs no key word, it opens without you having to order it explicitly. Examples: go door / cross the door',
    '- To look at the place: look, examine.',
    '- To look at a thing: look, see, read. Then the name of the object. Examples: look thing / read the letter',
    '- To break: break, force, smash. Then the object you want to break, then the word "with" and finally the object used to break the other. Examples: break the bust with the mace / break glass with stone',
    '- To pick something up: take, get, pick. Examples: take thing / get thing',
    '- To drop: drop, leave, put. If you say "all", you can imagine what happens. Examples: drop thing / drop all',
    '- To question: ask, talk, question. Then the name of the individual. Examples: ask the mute / question the deaf man',
    '- To open: open. Then the link you want to open. For magic words add "saying" and then the words. To answer a riddle use "answering" before the answer. Actually, "saying" is equivalent to "answering" and both can be replaced by "with". Examples: open some door saying abracadabra / open some door with the id number',
    '- To kill: kill, attack, slay. The syntax is similar to break. The murder weapon may be omitted. Examples: kill red riding hood / kill the wolf with the shotgun',
    '- To give: give. Then the object to give and then to whom, separated by "to". The object must be among your belongings. Examples: give poison to the granny / give wine bottle to the drunk',
    '- To list your belongings: inventory, items, bag, i. Next to each object appears how much it weighs, expressed in "sandbags from the Río Negro". For an exact idea, one "sandbag" equals one eighth of a cold-light tube filled with water.',
    '- To weigh an object: weigh, then the name of the object. Naturally, you need a scale.',
    '- To get an idea of how much is left: percent. It tells you what percentage of La Fortaleza you have covered so far (rooms visited divided by total rooms, times one hundred). This percentage is not an exact measure of how close you are to the solution: you may have covered all of La Fortaleza and still have no idea what to do to win. It happens.',
    '- To finish: quit, exit. This command ends up causing your death.',
    '## Tracks',
    'From the menu (⋯) you can save a track, which stores every command you have typed that affects the state of La Fortaleza (looking, seeing, inventory, asking, etc. do not count), and load it again at any time. Since it is a text file, it can be edited; Pascal-style comments "{ }" are allowed. Tracks are one of the most useful tools when solving a Fortress.',
    '## Typewrite',
    'If you turn on the "Typewrite" interaction in Settings, text is typed out letter by letter on a sheet of paper (tap the sheet or press Esc to see it all at once). Press Ctrl+L in the command line to tear off a fresh sheet, as in the original.',
    '## The usual commercial',
    'La Fortaleza is an open game. Therefore, all suggestions are welcome. You may well want to create your own game with features similar to those of La Fortaleza. You can contact the author: Miguel Enrique Cepero, Instituto de Informática, Universidad Central de Las Villas.',
    'La Fortaleza is a product of Merchise Software, which does not make little games only.'
  ]
}

export const content = {
  introduction: { 1: intro1, 2: intro2 },
  advices: advice
}
