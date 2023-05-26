using CourseWork.Models;
using Xunit;

namespace CourseWork.Tests;
public class FlightModelTest
{
    [Fact]
    public void ToFlightWithCountTest()
    {
        const int number = 5;
        Flight flight = new();

        FlightWithCount flightWithCount = flight.ToFlightWithCount(number);

        Assert.True(number == flightWithCount.TicketCount);
    }
}