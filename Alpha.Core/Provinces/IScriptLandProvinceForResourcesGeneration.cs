using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    [ScriptName("Province")]
    public interface IScriptLandProvinceForResourcesGeneration
    {
        bool IsCoastal { get; }
    }
}