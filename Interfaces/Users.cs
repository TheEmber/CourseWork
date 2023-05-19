namespace CourseWork.Users;

public interface IPurchasable
{
    void PurchaseTicket();
    void ReturnTicket();
}
public interface IFlightManagement
{
    void AddFlight();
    void ChangeFlight();
    void RemoveFlight();
    void AddTickets();
    void RemoveTickets();
}