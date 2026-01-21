namespace LimasIotDevices.Domain.Models;

public class TranslationKeyModel
{
    public TranslationKeyModel(
        string name,
        params string[] stringParameters)
    {
        Name = name;
        StringParameters = stringParameters;
    }


    public string Name { get; private set; }
    public string[] StringParameters { get; private set; } = [];


    public static implicit operator TranslationKeyModel(string name)
    {
        return new TranslationKeyModel(name);
    }
}

public static class TranslationKeyModelExtensions
{
    public static void Add(
        this IList<TranslationKeyModel> list,
        TranslationKeyModel keyModel,
        params string[] stringParameters)
    {
        var keyModelToAdd = new TranslationKeyModel(keyModel.Name, stringParameters);
        list.Add(keyModel);
    }
}