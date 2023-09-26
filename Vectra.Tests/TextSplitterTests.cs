using Vectra.Models;

namespace Vectra.Tests
{
    public class TextSplitterTests
    {
        [Fact]
        public async void Test_Split_Text()
        {
            // Arrange
            var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do";

            var config = new TextSplitterConfig
            {
                KeepSeparators = true,
                DocType = ".txt",
                ChunkSize = 10,
                ChunkOverlap = 2,
                Tokenizer = new GPT3Tokenizer()
            };

            var splitter = new TextSplitter(config);

            // Assert
            var chunks = await splitter.Split(text);


            // Assert
            Assert.NotNull(chunks);
        }
    }
}