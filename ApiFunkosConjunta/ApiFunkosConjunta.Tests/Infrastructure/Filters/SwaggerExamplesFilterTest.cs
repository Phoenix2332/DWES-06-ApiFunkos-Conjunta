using System.Text.Json.Nodes;
using ApiFunkosConjunta.Infrastructure.Filters;
using FluentAssertions;
using Microsoft.OpenApi;

namespace ApiFunkosConjunta.Tests.Infrastructure.Filters;

[TestFixture]
public class SwaggerExamplesFilterTest {
    private static OpenApiOperation CrearOperacion() {
        return new OpenApiOperation {
            RequestBody = new OpenApiRequestBody {
                Content = new Dictionary<string, OpenApiMediaType> {
                    ["application/json"] = new() { Schema = new OpenApiSchema() }
                }
            },
            Responses = new OpenApiResponses {
                ["404"] = new OpenApiResponse {
                    Description = "No encontrado",
                    Content = new Dictionary<string, OpenApiMediaType> {
                        ["application/json"] = new() { Schema = new OpenApiSchema() }
                    }
                },
                ["200"] = new OpenApiResponse { Description = "Correcto" }
            }
        };
    }

    [Test]
    public void Apply_SinEjemploEnElCuerpo_AnadeElEjemploGenerico() {
        // Arrange
        var operation = CrearOperacion();

        var filter = new SwaggerExamplesFilter();

        // Act
        filter.Apply(operation, null!);

        // Assert
        var ejemplo = operation.RequestBody.Content["application/json"].Example;

        ejemplo.Should().NotBeNull();
        ejemplo!["nombre"]!.GetValue<string>().Should().Be("Funko Pop! Batman");
    }

    [Test]
    public void Apply_ConEjemploEnElCuerpo_NoLoSobrescribe() {
        // Arrange
        var operation = CrearOperacion();

        operation.RequestBody.Content["application/json"].Example = JsonNode.Parse("""{"nombre":"Original"}""");

        var filter = new SwaggerExamplesFilter();

        // Act
        filter.Apply(operation, null!);

        // Assert
        operation.RequestBody.Content["application/json"].Example!["nombre"]!.GetValue<string>()
            .Should().Be("Original");
    }

    [Test]
    public void Apply_RespuestaDeError_AnadeElEjemploProblemDetails() {
        // Arrange
        var operation = CrearOperacion();

        var filter = new SwaggerExamplesFilter();

        // Act
        filter.Apply(operation, null!);

        // Assert
        var ejemplo = operation.Responses!["404"].Content["application/json"].Example;

        ejemplo.Should().NotBeNull();
        ejemplo!["status"]!.GetValue<int>().Should().Be(404);
        ejemplo["type"]!.GetValue<string>().Should().Contain("rfc9457");    }

    [Test]
    public void Apply_RespuestaCorrecta_NoAnadeEjemploDeError() {
        // Arrange
        var operation = CrearOperacion();

        var filter = new SwaggerExamplesFilter();

        // Act
        filter.Apply(operation, null!);

        // Assert
        operation.Responses!["200"].Content.Should().BeNull();
    }

    [Test]
    public void Apply_SinRequestBodyNiRespuestas_NoLanzaExcepcion() {
        // Arrange
        var operation = new OpenApiOperation();

        var filter = new SwaggerExamplesFilter();

        // Act
        var act = () => filter.Apply(operation, null!);

        // Assert
        act.Should().NotThrow();
    }
}
