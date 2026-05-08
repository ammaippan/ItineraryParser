using ItineraryParser.Core.Interfaces;
using ItineraryParser.Infrastructure.Interfaces;
using ItineraryParser.Infrastructure.Services;
using Moq;
using Xunit;

public class ParserServiceTests
{
    [Fact]
    public async Task Should_Parse_Valid_Response()
    {
        var llmMock = new Mock<ILLMService>();
        var promptMock = new Mock<IPromptLoader>();

        promptMock.Setup(x => x.Build(It.IsAny<string>()))
                  .Returns("dummy prompt");

        llmMock.Setup(x => x.ExtractAsync(It.IsAny<string>()))
            .ReturnsAsync(@"{
                ""SourceCity"": ""Chennai"",
                ""Destination"": ""Bali"",
                ""StartDate"": ""2024-06-10"",
                ""EndDate"": ""2024-06-16"",
                ""Adults"": 2,
                ""Children"": 1,
                ""Budget"": 150000,
                ""Currency"": ""INR""
            }");

        var service = new ParserService(llmMock.Object, promptMock.Object);

        var result = await service.ParseAsync("dummy input");

        Assert.NotNull(result);
        Assert.Equal("Chennai", result.SourceCity);
    }
}