using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Games;

public static class Fort1Builder
{
    private static readonly (LocalizedText Name, LocalizedText Description)[] RoomData =
    [
        (new("el exterior de la fortaleza", "the exterior of the fortress"),
            new("Las paredes son muy negras y al parecer, no tienen ventanas. Usted trata de ver el final de las torres, pero las nubes no se lo permiten.", "The walls are very black and apparently have no windows. You try to see the tops of the towers, but the clouds do not allow it.")),
        (new("el Salón de recepciones", "the Reception hall"),
            new("Parece hecho para criaturas cientos de veces más grandes que los humanos. Está desierto, pero usted siente que es observado.", "It seems made for creatures hundreds of times larger than humans. It is deserted, but you feel that you are being watched.")),
        (new("la Sala de juegos", "the Game room"),
            new("Usted trata de imagainar cómo eran los juegos del salón. Hay doce mesas y cada una tiene doce erizos.", "You try to imagine what the games of the hall were like. There are twelve tables and each one has twelve hedgehogs.")),
        (new("el Patio interior", "the Inner courtyard"),
            new("El patio tiene forma de caracol. Cada uno o dos pasos se levanta una columna de mármol negro y se confunde en la altura con los árboles. Usted siente el crujido de las hojas secas bajo sus zapatos.", "The courtyard is shaped like a snail shell. Every one or two steps a column of black marble rises and merges with the trees high above. You feel the crunch of dry leaves under your shoes.")),
        (new("el Cuarto de espejos", "the Room of mirrors"),
            new("Hay tantos espejos que no puede reconocer si usted es real o es una imagen de sí mismo.", "There are so many mirrors that you cannot tell whether you are real or an image of yourself.")),
        (new("la Biblioteca", "the Library"),
            new("Hay miles de libros en este lugar. Todos tienen alguna hoja marcada con una servilleta.", "There are thousands of books in this place. All of them have a page marked with a napkin.")),
        (new("Pasillo", "Corridor"),
            new("Tiene tres puertas al lado y una al final.", "It has three doors on the side and one at the end.")),
        (new("la Alcoba de la bruja", "the Witch's chamber"),
            new("Hasta un ciego descubriría que aquí habita una bruja. El cuarto tiene una ventana, pero usted no recuerda haberla visto desde afuera.", "Even a blind man would discover that a witch lives here. The room has a window, but you do not remember having seen it from outside.")),
        (new("el Cuarto del guerrero", "the Warrior's room"),
            new("El cuarto está muy ordenado. A usted le sorprende que no hayan armas.", "The room is very tidy. It surprises you that there are no weapons.")),
        (new("el Laboratorio de la Bestia", "the Laboratory of the Beast"),
            new("Hay muchos frascos. A usted le llaman la atención los últimos: están llenos de homúnculos.", "There are many jars. The last ones catch your attention: they are full of homunculi.")),
        (new("la Sala de armas", "the Armory"),
            new("Este lugar parece salido del sueño de un guerrero.", "This place seems to have come out of a warrior's dream.")),
        (new("los Baños de la Bestia", "the Baths of the Beast"),
            new("El baño es tan grande que usted no puede ver las paredes que lo limitan", "The bath is so large that you cannot see the walls that bound it.")),
        (new("el Jardín", "the Garden"),
            new("Se podrían construir varias ciudades en su interior.", "Several cities could be built inside it.")),
        (new("la Alcoba de la doncella", "the Maiden's chamber"),
            new("Es una habitación muy pequeña y húmeda.", "It is a very small and damp room.")),
        (new("la Sala de infusiones", "the Infusion room"),
            new("Hay varias mesas regadas por el suelo. Usted tiene la impresión de que ningún humano ha estado en ese lugar anteriormente.", "There are several tables scattered over the floor. You have the impression that no human has ever been in this place before.")),
        (new("los Calabozos", "the Dungeons"),
            new("Son tan húmedos que usted no resistiría ni unas horas detro de ellos.", "They are so damp that you would not last even a few hours inside them.")),
        (new("Boca de la Bestia", "Mouth of the Beast"),
            new("Es como una boca humana, pero del tamaño de una casa.", "It is like a human mouth, but the size of a house.")),
        (new("Interior de la Bestia", "Inside the Beast"),
            new("Usted puede sentir los latidos del corazón de la Bestia, al igual que su respiración o el ruido de los líquidos revueltos en su estómago.", "You can feel the beating of the Beast's heart, as well as its breathing and the noise of the liquids churning in its stomach.")),
        (new("Pulmones de la Bestia", "Lungs of the Beast"),
            new("Sopla un aire tan fuerte que usted se ha tirado al suelo.", "A wind blows so strongly that you have thrown yourself to the ground.")),
        (new("el Estómago de la Bestia", "the Stomach of the Beast"),
            new("Hay varios esqueletos humanos.", "There are several human skeletons.")),
        (new("el Corazón de la Bestia", "the Heart of the Beast"),
            new("Todo palpita rítmicamente. Usted siente cómo los latidos se adueñan del lugar, especialmente de su propio corazón.", "Everything throbs rhythmically. You feel the beats taking over the place, especially your own heart.")),
        (new("el Cerebro de la Bestia", "the Brain of the Beast"),
            new("Es como un tarjetero gigante. Cada idea está escrita en una tarjeta y, cuando es necesario, la tarjeta es sacada por una mano invisible y leída al resto de la Bestia.", "It is like a giant card index. Each idea is written on a card and, when necessary, the card is pulled out by an invisible hand and read to the rest of the Beast.")),
        (new("los Almacenes de la Fortaleza", "the Storehouses of La Fortaleza"),
            new("Están llenos de carne humana. Usted aparta el cadáver de una mujer para poder observar bien el lugar.", "They are full of human flesh. You push aside the corpse of a woman to be able to look at the place properly.")),
        (new("un extenso páramo", "a vast moor"),
            new("Hay muy poca vegetación y el suelo es pantanoso. Usted nunca imaginó que el interior de la fortaleza fuera tan grande.", "There is very little vegetation and the ground is marshy. You never imagined that the inside of the fortress would be so large.")),
        (new("Salón de cristal", "Crystal hall"),
            new("Es como un caleidoscopio gigante. Cada varios segundos las paredes cambian de color y usted siente como si lo hubieran trasladado a otro lugar.", "It is like a giant kaleidoscope. Every few seconds the walls change color and you feel as if you had been moved to another place.")),
        (new("celda pequeña", "small cell"),
            new("No hay forma alguna de salir.", "There is no way out.")),
        (new("un puente de mármol", "a marble bridge"),
            new("Es muy largo y estrecho.", "It is very long and narrow.")),
        (new("la Antesala del Laberinto", "the Antechamber of the Labyrinth"),
            new("Es una habitación muy confortable, pero usted siente algo malvado flotando en el aire.", "It is a very comfortable room, but you feel something evil floating in the air.")),
        (new("la cueva del Minotauro", "the Minotaur's cave"),
            new("Hay siete antorchas.", "There are seven torches.")),
        (new("el Salón de los elegidos", "the Hall of the chosen"),
            new("Usted se enorgullece: muy pocas personas han llegado hasta este salón. El cerebro de la Bestia está detrás de la puerta.", "You feel proud: very few people have reached this hall. The brain of the Beast is behind the door.")),
        (new("un pasillo del Laberinto", "a corridor of the Labyrinth"),
            new("El techo está tan pegado al suelo que usted tiene que andar agachado. Hay tres puerta simétricas.", "The ceiling is so close to the floor that you have to walk stooped. There are three symmetrical doors.")),
        (new("un pasillo del Laberinto", "a corridor of the Labyrinth"),
            new("Hay una puerta que le conducirá a la antesala del Laberinto.", "There is a door that will lead you to the antechamber of the Labyrinth.")),
        (new("el Jardín", "the Garden"),
            new("Se podrían construir varias ciudades en su interior.", "Several cities could be built inside it.")),
    ];

    private static int RoomDataIndex(int room) => room switch
    {
        <= 30 => room - 1,
        <= 48 => 30,
        49 => 31,
        _ => 32
    };

    public static Fort1 Build()
    {
        Player? player = null;
        Action kill = () => player!.Die("");
        var r = new Room[51];
        for (var i = 1; i <= 50; i++)
        {
            var data = RoomData[RoomDataIndex(i)];
            r[i] = new Room(data.Name, data.Description, Inventory.LWeight + 1);
        }

        r[1].Children.Add(new Item(
                new("Roble", "Oak"),
                new("Está completamente seco y lleno de mordiscos. Tiene las siguientes palabras talladas en el tronco: \"Las palabras mágicas para abrir la puerta son obvias.\"", "It is completely dry and full of bite marks. It has the following words carved on the trunk: \"The magic words to open the door are obvious.\""),
                200));
        r[1].Children.Add(new Item(
                new("Maza", "Mace"),
                new("Es muy pesada.", "It is very heavy."),
                Inventory.LWeight - 1));
        r[1].Children.Add(new Item(
                new("Pastel de cerezas", "Cherry pie"),
                new("¿Qué hace un pastel de cerezas en un lugar como este?", "What is a cherry pie doing in a place like this?"),
                2));
        r[1].Children.Add(new Troll(
                new("Llamador de bronce", "Bronze knocker"),
                new("Parece de carne, y tiene forma de murciélago.", "It seems to be made of flesh, and is shaped like a bat."),
                "Cigarro",
                GameKeys.K01,
                new("¿Me puede regalar un cigarro?", "Could you give me a cigar?")));
        r[1].Children.Add(new Link(
                new("Puerta principal", "Main door"),
                new("Inmensa y oprimente.", "Immense and oppressive."),
                new("Su sombra desaparece lentamente al cruzar la puerta. Siente cómo el miedo se le enreda en los zapatos.", "Your shadow slowly vanishes as you cross the door. You feel fear tangling around your shoes."),
                GameKeys.K02.Es,
                r[2]) { AltKey = GameKeys.K02.En });
        r[1].Children.Add(new OpenLink(
                new("Túnel", "Tunnel"),
                new("Parece muy largo.", "It looks very long."),
                new("El olor de la carne descompuesta lo golpea. Usted siente ganas de vomitar.", "The smell of rotting flesh hits you. You feel like vomiting."),
                r[4]));
        r[1].Children.Add(new Hidden(
                new("Pared solitaria", "Lonely wall"),
                new("Usted no puede imaginar con qué motivo fue colocada ahí.", "You cannot imagine for what reason it was placed there."),
                GameKeys.K03,
                new DangerLink(
                    new("Puerta secreta", "Secret door"),
                    new("Usted se pregunta a dónde conducirá.", "You wonder where it might lead."),
                    new("Usted atraviesa la puerta.", "You go through the door."),
                    "",
                    "Antorcha",
                    r[27],
                    kill)));

        r[2].Children.Add(new Hidden(
                new("Monolito de mármol", "Marble monolith"),
                new("Es muy oscuro, pero a usted le parece que hay algo vivo en su interior.", "It is very dark, but you get the feeling that there is something alive inside it."),
                GameKeys.K04,
                new Troll(
                    new("Trebol", "Clover"),
                    new("Tiene cinco hojas.", "It has five leaves."),
                    "Vaso de agua",
                    GameKeys.K05,
                    new("Dame agua, por favor...", "Give me water, please..."))));
        r[2].Children.Add(new Link(
                new("Puerta principal", "Main door"),
                new("Inmensa y oprimente.", "Immense and oppressive."),
                new("Usted se siente de regreso a la vida. Ha salido de La Fortaleza.", "You feel back to life. You have left La Fortaleza."),
                GameKeys.K02.Es,
                r[1]) { AltKey = GameKeys.K02.En });
        r[2].Children.Add(new Link(
                new("Puerta negra", "Black door"),
                new("Es muy negra.", "It is very black."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[3]));
        r[2].Children.Add(new Item(
                new("Retrato", "Portrait"),
                new("Es de Hitchcock (un anacronismo). Usted recuerda instintivamente algunas películas que le erizaron la piel: Psicosis, Vértigo y Los treinta y nueve escalones.", "It is of Hitchcock (an anachronism). You instinctively remember some movies that gave you goosebumps: Psycho, Vertigo and The Thirty-Nine Steps."),
                1));

        r[3].Children.Add(new Item(
                new("Escoba", "Broom"),
                new("Es una escoba voladora.", "It is a flying broom."),
                1));
        r[3].Children.Add(new RiddleLink(
                new("Escalera", "Staircase"),
                new("La escalera está detrás de una puerta de cristal. Usted trata de ver cuántos escalones tiene, pero no puede.", "The staircase is behind a crystal door. You try to see how many steps it has, but you cannot."),
                new("¿Cuántos peldaños tiene la escalera?", "How many steps does the staircase have?"),
                GameKeys.K06.Es,
                new("Usted sube por la escalera. No se explica cómo, siendo tan larga, solo tiene " + GameKeys.K06.Es + " escalones.", "You climb the staircase. You cannot explain how, being so long, it only has " + GameKeys.K06.En + " steps."),
                r[5]) { AltAnswer = GameKeys.K06.En });
        r[3].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice: \"Cuando hayas llegado casi al final, tendrás que regresar, a no ser que hayas hablado con el murciélago.", "It says: \"When you have almost reached the end, you will have to go back, unless you have spoken with the bat."),
                10));
        r[3].Children.Add(new Link(
                new("Puerta negra", "Black door"),
                new("Es muy negra.", "It is very black."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[2]));
        r[3].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Esta puerta conduce al patio interior.", "This door leads to the inner courtyard."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[4]));
        r[3].Children.Add(new Item(
                new("Hilo de Ariadna", "Ariadne's thread"),
                new("Debe tener varios kilómetros de largo.", "It must be several kilometers long."),
                1));

        r[4].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Esta puerta conduce a la sala de juegos.", "This door leads to the game room."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[3]));
        r[4].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Esta puerta conduce a la biblioteca.", "This door leads to the library."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[6]));
        r[4].Children.Add(new Troll(
                new("Estatua de Atenea", "Statue of Athena"),
                new("Fue hecha por Fidias, siete años después de su muerte. Le falta la lanza.", "It was made by Phidias, seven years after his death. It is missing the spear."),
                GameKeys.K07.Es,
                GameKeys.K08,
                new("¡Sal de mi presencia, estúpido mortal!", "Get out of my presence, stupid mortal!")));
        r[4].Children.Add(new Troll(
                new("Estatua de Hermes", "Statue of Hermes"),
                new("Desde el techo caen gotas continuamente sobre su cabeza.", "Drops fall continuously from the ceiling onto its head."),
                GameKeys.K09.Es,
                GameKeys.K10,
                new("La humedad me destruye.", "The dampness is destroying me.")));
        r[4].Children.Add(new Item(
                new("Balanza", "Scale"),
                new("La balanza tiene el plato en forma de mano. La aguja es muy delgada, hecha con varios bigotes de gato trenzados entre sí. ", "The scale has its pan shaped like a hand. The needle is very thin, made of several cat whiskers braided together. "),
                1));

        r[5].Children.Add(new Item(
                new("Hueso de gato", "Cat bone"),
                new("Este es un hueso de gato (o de la imagen de un gato).", "This is a cat bone (or the bone of the image of a cat)."),
                1));
        r[5].Children.Add(new Link(
                new("Escalera", "Staircase"),
                new("Usted trata de ver hasta dónde llega, pero no puede. Está detrás de una puerta de cristal.", "You try to see how far it goes, but you cannot. It is behind a crystal door."),
                new("Usted se sorprende: La escalera tiene ahora muchos escalones más.", "You are surprised: the staircase now has many more steps."),
                "treinta y nueve",
                r[3]) { AltKey = "thirty nine" });
        r[5].Children.Add(new Hidden(
                new("Espejo opaco", "Opaque mirror"),
                new("En realidad, usted no tiene idea de por qué es un espejo. Hay algo muy extraño en él.", "In truth, you have no idea why it is a mirror. There is something very strange about it."),
                GameKeys.K11,
                new Link(
                    new("Puerta oculta", "Hidden door"),
                    new("Parece formada por diminutos fragmentos de cristal.", "It seems to be made of tiny fragments of glass."),
                    new("Su imagen regresa al atravesar la puerta.", "Your image returns as you go through the door."),
                    "",
                    r[14])));

        r[6].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Esta puerta conduce al patio interior.", "This door leads to the inner courtyard."),
                new("Usted atraviesa la puerta sin dificultades.", "You go through the door without difficulty."),
                "",
                r[4]));
        r[6].Children.Add(new Item(
                new("Vaso de agua", "Glass of water"),
                new("Por un momento cree ver un bote en el vaso.", "For a moment you think you see a boat in the glass."),
                1));
        r[6].Children.Add(new Item(
                new("Candelabro", "Candelabrum"),
                new("La llama en su extremo está inmóvil, como si fuera de cera.", "The flame at its tip is motionless, as if it were made of wax."),
                1));
        r[6].Children.Add(new Link(
                new("Libro", "Book"),
                new("Es un libro normal.", "It is an ordinary book."),
                new("¡Usted ha atravesado el libro!", "You have gone through the book!"),
                "",
                r[13]));
        r[6].Children.Add(new Guard(
                new("Cíclope", "Cyclops"),
                new("Parece estar cuidando la puerta.", "He seems to be guarding the door."),
                new("¡Arrrggghhhh! ¡Me has matado!" + GameKeys.K12.Es, "Arrrggghhhh! You have killed me!" + GameKeys.K12.En),
                GameKeys.K13));
        r[6].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Es una puerta para gigantes.", "It is a door for giants."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                GameKeys.K14.Es,
                r[7]));

        r[7].Children.Add(new Item(
                new("Antorcha", "Torch"),
                new("La llama parece bailar en su propia luz.", "The flame seems to dance in its own light."),
                1));
        r[7].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Es una puerta para gigantes.", "It is a door for giants."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                GameKeys.K14.Es,
                r[6]));
        r[7].Children.Add(new Link(
                new("Puerta vieja", "Old door"),
                new("Es la puerta de las habitaciones de una bruja.", "It is the door to a witch's rooms."),
                new("Usted ha entrado.", "You have entered."),
                "",
                r[8]));
        r[7].Children.Add(new Link(
                new("Puerta roja", "Red door"),
                new("Es la puerta del guerrero.", "It is the warrior's door."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                "",
                r[9]));
        r[7].Children.Add(new Link(
                new("Puerta prohibida", "Forbidden door"),
                new("Esta puerta está prohibida para los humanos.", "This door is forbidden to humans."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                GameKeys.K15.Es,
                r[10]) { AltKey = GameKeys.K15.En });
        r[7].Children.Add(new Link(
                new("Puerta gris", "Gray door"),
                new("Esta puerta conduce a la sala de las infusiones.", "This door leads to the infusion room."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                "",
                r[15]));

        r[8].Children.Add(new Link(
                new("Puerta vieja", "Old door"),
                new("Da al pasillo.", "It opens onto the corridor."),
                new("Usted ha salido.", "You have left."),
                "",
                r[7]));
        r[8].Children.Add(new Item(
                new("Polvo mágico", "Magic dust"),
                new("Puede hacer invisible a cualquiera.", "It can make anyone invisible."),
                1));
        r[8].Children.Add(new Item(
                new("Poción para crecer", "Growth potion"),
                new("Tiene un olor muy repugnante.", "It has a very repugnant smell."),
                5));
        r[8].Children.Add(new DangerLink(
                new("Escaparate", "Display cabinet"),
                new("Tiene las puertas llenas de arañazos.", "Its doors are covered with scratches."),
                GameKeys.K16,
                "",
                "Candelabro",
                r[14],
                kill));
        r[8].Children.Add(new Troll(
                new("Bruja", "Witch"),
                new("Parece tener mil años.", "She seems to be a thousand years old."),
                "Escoba",
                GameKeys.K17,
                new("Tráeme mi escoba y te daré un consejo.", "Bring me my broom and I will give you advice.")));

        r[9].Children.Add(new Link(
                new("Puerta roja", "Red door"),
                new("Da al pasillo.", "It opens onto the corridor."),
                new("Usted está en el pasillo nuevamente.", "You are in the corridor again."),
                "",
                r[7]));
        r[9].Children.Add(new Item(
                new("Vendajes", "Bandages"),
                new("Están manchados de sangre.", "They are stained with blood."),
                3));
        r[9].Children.Add(new Hidden(
                new("Cama", "Bed"),
                new("Es tan fuerte que solo con un hacha podría romperla.", "It is so strong that you could only break it with an axe."),
                GameKeys.K18,
                new Link(
                    new("Puerta", "Door"),
                    new("Está en el suelo.", "It is on the floor."),
                    new("Usted entra a la armería.", "You enter the armory."),
                    "",
                    r[11])));
        r[9].Children.Add(new Item(
                new("Grabado", "Engraving"),
                new("En el grabado está usted, hablando con una muchacha.", "In the engraving there you are, talking with a girl."),
                3));

        r[10].Children.Add(new Link(
                new("Puerta prohibida", "Forbidden door"),
                new("Esta puerta está prohibida para los humanos.", "This door is forbidden to humans."),
                new("Usted se encuentra en el otro lado de la puerta.", "You find yourself on the other side of the door."),
                GameKeys.K15.Es,
                r[7]) { AltKey = GameKeys.K15.En });
        r[10].Children.Add(new Troll(
                new("Homúnculo", "Homunculus"),
                new("Parece que escapó de algún frasco. Es tan pequeño que usted se arrodilla para verlo.", "It looks like it escaped from some jar. It is so small that you kneel to see it."),
                "Poción para crecer",
                GameKeys.K20,
                new("¡Quiero crecer!", "I want to grow!")));
        r[10].Children.Add(new Item(
                new("Piedra filosofal", "Philosopher's stone"),
                new("Usted es el primer humano en ver la quinta escencia.", "You are the first human to see the fifth essence."),
                10));
        r[10].Children.Add(new Item(
                new("Máquina del tiempo", "Time machine"),
                new("Es muy parecida a una máquina de moler carne.", "It looks a lot like a meat grinder."),
                3));

        r[11].Children.Add(new Item(
                new("Espada", "Sword"),
                new("Es muy brillante.", "It is very bright."),
                5));
        r[11].Children.Add(new Item(
                new("Lanza", "Spear"),
                new("Está hecha de piedra.", "It is made of stone."),
                10));
        r[11].Children.Add(new Item(
                new("Arco", "Bow"),
                new("Es el arco de Odiseo.", "It is the bow of Odysseus."),
                5));
        r[11].Children.Add(new Item(
                new("Daga", "Dagger"),
                new("Es afilada como la lengua de la serpiente bíblica.", "It is as sharp as the tongue of the biblical serpent."),
                3));
        r[11].Children.Add(new Item(
                new("Ariete", "Battering ram"),
                new("Demasiado grande y pesado.", "Too big and heavy."),
                Inventory.LWeight - 10));
        r[11].Children.Add(new Item(
                new("Látigo", "Whip"),
                new("Parece una culebra.", "It looks like a snake."),
                2));
        r[11].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Está en el techo.", "It is on the ceiling."),
                new("Usted regresa al cuarto del guerrero.", "You return to the warrior's room."),
                "",
                r[9]));

        r[12].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted entra al jardín.", "You enter the garden."),
                "",
                r[13]));
        r[12].Children.Add(new Item(
                new("Rosa", "Rose"),
                new("Es más hermosa que todas las doncellas que usted no ha visto.", "It is more beautiful than all the maidens you have not seen."),
                1));
        r[12].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice: \n¡Mate al Troll!\nY no haga sus necesidades en el suelo.", "It says: \nKill the Troll!\nAnd do not relieve yourself on the floor."),
                2));
        r[12].Children.Add(new Item(
                new("Espejo", "Mirror"),
                new("Usted se ve a sí mismo entregándole unos polvos al Troll.", "You see yourself handing some powder to the Troll."),
                20));
        r[12].Children.Add(new Troll(
                new("Troll", "Troll"),
                new("Está mirándose en un espejo. Tiene una figura tan repulsiva que usted aparta la vista.", "He is looking at himself in a mirror. His figure is so repulsive that you look away."),
                "Polvo mágico",
                GameKeys.K22,
                new("No me mates, por favor. Haz que nadie pueda ver mi fealdad.", "Do not kill me, please. Make it so that nobody can see my ugliness.")));

        r[13].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Es un buen lugar para instalar una tienda de campaña ¿no cree?", "It is a good place to pitch a tent, don't you think?"),
                1));
        r[13].Children.Add(new Item(
                new("Cedro", "Cedar"),
                new("Le faltan algunas ramas. Al parecer, alguien las cortó para hacerse una cama.", "It is missing some branches. Apparently, someone cut them to make a bed."),
                Inventory.LWeight + 1));
        r[13].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce a los baños de la Bestia.", "It leads to the baths of the Beast."),
                new("Usted sale del jardín.", "You leave the garden."),
                "",
                r[12]));
        r[13].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Debe haber algo hemoso detrás de esta puerta.", "There must be something beautiful behind this door."),
                new("Usted sale del jardín.", "You leave the garden."),
                "",
                r[14]));
        r[13].Children.Add(new Link(
                new("Puerta amarilla", "Yellow door"),
                new("Conduce a la sala de infusiones.", "It leads to the infusion room."),
                new("Usted sale del jardín.", "You leave the garden."),
                "",
                r[15]));
        r[13].Children.Add(new Link(
                new("Puerta de hierro", "Iron door"),
                new("Conduce los calabozos.", "It leads to the dungeons."),
                new("Usted entra a los calabozos.", "You enter the dungeons."),
                "",
                r[16]));
        r[13].Children.Add(new Link(
                new("Puerta de madera", "Wooden door"),
                new("Conduce al almacén.", "It leads to the storehouse."),
                new("Usted sale del jardín.", "You leave the garden."),
                "",
                r[23]));

        r[14].Children.Add(new Link(
                new("Puerta azul", "Blue door"),
                new("Debe haber algo hermoso detrás de esta puerta.", "There must be something beautiful behind this door."),
                new("Usted sale al jardín.", "You go out to the garden."),
                "",
                r[13]));
        r[14].Children.Add(new Link(
                new("Puerta oculta", "Hidden door"),
                new("Parece formada por diminutos fragmentos de cristal.", "It seems to be made of tiny fragments of glass."),
                new("Su imagen regresa al atravesar la puerta.", "Your image returns as you go through the door."),
                "",
                r[5]));
        r[14].Children.Add(new Item(
                new("Cuadro", "Painting"),
                new("Está completamente pintado de negro.", "It is painted completely black."),
                10));
        r[14].Children.Add(new Item(
                new("Corazón de unicornio", "Unicorn heart"),
                new("Aún late.", "It still beats."),
                1));
        r[14].Children.Add(new Link(
                new("Escaparate", "Display cabinet"),
                new("Las puertas son doradas.", "The doors are golden."),
                new("Usted sube durante varios minutos hasta llegar al escaparate de la bruja.", "You climb for several minutes until you reach the witch's display cabinet."),
                "",
                r[8]));
        r[14].Children.Add(new Troll(
                new("Doncella", "Maiden"),
                new("Nunca antes había contemplado tanta hermosura. Usted está como hipnotizado.", "You had never before beheld such beauty. You are as if hypnotized."),
                GameKeys.K24.Es,
                GameKeys.K25,
                new("¡Qué brusco! Un caballero debe dirigirse a una dama de otra forma.", "How rude! A gentleman must address a lady in another way.")));

        r[15].Children.Add(new Link(
                new("Puerta gris", "Gray door"),
                new("Da al pasillo.", "It opens onto the corridor."),
                new("Usted se encuentra del otro lado de la puerta.", "You find yourself on the other side of the door."),
                "",
                r[7]));
        r[15].Children.Add(new Link(
                new("Puerta amarilla", "Yellow door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted sale al jardín.", "You go out to the garden."),
                "",
                r[13]));
        r[15].Children.Add(new Item(
                new("Taza de café", "Cup of coffee"),
                new("Humea.", "It steams."),
                1));
        r[15].Children.Add(new Item(
                new("Cigarro", "Cigar"),
                new("Está encendido.", "It is lit."),
                1));
        r[15].Children.Add(new Guard(
                new("Arpía", "Harpy"),
                new("Está desayunando. Usted recuerda a su suegra sin darse cuenta.", "She is having breakfast. You remember your mother-in-law without realizing it."),
                GameKeys.K26,
                GameKeys.K27));

        r[16].Children.Add(new Link(
                new("Puerta de hierro", "Iron door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted sale al jardín.", "You go out to the garden."),
                "",
                r[13]));
        r[16].Children.Add(new Item(
                new("Paraguas", "Umbrella"),
                new("Está cerrado.", "It is closed."),
                2));
        r[16].Children.Add(new Item(
                new("Talismán de aire", "Air talisman"),
                new("Está hecho de cinco vientos amarrados con un cordón negro.", "It is made of five winds tied with a black cord."),
                1));
        r[16].Children.Add(new Troll(
                new("Esqueleto", "Skeleton"),
                new("Debe tener varios siglos.", "It must be several centuries old."),
                "Taza de café",
                GameKeys.K29,
                new("Dame algo para calentarme la barriga, por favor.", "Give me something to warm my belly, please.")));

        r[17].Children.Add(new Link(
                new("Puerta triangular", "Triangular door"),
                new("Tiene forma de un triángulo de siete lados.", "It is shaped like a triangle with seven sides."),
                new("Usted sale de la boca de la Bestia.", "You leave the mouth of the Beast."),
                "",
                r[25]));
        r[17].Children.Add(new OpenLink(
                new("Garganta", "Throat"),
                new("Es roja y carnosa.", "It is red and fleshy."),
                new("Usted atraviesa por la garganta de la Bestia.", "You pass through the throat of the Beast."),
                r[18]));

        r[18].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("No continúe si no está protegido.", "Do not continue if you are not protected."),
                10));
        r[18].Children.Add(new Link(
                new("Garganta", "Throat"),
                new("Es roja y carnosa.", "It is red and fleshy."),
                new("Usted atraviesa por la garganta de la Bestia.", "You pass through the throat of the Beast."),
                Link.OpenDoor,
                r[17]));
        r[18].Children.Add(new DangerLink(
                new("Traquea", "Trachea"),
                new("Luce igual que en los libros de biología que había en su escuela.", "It looks just like in the biology books you had at school."),
                new("Usted atraviesa por la tráquea de la Bestia.", "You pass through the trachea of the Beast."),
                Link.OpenDoor,
                GameKeys.K30.Es,
                r[19],
                kill));
        r[18].Children.Add(new DangerLink(
                new("Esofago", "Esophagus"),
                new("Parece un túnel.", "It looks like a tunnel."),
                new("Usted atraviesa por el esófago de la Bestia.", "You pass through the esophagus of the Beast."),
                Link.OpenDoor,
                GameKeys.K31.Es,
                r[20],
                kill));
        r[18].Children.Add(new DangerLink(
                new("Arteria principal", "Main artery"),
                new("Conduce al corazón seguramente.", "It surely leads to the heart."),
                new("Usted atraviesa por la arteria principal de la Bestia.", "You pass through the main artery of the Beast."),
                Link.OpenDoor,
                "Corazon de unicornio",
                r[21],
                kill));

        r[19].Children.Add(new Link(
                new("Tráquea", "Trachea"),
                new("Luce igual que en los libros de biología que había en su escuela.", "It looks just like in the biology books you had at school."),
                new("Usted atraviesa por la tráquea de la Bestia.", "You pass through the trachea of the Beast."),
                Link.OpenDoor,
                r[18]));
        r[19].Children.Add(new Guard(
                new("Centro de los pulmones", "Center of the lungs"),
                new("De aquí surge la vida de los pulmones.", "The life of the lungs springs from here."),
                GameKeys.K35,
                GameKeys.K32));

        r[20].Children.Add(new Link(
                new("Esófago", "Esophagus"),
                new("Parece un túnel.", "It looks like a tunnel."),
                new("Usted atraviesa por el esófago de la Bestia.", "You pass through the esophagus of the Beast."),
                Link.OpenDoor,
                r[18]));
        r[20].Children.Add(new Guard(
                new("Centro del estomago", "Center of the stomach"),
                new("De aquí surge la vida del estómago.", "The life of the stomach springs from here."),
                new("¡Usted ha matado al centro del estómago!", "You have killed the center of the stomach!"),
                GameKeys.K33));

        r[21].Children.Add(new Link(
                new("Arteria principal", "Main artery"),
                new("Conduce al corazón seguramente.", "It surely leads to the heart."),
                new("Usted atraviesa por la arteria principal de la Bestia.", "You pass through the main artery of the Beast."),
                Link.OpenDoor,
                r[18]));
        r[21].Children.Add(new Guard(
                new("Centro del corazón", "Center of the heart"),
                new("De aquí surge la vida del corazón.", "The life of the heart springs from here."),
                GameKeys.K37,
                GameKeys.K34));

        r[22].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                GameKeys.K39,
                10));
        r[22].Children.Add(new Guard(
                new("Centro del cerebro", "Center of the brain"),
                new("¡Usa la antorcha!", "Use the torch!"),
                new("XVBCZXV XBVCZX BVZXC!!!!! ", "XVBCZXV XBVCZX BVZXC!!!!! "),
                GameKeys.K38));

        r[23].Children.Add(new Item(
                new("Hacha", "Axe"),
                new("Está muy afilada.", "It is very sharp."),
                10));
        r[23].Children.Add(new Link(
                new("Puerta de madera", "Wooden door"),
                new("Conduce al jardín.", "It leads to the garden."),
                new("Usted sale del almacén.", "You leave the storehouse."),
                "",
                r[13]));
        r[23].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Usted presiente que hay algo muy grande detrás de ella.", "You sense that there is something very big behind it."),
                new("Usted entra al páramo.", "You enter the moor."),
                "",
                r[24]));
        r[23].Children.Add(new Troll(
                new("Ratón", "Mouse"),
                new("Tiene puesto un delantal de alquimista.", "He is wearing an alchemist's apron."),
                GameKeys.K40.Es,
                GameKeys.K41,
                new("No tengo tiempo para atenderte. Llevo medio siglo buscando la quinta escencia.", "I have no time to attend to you. I have been looking for the fifth essence for half a century.")));

        r[24].Children.Add(new Link(
                new("Puerta verde", "Green door"),
                new("Conduce al almacén.", "It leads to the storehouse."),
                new("Usted sale del páramo.", "You leave the moor."),
                "",
                r[23]));
        r[24].Children.Add(new Link(
                new("Puerta de cristal", "Crystal door"),
                new("Es muy delgada.", "It is very thin."),
                new("Usted sale del páramo.", "You leave the moor."),
                GameKeys.K42.Es,
                r[25]) { AltKey = GameKeys.K42.En });
        r[24].Children.Add(new OpenLink(
                new("Pozo", "Well"),
                new("Es muy profundo.", "It is very deep."),
                new("Usted cae vertiginosamente por el pozo y, antes de que pueda darse cuenta, está en el exterior de la fortaleza.", "You fall dizzyingly down the well and, before you realize it, you are outside the fortress."),
                r[1]));
        r[24].Children.Add(new DangerLink(
                new("Túnel", "Tunnel"),
                new("Parece más acogedor que el túnel del exterior de la fortaleza.", "It looks cozier than the tunnel outside the fortress."),
                new("", ""),
                Link.OpenDoor,
                GameKeys.K43.Es,
                r[24],
                kill));
        r[24].Children.Add(new Guard(
                new("Lobo", "Wolf"),
                new("Es del tamaño de un ternero. La única forma de pasar por la puerta de cristal es matándolo.", "It is the size of a calf. The only way to go through the crystal door is to kill it."),
                GameKeys.K44,
                new("Latigo", "Whip")));

        r[25].Children.Add(new Link(
                new("Puerta de cristal", "Crystal door"),
                new("Es muy delgada.", "It is very thin."),
                new("Usted sale al páramo.", "You go out to the moor."),
                GameKeys.K42.Es,
                r[24]) { AltKey = GameKeys.K42.En });
        r[25].Children.Add(new Link(
                new("Puerta de tela", "Cloth door"),
                new("Es una puerta normal, pero de tela.", "It is an ordinary door, but made of cloth."),
                new("Usted sale del salón de cristal.", "You leave the crystal hall."),
                "",
                r[26]));
        r[25].Children.Add(new RiddleLink(
                new("Puerta triangular", "Triangular door"),
                new("Tiene forma de un triángulo de siete lados.", "It is shaped like a triangle with seven sides."),
                new("¿Cuál es el nombre del duende?", "What is the name of the elf?"),
                GameKeys.K23.Es,
                new("Usted entra a la boca de la Bestia.", "You enter the mouth of the Beast."),
                r[17]));
        r[25].Children.Add(new Troll(
                new("Bailarina", "Dancer"),
                new("Es muy joven.", "She is very young."),
                "Máquina del tiempo",
                GameKeys.K47,
                new("Quiero ser joven para siempre.", "I want to be young forever.")));

        r[26].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("Dice que usted pasará en esta celda el resto de sus días.", "It says that you will spend the rest of your days in this cell."),
                Inventory.LWeight + 1));

        r[27].Children.Add(new Troll(
                new("Araña", "Spider"),
                new("Es un poco mayor que usted.", "It is a little bigger than you."),
                "Hilo de Ariadna",
                new("Busca al Minotauro y mátalo con la espada.", "Look for the Minotaur and kill him with the sword."),
                new("¡Pssssssss!", "Pssssssss!")));
        r[27].Children.Add(new Item(
                new("Martillo", "Hammer"),
                new("Tiene dos mangos y tres cabezas.", "It has two handles and three heads."),
                2));
        r[27].Children.Add(new DangerLink(
                new("Puerta secreta", "Secret door"),
                new("Conduce al exterior de la fortaleza.", "It leads to the outside of the fortress."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                "",
                "Antorcha",
                r[1],
                kill));
        r[27].Children.Add(new Hidden(
                new("Estatua de Satanás", "Statue of Satan"),
                new("Es infinitamente negra.", "It is infinitely black."),
                new("Cuadro", "Painting"),
                new Link(
                    new("Puerta oculta", "Hidden door"),
                    new("Conduce a la antesala del laberinto.", "It leads to the antechamber of the Labyrinth."),
                    new("Usted atraviesa la puerta.", "You go through the door."),
                    "",
                    r[28])));
        r[27].Children.Add(new DangerLink2(
                new("Puerta", "Door"),
                new("Conduce a la antesala del Laberinto.", "It leads to the antechamber of the Labyrinth."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                "",
                "Espada",
                r[28],
                kill));

        r[28].Children.Add(new Link(
                new("Puerta", "Door"),
                new("Conduce al puente.", "It leads to the bridge."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                "",
                r[27]));
        r[28].Children.Add(new Troll(
                new("Crunch", "Crunch"),
                new("Es muy similar a una boca con patas.", "It looks very much like a mouth with legs."),
                "Pastel de cerezas",
                new("El 12 te guiará en el Laberinto. Luego solo debes dar un paso.", "The 12 will guide you in the Labyrinth. Then you only have to take one step."),
                new("¡Qué hambre!", "I'm so hungry!")));
        r[28].Children.Add(new Hidden(
                new("Columna de Cristal", "Crystal column"),
                new("Es muy alta.", "It is very tall."),
                new("Antorcha 3", "Torch 3"),
                new Hidden(
                    new("Puerta de madera", "Wooden door"),
                    new("Parece muy frágil.", "It looks very fragile."),
                    new("Martillo", "Hammer"),
                    new Link(
                        new("Puerta negra", "Black door"),
                        new("Conduce al Jardín", "It leads to the Garden"),
                        new("Usted entra al Jardín.", "You enter the garden."),
                        "",
                        r[50]))));
        r[28].Children.Add(new OpenLink(
                new("Puerta verde", "Green door"),
                new("Conduce al Laberinto.", "It leads to the Labyrinth."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[28].Children.Add(new Troll(
                new("Dédalo", "Daedalus"),
                new("Parece haber sido herido por el cuerno de un toro gigante.", "He seems to have been wounded by the horn of a giant bull."),
                "Vendajes",
                new("Solo el hierro te protegerá contra las flores.", "Only iron will protect you against the flowers."),
                new("Me muero...", "I am dying...")));

        r[29].Children.Add(new OpenLink(
                new("Puerta", "Door"),
                new("Conduce al Laberinto.", "It leads to the Labyrinth."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[47]));
        r[29].Children.Add(new Item(
                new("Antorcha 1", "Torch 1"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 2", "Torch 2"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 3", "Torch 3"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 4", "Torch 4"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 5", "Torch 5"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 6", "Torch 6"),
                new("Está apagada.", "It is out."),
                2));
        r[29].Children.Add(new Item(
                new("Antorcha 7", "Torch 7"),
                new("Es muy brillante.", "It is very bright."),
                2));
        r[29].Children.Add(new Guard(
                new("Minotauro", "Minotaur"),
                new("Está sentado en un enorme trono de rocas.", "He is sitting on an enormous throne of rocks."),
                new("¡Me has matado! Coge la antorcha del número divino y rompe la columna de cristal.", "You have killed me! Take the torch with the divine number and break the crystal column."),
                new("Espada", "Sword")));

        r[30].Children.Add(new RiddleLink(
                new("Puerta dorada", "Golden door"),
                new("Es de oro macizo.", "It is made of solid gold."),
                new("Invente un alfabeto (el mayor que pueda) con el que no pueda crearse a la Bestia.", "Invent an alphabet (the largest you can) with which the Beast cannot be created."),
                "cdfghjklmnopqruvwxyz",
                new("¡Usted ha entrado al Cerebro de la Bestia!", "You have entered the Brain of the Beast!"),
                r[22]));

        r[31].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[32]));
        r[31].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[28]));
        r[31].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[38]));

        r[32].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[32].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[33]));
        r[32].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[44]));

        r[33].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[26]));
        r[33].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[32]));
        r[33].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[34]));

        r[34].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[35]));
        r[34].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[41]));
        r[34].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[33]));

        r[35].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[34]));
        r[35].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[36]));
        r[35].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[46]));

        r[36].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[49]));
        r[36].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[35]));
        r[36].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[37]));

        r[37].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[29]));
        r[37].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[37].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[36]));

        r[38].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[26]));
        r[38].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[39]));
        r[38].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));

        r[39].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[39].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[38]));
        r[39].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[40]));

        r[40].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[40].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[26]));
        r[40].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[39]));

        r[41].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[33]));
        r[41].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[34]));
        r[41].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[42]));

        r[42].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[32]));
        r[42].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[43]));
        r[42].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[41]));

        r[43].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[26]));
        r[43].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[42]));
        r[43].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[44]));

        r[44].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[45]));
        r[44].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[32]));
        r[44].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));

        r[45].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[44]));
        r[45].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[32]));
        r[45].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));

        r[46].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[47]));
        r[46].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[33]));
        r[46].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[35]));

        r[47].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[46]));
        r[47].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[34]));
        r[47].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[48]));

        r[48].Children.Add(new OpenLink(
                new("Puerta 1", "Door 1"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[31]));
        r[48].Children.Add(new OpenLink(
                new("Puerta 2", "Door 2"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[49]));
        r[48].Children.Add(new OpenLink(
                new("Puerta 3", "Door 3"),
                new("Es idéntica a las otras dos.", "It is identical to the other two."),
                new("Usted atraviesa la puerta.", "You go through the door."),
                r[47]));

        r[49].Children.Add(new OpenLink(
                new("Salida", "Exit"),
                new("Conduce a la entrada del laberinto.", "It leads to the entrance of the Labyrinth."),
                new("Usted regresa a la entrada del Laberinto.", "You return to the entrance of the Labyrinth."),
                r[28]));

        r[50].Children.Add(new Item(
                new("Inscripción", "Inscription"),
                new("¡Cuidado! Este no es el Jardín verdadero.", "Careful! This is not the true Garden."),
                1));
        r[50].Children.Add(new Item(
                new("Cedro", "Cedar"),
                new("Le faltan algunas ramas. Al parecer, alguien las cortó para hacerse una cama.", "It is missing some branches. Apparently, someone cut them to make a bed."),
                Inventory.LWeight + 1));
        r[50].Children.Add(new OpenLink(
                new("Puerta verde", "Green door"),
                new("Conduce a los baños de la Bestia.", "It leads to the baths of the Beast."),
                new("Usted sale del jardín.", "You leave the garden."),
                r[26]));
        r[50].Children.Add(new OpenLink(
                new("Puerta azul", "Blue door"),
                new("Debe haber algo hemoso detrás de esta puerta.", "There must be something beautiful behind this door."),
                new("Usted sale del jardín.", "You leave the garden."),
                r[26]));
        r[50].Children.Add(new OpenLink(
                new("Puerta amarilla", "Yellow door"),
                new("Conduce a la sala de infusiones.", "It leads to the infusion room."),
                new("Usted sale del jardín.", "You leave the garden."),
                r[26]));
        r[50].Children.Add(new RiddleLink(
                new("Puerta de hierro", "Iron door"),
                new("Conduce los calabozos.", "It leads to the dungeons."),
                new("¿Cuántas antorchas iluminan al Minotauro?", "How many torches light the Minotaur?"),
                "Seis",
                new("Usted entra al salón de los elegidos.", "You enter the hall of the chosen."),
                r[30]) { AltAnswer = "Six" });
        r[50].Children.Add(new OpenLink(
                new("Puerta de madera", "Wooden door"),
                new("Conduce al almacén.", "It leads to the storehouse."),
                new("Usted sale del jardín.", "You leave the garden."),
                r[26]));

        var game = new Fort1(r[1]);
        for (var i = 1; i <= RoomData.Length; i++)
            game.AddRoom(r[i]);
        player = game.Player;
        return game;
    }
}
