using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Games;

public static class Fort2Builder
{
    private static readonly (LocalizedText Name, LocalizedText Description)[] RoomData =
    [
        (new("una habitación para huéspedes", "a guest room"),
            new("Está tan oscura que usted cree que las paredes y el techo se han avalanzado sobre su cuerpo. En la pared que tiene delante se distingue un plateado hilo de luz. Es el filo de una puerta.", "It is so dark that you think the walls and ceiling have fallen on your body. On the wall in front of you a silvery thread of light can be made out. It is the edge of a door.")),
        (new("el pasillo", "the corridor"),
            new("Hay seis puertas distribuidas simétricamente. Las paredes son de mármol blanco, pero usted tiene la certeza de que hay algo terrible encerrado detrás de toda esa pulcritud.", "There are six doors distributed symmetrically. The walls are of white marble, but you are certain that something terrible is locked away behind all that tidiness.")),
        (new("el Salón de Fumar", "the Smoking Room"),
            new("Hay sesenta y seis sillas en el salón. Al lado de cada silla crece un tentáculo rosado con un pitillo en el extremo. Los pitillos humean.", "There are sixty-six chairs in the hall. Next to each chair grows a pink tentacle with a cigarette at the end. The cigarettes smoke.")),
        (new("el cuarto de la hija del Hechicero", "the Sorcerer's daughter's room"),
            new("Se pregunta cómo luciría la hija de un hechicero. En las paredes hay cientos de retratos, todos de muchachas diferentes. ¿Cuál será la real?... ¿O todas son una misma?", "You wonder what a sorcerer's daughter would look like. On the walls there are hundreds of portraits, all of different girls. Which one will be the real one?... Or are they all the same?")),
        (new("la casa de muñecas", "the dollhouse"),
            new("Es tan pequeña como cualquier casa de muñecas. Usted no comprende cómo pudo entrar. Hay un baúl en el fondo.", "It is as small as any dollhouse. You do not understand how you were able to get in. There is a trunk at the back.")),
        (new("el cuarto del Leñador", "the Woodcutter's room"),
            new("Las paredes, el suelo y el techo están hechos de madera. Usted camina con cuidado: hay espinas por todas partes.", "The walls, floor and ceiling are made of wood. You walk carefully: there are thorns everywhere.")),
        (new("la Cocina", "the Kitchen"),
            new("En realidad, se parece mucho más a un matadero. El olor a sangre coagulada y carroña no le permite apartar el pañuelo de la nariz. Hay unas marcas de sangre en la pared que le llaman mucho la atención: fueron hechas por una mano de siete dedos.", "It actually looks much more like a slaughterhouse. The smell of clotted blood and carrion does not let you take the handkerchief away from your nose. There are some bloodstains on the wall that catch your attention: they were made by a hand with seven fingers.")),
        (new("las Terrazas", "the Terraces"),
            new("No puede reprimir su asombro: las terrazas están invertidas. Usted se siente muy natural parado cabeza abajo. Se escucha el correr de un río.", "You cannot suppress your astonishment: the terraces are inverted. You feel quite natural standing upside down. You hear the running of a river.")),
        (new("el calabozo del Monstruo", "the Monster's dungeon"),
            new("Las paredes son de barro. Hay cientos de cadenas embotadas en el techo, moviéndose constantemente, como si las arrastrara un viento de acero.", "The walls are made of mud. There are hundreds of blunted chains on the ceiling, moving constantly, as if dragged by a wind of steel.")),
        (new("el interior del baúl", "the inside of the trunk"),
            new("Ahora sí está confundido. El baúl es inmensamente mayor que la casa de muñecas, a pesar de que está dentro de ella. Luce como una habitación cualquiera.", "Now you are really confused. The trunk is immensely bigger than the dollhouse, even though it is inside it. It looks like any ordinary room.")),
        (new("una orilla del Río Negro", "a bank of the Black River"),
            new("Las aguas del Río Negro hacen honor a su nombre. Más que aguas parecen una mezcla de muerte y olvido. ", "The waters of the Black River live up to their name. More than waters they look like a mixture of death and oblivion. ")),
        (new("una orillas del Río Negro", "a bank of the Black River"),
            new("La espesa niebla casi no deja ver las terrazas al otro lado.", "The thick fog hardly lets you see the terraces on the other side.")),
        (new("los Jardines del Hechicero", "the Sorcerer's Gardens"),
            new("Son tan extensos que usted no puede ver dónde terminan. Todos los árboles tienen el mismo tamaño y la misma forma. El jardín parece un gran juego de espejos.", "They are so vast that you cannot see where they end. All the trees have the same size and the same shape. The garden looks like a great game of mirrors.")),
        (new("el Comedor", "the Dining room"),
            new("La mesa es tan alta que usted no puede ver lo que hay servido. Alguien se lamenta allá arriba.", "The table is so high that you cannot see what is served on it. Someone is lamenting up there.")),
        (new("la Sastrería", "the Tailor's shop"),
            new("Parece la casa de una araña gigantesca. Hay hilos por todas partes.", "It looks like the house of a gigantic spider. There are threads everywhere.")),
        (new("el cuarto del jardinero", "the gardener's room"),
            new("Es una habitación muy modesta. A usted le sorprende que los instrumentos no sean de jardinería, sino de fabricar espejos.", "It is a very modest room. You are surprised that the tools are not for gardening, but for making mirrors.")),
        (new("la habitación de los tesoros", "the treasure room"),
            new("Nunca había visto tanta riqueza. Las paredes están forradas de láminas de platino y hasta el fuego de las antorchas es de oro. Usted no comprende de dónde viene la luz.", "You had never seen such wealth. The walls are lined with sheets of platinum and even the fire of the torches is gold. You do not understand where the light comes from.")),
        (new("el salón de las velas", "the hall of candles"),
            new("Deben ser varios kilómetros sembrados de velas. En el centro hay un círculo vacío.", "There must be several kilometers sown with candles. In the center there is an empty circle.")),
        (new("el cuarto del Inmortal", "the Immortal's room"),
            new("Esta habitación luce más vieja que el resto de la Fortaleza. Hay muchas cosas colgadas de la pared: retratos, cartas, reconocimientos, estandartes y muchas telas de arañas.", "This room looks older than the rest of La Fortaleza. There are many things hanging on the wall: portraits, letters, certificates, banners and many spider webs.")),
        (new("el cuarto de juguetes", "the toy room"),
            new("Hay miles de juguetes regados por todas partes. Usted siente deseos de volver a ser un niño.", "There are thousands of toys scattered everywhere. You feel the desire to be a child again.")),
        (new("las Ruinas", "the Ruins"),
            new("Este lugar debe haber sido abandonado hace siglos. Usted trata de imaginar cómo eran las construcciones originalmente, pero no lo consigue. Al sur se extiende un desierto y al oeste se encuentra la casa del Ogro.", "This place must have been abandoned centuries ago. You try to imagine what the buildings were like originally, but you cannot. To the south a desert stretches out and to the west lies the Ogre's house.")),
        (new("el Salón de Recepciones", "the Reception Hall"),
            new("Todo está muy sucio y descuidado. Un fuerte olor a carroña le hace perder el equilibrio por momentos. Seguramente el Ogro usa este lugar para recibir a sus enemigos.", "Everything is very dirty and neglected. A strong smell of carrion makes you lose your balance at times. The Ogre surely uses this place to receive his enemies.")),
        (new("el cuarto de un pintor", "a painter's room"),
            new("Hay muchos cuadros en las paredes, pero ninguno está terminado.", "There are many paintings on the walls, but none is finished.")),
        (new("el Patio Interior", "the Inner Courtyard"),
            new("Por el patio están diseminados varios árboles gigantescos, todos sin hojas. Parece como si el invierno reinase siempre en este lugar.", "Several gigantic trees, all leafless, are scattered around the courtyard. It looks as if winter always reigned in this place.")),
        (new("las habitaciones del Juglar", "the Minstrel's rooms"),
            new("El ambiente de la habitación le induce a creer que el juglar no era exactamente lo que se llama una persona alegre.", "The atmosphere of the room leads you to believe that the minstrel was not exactly what you would call a cheerful person.")),
        (new("la alcoba del Ogro", "the Ogre's bedroom"),
            new("El aire se ha hecho casi irrespirable. Ahora usted comprende de dónde venía el mal olor reinante en el Salón de Recepciones. Nunca antes había estado en un lugar tan repulsivo.", "The air has become almost unbreathable. Now you understand where the foul smell in the Reception Hall came from. You had never been in such a repulsive place.")),
        (new("los Subterráneos", "the Underground passages"),
            new("Deben ser contemporáneos de las Ruinas. Usted siente como la humedad lo invade y le perfora los huesos.", "They must be contemporary with the Ruins. You feel the dampness invade you and pierce your bones.")),
        (new("las Catacumbas", "the Catacombs"),
            new("Nunca antes había visto tantos esqueletos juntos. Más que enterrados en ese lugar, parecían haber muerto ahí. En el fondo usted divisa un hueco sin cadáver con una inscripción. La inscripción dice: \"Este es para tí\".", "You had never seen so many skeletons together. Rather than buried in that place, they seemed to have died there. At the far end you make out an empty hole, without a corpse, with an inscription. The inscription says: \"This one is for you\".")),
        (new("el punto donde comienzan unas enormes cataratas", "the point where an enormous waterfall begins"),
            new("Un repentino acceso de vértigo hace que sus rodillas se doblen. A menos de un pie de donde usted se encuentra el agua se precipita hacia el abismo.", "A sudden fit of vertigo makes your knees buckle. Less than a foot from where you stand the water plunges into the abyss.")),
        (new("los bajos de las cataratas", "the foot of the waterfall"),
            new("Hay un ruido ensordecedor, como si millones de tambores redoblaran al lado de sus tímpanos. Usted siente como si su cabeza fuese a explotar.", "There is a deafening noise, as if millions of drums were beating next to your eardrums. You feel as if your head were going to explode.")),
        (new("fondo del Lago", "the bottom of the Lake"),
            new("Contrariamente a lo que le dijeron en su infancia, usted puede respirar debajo del agua. Usted siente una paz extraña al estar en el fondo del Lago, tal vez debida a posibles asociaciones subconcientes con su pasado embrionario, en el vientre de su madre.", "Contrary to what you were told in your childhood, you can breathe underwater. You feel a strange peace at the bottom of the Lake, perhaps due to possible subconscious associations with your embryonic past, in your mother's womb.")),
        (new("la Cueva de Cristal", "the Crystal Cave"),
            new("Hay resplandores de cristales por todas partes. En este lugar el agua se ha hecho más densa y le resulta difícil respirar.", "There are gleams of crystals everywhere. In this place the water has become denser and it is hard for you to breathe.")),
        (new("la orilla del Lago", "the shore of the Lake"),
            new("Frente a usted se extiende el Lago, de aguas claras y tranquilas.", "Before you stretches the Lake, with clear, calm waters.")),
        (new("un Oasis", "an Oasis"),
            new("Es un oasis tradicional, con varias palmeras de dátiles y un pequeño estanque de agua turbia en el centro.", "It is a traditional oasis, with several date palms and a small pond of murky water in the center.")),
        (new("la Catedral", "the Cathedral"),
            new("Es un lugar impresionante. Las paredes crecen hasta una altura increíble y se unen en una serie de simétricas torres. Usted se siente oprimido por la arquitectura, como si en realidad estuviese ante los ojos de Dios, en el día del juicio final.", "It is an impressive place. The walls rise to an incredible height and join in a series of symmetrical towers. You feel oppressed by the architecture, as if you were really standing before the eyes of God on the day of the last judgment.")),
        (new("el Desierto otra vez", "the Desert again"),
            new("El calor lo asfixia. Usted no comprende cómo es posible que el clima varíe tanto de un lugar a otro de la Fortaleza.", "The heat suffocates you. You cannot understand how the climate can vary so much from one place to another in La Fortaleza.")),
        (new("un extenso valle", "a vast valley"),
            new("Siente deseos de construir una casa y quedarse a vivir en el valle. Por todos lados crece una especie de árbol frutal que, valiéndose de dos manos que salen de su tronco, es capaz de cultivarse ella misma.", "You feel like building a house and staying to live in the valley. Everywhere grows a kind of fruit tree that, using two hands that come out of its trunk, is able to cultivate itself.")),
        (new("la cima del Pico Negro", "the summit of the Black Peak"),
            new("Nunca antes había estado a tanta altura. Usted se pregunta por qué la montaña es llamada \"El Pico Negro\" si en realidad está cubierta de nieve.", "You had never been at such a height. You wonder why the mountain is called \"The Black Peak\" when it is actually covered in snow.")),
        (new("la choza de un monje", "a monk's hut"),
            new("El suelo está muy limpio, al igual que los muebles y las ventanas. Hay una gran cantidad de cuentas regadas por el suelo.", "The floor is very clean, as are the furniture and the windows. There are a great many beads scattered over the floor.")),
        (new("la Ciudad Abandonada", "the Abandoned City"),
            new("A pesar de ser más antigua que las ruinas, la ciudad se conserva intacta. Usted oye el ruido de la gente en las calles y siente los olores de los alimentos en las cocinas, pero no son más que ilusiones. La ciudad está abandonada desde hace siglos.", "Although it is older than the ruins, the city is perfectly preserved. You hear the noise of people in the streets and smell the food in the kitchens, but they are nothing more than illusions. The city has been abandoned for centuries.")),
        (new("la choza del Sabio", "the Sage's hut"),
            new("El lugar se ajusta perfectamente a su concepto de desorden. Usted se pregunta cómo el sabio puede vivir en semejante caos. Hay varios esqueletos, al parecer, una colección incipiente.", "The place fits perfectly your concept of disorder. You wonder how the sage can live in such chaos. There are several skeletons, apparently an incipient collection.")),
        (new("un campo cultivado", "a cultivated field"),
            new("Todos los sembrados parecen bastante descuidados. Probablemente el labrador no sea capaz de atender una cantidad tan grande de tierra al mismo tiempo.", "All the crops look rather neglected. The farmer is probably not able to tend such a large amount of land at the same time.")),
        (new("el exterior de la Torre de Cristal", "the exterior of the Crystal Tower"),
            new("La Torre de Cristal crece vertiginosamente hasta perderse entre las nubes.", "The Crystal Tower rises dizzyingly until it is lost among the clouds.")),
        (new("la punta de la Torre de Cristal", "the top of the Crystal Tower"),
            new("Probablemente se encuentre a varios kilómetros de altura. Usted se alegra de que no hayan ventanas en el lugar.", "It is probably several kilometers high. You are glad there are no windows in the place.")),
        (new("la alcoba de la hija del Hechicero", "the Sorcerer's daughter's bedroom"),
            new("Esta es la alcoba donde la hija del Hechicero se alojaba temporalmente. Es muy cómoda y tiene muchos cuadros iguales al que usted lleva.", "This is the bedroom where the Sorcerer's daughter stayed temporarily. It is very comfortable and has many paintings identical to the one you are carrying.")),
        (new("la Alcoba Secreta", "the Secret Bedroom"),
            new("Es mucho mayor que la otra alcoba. Es casi seguro que la hija del Hechicero viva en este lugar permanentemente.", "It is much bigger than the other bedroom. It is almost certain that the Sorcerer's daughter lives in this place permanently.")),
        (new("los Calabozos", "the Dungeons"),
            new("Los calabozos están formados por varios pasillos paralelos, llenos de celdas a ambos lados.", "The dungeons are made up of several parallel corridors, full of cells on both sides.")),
        (new("una celda", "a cell"),
            new("Debe ser un verdadero suplicio el estar encerrado aquí. Todo el suelo está cubierto de afiladas hojas, excepto en una esquina, donde se supone que esté el prisionero.", "It must be a real torment to be locked up here. The whole floor is covered with sharp blades, except in one corner, where the prisoner is supposed to be.")),
        (new("la Antesala de la Prueba", "the Antechamber of the Trial"),
            new("Es un salón circular. La luz del sol entra por unos vitrales rojos y el lugar luce como pintado con sangre.", "It is a circular hall. Sunlight enters through some red stained-glass windows and the place looks as if it were painted with blood.")),
        (new("el Salón Verde", "the Green Hall"),
            new("Es un salón de pruebas tradicional. Este tipo de salón fue usado ampliamente por los horantes para educar a sus hijos.", "It is a traditional trial hall. This type of hall was widely used by the horantes to educate their children.")),
        (new("el Salón Azul", "the Blue Hall"),
            new("Es un salón de pruebas tradicional. Este tipo de salón fue usado ampliamente por los horantes para educar a sus hijos.", "It is a traditional trial hall. This type of hall was widely used by the horantes to educate their children.")),
        (new("el Salón Blanco", "the White Hall"),
            new("Es un salón de pruebas tradicional. Este tipo de salón fue usado ampliamente por los horantes para educar a sus hijos. Es el último de la prueba.", "It is a traditional trial hall. This type of hall was widely used by the horantes to educate their children. It is the last one of the trial.")),
        (new("los límites de la Fortaleza", "the limits of La Fortaleza"),
            new("A su derecha crece la tercera muralla, hasta una altura irracional. Usted no puede creer que aquí termine la Fortaleza. Del otro lado de la muralla se escucha (usted no entiende cómo) el canto de los pájaros y el sonido del viento en los árboles.", "On your right the third wall rises to an irrational height. You cannot believe La Fortaleza ends here. From the other side of the wall you hear (you do not understand how) the singing of birds and the sound of the wind in the trees.")),
        (new("la Pirámide del Hechicero", "the Sorcerer's Pyramid"),
            new("Es una construcción gigantesca, a base de piedras. Al parecer, las piedras fueron colocadas sin orden alguno, olvidando las ventanas. El lugar está iluminado con numerosas antorchas.", "It is a gigantic construction made of stones. Apparently, the stones were placed in no order at all, forgetting the windows. The place is lit by numerous torches.")),
        (new("las Habitaciones del Hechicero", "the Sorcerer's Rooms"),
            new("Las habitaciones del Hechicero están dentro de una pirámide interior. Todo es igual a la gran pirámide, pero en una escala menor. Hay miles de espadas colgadas de la pared.", "The Sorcerer's rooms are inside an inner pyramid. Everything is the same as the great pyramid, but on a smaller scale. There are thousands of swords hanging on the wall.")),
    ];

    private static int RoomDataIndex(int room) => room - 1;

    public static Fort2 Build()
    {
        Player? player = null;
        Action kill = () => player!.Die("");
        var r = new Room[56];
        for (var i = 1; i <= 55; i++)
        {
            var data = RoomData[RoomDataIndex(i)];
            r[i] = new Room(data.Name, data.Description, Inventory.LWeight + 1);
        }

        r[1].Children.Add(new Link(
                new("Puerta", "Door"),
                new("En realidad, lo que usted puede distinguir es el delicado filón de luz que deja escapar la unión de la puerta y el marco.", "What you can actually make out is the delicate sliver of light that escapes from the joint between the door and the frame."),
                new("Usted cierra los ojos, cegado por la luz.", "You close your eyes, blinded by the light."),
                "",
                r[2]));

        r[2].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Es de madera trabajada, una verdadera obra de arte.", "It is made of carved wood, a true work of art."),
                new("Usted entra a la oscuridad nuevamente.", "You enter the darkness again."),
                "",
                r[1]));
        r[2].Children.Add(new Link(
                new("Puerta de espinos", "Thorn door"),
                new("Está erizada de espinas. Usted tiene miedo hasta de acercarse.", "It is bristling with thorns. You are afraid even to approach it."),
                new("Usted entra lentamente, tratando de no rozar la puerta.", "You enter slowly, trying not to brush against the door."),
                "",
                r[6]));
        r[2].Children.Add(new DangerLink(
                new("Puerta de metal", "Metal door"),
                new("Es de acero bruñido. Tiene encima un cartel que dice \"Cocina. Cúbrase la nariz.\"", "It is made of burnished steel. On top there is a sign that says \"Kitchen. Cover your nose.\""),
                new("Al pasar siente un olor muy fuerte, que lo envuelve y lo asfixia. Usted sobrevive gracias al pañuelo que lleva consigo.", "As you pass you smell a very strong odor that envelops and chokes you. You survive thanks to the handkerchief you are carrying."),
                "",
                "Pañuelo",
                r[7],
                kill));
        r[2].Children.Add(new Link(
                new("Puerta Negra", "Black door"),
                new("Usted ve unas delgadas cortinas de humo saliendo por debajo de la puerta.", "You see thin curtains of smoke coming out from under the door."),
                new("Usted tose fuertemente, ha entrado al Salón de Fumar.", "You cough violently, you have entered the Smoking Room."),
                "",
                r[3]));
        r[2].Children.Add(new RiddleLink(
                new("Puerta Azul", "Blue door"),
                new("Es más pequeña que las puertas restantes.", "It is smaller than the remaining doors."),
                new("¿Cuál es el nombre de la hija del Hechicero?", "What is the name of the Sorcerer's daughter?"),
                "Aura",
                new("Usted entra a la alcoba de la hija del Hechicero.", "You enter the Sorcerer's daughter's bedroom."),
                r[4]));
        r[2].Children.Add(new Link(
                new("Puerta de oro", "Gold door"),
                new("Usted nunca había visto tanto oro fundido en una misma pieza.", "You had never seen so much molten gold in a single piece."),
                new("El resplandor del oro lo ciega por un momento. Usted ha salido a las terrazas.", "The glare of the gold blinds you for a moment. You have come out onto the terraces."),
                "",
                r[8]));
        r[2].Children.Add(new Guard(
                new("Grifo", "Griffin"),
                new("Usted no logra distinguir si es un león o un águila. Parece dormido.", "You cannot tell whether it is a lion or an eagle. It seems to be asleep."),
                new("El grifo se contorsiona tratando de arrancarse la daga y grita: \"¡Aura!\".", "The griffin writhes, trying to pull out the dagger, and shouts: \"Aura!\"."),
                new("Daga", "Dagger")));

        r[3].Children.Add(new Link(
                new("Puerta Negra", "Black door"),
                new("La puerta por este lado está muy manchada por el humo.", "The door on this side is heavily stained with smoke."),
                new("Usted sale del Salón de Fumar. A fin puede respirar normalmente.", "You leave the Smoking Room. At last you can breathe normally."),
                "",
                r[2]));
        r[3].Children.Add(new Item(
                new("Balanza", "Scale"),
                new("La balanza tiene el plato en forma de mano. La aguja es muy delgada, hecha con varios bigotes de gato trenzados. ", "The scale's pan is shaped like a hand. The needle is very thin, made of several braided cat whiskers. "),
                1));
        r[3].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("La inscripción dice:\nLa muerte es un acto de la vida. Yo, la Bestia, he de morir como he vivido. Esta es mi venganza, y tú mismo serás el instrumento que la lleve a cabo. La naturaleza humana es el arma más poderosa que puede utilizarse contra el hombre.", "The inscription says:\\nDeath is an act of life. I, the Beast, must die as I have lived. This is my revenge, and you yourself will be the instrument that carries it out. Human nature is the most powerful weapon that can be used against man."),
                1));
        r[3].Children.Add(new Item(
                new("Piedra de Roseta", "Rosetta Stone"),
                new("Esta no es la piedra original. Tiene grabadas las siguientes palabras:\n\n\"Fuimos grandes una vez,   │   Fio rne n e,atsd absi\n antes de la Bestia        │   Umsgadsuavz ne el eta\"\n\nOjalá Champollión estuviera aquí.", "This is not the original stone. It has the following words engraved:\n\n\"We were great once,       │   W eegetoc,bfr h es\n before the Beast          │   Ewr ra ne eoeteBat\"\n\nIf only Champollion were here."),
                5));

        r[4].Children.Add(new Link(
                new("Puerta Azul", "Blue door"),
                new("Conduce al pasillo.", "It leads to the corridor."),
                new("Usted sale al pasillo.", "You go out into the corridor."),
                "",
                r[2]));
        r[4].Children.Add(new Item(
                new("Pañuelo", "Handkerchief"),
                new("Tiene bordadas una A y una S.", "It has an A and an S embroidered on it."),
                1));
        r[4].Children.Add(new Item(
                new("Carta", "Letter"),
                new("La carta dice:\n\"¡Dios te salve, mi única esperanza! \nMi padre me ha encerrado en la Torre de Cristal, que está más allá del segundo río. El sabe que solo yo podría mostrarte el camino de salida. Búscame.\"", "The letter says:\\n\"God save you, my only hope! \\nMy father has locked me in the Crystal Tower, which is beyond the second river. He knows that only I could show you the way out. Look for me.\""),
                1));
        r[4].Children.Add(new OpenLink(
                new("Agujero", "Hole"),
                new("Parece una guarida de ratones.", "It looks like a mouse den."),
                new("Usted se agacha y (con grandes dolores en todos los huesos) atraviesa el agujero.", "You stoop down and (with great pain in all your bones) go through the hole."),
                r[5]));

        r[5].Children.Add(new OpenLink(
                new("Agujero", "Hole"),
                new("Es la salida de la casa de muñecas.", "It is the exit from the dollhouse."),
                new("Usted regresa a la alcoba.", "You return to the bedroom."),
                r[4]));
        r[5].Children.Add(new Hidden(
                new("Ventana", "Window"),
                new("Es de un cristal muy grueso. Usted trata de ver al otro lado, pero la ventana es tan opaca que solo puede divisar algunas sombras moviéndose.", "It is made of very thick crystal. You try to see the other side, but the window is so opaque that you can only make out some shadows moving."),
                new("Corta-cristales", "Glass cutter"),
                new OpenLink(
                    new("Pasadizo", "Passage"),
                    new("¿A dónde conducirá?", "Where will it lead?"),
                    new("Usted oye una voz que le dice: \"Usa tu equilibrio para vencer la corriente\". Al cabo de varios minutos encuentra la salida.", "You hear a voice say to you: \"Use your balance to overcome the current\". After several minutes you find the exit."),
                    r[13])));
        r[5].Children.Add(new Link(
                new("Puerta del baúl", "Trunk door"),
                new("Parece muy antigua. Usted está seguro de que ahí hay \"gato encerrado\".", "It looks very old. You are sure there is a \"cat locked up\" there."),
                new("¡Usted ha caído dentro del baúl!", "You have fallen inside the trunk!"),
                "MIAU",
                r[10]) { AltKey = "MEOW" });

        r[6].Children.Add(new Link(
                new("Puerta de espinos", "Thorn door"),
                new("Al igual que del otro lado, está erizada de espinas.", "Like on the other side, it is bristling with thorns."),
                new("Usted sale lentamente, tratando de no rozar la puerta.", "You leave slowly, trying not to brush against the door."),
                "",
                r[2]));
        r[6].Children.Add(new Item(
                new("Daga", "Dagger"),
                new("La empuñadura tiene forma de dragón.", "The hilt is shaped like a dragon."),
                2));
        r[6].Children.Add(new Item(
                new("Botella de vino", "Bottle of wine"),
                new("Parece una botella de agua sucia.", "It looks like a bottle of dirty water."),
                3));
        r[6].Children.Add(new Item(
                new("Hacha", "Axe"),
                new("Está muy mohosa.", "It is very moldy."),
                17));

        r[7].Children.Add(new DangerLink(
                new("Puerta de metal", "Metal door"),
                new("Conduce al pasillo.", "It leads to the corridor."),
                new("Al salir puede apartar el pañuelo de la nariz.", "On leaving you can take the handkerchief away from your nose."),
                "",
                "Pañuelo",
                r[2],
                kill));
        r[7].Children.Add(new Item(
                new("Receta", "Recipe"),
                new("Es una receta de cocina. Dice:\n\n  5 Ojos de rana\n  1 Mano de gigante\n  2 Cucharadas de puré de lombrices\n  1 Hígado de muchacha virgen\n  9 Libras de azúcar\n  1 Tanque de agua del pantano verde\n\nUsted recuerda haber probado esto en alguna parte.", "It is a cooking recipe. It says:\\n\\n  5 Frog eyes\\n  1 Giant's hand\\n  2 Spoonfuls of worm puree\\n  1 Virgin girl's liver\\n  9 Pounds of sugar\\n  1 Tank of green swamp water\\n\\nYou remember having tasted this somewhere."),
                1));
        r[7].Children.Add(new Item(
                new("Muslo de carnero", "Leg of lamb"),
                new("Más bien parece el muslo de una persona.", "It looks more like a person's thigh."),
                10));
        r[7].Children.Add(new Item(
                new("Pescado", "Fish"),
                new("¿Qué tiempo llevará en este lugar?", "How long will it have been in this place?"),
                3));

        r[8].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("La inscripción dice:\n\"No todos los dragones merecen la muerte.\"", "The inscription says:\\n\"Not all dragons deserve death.\""),
                1));
        r[8].Children.Add(new Link(
                new("Puerta de oro", "Gold door"),
                new("Usted nunca había visto tanto oro fundido en una misma pieza.", "You had never seen so much molten gold in a single piece."),
                new("Usted regresa al pasillo. El resplandor del oro lo ciega por un momento.", "You return to the corridor. The glare of the gold blinds you for a moment."),
                "",
                r[2]));
        r[8].Children.Add(new OpenLink(
                new("Escalera de caracol", "Spiral staircase"),
                new("Parece la bajada a un oído gigante.", "It looks like the descent into a giant ear."),
                new("El viento helado que viene del fondo de la escalera lo envuelve en continuos temblores.", "The icy wind coming from the bottom of the stairs wraps you in continuous shivers."),
                r[11]));
        r[8].Children.Add(new Item(
                new("Reloj de arena", "Hourglass"),
                new("Usted escucha un callado tic-tac.", "You hear a muffled tick-tock."),
                2));
        r[8].Children.Add(new Item(
                new("Silla", "Chair"),
                new("Es una silla para tomar el sol. Usted tiene la certeza de que había alguien sentado en ella cuando entró a las terrazas.", "It is a sun lounger. You are certain that someone was sitting on it when you entered the terraces."),
                5));

        r[9].Children.Add(new OpenLink(
                new("Escalera de caracol", "Spiral staircase"),
                new("Conduce a las terrazas", "It leads to the terraces"),
                new("Su condicionamiento euclidiano le impide comprender cómo es posible que una misma escalera sea origen y destino de tres lugares distintos a la vez.\nUsted ha subido a las terrazas.", "Your Euclidean conditioning keeps you from understanding how the same staircase can be the origin and destination of three different places at once.\\nYou have climbed up to the terraces."),
                r[8]));
        r[9].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("La inscripción dice:\n\"No alimente al monstruo.\"", "The inscription says:\\n\"Do not feed the monster.\""),
                1));
        r[9].Children.Add(new Troll(
                new("Monstruo", "Monster"),
                new("El Monstruo está encadenado a la pared. Usted aparta la vista rápidamente. Nunca había visto tanta fealdad concentrada en una criatura.", "The Monster is chained to the wall. You quickly look away. You had never seen so much ugliness concentrated in one creature."),
                "Muslo de carnero",
                new("¡Tú cuidarte de Hechicero! ¡Hechicero ser malo! ¡Malo! ¡Muy malo!\nHechicero puso yo feo. A tú no poner feo... ¡A tú arrancal tripa, sacal ojo, rompel hueso y echal dragrón hambroso!\nYo consejo tú: Busca espada Marmidosa... ¡y pincha todo!", "You beware of Sorcerer! Sorcerer be bad! Bad! Very bad!\\nSorcerer made me ugly. To you not make ugly... To you pull out gut, take out eye, break bone and throw hungry dragon!\\nMe advice you: Look for sword Marmidosa... and stab everything!"),
                new("¡Grrrr! ¡Hambre!", "Grrrr! Hungry!")));

        r[10].Children.Add(new Link(
                new("Puerta del baúl", "Trunk door"),
                new("Está en el techo.", "It is on the ceiling."),
                new("Usted sale del baúl y vuelve a sentir la estrechez de la casa de muñecas.", "You leave the trunk and feel again the tightness of the dollhouse."),
                "",
                r[5]));
        r[10].Children.Add(new Item(
                new("Piedra verde", "Green stone"),
                new("Es increíblemente sólida.", "It is incredibly solid."),
                9));
        r[10].Children.Add(new Troll(
                new("Gato", "Cat"),
                new("Está concentrado en sacarle las últimas partículas de carne a una cabeza de pescado.", "He is concentrating on getting the last scraps of meat off a fish head."),
                "Pescado",
                new("La piedra verde te servirá para romper la segunda muralla.", "The green stone will serve you to break the second wall."),
                new("¡No molestes!", "Do not disturb!")));

        r[11].Children.Add(new OpenLink(
                new("Escalera de caracol", "Spiral staircase"),
                new("Contrariamente a lo que usted espera, la escalera en este lugar asciende hasta el calabozo del Monstruo.", "Contrary to what you expect, the staircase in this place climbs up to the Monster's dungeon."),
                new("Usted sube durante unos minutos y llega al calabozo del Monstruo.", "You climb for a few minutes and reach the Monster's dungeon."),
                r[9]));
        r[11].Children.Add(new Troll(
                new("Caronte", "Charon"),
                new("Una capucha negra esconde su rostro. Por una de las mangas de su vestimenta asoma una mano descarnada casi hasta el hueso. Usted se alegra de no poder ver su cara.", "A black hood hides his face. From one of the sleeves of his garment peeks a hand fleshed down almost to the bone. You are glad you cannot see his face."),
                "Reloj de arena",
                new("Dentro del blanco está la llave de la muralla.", "Inside the white one is the key to the wall."),
                new("Tráeme el reloj de arena, pero víralo antes de entregármelo. ", "Bring me the hourglass, but turn it upside down before handing it to me. ")));
        r[11].Children.Add(new Item(
                new("Bote", "Boat"),
                new("Es el célebre bote de Caronte.", "It is Charon's famous boat."),
                Inventory.LWeight - 1));
        r[11].Children.Add(new OpenLink(
                new("Río Negro", "Black River"),
                new("Es el Estigia.", "It is the Styx."),
                new("Usted se sumerge en las oscuras aguas del Estigia (tiene miedo de montar el bote de Caronte). Pocos mortales se han atrevido a tanto. Al poco rato está del otro lado.", "You plunge into the dark waters of the Styx (you are afraid to ride Charon's boat). Few mortals have dared so much. After a little while you are on the other side."),
                r[12]));

        r[12].Children.Add(new OpenLink(
                new("Río Negro", "Black River"),
                new("Es el Estigia.", "It is the Styx."),
                new("Usted regresa nadando a la otra orilla.", "You swim back to the other bank."),
                r[11]));
        r[12].Children.Add(new Hidden(
                new("Arbol de marfil", "Ivory tree"),
                new("Es infinitamente blanco.", "It is infinitely white."),
                new("Hacha", "Axe"),
                new Item(
                    new("Maza", "Mace"),
                    new("Es muy pesada.", "It is very heavy."),
                    Inventory.LWeight - 3)));
        r[12].Children.Add(new Hidden(
                new("Muralla", "Wall"),
                new("Al lado de esta, la Gran Muralla China es el juego de un niño en la arena.", "Next to this, the Great Wall of China is a child's game in the sand."),
                new("Maza", "Mace"),
                new OpenLink(
                    new("Avenida de hierro", "Iron avenue"),
                    new("Toda la avenida está rodeada de árboles de metal. Usted se cubre los oídos tratando de escapar de los chirridos provocados por el viento en las hojas.", "The whole avenue is lined with metal trees. You cover your ears trying to escape the screeching caused by the wind in the leaves."),
                    new("Usted camina durante varios minutos hasta salir de la avenida.", "You walk for several minutes until you leave the avenue."),
                    r[13])));

        r[13].Children.Add(new OpenLink(
                new("Avenida de hierro", "Iron avenue"),
                new("Toda la avenida está rodeada de árboles de metal. Usted se cubre los oídos tratando de escapar de los chirridos provocados por el viento en las hojas.", "The whole avenue is lined with metal trees. You cover your ears trying to escape the screeching caused by the wind in the leaves."),
                new("Usted camina durante varios minutos hasta regresar al río.", "You walk for several minutes until you return to the river."),
                r[12]));
        r[13].Children.Add(new Link(
                new("Puerta de madera", "Wooden door"),
                new("Es una puerta bastante vulgar. Conduce al comedor.", "It is a rather vulgar door. It leads to the dining room."),
                new("Usted entra al comedor.", "You enter the dining room."),
                "",
                r[14]));
        r[13].Children.Add(new Link(
                new("Puerta amarilla", "Yellow door"),
                new("Conduce al cuarto del tesoro.", "It leads to the treasure room."),
                new("Usted entra al cuarto del tesoro. Los resplandores de las piedras preciosas lo ciegan por unos instantes.", "You enter the treasure room. The glitter of the precious stones blinds you for a few moments."),
                "Omicuos Ihanti",
                r[17]));
        r[13].Children.Add(new Link(
                new("Puerta de tela", "Cloth door"),
                new("La tela de la puerta es tan dura como cualquier acero.", "The cloth of the door is as hard as any steel."),
                new("Usted entra a las habitaciones del sastre.", "You enter the tailor's quarters."),
                "",
                r[15]));
        r[13].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce a las habitaciones del jardinero.", "It leads to the gardener's quarters."),
                new("Usted entra a las habitaciones del jardinero.", "You enter the gardener's quarters."),
                "",
                r[16]));
        r[13].Children.Add(new Link(
                new("Puerta roja", "Red door"),
                new("Conduce al salón de las velas. La puerta está caliente.", "It leads to the hall of candles. The door is hot."),
                new("Usted entra al salón de las velas.", "You enter the hall of candles."),
                "",
                r[18]));
        r[13].Children.Add(new Item(
                new("Piedra", "Stone"),
                new("Es una piedra común. Usted se pregunta qué hace en la Fortaleza, donde todo es tan exótico.", "It is an ordinary stone. You wonder what it is doing in La Fortaleza, where everything is so exotic."),
                5));
        r[13].Children.Add(new Troll(
                new("Jardinero", "Gardener"),
                new("Es un anciano pequeño y encorvado.", "He is a small, hunched old man."),
                "Hacha",
                new("En tu camino encontrarás varios anillos. Debes guardar el que tú consideres más apropiado, el que se ajuste más a tu espíritu y a tu empresa. Los otros anillos solo te traerán peligros y muerte. Elige bien.", "On your way you will find several rings. You must keep the one you consider most appropriate, the one that best fits your spirit and your undertaking. The other rings will bring you only danger and death. Choose well."),
                new("El jardinero no le presta atención. Está muy ocupado cortando un abeto con una navaja.", "The gardener pays no attention to you. He is very busy cutting down a fir tree with a pocketknife.")));

        r[14].Children.Add(new Link(
                new("Puerta de Madera", "Wooden door"),
                new("Usted distingue algunas manchas de salsa en la puerta. Parece los comensales acostumbran a lanzarse la comida.", "You make out some sauce stains on the door. It seems the diners are accustomed to throwing food at each other."),
                new("Usted sale del comedor.", "You leave the dining room."),
                "",
                r[13]));
        r[14].Children.Add(new DangerLink(
                new("Túnel", "Tunnel"),
                new("Usted se pregunta qué hace un túnel en un comedor. ¡No intente cruzar desprotegido!", "You wonder what a tunnel is doing in a dining room. Do not try to cross unprotected!"),
                new("Casi a mitad de camino usted es atacado por una bandada de murciélagos gigantes. Gracias al arco, usted logra ahuyentarlos.", "About halfway along you are attacked by a flock of giant bats. Thanks to the bow, you manage to drive them away."),
                Link.OpenDoor,
                "Arco",
                r[49],
                kill));
        r[14].Children.Add(new Hidden(
                new("Huevo", "Egg"),
                new("Por el tamaño debe ser el huevo de un dinosaurio. Y de uno grande.", "By its size it must be a dinosaur egg. And a big one."),
                new("Piedra", "Stone"),
                new Troll(
                    new("Dinosaurio", "Dinosaur"),
                    new("Debía sentirse muy apretado en el huevo. Es un pequeño tiranosaurio.", "It must have felt very cramped in the egg. It is a small tyrannosaur."),
                    "Sonajero",
                    new("Maúllale al gato.\nUsted se sonríe. Es una respuesta típica en un cachorro de dinosaurio.", "Meow to the cat.\\nYou smile. It is a typical answer for a baby dinosaur."),
                    new("¡Guaaaaaa! ¡Guaaaaaa!\nUsted se pregunta con qué podría calmarlo.", "Waaaaah! Waaaaah!\\nYou wonder what could calm it."))));

        r[15].Children.Add(new Link(
                new("Puerta de Tela", "Cloth door"),
                new("La tela de la puerta es tan dura como cualquier acero.", "The cloth of the door is as hard as any steel."),
                new("Usted regresa al jardín.", "You return to the garden."),
                "",
                r[13]));
        r[15].Children.Add(new Item(
                new("Muñeco diabólico", "Evil doll"),
                new("Es un artefacto de brujería. Representa a un hombre joven y tiene un alfiler clavado en una pierna.", "It is a witchcraft artifact. It represents a young man and has a pin stuck in one leg."),
                2));
        r[15].Children.Add(new Item(
                new("Aguja", "Needle"),
                new("Es una aguja de plata, tan larga que usted se pregunta cómo el sastre puede coser con ella.", "It is a silver needle, so long that you wonder how the tailor can sew with it."),
                1));

        r[16].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted sale de las habitaciones del jardinero.", "You leave the gardener's quarters."),
                "",
                r[13]));
        r[16].Children.Add(new Item(
                new("Corta-cristales", "Glass cutter"),
                new("Tiene un diamante en la punta.", "It has a diamond at the tip."),
                3));
        r[16].Children.Add(new Item(
                new("Sombrero", "Hat"),
                new("Es muy grande. Al jardinero no debe servirle.", "It is very big. It cannot be of use to the gardener."),
                2));

        r[17].Children.Add(new Link(
                new("Puerta amarilla", "Yellow door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted regresa a los jardines.", "You return to the gardens."),
                "",
                r[13]));
        r[17].Children.Add(new Item(
                new("Rosa diamante", "Diamond rose"),
                new("Es increíblemete hermosa.", "It is incredibly beautiful."),
                3));
        r[17].Children.Add(new Item(
                new("Anillo de oro", "Gold ring"),
                new("El anillo es sencillo. Solo un aro, sin ningún tipo de adorno.", "The ring is plain. Just a band, without any kind of ornament."),
                1));
        r[17].Children.Add(new Item(
                new("Arpa", "Harp"),
                new("Es un arpa de diciocho cuerdas.", "It is a harp with eighteen strings."),
                3));
        r[17].Children.Add(new Item(
                new("Bolsa", "Bag"),
                new("Está llena de oro.", "It is full of gold."),
                5));

        r[18].Children.Add(new Link(
                new("Puerta roja", "Red door"),
                new("Conduce a los jardines.", "It leads to the gardens."),
                new("Usted regresa a los jardines.", "You return to the gardens."),
                "",
                r[13]));
        r[18].Children.Add(new Link(
                new("Puerta blanca", "White door"),
                new("Conduce al cuarto de juguetes.", "It leads to the toy room."),
                new("Usted entra al cuarto de juguetes.", "You enter the toy room."),
                "",
                r[20]));
        r[18].Children.Add(new OpenLink(
                new("Escalera", "Stairs"),
                new("Baja hasta el cuarto del Inmortal.", "It goes down to the Immortal's room."),
                new("Usted desciende las escaleras y entra al cuarto del Inmortal.", "You go down the stairs and enter the Immortal's room."),
                r[19]));
        r[18].Children.Add(new Troll(
                new("Dragon", "Dragon"),
                new("A usted se le erizan los pelos de la nuca de tan solo mirarlo. Es una criatura extremadamente hermosa y terrible a la vez.", "The hair on the back of your neck stands up just from looking at it. It is an extremely beautiful and terrible creature at the same time."),
                "Daga",
                new("Las palabras mágicas para entrar en el cuarto del tesoro son: \"Omicuos Ihanti\".", "The magic words to enter the treasure room are: \"Omicuos Ihanti\"."),
                new(Localizer.Msg("Silence", Language.Spanish), Localizer.Msg("Silence", Language.English))));

        r[19].Children.Add(new OpenLink(
                new("Escalera", "Stairs"),
                new("Lleva hasta el salón de las velas.", "It leads to the hall of candles."),
                new("Usted sube lentamente hasta ver el resplandor de las velas otra vez.", "You climb slowly until you see the glow of the candles again."),
                r[18]));
        r[19].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("¿Quién quiere vivir para siempre?", "Who wants to live forever?"),
                1));
        r[19].Children.Add(new Guard(
                new("Inmortal", "Immortal"),
                new("No parece tener ninguna edad en específico. Hay mucho cansancio en la expresión de su rostro.", "He does not seem to have any particular age. There is a lot of weariness in the expression of his face."),
                new("El Inmortal lo mira agradecido y le dice antes de morir:\n\"No confundas la tercera muralla con la tercera barrera que tienes que romper. La tercera barrera está en tí mismo.\"", "The Immortal looks at you gratefully and says before dying:\\n\"Do not confuse the third wall with the third barrier you have to break. The third barrier is within yourself.\""),
                new("Arco", "Bow")));

        r[20].Children.Add(new Link(
                new("Puerta blanca", "White door"),
                new("Conduce al salón de las velas.", "It leads to the hall of candles."),
                new("Usted sale del cuarto de juguetes.", "You leave the toy room."),
                "",
                r[18]));
        r[20].Children.Add(new Link(
                new("Puerta de piedra", "Stone door"),
                new("Es muy antigua. Probablemente sea más vieja que el propio cuarto de juguetes. ", "It is very old. It is probably older than the toy room itself. "),
                new("Usted sale del cuarto de juguetes y entra a las ruinas.", "You leave the toy room and enter the ruins."),
                "",
                r[21]));
        r[20].Children.Add(new Item(
                new("Saltador", "Jumper"),
                new("Es un tronco de abeto con un resorte en el extremo inferior.", "It is a fir trunk with a spring at the lower end."),
                7));
        r[20].Children.Add(new Item(
                new("Sonajero", "Rattle"),
                new("Es un jugete para recién nacidos.", "It is a toy for newborns."),
                2));
        r[20].Children.Add(new Item(
                new("Talismán de Nieve", "Snow Talisman"),
                new("Hay una pequeña tormenta de nieve encerrada en el cristal", "There is a small snowstorm trapped in the crystal"),
                1));

        r[21].Children.Add(new Link(
                new("Puerta de piedra", "Stone door"),
                new("Está muy a tono con el lugar. Las ruinas están más cerca de ser el juego del viento en las piedras que cualquier otra cosa. ", "It is very much in tune with the place. The ruins are closer to being the wind's game among the stones than anything else. "),
                new("Usted sale de las ruinas y regresa al cuarto de juguetes.", "You leave the ruins and return to the toy room."),
                "",
                r[20]));
        r[21].Children.Add(new Link(
                new("Puerta de hierro", "Iron door"),
                new("Es la entrada a la casa del Ogro.", "It is the entrance to the Ogre's house."),
                new("Usted entra a la casa del Ogro.", "You enter the Ogre's house."),
                "",
                r[22]));
        r[21].Children.Add(new OpenLink(
                new("Escalera", "Stairs"),
                new("Desde el final asciende un aire frío y húmedo.", "A cold, damp air rises from the end."),
                new("Los peldaños están llenos de musgo. Usted desciende cuidadosamente hasta llegar a los subterráneos.", "The steps are full of moss. You descend carefully until you reach the underground passages."),
                r[27]));
        r[21].Children.Add(new OpenLink(
                new("Desierto", "Desert"),
                new("Promete ser largo y abrasador.", "It promises to be long and scorching."),
                new("Usted camina duarante muchos días. El sol y el roce de la arena llagan todo su cuerpo. Por las noches, el viento helado del desierto no lo deja dormir. La sed invade su cuerpo y su mente.\nY cuando cree que nunca llegará descubre que está en un Oasis.", "You walk for many days. The sun and the rubbing of the sand wound your whole body. At night, the icy desert wind does not let you sleep. Thirst invades your body and your mind.\\nAnd when you think you will never arrive you discover that you are in an Oasis."),
                r[34]));

        r[22].Children.Add(new Link(
                new("Puerta de hierro", "Iron door"),
                new("Usted no comprende cómo puede estar tan sucia. Conduce a las ruinas.", "You do not understand how it can be so dirty. It leads to the ruins."),
                new("Usted regresa a las ruinas.", "You return to the ruins."),
                "",
                r[21]));
        r[22].Children.Add(new Link(
                new("Puerta gris", "Gray door"),
                new("Conduce al patio.", "It leads to the courtyard."),
                new("Usted sale al patio.", "You go out into the courtyard."),
                "",
                r[24]));
        r[22].Children.Add(new Item(
                new("Anillo de bronce", "Bronze ring"),
                new("Es más pesado que los demás anillos.", "It is heavier than the other rings."),
                2));

        r[23].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Conduce al patio.", "It leads to the courtyard."),
                new("Usted sale al patio.", "You go out into the courtyard."),
                "",
                r[24]));
        r[23].Children.Add(new Item(
                new("Lienzo", "Canvas"),
                new("Es un cuadro muy extraño. Tiene dibujadas tres líneas que se cortan entre sí en puntos distintos.", "It is a very strange painting. It has three lines drawn that cross each other at different points."),
                8));
        r[23].Children.Add(new Item(
                new("Grabado", "Engraving"),
                new("En el grabado aparece usted cruzando un puente con el propio grabado en la mano. Una estructura en abismo interesante.", "In the engraving you appear crossing a bridge with the engraving itself in your hand. An interesting mise en abyme."),
                2));

        r[24].Children.Add(new Link(
                new("Puerta gris", "Gray door"),
                new("Conduce al Salón de Recepciones.", "It leads to the Reception Hall."),
                new("Usted sale al Salón de Recepciones.", "You go out into the Reception Hall."),
                "",
                r[22]));
        r[24].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Conduce al cuarto del pintor.", "It leads to the painter's room."),
                new("Usted entra al cuarto del pintor. El olor del óleo golpea sus narices.", "You enter the painter's room. The smell of oil paint hits your nostrils."),
                "",
                r[23]));
        r[24].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce al cuarto del juglar.", "It leads to the minstrel's room."),
                new("Usted entra al cuarto del juglar.", "You enter the minstrel's room."),
                "",
                r[25]));
        r[24].Children.Add(new Link(
                new("Puerta negra", "Black door"),
                new("Conduce a la alcoba del Ogro.", "It leads to the Ogre's bedroom."),
                new("Usted entra la alcoba del Ogro.", "You enter the Ogre's bedroom."),
                "",
                r[26]));

        r[25].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce al patio.", "It leads to the courtyard."),
                new("Usted sale al patio.", "You go out into the courtyard."),
                "",
                r[24]));
        r[25].Children.Add(new DangerLink(
                new("Escalera", "Stairs"),
                new("Conduce a las Catacumbas.", "It leads to the Catacombs."),
                new("Casi al llegar al final, los peldaños se abrieron dejando abierto un precipicio lleno de lava. Por suerte, el escudo de Aquiles quedó trabado en un saliente y usted pudo regresar a la superficie.", "Almost at the end, the steps opened up, leaving a precipice full of lava. Luckily, the Shield of Achilles got stuck on a ledge and you were able to return to the surface."),
                Link.OpenDoor,
                "Escudo de Aquiles",
                r[28],
                kill));
        r[25].Children.Add(new Item(
                new("Cuerda", "Rope"),
                new("Esta cuerda era usada por el juglar en los actos de equilibrismo. Tiene un gancho atado en uno de sus extremos.", "This rope was used by the minstrel in his tightrope acts. It has a hook tied to one end."),
                4));
        r[25].Children.Add(new Troll(
                new("Juglar", "Minstrel"),
                new("Usted lo observa detenidamente. Su cara es tan larga y tan triste que se asemeja a la de un caballo.", "You look at him closely. His face is so long and so sad that it resembles that of a horse."),
                "Arpa",
                new("Lleva algo de la fragua de Vulcano al bajar a las Catacumbas.", "Take something from Vulcan's forge when going down to the Catacombs."),
                new("¡Qué aburrimiento!", "How boring!")));

        r[26].Children.Add(new Link(
                new("Puerta negra", "Black door"),
                new("Conduce al patio.", "It leads to the courtyard."),
                new("Usted sale al patio.", "You go out into the courtyard."),
                "",
                r[24]));
        r[26].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("¡BUUUU!", "BOOO!"),
                1));
        r[26].Children.Add(new Guard(
                new("Ogro", "Ogre"),
                new("Es una repugnante mezcla de hombre, cerdo y buitre. Parece dormido.", "It is a repugnant mix of man, pig and vulture. It seems to be asleep."),
                new("El Ogro despierta sobresaltado. El agua lo quema como si fuese ácido. Antes de volverse un charco de pasta pestilente le grita, víctima de un rencor casi infantil:\n\"¡No saldrás del lago con Marmidosa tan fácilmente!\"\nEl cubo de agua vuelve a llenarse.", "The Ogre wakes up with a start. The water burns him as if it were acid. Before turning into a puddle of stinking paste he shouts at you, victim of an almost childish resentment:\\n\"You will not get out of the lake with Marmidosa so easily!\"\\nThe bucket of water fills up again."),
                new("Cubo de agua", "Bucket of water")));

        r[27].Children.Add(new OpenLink(
                new("Escalera", "Stairs"),
                new("Asciende hasta las ruinas.", "It climbs up to the ruins."),
                new("Los peldaños están llenos de musgo. Usted asciende cuidadosamente hasta regresar a las ruinas.", "The steps are full of moss. You climb carefully until you return to the ruins."),
                r[21]));
        r[27].Children.Add(new DangerLink(
                new("Túnel", "Tunnel"),
                new("Conduce a las catacumbas. Usted huele alguna trampa.", "It leads to the catacombs. You smell a trap."),
                new("Al dar el tercer paso, usted activó un mecanismo que lanzó un dardo envenenado hacia su pecho. Usted sobrevive gracias al escudo de Aquiles.", "On taking the third step, you triggered a mechanism that shot a poisoned dart at your chest. You survive thanks to the Shield of Achilles."),
                Link.OpenDoor,
                "Escudo de Aquiles",
                r[28],
                kill));
        r[27].Children.Add(new Item(
                new("Antorcha", "Torch"),
                new("Es muy brillante y despide mucho calor.", "It is very bright and gives off a lot of heat."),
                3));

        r[28].Children.Add(new DangerLink(
                new("Túnel", "Tunnel"),
                new("Conduce a los subterráneos. Usted recuerda que debe llevar el Escudo de Aquiles.", "It leads to the underground passages. You remember that you must carry the Shield of Achilles."),
                new("Al dar el tercer paso, usted activó un mecanismo que lanzó un dardo envenenado hacia su pecho. Usted sobrevive gracias al escudo de Aquiles.", "On taking the third step, you triggered a mechanism that shot a poisoned dart at your chest. You survive thanks to the Shield of Achilles."),
                Link.OpenDoor,
                "Escudo de Aquiles",
                r[27],
                kill));
        r[28].Children.Add(new DangerLink(
                new("Escalera", "Stairs"),
                new("Conduce a una de las habitaciones de la casa del Ogro.", "It leads to one of the rooms of the Ogre's house."),
                new("Casi al llegar al final, los peldaños se abrieron dejando abierto un precipicio lleno de lava. Por suerte, el escudo de Aquiles quedó trabado en un saliente y usted pudo regresar a la superficie.", "Almost at the end, the steps opened up, leaving a precipice full of lava. Luckily, the Shield of Achilles got stuck on a ledge and you were able to return to the surface."),
                Link.OpenDoor,
                "Escudo de Aquiles",
                r[25],
                kill));
        r[28].Children.Add(new OpenLink(
                new("Pasadizo", "Passage"),
                new("Parece más seguro que la escalera y el túnel.", "It seems safer than the staircase and the tunnel."),
                new("Usted cruza esperando algo terrible, pero no sucede nada.", "You cross expecting something terrible, but nothing happens."),
                r[29]));
        r[28].Children.Add(new Item(
                new("Esqueleto de Murciélago", "Bat skeleton"),
                new("Usted se alegra de que sea solo el esqueleto. Mide tres metros de ala a ala.", "You are glad it is only the skeleton. It measures three meters from wing to wing."),
                7));

        r[29].Children.Add(new OpenLink(
                new("Pasadizo", "Passage"),
                new("Conduce a las Catacumbas.", "It leads to the Catacombs."),
                new("Usted regresa a las Catacumbas sin mayores contratiempos.", "You return to the Catacombs without any major mishaps."),
                r[28]));
        r[29].Children.Add(new Item(
                new("Anillo de plata", "Silver ring"),
                new("Es muy brillante.", "It is very bright."),
                1));
        r[29].Children.Add(new OpenLink(
                new("Catarata", "Waterfall"),
                new("Usted siente mareos de tan solo pensar en ella.", "You feel dizzy just thinking about her."),
                new("Usted hace un acopio de valor, cierra los ojos y se lanza al abismo. Al cabo de varias horas, usted cae en un profundo lago.", "You gather your courage, close your eyes and throw yourself into the abyss. After several hours, you fall into a deep lake."),
                r[31]));

        r[31].Children.Add(new DangerLink(
                new("Grieta", "Crack"),
                new("Usted logra ver algunos destellos de una luz muy fuerte al otro lado.", "You can see some flashes of a very strong light on the other side."),
                new("Usted cruza sin problemas, protegido con el Talismán de Nieve.", "You cross without problems, protected by the Snow Talisman."),
                Link.OpenDoor,
                "Talismán de Nieve",
                r[32],
                kill));
        r[31].Children.Add(new Hidden(
                new("Reja", "Gate"),
                new("Los barrotes de la reja son muy gruesos. Al otro lado se divisa una escalera.", "The bars of the grating are very thick. On the other side a staircase can be seen."),
                new("Tenazas", "Pliers"),
                new OpenLink(
                    new("Escalera", "Stairs"),
                    new("Esta es la única vía de sacar a Marmidosa del Lago.", "This is the only way to get Marmidosa out of the Lake."),
                    new("Usted sube hasta la Catedral.", "You climb up to the Cathedral."),
                    r[35])));
        r[31].Children.Add(new DangerLink2(
                new("Sendero dorado", "Golden path"),
                new("Es muy hermoso.", "It is very beautiful."),
                new("Usted sale del Lago.", "You leave the Lake."),
                Link.OpenDoor,
                "Marmidosa",
                r[33],
                kill));
        r[31].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("La inscripción dice:\n\"Has llegado hasta Marmidosa, la rival de Excalibur. Hazla parte de tu mano y ella será parte de tu andar. Tómala, está detrás de la grieta.\"", "The inscription says:\\n\"You have reached Marmidosa, the rival of Excalibur. Make her part of your hand and she will be part of your journey. Take her, she is behind the crack.\""),
                1));
        r[31].Children.Add(new Troll(
                new("Doncella del Lago", "Lady of the Lake"),
                new("Es tan hermosa que usted casi se ahoga. Usted siente ganas de quedarse en el Lago durante el resto de su vida.", "She is so beautiful that you almost drown. You feel like staying in the Lake for the rest of your life."),
                "Talismán de Nieve",
                new("Las dos partes del manuscrito están guardadas en la Torre de Cristal. Y cuando vayas en busca de la segunda parte, lleva el triángulo contigo.", "The two parts of the manuscript are kept in the Crystal Tower. And when you go in search of the second part, take the triangle with you."),
                new("La doncella le mira con cierto aire de complicidad y exclama: ¡La cueva de cristal es tan hermosa! Si pudiera entrar al menos una vez...", "The maiden looks at you with a certain air of complicity and exclaims: The crystal cave is so beautiful! If only I could go in at least once...")));

        r[32].Children.Add(new OpenLink(
                new("Grieta", "Crack"),
                new("Es la salida de la Cueva de Cristal.", "It is the exit from the Crystal Cave."),
                new("Usted sale de la Cueva de Cristal.", "You leave the Crystal Cave."),
                r[31]));
        r[32].Children.Add(new Item(
                new("Marmidosa", "Marmidosa"),
                new("Es algo más allá de la comprensión humana. Usted se siente orgulloso de haberla encontrado antes del Hechicero, que lleva siglos buscándola.", "It is something beyond human comprehension. You feel proud of having found it before the Sorcerer, who has been searching for it for centuries."),
                5));

        r[33].Children.Add(new DangerLink2(
                new("Sendero dorado", "Golden path"),
                new("Es muy hermoso.", "It is very beautiful."),
                new("Usted se sumerge en el lago.", "You dive into the lake."),
                Link.OpenDoor,
                "Marmidosa",
                r[31],
                kill));
        r[33].Children.Add(new OpenLink(
                new("Camino", "Road"),
                new("El camino se pierde como una serpiente en la rectitud del horizonte.", "The road is lost like a snake in the straightness of the horizon."),
                new("Usted nota como el paisaje va cambiando a medida que avanza. Cada vez se va pareciendo más a un desierto.", "You notice how the landscape changes as you advance. It looks more and more like a desert."),
                r[36]));

        r[34].Children.Add(new OpenLink(
                new("Desierto", "Desert"),
                new("Usted se estremece al recordar la última vez que atravesó el desierto.", "You shudder as you remember the last time you crossed the desert."),
                new("Usted camina duarante muchos días. El sol y el roce de la arena llagan todo su cuerpo. Por las noches, el viento helado del desierto no lo deja dormir. La sed invade su cuerpo y su mente.\nPero al final logra regresar a las ruinas.", "You walk for many days. The sun and the rubbing of the sand wound your whole body. At night, the icy desert wind does not let you sleep. Thirst invades your body and your mind.\\nBut in the end you manage to return to the ruins."),
                r[21]));
        r[34].Children.Add(new OpenLink(
                new("Ruta de los camellos", "Camel route"),
                new("Está formada por miles de pisadas de camellos. Seguramente conduce a algún lugar agradable.", "It is formed by thousands of camel footprints. It surely leads to some pleasant place."),
                new("Usted avanza siguiendo las huellas que oscilan de un lado a otro y hasta describen círculos. Todo parece indicar que los camellos estaban ebrios.\nUsted arriba a la Catedral.", "You advance following the footprints that swing from side to side and even describe circles. Everything indicates that the camels were drunk.\\nYou arrive at the Cathedral."),
                r[35]));
        r[34].Children.Add(new Item(
                new("Cubo de agua", "Bucket of water"),
                new("Es un balde de madera, lleno de agua hasta el tope.", "It is a wooden bucket, filled to the brim with water."),
                4));
        r[34].Children.Add(new Troll(
                new("Pordiosero", "Beggar"),
                new("Usted no sabe si es en realidad un pordiosero, o alguien que acaba de atravesar el desierto.", "You do not know whether he is really a beggar, or someone who has just crossed the desert."),
                "Bolsa",
                new("Si el Ogro se baña, se muere.", "If the Ogre takes a bath, he dies."),
                new("El pordiosero guarda silencio y extiende su mano pidiendo una limosna.", "The beggar keeps silent and holds out his hand asking for alms.")));

        r[35].Children.Add(new OpenLink(
                new("Ruta de los camellos", "Camel route"),
                new("Es la única vía de regresar a las ruinas. ", "It is the only way back to the ruins. "),
                new("Luego de varios días de camino; usted regresa, más muerto que vivo, al Oasis.", "After several days of walking, you return, more dead than alive, to the Oasis."),
                r[34]));
        r[35].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Es la salida de la Catedral.", "It is the exit from the Cathedral."),
                new("Usted sale de la Catedral y el viento seco del desierto le golpea la cara.", "You leave the Cathedral and the dry desert wind hits your face."),
                "",
                r[36]));
        r[35].Children.Add(new Item(
                new("Rosario", "Rosary"),
                new("Es un rosario bastante común.", "It is a fairly ordinary rosary."),
                1));
        r[35].Children.Add(new Troll(
                new("Obispo", "Bishop"),
                new("Es un anciano de aspecto venerable.", "He is an old man of venerable appearance."),
                "Silla",
                new("Solo con el Talismán de Nieve podrás entrar hasta Marmidosa.", "Only with the Snow Talisman will you be able to get in to Marmidosa."),
                new("El Obispo lo mira y fuerza una sonrisa. Está muy cansado para conversar y en toda la Catedral no hay dónde sentarse.", "The Bishop looks at you and forces a smile. He is too tired to talk and in the whole Cathedral there is nowhere to sit.")));

        r[36].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice:\n\"El Obispo le recomendó cierto objeto para protegerse a la hora de coger a Marmidosa. ¡No intente cruzar el camino en esta dirección con dicho objeto encima!\"", "It says:\\n\"The Bishop recommended a certain object to protect yourself when taking Marmidosa. Do not try to cross the road in this direction carrying that object!\""),
                1));
        r[36].Children.Add(new DangerLink2(
                new("Camino", "Road"),
                new("El camino se pierde como una serpiente en la rectitud del horizonte. ", "The road is lost like a snake in the straightness of the horizon. "),
                new("Usted nota como el paisaje va cambiando a medida que avanza. El desierto va perdiendo terreno y cada vez hay más vegetación.", "You notice how the landscape changes as you advance. The desert is losing ground and there is more and more vegetation."),
                Link.OpenDoor,
                "Talismán de Nieve",
                r[33],
                kill));
        r[36].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Es la puerta de salida de la Catedral.", "It is the exit door of the Cathedral."),
                new("Usted entra a la Catedral", "You enter the Cathedral"),
                "",
                r[35]));
        r[36].Children.Add(new Hidden(
                new("Muralla", "Wall"),
                new("Es algo más pequeña que la muralla anterior.", "It is somewhat smaller than the previous wall."),
                new("Piedra verde", "Green stone"),
                new OpenLink(
                    new("Avenida de las flores", "Avenue of flowers"),
                    new("Toda la avenida está sembrada de flores carnívoras.", "The whole avenue is planted with carnivorous flowers."),
                    new("Usted se ve obligado a echarse a correr para escapar de los constantes mordiscos.", "You are forced to start running to escape the constant bites."),
                    r[37])));
        r[36].Children.Add(new Item(
                new("Escudo de Aquiles", "Shield of Achilles"),
                new("Usted nunca había visto algo tan hermoso.\nEste escudo formaba parte de la nueva armadura que Hefestos le hizo a Aquiles cuando la anterior cayó en manos de los troyanos al morir Patroclo.", "You had never seen anything so beautiful.\\nThis shield was part of the new armor that Hephaestus made for Achilles when the previous one fell into the hands of the Trojans on the death of Patroclus."),
                10));

        r[37].Children.Add(new OpenLink(
                new("Avenida de las flores", "Avenue of flowers"),
                new("Toda la avenida está sembrada de flores carnívoras.", "The whole avenue is planted with carnivorous flowers."),
                new("Usted regresa corriendo al desierto.", "You run back to the desert."),
                r[36]));
        r[37].Children.Add(new OpenLink(
                new("Montañas", "Mountains"),
                new("Las montañas rodean todo el sur del Valle. Parece el espinazo de un dragón gigantesco.", "The mountains surround the whole south of the Valley. They look like the spine of a gigantic dragon."),
                new("Luego de varios días de camino usted llega al punto más alto de la cordillera.", "After several days of walking you reach the highest point of the mountain range."),
                r[38]));
        r[37].Children.Add(new Troll(
                new("Camello", "Camel"),
                new("Está muy viejo y maltratado. Tiene las pezuñas llenas de arena.", "He is very old and mistreated. His hooves are full of sand."),
                "Botella de vino",
                new("Las dos últimas letras del segundo nombre de la Hija del Hechicero son \"KA\".", "The last two letters of the Sorcerer's daughter's second name are \"KA\"."),
                new("¡Dame más!", "Give me more!")));
        r[37].Children.Add(new Item(
                new("Bolsa de semillas", "Bag of seeds"),
                new("La bolsa está llena de semillas de los árboles que crecen en el valle.", "The bag is full of seeds from the trees that grow in the valley."),
                2));

        r[38].Children.Add(new OpenLink(
                new("Montañas", "Mountains"),
                new("Desde este lugar, las montañas lucen como grades piedras envueltasen nubes.", "From this place, the mountains look like great stones wrapped in clouds."),
                new("Luego de varios días de camino usted regresa al valle.", "After several days of walking you return to the valley."),
                r[37]));
        r[38].Children.Add(new DangerLink(
                new("Precipicio", "Precipice"),
                new("Usted se inclina en el borde del precipicio, pero no consigue ver el fondo.", "You lean over the edge of the precipice, but you cannot see the bottom."),
                new("Usted logra sobreponerse al temblor de sus manos, se aferra al saltador y se lanza al vacío. Al tercer día de caída, el muelle del saltador rebota suavemente en el suelo.", "You manage to overcome the trembling of your hands, cling to the jumper and throw yourself into the void. On the third day of falling, the jumper's spring bounces gently on the ground."),
                Link.OpenDoor,
                "Saltador",
                r[40],
                kill));
        r[38].Children.Add(new OpenLink(
                new("Ladera", "Slope"),
                new("La ladera desciende suavemente hasta un campo sembrado.", "The hillside descends gently to a sown field."),
                new("Usted desciende cuidadosamente hasta las tierras del Labrador.", "You carefully descend to the Farmer's lands."),
                r[42]));
        r[38].Children.Add(new Link(
                new("Puerta de Madera", "Wooden door"),
                new("Es la entrada a la choza del Monje.", "It is the entrance to the Monk's hut."),
                new("Usted entra a la choza del Monje.", "You enter the Monk's hut."),
                "",
                r[39]));

        r[39].Children.Add(new Link(
                new("Puerta de Madera", "Wooden door"),
                new("Conduce a la cima del Pico Negro.", "It leads to the summit of the Black Peak."),
                new("Usted sale de la choza del Monje.", "You leave the Monk's hut."),
                "",
                r[38]));
        r[39].Children.Add(new Troll(
                new("Monje", "Monk"),
                new("Parece ser bastante joven.", "He seems to be quite young."),
                "Rosario",
                new("Para descender por el precipicio debes llevar algo que amortigüe el golpe al final.", "To go down the precipice you must carry something that cushions the blow at the end."),
                new("El monje no le responde. Está muy ocupado recogiendo las cuentas del suelo.", "The monk does not answer. He is very busy picking up the beads from the floor.")));

        r[40].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice:\n\"Aquí habitaron los horantes durante mucho tiempo, hasta que La Bestia ocupó el lugar. Ellos levantaron cada pared de la Fortaleza, solo ellos saben cómo salir.\"", "It says:\\n\"The horantes lived here for a long time, until the Beast took the place. They raised every wall of La Fortaleza, only they know how to get out.\""),
                1));
        r[40].Children.Add(new OpenLink(
                new("Túnel", "Tunnel"),
                new("Conduce a las ruinas.", "It leads to the ruins."),
                new("Usted camina hasta regresar a las ruinas.", "You walk until you return to the ruins."),
                r[21]));
        r[40].Children.Add(new OpenLink(
                new("Escalera", "Stairs"),
                new("Usted se pregunta a dónde conducirá.", "You wonder where it will lead."),
                new("Usted sube hasta regresar al Valle.", "You climb until you return to the Valley."),
                r[37]));
        r[40].Children.Add(new Link(
                new("Puerta de Hierro", "Iron door"),
                new("Es la entrada de la casa del sabio.", "It is the entrance to the sage's house."),
                new("Usted ha entrado a la casa del sabio.", "You have entered the sage's house."),
                "",
                r[41]));
        r[40].Children.Add(new Hidden(
                new("Estatua de Cristal", "Crystal statue"),
                new("Usted no logra descubrir qué representa.", "You cannot make out what it represents."),
                new("Corta-Cristales", "Glass cutter"),
                new Troll(
                    new("Horante", "Horante"),
                    new("Es el último de los horantes.", "He is the last of the horantes."),
                    "Muñeco diabólico",
                    new("Busca las dos mitades del Manuscrito de los Horantes y descífralo. Luego coloca las marcas para abrir la Fortaleza.", "Look for the two halves of the Manuscript of the Horantes and decipher it. Then place the marks to open La Fortaleza."),
                    new("El horante trata de articular algo, pero un terrible dolor en su pierna derecha no lo deja.", "The horante tries to articulate something, but a terrible pain in his right leg will not let him."))));

        r[41].Children.Add(new Link(
                new("Puerta de Hierro", "Iron door"),
                new("Conduce a la Ciudad Abandonada.", "It leads to the Abandoned City."),
                new("Usted regresa a la Ciudad Abandonada.", "You return to the Abandoned City."),
                "",
                r[40]));
        r[41].Children.Add(new Troll(
                new("Sabio", "Sage"),
                new("Es sabio es un hombre pequeño. Lleva unos espejuelos que cubren casi todo su rostro", "The sage is a small man. He wears glasses that cover almost his whole face"),
                "Esqueleto de Murciélago",
                new("De todas las cosas sobre la tierra, la que más me fascina es la cinta de Moebius. Empiezas a recorrerla por adentro y cuando vienes a ver... ¡estás afuera!", "Of all the things on earth, the one that fascinates me most is the Moebius strip. You start going along it on the inside and when you look... you are on the outside!"),
                new("El sabio está muy distraído haciendo un anillo de papel algo torcido. ", "The sage is very absent-mindedly making a somewhat crooked paper ring. ")));
        r[41].Children.Add(new Item(
                new("Cinta de Moebius", "Moebius strip"),
                new("Es una cinta de papel con una torcedura y ambos extremos pegados.", "It is a paper strip with a twist and both ends glued together."),
                1));
        r[41].Children.Add(new Item(
                new("Péndulo", "Pendulum"),
                new("El péndulo oscila levemente, colgado de un punto invisible del espacio.", "The pendulum swings slightly, hanging from an invisible point in space."),
                3));

        r[42].Children.Add(new OpenLink(
                new("Ladera", "Slope"),
                new("Es por dónde único se puede subir al Pico Negro de este lado de la cordillera.", "It is the only way up the Black Peak on this side of the mountain range."),
                new("Usted asciende el Pico Negro.", "You climb the Black Peak."),
                r[38]));
        r[42].Children.Add(new DangerLink(
                new("Río", "River"),
                new("La corriente es tan fuerte que arrastraría hasta la embarcación más pesada.", "The current is so strong that it would carry away even the heaviest vessel."),
                new("Usted lanza la soga con el garfio hasta la otra orilla y forma una especie de cuerda floja. Luego cruza el río haciendo una impresionante demostración de equilibrio.", "You throw the rope with the hook to the other bank and form a sort of tightrope. Then you cross the river making an impressive display of balance."),
                Link.OpenDoor,
                "Cuerda",
                r[43],
                kill));
        r[42].Children.Add(new Item(
                new("Tenazas", "Pliers"),
                new("Parecen ser muy fuertes.", "They seem to be very strong."),
                2));
        r[42].Children.Add(new Troll(
                new("Labrador", "Farmer"),
                new("Es un hobre robusto y muy maltratado por el trabajo en el campo.", "He is a robust man, badly worn by work in the fields."),
                "Bolsa de semillas",
                new("El Labrador lo mira y le dice:\n\"No siempre las cosas son lo que parecen. La realidad tiene muchas caras y nunca sabemos si estamos mirando la verdadera. Mantén tu mente abierta\".", "The Farmer looks at you and says:\\n\"Things are not always what they seem. Reality has many faces and we never know whether we are looking at the true one. Keep your mind open.\""),
                new("¡Qué cansancio! Creo que es demasiada tierra para una sola persona.", "How tiring! I think it is too much land for one person.")));

        r[43].Children.Add(new DangerLink(
                new("Río", "River"),
                new("La corriente es tan fuerte que arrastraría hasta la embarcación más pesada.", "The current is so strong that it would carry away even the heaviest vessel."),
                new("Usted lanza la soga con el garfio hasta la otra orilla y forma una especie de cuerda floja. Luego cruza el río haciendo una impresionante demostración de equilibrio.", "You throw the rope with the hook to the other bank and form a sort of tightrope. Then you cross the river making an impressive display of balance."),
                Link.OpenDoor,
                "Cuerda",
                r[42],
                kill));
        r[43].Children.Add(new Hidden(
                new("Ebano", "Ebony"),
                new("Es muy alto.", "It is very tall."),
                new("NADA", "NOTHING"),
                null));
        r[43].Children.Add(new Link(
                new("Puerta de Hierro", "Iron door"),
                new("Es la entrada a los calabozos.", "It is the entrance to the dungeons."),
                new("Usted entra a los calabozos.", "You enter the dungeons."),
                "",
                r[47]));
        r[43].Children.Add(new OpenLink(
                new("Escalera de Caracol", "Spiral staircase"),
                new("Sube hasta el tope de la la Torre de Cristal.", "It climbs to the top of the Crystal Tower."),
                new("Usted asciende durante varias horas.", "You climb for several hours."),
                r[44]));

        r[44].Children.Add(new OpenLink(
                new("Escalera de Caracol", "Spiral staircase"),
                new("Conduce a la parte inferior de la Torre de Cristal.", "It leads to the lower part of the Crystal Tower."),
                new("Usted regresa a la orilla del río.", "You return to the riverbank."),
                r[43]));
        r[44].Children.Add(new Hidden(
                new("Esfera", "Sphere"),
                new("Tiene aproximadamente tres metros de diámetro y parece ser de carne. Es algo realmente insólito.", "It is about three meters in diameter and seems to be made of flesh. It is truly unusual."),
                new("Marmidosa", "Marmidosa"),
                new Item(
                    new("Tablilla de Madera", "Wooden tablet"),
                    new("Tiene las siguientes palabras grabadas:\n\n \"A acsd o oatssn\n  Oadaat lfnodllg\n  Oed aot nl rlaousa\n  Nocae lcat ehépds\n  Lail lgd ut léao\n  Suod qie nl iddd o oats\n  Seod ahj e ehcr ns uro\n  Edl ne aó efmr\"", "It has the following words engraved:\n\n \"H ak ftehrne r:\n  Imn oea h otmo h ae\n  Hrnsba nteopst hr\n  Oc ntegetro\n  H hsnrn ett h bn\n  Hedo cilsi h iyo h oats\n  Irro h ocrrsduhe nhrro\n  Edlmi h mkn om\""),
                    2)));
        r[44].Children.Add(new RiddleLink(
                new("Puerta de Cristal", "Crystal door"),
                new("Es de un cristal muy grueso y opaco.", "It is made of a very thick, opaque crystal."),
                new("¿Cuál es el nombre completo de la hija del Hechicero?", "What is the full name of the Sorcerer's daughter?"),
                "Aura Srka",
                new("Usted entra a la alcoba.", "You enter the bedroom."),
                r[45]));

        r[45].Children.Add(new Link(
                new("Puerta de Cristal", "Crystal door"),
                new("Es la salida de la alcoba.", "It is the exit from the bedroom."),
                new("Usted sale de la alcoba.", "You leave the bedroom."),
                "",
                r[44]));
        r[45].Children.Add(new Hidden(
                new("Carta", "Letter"),
                new("La carta dice:\n\"Mi querido salvador:\n¿Por qué has demorado tanto? Hace mucho que mi padre me llevó al Abismo Infranqueable. Ahora no podré marcharme contigo. La único que puedes hacer para salir es matarlo con Marmidosa. Por favor, trata de que no sufra.\"", "The letter says:\\n\"My dear savior:\\nWhy have you taken so long? A long time ago my father took me to the Impassable Abyss. Now I will not be able to leave with you. The only thing you can do to get out is to kill him with Marmidosa. Please, try not to make him suffer.\""),
                new("", ""),
                new DangerLink(
                    new("Puerta Secreta", "Secret door"),
                    new("Es muy negra y tiene una tela de araña dibujada. Está abierta.", "It is very black and has a spider web drawn on it. It is open."),
                    new("Usted atraviesa la puerta lentamente. Los pelos de la nuca se le erizan sin poder evitarlo.", "You pass through the door slowly. The hair on the back of your neck stands up and you cannot help it."),
                    Link.OpenDoor,
                    "Lienzo",
                    r[46],
                    kill)));
        r[45].Children.Add(new Item(
                new("Espejo", "Mirror"),
                new("Está roto.", "It is broken."),
                1));

        r[46].Children.Add(new Link(
                new("Puerta Secreta", "Secret door"),
                new("Conduce a la alcoba.", "It leads to the bedroom."),
                new("Usted regresa a la alcoba.", "You return to the bedroom."),
                Link.OpenDoor,
                r[45]));
        r[46].Children.Add(new Daughter(
                new("Hija del Hechicero", "Sorcerer's daughter"),
                new("Usted está a punto de desmayarse. La hija del Hechicero no es exactamente como usted esperaba. En realidad se parece más a una serpiente de quince metros y tres cabezas, con la piel llena de gruesas escamas.", "You are about to faint. The Sorcerer's daughter is not exactly as you expected. In reality she looks more like a fifteen-meter serpent with three heads, her skin covered in thick scales."),
                new("Usted clava fuertemente la aguja entre las escamas. Las cabezas le miran llenas de ira y hay una que exclama: \"¡Mata a todos los que te mintieron si quieres salir!\"", "You drive the needle hard between the scales. The heads look at you full of rage and one of them exclaims: \"Kill everyone who lied to you if you want to get out!\""),
                new("Aguja", "Needle"),
                kill));

        r[47].Children.Add(new Link(
                new("Puerta de Hierro", "Iron door"),
                new("Es la salida de los calabozos. Conduce a la orilla del río.", "It is the exit from the dungeons. It leads to the riverbank."),
                new("Usted regresa al río.", "You return to the river."),
                "",
                r[43]));
        r[47].Children.Add(new Link(
                new("Reja", "Gate"),
                new("Conduce a la celda. Los barrotes son tan gruesos que la reja es prácticamente una puerta maciza.", "It leads to the cell. The bars are so thick that the grating is practically a solid door."),
                new("Usted estornuda. Ha entrado a la celda.", "You sneeze. You have entered the cell."),
                "Grifo",
                r[48]) { AltKey = "Griffin" });
        r[47].Children.Add(new Link(
                new("Puerta Gris", "Gray door"),
                new("Conduce a la sala de La Prueba.", "It leads to the room of The Trial."),
                new("Usted entra a la sala de La Prueba.", "You enter the room of The Trial."),
                "",
                r[49]));
        r[47].Children.Add(new Guard(
                new("Carcelero", "Jailer"),
                new("Parece un hombre rudo y cumplidor de sus deberes.", "He looks like a rough man who does his duty."),
                new("ARRRGG!!! ¡Te será muy difícil abrir la celda, porque la palabra mágica es el nombre de la primera criatura que viste al salir del cuarto de huéspedes! \nUsted sonríe porque sabe que el problema es fácil.", "ARRRGG!!! It will be very hard for you to open the cell, because the magic word is the name of the first creature you saw on leaving the guest room! \\nYou smile because you know the problem is easy."),
                new("Marmidosa", "Marmidosa")));

        r[48].Children.Add(new Link(
                new("Reja", "Gate"),
                new("Es la salida de la celda.", "It is the exit from the cell."),
                new("Usted sale de la celda.", "You leave the cell."),
                "Grifo",
                r[47]) { AltKey = "Griffin" });
        r[48].Children.Add(new Troll(
                new("Encapuchado", "Hooded figure"),
                new("Es una criatura realmente asombrosa. Toda su anatomía parece regida por el número siete. Siete brazos, siete pies, siete dedos en cada una de las siete manos.", "It is a truly amazing creature. All its anatomy seems to be ruled by the number seven. Seven arms, seven feet, seven fingers on each of the seven hands."),
                "Receta",
                new("El nombre de la hija del Hechicero tiene ocho letras, una está repetida tres veces y otra dos.", "The name of the Sorcerer's daughter has eight letters, one repeated three times and another twice."),
                new("Pareces haber estado viajando. Seguramente has visto algo que me pertenece por ahí... ¿podrías traérmelo?", "You look like you have been traveling. Surely you have seen something of mine out there... could you bring it to me?")));

        r[49].Children.Add(new Link(
                new("Puerta Gris", "Gray door"),
                new("Conduce a los calabozos.", "It leads to the dungeons."),
                new("Usted entra a los calabozos.", "You enter the dungeons."),
                "",
                r[47]));
        r[49].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice:\n\"En este punto comienza la prueba de los anillos. Continúa con el que hayas elegido y deja los demás.\"", "It says:\\n\"At this point the trial of the rings begins. Continue with the one you have chosen and leave the others.\""),
                1));
        r[49].Children.Add(new Item(
                new("Arco", "Bow"),
                new("Con este arco mataron a Hycrk, uno de los doce inmortales.", "With this bow they killed Hycrk, one of the twelve immortals."),
                5));
        r[49].Children.Add(new DangerLink(
                new("Túnel", "Tunnel"),
                new("Parece ser muy largo. Usted no se atrevería a cruzarlo sin el arco.", "It seems to be very long. You would not dare cross it without the bow."),
                new("Casi a mitad de camino usted es atacado por una bandada de murciélagos gigantes. Gracias al arco, usted logra ahuyentarlos.", "About halfway along you are attacked by a flock of giant bats. Thanks to the bow, you manage to drive them away."),
                Link.OpenDoor,
                "Arco",
                r[14],
                kill));
        r[49].Children.Add(new DangerLink2(
                new("Puerta Verde", "Green door"),
                new("Es la primera puerta de la prueba. ", "It is the first door of the trial. "),
                new("Usted cierra los ojos y cruza la puerta. Dejar el anillo de oro fue una idea excelente.", "You close your eyes and cross the door. Leaving the gold ring was an excellent idea."),
                Link.OpenDoor,
                "Anillo de oro",
                r[50],
                kill));

        r[50].Children.Add(new DangerLink2(
                new("Puerta Verde", "Green door"),
                new("Conduce al primer salón.", "It leads to the first hall."),
                new("Usted atraviesa la puerta sin dificultades.", "You pass through the door without difficulty."),
                Link.OpenDoor,
                "Anillo de oro",
                r[49],
                kill));
        r[50].Children.Add(new DangerLink2(
                new("Puerta Azul", "Blue door"),
                new("Es la segunda puerta de la prueba.", "It is the second door of the trial."),
                new("Usted piensa durante unos segundos antes de avanzar. Hizo bien en dejar el anillo de plata.", "You think for a few seconds before advancing. You did well to leave the silver ring."),
                Link.OpenDoor,
                "Anillo de plata",
                r[51],
                kill));

        r[51].Children.Add(new DangerLink2(
                new("Puerta Azul", "Blue door"),
                new("Conduce al salón verde.", "It leads to the green hall."),
                new("Usted atraviesa la puerta sin dificultades.", "You pass through the door without difficulty."),
                Link.OpenDoor,
                "Anillo de plata",
                r[50],
                kill));
        r[51].Children.Add(new DangerLink2(
                new("Puerta Blanca", "White door"),
                new("Es la tercera puerta de la prueba.", "It is the third door of the trial."),
                new("Usted levanta los ojos buscando valor y da los primeros pasos. Usted se alegra de no haber traído el anillo de bronce.", "You raise your eyes looking for courage and take the first steps. You are glad you did not bring the bronze ring."),
                Link.OpenDoor,
                "Anillo de bronce",
                r[52],
                kill));

        r[52].Children.Add(new DangerLink2(
                new("Puerta Blanca", "White door"),
                new("Conduce al salón azul.", "It leads to the blue hall."),
                new("Usted atraviesa la puerta sin dificultades.", "You pass through the door without difficulty."),
                Link.OpenDoor,
                "Anillo de bronce",
                r[51],
                kill));
        r[52].Children.Add(new DangerLink(
                new("Puerta triangular", "Triangular door"),
                new("Es la última puerta de la prueba.", "It is the last door of the trial."),
                new("Esta vez usted no se detiene a pensarlo y avanza decididamente. Todo parece indicar que eligió el anillo correcto: la cinta de Moebius.", "This time you do not stop to think about it and advance resolutely. Everything seems to indicate that you chose the right ring: the Moebius strip."),
                Link.OpenDoor,
                "Cinta de Moebius",
                r[53],
                kill));

        r[53].Children.Add(new DangerLink(
                new("Puerta triangular", "Triangular door"),
                new("Conduce al salón blanco.", "It leads to the white hall."),
                new("Usted atraviesa la puerta sin dificultades y regresa al salón blanco.", "You pass through the door without difficulty and return to the white hall."),
                Link.OpenDoor,
                "Cinta de Moebius",
                r[52],
                kill));
        r[53].Children.Add(new DangerLink(
                new("Puente", "Bridge"),
                new("Está hecho de madera y soga. Bajo él se abre, infinito, el Abismo Infranqueable. Del otro lado se divisa una pirámide.", "It is made of wood and rope. Beneath it opens, infinite, the Impassable Abyss. On the other side a pyramid can be seen."),
                new("Usted logra cruzar el puente gracias a la protección del grabado.", "You manage to cross the bridge thanks to the protection of the engraving."),
                Link.OpenDoor,
                "Grabado",
                r[54],
                kill));
        r[53].Children.Add(new Hidden(
                new("Muralla", "Wall"),
                new("Es la tercera muralla con que usted tropieza en su camino. algo le dice que también es la última.", "It is the third wall you have come across on your way. Something tells you it is also the last."),
                new("NADA", "NOTHING"),
                null));
        r[53].Children.Add(new Troll(
                new("Guardián", "Guardian"),
                new("Está muy viejo para vigilar algo. Probablemente alguien lo haya olvidado en este lugar. Está muy quemado por el sol.", "He is too old to guard anything. Someone has probably forgotten him in this place. He is badly sunburned."),
                "Sombrero",
                new("El anciano le cuenta una historia incoherente sobre un héroe que derrota a una serpiente gigante armado de una aguja.", "The old man tells you an incoherent story about a hero who defeats a giant serpent armed with a needle."),
                new("¡Qué calor!", "How hot!")));

        r[54].Children.Add(new DangerLink(
                new("Puente", "Bridge"),
                new("De este lado se puede apreciar mejor lo frágil que es el puente y lo infinito que es el Abismo.", "From this side you can better appreciate how fragile the bridge is and how infinite the Abyss is."),
                new("Usted logra cruzar el puente gracias a la protección del grabado.", "You manage to cross the bridge thanks to the protection of the engraving."),
                Link.OpenDoor,
                "Grabado",
                r[53],
                kill));
        r[54].Children.Add(new Hidden(
                new("Columna de Hielo", "Ice column"),
                new("Es una ancha estructura de hielo pegada a la pared.", "It is a wide structure of ice stuck to the wall."),
                new("Antorcha", "Torch"),
                new RiddleLink(
                    new("Puerta de Roble", "Oak door"),
                    new("Usted presiente que el Hechicero se encuentra detrás de esa puerta.", "You sense that the Sorcerer is behind that door."),
                    new("¿Quién invirtió el reloj de Caronte?", "Who turned Charon's clock upside down?"),
                    "Yo",
                    new("Usted entra a las habitaciones del Hechicero.", "You enter the Sorcerer's rooms."),
                    r[55]) { AltAnswer = "Me" }));

        r[55].Children.Add(new Link(
                new("Puerta de Roble", "Oak door"),
                new("Es de un roble tan sólido que mellaría cualquier acero.", "It is made of oak so solid that it would dull any steel."),
                new("Usted sale de las habitaciones del Hechicero.", "You leave the Sorcerer's rooms."),
                "",
                r[54]));
        r[55].Children.Add(new Troll(
                new("Hechicero", "Sorcerer"),
                new("Es un hombre inmenso, de barba blanca. Viste una túnica negra y sostiene una vara con una caravela en su mano.", "He is an immense man with a white beard. He wears a black tunic and holds a staff with a caravel on top."),
                "Marmidosa",
                new("El Hechicero le sonríe y le dice: \n\"No soy tu enemigo. Es mi hija quien ha elaborado este diabólico plan para eliminarnos. Rompe la carta que te dejó en la Torre de Cristal y ocurrirá un milagro.\"", "The Sorcerer smiles at you and says: \\n\"I am not your enemy. It is my daughter who has devised this diabolical plan to eliminate us. Tear up the letter she left you in the Crystal Tower and a miracle will happen.\""),
                new(Localizer.Msg("Silence", Language.Spanish), Localizer.Msg("Silence", Language.English))));

        var game = new Fort2(r[1]);
        for (var i = 1; i <= 55; i++)
            game.AddRoom(r[i]);
        player = game.Player;
        return game;
    }
}
