using UnityEngine;
/// <summary>
/// Cette classe représente l'état "Idle" de la catapulte dans le système de machine à états. Lorsque la catapulte est dans cet état, elle est au repos et n'effectue aucune action spécifique. Ce code définit les méthodes de base pour entrer, sortir et mettre à jour cet état, mais elles sont actuellement vides, ce qui signifie que la catapulte ne fait rien de particulier lorsqu'elle est dans cet état. Les transitions vers d'autres états (comme le tir ou la préparation) seraient gérées par le contrôleur d'état en fonction des conditions définies dans le jeu.
/// </summary>
public class Catapult_IdleState : State
{
    private Catapult_StateController _catapult;

    public Catapult_IdleState(Catapult_StateController sm) : base(sm) // Le constructeur de l'état Idle prend une référence au contrôleur d'état de la catapulte, ce qui lui permet d'accéder aux méthodes et propriétés du contrôleur pour gérer les transitions d'état et les comportements spécifiques à cet état.
    {
        _catapult = (Catapult_StateController)sm; // J'ai du faire un cast pour assigner le contrôleur d'état à la variable locale, car le constructeur de la classe de base State prend une référence générique à une machine à états (StateMachine), et ici nous savons que c'est spécifiquement un Catapult_StateController.
    }
    /// <summary>
    /// La méthode Enter() est appelée lorsque la catapulte entre dans l'état Idle. Actuellement, elle est vide, ce qui signifie que la catapulte ne fait rien de particulier lorsqu'elle entre dans cet état. Cependant, c'est ici que vous pourriez ajouter des comportements spécifiques à l'état Idle, comme jouer une animation de repos ou réinitialiser certaines variables.
    /// </summary>
    public override void Enter() { }
    /// <summary>
    /// La méthode Exit() est appelée lorsque la catapulte quitte l'état Idle pour passer à un autre état. Actuellement, elle est vide, ce qui signifie que la catapulte ne fait rien de particulier lorsqu'elle quitte cet état. Cependant, c'est ici que vous pourriez ajouter des comportements spécifiques à la transition hors de l'état Idle, comme arrêter une animation de repos ou préparer certaines variables pour le prochain état.
    /// </summary>
    public override void Exit() { }
    /// <summary>
    /// La méthode Update() est appelée une fois par frame lorsque la catapulte est dans l'état Idle. Actuellement, elle est vide, ce qui signifie que la catapulte ne fait rien de particulier pendant qu'elle est dans cet état. Cependant, c'est ici que vous pourriez ajouter des comportements spécifiques à l'état Idle, comme vérifier les conditions pour passer à un autre état (par exemple, si le joueur appuie sur un bouton pour préparer le tir) ou gérer des animations de repos.
    /// </summary>
    public override void Update() { }  //La classe StateMachine appelle cette méthode à chaque frame lorsque la catapulte est dans l'état Idle, ce qui permet de gérer les comportements spécifiques à cet état de manière continue pendant que la catapulte est au repos.s
}