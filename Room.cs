namespace App;

//// enum istället för fria strängar. billig försäkring mot stavfel.
public enum RoomStatus
{
    Available,    // Kan bokas
    Occupied,     // Någon bor där
    Maintenance   // Tillfälligt avstängt
}

public class Room
{
  private int _number;              // rumsnummer
  private RoomStatus _status;       // current state
  private string? _currentGuest;    // vem som bor där (null = ingen)

  // default: rum är ledigt när det skapas
  public Room(int number)
  {
    _number = number;                // spara numret
    _status = RoomStatus.Available;  // startläge
    _currentGuest = null;            // ingen gäst
  }

  // enkla getters, properties är overkill i detta fallet imo
  public int GetNumber() { return _number; }
  public RoomStatus GetStatus() { return _status; }
  public string? GetCurrentGuest() { return _currentGuest; }

  // sätter rummet till ledigt igen, rensar gäst
  public void SetAvailable()
  {
    _status = RoomStatus.Available;
    _currentGuest = null;
  }

  // markerar upptaget och sparar namnet
  public void SetOccupied(string guestName)
  {
    _status = RoomStatus.Occupied;
    _currentGuest = guestName;
  }

  // markerar som underhåll. låter ev. gäst ligga kvar, det får actions lösa.
  public void SetMaintenance()
  {
    _status = RoomStatus.Maintenance;
  }
}
