using Ardalis.GuardClauses;
using IdGen;

namespace BuildingBlocks.Core.IdsGenerator;

public class SnowFlakIdGenerator : Abstractions.Core.IIdGenerator<long>
{
    private static IdGenerator? _generator;

    public long New()
    {
        return NewId();
    }

    public static long NewId()
    {
        return _generator.CreateId();
    }

    public static void Configure(int generatorId)
    {
        Guard.Against.NegativeOrZero(generatorId, nameof(generatorId));

        // Let's say we take jan 1st 2023 as our epoch
        var epoch = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local);

        // Create an ID with 45 bits for timestamp, 2 for generator-id
        // and 16 for sequence
        var structure = new IdStructure(45, 2, 16);

        // Prepare options
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

        // Create an IdGenerator with it's generator-id set to 0, our custom epoch
        // and id-structure
        _generator = new IdGenerator(0, options);
    }
}
