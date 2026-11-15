using Fortaleza.Engine;
using Fortaleza.Localization;

namespace Fortaleza.Games;

internal sealed class Fort2(Room start) : GameEngine(start)
{
    public static readonly IReadOnlyList<LocalizedText> Escape =
    [
        new(
            "De repente las paredes comienzan a desvanecerse.\nUsted no quiere dar crédito a sus ojos: \n¡Ha logrado salir de la Fortaleza!",
            "Suddenly the walls begin to fade away.\nYou cannot believe your eyes: \nYou have managed to get out of La Fortaleza!"),
    ];

    public static readonly IReadOnlyList<LocalizedText> EpilogueFirstPage =
    [
        new("                               EPILOGO", "                               EPILOGUE"),
        new(string.Empty, string.Empty),
        new("Usted mira al cielo agradecido y besa la tierra.", "You look at the sky gratefully and kiss the ground."),
        new(
            "Después de que la emoción del primer momento hubo pasado usted se sienta sobre una piedra al borde de la " +
                "carretera (recuerde que ha regresado a la civilización) y comienza a pensar. \"¿Para qué salí?\", se " +
                "pregunta, \"si el exterior nunca me gustó tanto\".",
            "After the excitement of the first moment has passed you sit on a stone at the edge of the road (remember that " +
                "you have returned to civilization) and begin to think. \"Why did I leave?\", you ask yourself, \"if I never " +
                "liked the outside that much\"."),
        new(
            "Usted recuerda a la Doncella del Lago, y piensa en la vida que hubiera podido llevar junto a ella.",
            "You remember the Lady of the Lake, and think of the life you could have led by her side."),
        new(
            "Piensa también en el Hechicero, un poderoso amigo, y en la remota y hermosa posibilidad de hacer renacer la " +
                "civilización de los horantes. Ahora, sin la Bestia y sin la Hija del Hechicero, la Fortaleza podría ser " +
                "convertida en un verdadero paraíso.",
            "You also think of the Sorcerer, a powerful friend, and of the remote and beautiful possibility of reviving " +
                "the civilization of the horantes. Now, without the Beast and without the Sorcerer's Daughter, La Fortaleza " +
                "could be turned into a true paradise."),
        new(string.Empty, string.Empty),
        new("¿Quién está al otro extremo de los hilos que manejan al hombre?", "Who is at the other end of the strings that move man?"),
        new("¿Quién nos hace confundir lo que somos y lo que queremos ser?", "Who makes us confuse what we are with what we want to be?"),
        new("¿Quién es el responsable de nuestros errores?", "Who is responsible for our mistakes?"),
        new("¿Quién sino nosotros mismos?", "Who but ourselves?"),
        new(string.Empty, string.Empty),
    ];

    public static readonly IReadOnlyList<LocalizedText> EpilogueLastPage =
    [
        new(
            "Usted vuelve la cabeza hacia el lugar que ocupó la Fortaleza. Ahora lo comprende todo claramente, esta era la venganza de la Bestia: dejarlo vivir, pero fuera de la Fortaleza.",
            "You turn your head toward the place La Fortaleza occupied. Now you understand everything clearly, this was the Beast's revenge: to let you live, but outside La Fortaleza."),
        new(string.Empty, string.Empty),
        new("¿Quería un final feliz? Espere la próxima versión.", "Did you want a happy ending? Wait for the next version."),
        new(string.Empty, string.Empty),
        new(string.Empty, string.Empty),
        new("                                                       M. Cepero", "                                                       M. Cepero"),
        new(string.Empty, string.Empty),
    ];

    private static readonly LocalizedText ChaliceName = new("Cáliz", "Chalice");

    private static readonly LocalizedText ChaliceDescription = new(
        "Tiene grabadas las siguientes palabras:\n\n"
        + " \"Lsmra elshrne o:\n  Rs imnee od e ao\n  Bt ecrnee aoil pet\n  Atrh ne urod usee\n"
        + "  E nloeeiojnoa bn\n  Ecd eaulse acua elshrne\n  Epj el iadlhcieoe ucat\n  Pnuoe lslnd ua\"",
        "It has the following words engraved:\n\n"
        + " \"Temrso h oatsae\n  Daodrs ttebto ftelk\n  Cao' oto h poiesoe\n  Trhi h us om\n"
        + "  Tecoe ignx oteeoy\n  Sil fAhle ntect ftehrne\n  Mro ftesree' agtri e om\n  Pnuu ntesoigro\"");

    private static readonly (int Room, string Name)[] Carried =
    [
        (30, "Rosa diamante"),
        (11, "Bote"),
        (0, "Antorcha"),
        (42, "Cinta de Moebius"),
        (39, "Escudo de Aquiles"),
        (3, "Espejo"),
        (2, "Péndulo"),
    ];

    private static readonly (int Room, string Name)[] Gone =
    [
        (45, "Hija del Hechicero"),
        (8, "Monstruo"),
    ];

    private bool _sorcererDead;
    private bool _daughterDead;

    public override bool CheckGoal()
    {
        if (!_sorcererDead && GetRoom(54).Get("Hechicero") is null)
        {
            Thing.Output(new LocalizedText(
                "Usted solo alcanza a herir al Hechicero, que ha desaparecido misteriosamente.",
                "You only manage to wound the Sorcerer, who has mysteriously disappeared.").Get());
            Thing.Output(new LocalizedText(
                "Se escucha una voz que dice: \"¡Estúpido! Vas por el camino equivocado.\"",
                "A voice is heard saying: \"Fool! You are going the wrong way.\"").Get());
            _sorcererDead = true;
        }

        if (!_daughterDead && GetRoom(45).Get("Hija del Hechicero") is null)
        {
            Thing.Output(new LocalizedText(
                "En el lugar que ocupaba la Hija del Hechicero ha aparecido un objeto.",
                "An object has appeared where the Sorcerer's Daughter stood.").Get());
            GetRoom(45).Children.Add(new Item(ChaliceName, ChaliceDescription, 3));
            _daughterDead = true;
        }

        if (GetRoom(6).Get("Pañuelo") is not null)
        {
            Player.Die(string.Empty);
        }

        return Array.TrueForAll(Carried, target => GetRoom(target.Room).Get(target.Name) is not null)
            && Array.TrueForAll(Gone, target => GetRoom(target.Room).Get(target.Name) is null);
    }
}
