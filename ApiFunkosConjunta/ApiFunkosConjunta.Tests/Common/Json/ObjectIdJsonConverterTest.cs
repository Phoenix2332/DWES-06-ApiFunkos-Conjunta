using System.Text;
using System.Text.Json;
using ApiFunkosConjunta.Common.Json;
using FluentAssertions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Common.Json;

[TestFixture]
public class ObjectIdJsonConverterTest {
    private readonly ObjectIdJsonConverter _converter = new();

    [Test]
    public void Read_ConObjectIdValido_DevuelveElObjectId() {
        // Arrange
        const string Id = "64b000000000000000000002";

        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes($"\"{Id}\""));

        reader.Read();

        // Act
        var resultado = _converter.Read(ref reader, typeof(ObjectId), JsonSerializerOptions.Default);

        // Assert
        resultado.Should().Be(ObjectId.Parse(Id));
    }

    [Test]
    public void Read_ConCadenaVacia_DevuelveElObjectIdVacio() {
        // Arrange
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes("\"\""));

        reader.Read();

        // Act
        var resultado = _converter.Read(ref reader, typeof(ObjectId), JsonSerializerOptions.Default);

        // Assert
        resultado.Should().Be(ObjectId.Empty);
    }

    [Test]
    public void Write_ConObjectId_EscribeLaCadenaHexadecimal() {
        // Arrange
        var id = ObjectId.Parse("64b000000000000000000002");

        using var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(stream)) {
            // Act
            _converter.Write(writer, id, JsonSerializerOptions.Default);
        }

        // Assert
        JsonSerializer.Deserialize<string>(stream.ToArray()).Should().Be("64b000000000000000000002");
    }
}
