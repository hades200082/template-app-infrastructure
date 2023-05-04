using Microsoft.Azure.Cosmos;
using Shared.Core;

namespace Infrastructure.Cosmos;

internal static class JsonPatchExtensions
{
    internal static IReadOnlyList<PatchOperation> ToCosmosPatchOperations(this JsonPatch jsonPatchOperations)
    {

        List<PatchOperation> cosmosPatchOperations = new List<PatchOperation>(jsonPatchOperations.Count);
        foreach (JsonPatchOperation jsonPatchOperation in jsonPatchOperations)
        {
            switch (jsonPatchOperation.Op)
            {
                case "add":
                case "Add":
                    cosmosPatchOperations.Add(PatchOperation.Add(jsonPatchOperation.Path, jsonPatchOperation.Value));
                    break;
                case "remove":
                case "Remove":
                    cosmosPatchOperations.Add(PatchOperation.Remove(jsonPatchOperation.Path));
                    break;
                case "replace":
                case "Replace":
                    cosmosPatchOperations.Add(PatchOperation.Replace(jsonPatchOperation.Path, jsonPatchOperation.Value));
                    break;
                case "set":
                case "Set":
                    cosmosPatchOperations.Add(PatchOperation.Set(jsonPatchOperation.Path, jsonPatchOperation.Value));
                    break;
            }
        }

        return cosmosPatchOperations;
    }
}
