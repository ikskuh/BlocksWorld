namespace BlocksWorld
{
    public interface IInteractiveEnvironment
    {
        PhraseTranslator Server { get; }

        World World { get; }
    }
}