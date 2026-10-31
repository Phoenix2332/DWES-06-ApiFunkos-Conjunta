using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ApiFunkosConjunta.IdGenerators;

public class YouTubeIdValueGenerator : ValueGenerator<string> {
    private const string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) {
        return string.Concat(Enumerable.Range(0, 11)
            .Select(_ => _alphabet[Random.Shared.Next(_alphabet.Length)])
        );
    }
}