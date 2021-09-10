using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public class MaskingTests
    {
        [Theory]
        [InlineAutoData("originalValue", 4,    '*', "*********alue")]
        [InlineAutoData("originalValue", 5,    '*', "********Value")]
        [InlineAutoData("small",         10,   '*', "small")]
        [InlineAutoData("small",         null, '*', "*****")]
        [InlineAutoData(null,            null, '*', null)]
        [InlineAutoData("",              null, '*', "")]
        [InlineAutoData("   ",           null, '*', "   ")]
        public void MaskValue_DefaultParameters_ValueIsMaskedCompletely(
            string value,
            int? visibleCount,
            char maskChar,
            string expected
        )
        {
            // Act
            var result = value.MaskValue(visibleCount, maskChar);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
