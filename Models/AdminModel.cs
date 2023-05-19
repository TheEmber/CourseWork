using CourseWork.Users;

namespace CourseWork.Models;

public class Admin : User, IFlightManagement
{
    public void ChangeFlight()
    {
        throw new NotImplementedException();
    }

    void IFlightManagement.AddFlight()
    {
        throw new NotImplementedException();
    }

    void IFlightManagement.AddTickets()
    {
        throw new NotImplementedException();
    }

    void IFlightManagement.RemoveFlight()
    {
        throw new NotImplementedException();
    }

    void IFlightManagement.RemoveTickets()
    {
        throw new NotImplementedException();
    }
}