using System.Text.Json;

namespace Shared.Core;

public class JsonPatch : List<JsonPatchOperation>
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
